using ConnectFour.Data.Pacman;
using Microsoft.AspNetCore.Components.Web;
using static ConnectFour.Data.Pacman.PacGridBox;

namespace ConnectFour.Pages
{
    public partial class Pacman
    {
        private readonly float _tickDuration = .35f;
        private readonly float _poweredUpDuration = 5f;

        private GameStatus Status = GameStatus.None;
        private MoveDir MoveDirection = MoveDir.None;

        private List<PacGridBox> GridBoxes = new();
        private PacGridBox? CurrentPlayerBox;
        private PacMap Map = new PacMap();

        private int Score { get; set; }

        private bool IsPoweredUp { get; set; }
        private float poweredUpElasped { get; set; }
        private int GhostsEatenCombo { get; set; }

        private PacEntity OrangeGhost { get; set; } = new();
        private PacEntity BlueGhost { get; set; } = new();
        private PacEntity PinkGhost { get; set; } = new();
        private PacEntity RedGhost { get; set; } = new();

        private CancellationTokenSource playerCancel = new();
        private List<CancellationTokenSource> ghostCancels = new();

        #region Config
        protected override void OnInitialized()
        {
            ResetMap();
        }

        public void Dispose()
        {
            StopAllMovement();
        }

        private void ResetMap()
        {
            GridBoxes.Clear();
            Map = new PacMap();

            OrangeGhost = new();
            BlueGhost = new();
            PinkGhost = new();
            RedGhost = new();

            MoveDirection = MoveDir.None;
            Status = GameStatus.None;
            poweredUpElasped = 0;
            IsPoweredUp = false;
            Score = 0;

            var x = 0;
            var y = 0;

            foreach (var gridItem in Map.Grid)
            {
                gridItem.Coordinates = (x, y);
                x++;

                if (x == 17)
                {
                    y++;
                    x = 0;
                }

                GridBoxes.Add(gridItem);
            }

            playerCancel = new();
            CurrentPlayerBox = GridBoxes.First(x => x.Entities.Any(y => y.Creature == Creatures.Pacman));

            StateHasChanged();
            GhostCountdown();

            _ = PlayerTick();
        }
        #endregion

        #region Ticks
        private async Task PlayerTick()
        {
            try
            {
                while (true)
                {
                    if (playerCancel.IsCancellationRequested)
                    {
                        break;
                    }

                    HandlePoweredUp();
                    MovePacman();

                    CheckWin();
                    StateHasChanged();

                    if (Status != GameStatus.None)
                    {
                        break;
                    }

                    await Task.Delay((int)(_tickDuration * 1000), playerCancel.Token);
                }
            }
            catch (TaskCanceledException)
            {
                // expected
            }
        }

        private async Task GhostTick(PacGhost ghost, CancellationTokenSource ghostCancel)
        {
            try
            {
                var isFirstMove = true;

                while (true)
                {
                    if (!isFirstMove)
                    {
                        var tickTime = 0f;

                        if (ghost.Retreating)
                        {
                            tickTime = ghost.RetreatTickTime;
                        }
                        else if (ghost.GoingHome)
                        {
                            tickTime = ghost.GoingHomeTickTime;
                        }
                        else if (ghost.Recovering)
                        {
                            tickTime = ghost.RecoverTickTime;
                        }
                        else
                        {
                            tickTime = ghost.TickTime;
                        }
                        
                        await Task.Delay((int)(tickTime * 1000), ghostCancel.Token);
                    }
                    else
                    {
                        isFirstMove = false;
                    }

                    if (ghostCancel.IsCancellationRequested)
                    {
                        break;
                    }

                    ghost.Move(GridBoxes);

                    CheckWin();
                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (TaskCanceledException)
            {
                // expected
            }
        }

        public void StopAllMovement()
        {
            ghostCancels.ForEach(x => x.Cancel());
            playerCancel.Cancel();

            ghostCancels.Clear();

            OrangeGhost = new();
            BlueGhost = new();
            PinkGhost = new();
            RedGhost = new();
        }
        #endregion

        #region Move
        private void MovePacman()
        {
            if (MoveDirection == MoveDir.Up)
            {
                var stopBlockers = new List<Blockers>() { Blockers.Top, Blockers.TopLeftCorner, Blockers.TopRightCorner };
                TryMoveBox(stopBlockers, -1, false);
            }
            else if (MoveDirection == MoveDir.Down)
            {
                var stopBlockers = new List<Blockers>() { Blockers.Bottom, Blockers.BottomLeftCorner, Blockers.BottomRightCorner };
                TryMoveBox(stopBlockers, 1, false);
            }
            else if (MoveDirection == MoveDir.Right)
            {
                var stopBlockers = new List<Blockers>() { Blockers.Right, Blockers.TopRightCorner, Blockers.BottomRightCorner };
                TryMoveBox(stopBlockers, 1, true);
            }
            else if (MoveDirection == MoveDir.Left)
            {
                var stopBlockers = new List<Blockers>() { Blockers.Left, Blockers.TopLeftCorner, Blockers.BottomLeftCorner };
                TryMoveBox(stopBlockers, -1, true);
            }
        }

        private void TryMoveBox(List<Blockers> stopBlockers, int moveIndex, bool horizontal)
        {
            if (CurrentPlayerBox != null && !stopBlockers.Contains(CurrentPlayerBox.Blocker))
            {
                PacGridBox? targetBox = null;

                if (horizontal)
                {
                    targetBox = GridBoxes.Find(x => x.Coordinates.x == CurrentPlayerBox.Coordinates.x + moveIndex &&
                                                    x.Coordinates.y == CurrentPlayerBox.Coordinates.y);
                }
                else
                {
                    targetBox = GridBoxes.Find(x => x.Coordinates.x == CurrentPlayerBox.Coordinates.x &&
                                                    x.Coordinates.y == CurrentPlayerBox.Coordinates.y + moveIndex);
                }

                if (targetBox != null && targetBox.Blocker != Blockers.Full)
                {
                    var pac = CurrentPlayerBox.Entities.First(x => x.Creature == Creatures.Pacman);

                    CurrentPlayerBox.Entities.Remove(pac);
                    CurrentPlayerBox = targetBox;
                    CurrentPlayerBox.Entities.Add(pac);

                    if (CurrentPlayerBox.Item == BoxItem.Pellet)
                    {
                        Score += 200;
                    }
                    else if (CurrentPlayerBox.Item == BoxItem.PowerPellet)
                    {
                        Score += 500;
                        IsPoweredUp = true;
                        poweredUpElasped = 0;

                        ToggleGhostsRetreat(true);
                    }

                    CurrentPlayerBox.Item = BoxItem.None;
                }
            }
        }

        private void ChangeDirection(KeyboardEventArgs e)
        {
            var key = e.Key.ToLower();

            switch (key)
            {
                case "w":
                    MoveDirection = MoveDir.Up;
                    break;
                case "a":
                    MoveDirection = MoveDir.Left;
                    break;
                case "s":
                    MoveDirection = MoveDir.Down;
                    break;
                case "d":
                    MoveDirection = MoveDir.Right;
                    break;
            }
        }
        #endregion

        #region Ghosts
        private async void GhostCountdown()
        {
            var ghosts = new List<PacEntity>();
            var reference = new List<Creatures>()
            {
                { Creatures.OrangeGhost },
                { Creatures.BlueGhost },
                { Creatures.PinkGhost },
                { Creatures.RedGhost },
            };

            foreach (var ghost in reference)
            {
                var entity = ghost switch
                {
                    Creatures.OrangeGhost => OrangeGhost,
                    Creatures.BlueGhost => BlueGhost,
                    Creatures.PinkGhost => PinkGhost,
                    _ => RedGhost
                };

                entity.ConfigureGhost(GridBoxes, ghost);
                ghosts.Add(entity);
            }

            foreach (var ghost in ghosts)
            {
                await Task.Delay(2500);

                var ghostBox = GridBoxes.First(x => x.Entities.Any(x => x.Creature == ghost.Creature));
                var entrance = GridBoxes.First(x => x.IsEntrance);

                ghostBox.Entities.Clear();

                var ghostCancel = new CancellationTokenSource();
                ghostCancels.Add(ghostCancel);

                _ = GhostTick(ghost.Ghost, ghostCancel);
            }
        }

        private void ToggleGhostsRetreat(bool retreating)
        {
            if (retreating)
            {
                OrangeGhost.Ghost.Retreating = true;
                BlueGhost.Ghost.Retreating = true;
                PinkGhost.Ghost.Retreating = true;
                RedGhost.Ghost.Retreating = true;
            }
            else
            {
                OrangeGhost.Ghost.EndRetreat();
                BlueGhost.Ghost.EndRetreat();
                PinkGhost.Ghost.EndRetreat();
                RedGhost.Ghost.EndRetreat();
            }
        }
        #endregion

        private void HandlePoweredUp()
        {
            if (IsPoweredUp)
            {
                if (poweredUpElasped < _poweredUpDuration)
                {
                    poweredUpElasped += _tickDuration;

                    if (CurrentPlayerBox!.Entities.Count > 1)
                    {
                        foreach (var entity in CurrentPlayerBox.Entities.Where(x => x.Creature != Creatures.Pacman))
                        {
                            GhostsEatenCombo += 1;

                            entity.Ghost.GoingHome = true;
                            entity.Ghost.Retreating = false;

                            Score += 800 * GhostsEatenCombo;
                        }
                    }
                }
                else
                {
                    IsPoweredUp = false;
                    poweredUpElasped = 0;
                    GhostsEatenCombo = 0;

                    ToggleGhostsRetreat(false);
                }
            }
        }

        private void CheckWin()
        {
            if (!GridBoxes.Any(x => x.Item == BoxItem.Pellet))
            {
                Status = GameStatus.Win;
                StopAllMovement();
                return;
            }

            if (!IsPoweredUp && GridBoxes.Any(x => x.Entities.Count > 1 && x.Entities.Any(x => x.Creature == Creatures.Pacman)))
            {
                var contestedBoxes = GridBoxes.FindAll(x => x.Entities.Count > 2);

                foreach (var box in contestedBoxes)
                {
                    var ghosts = box.Entities.FindAll(x => x.Ghost != null);

                    if (ghosts.All(x => !x.Ghost.GoingHome && !x.Ghost.Retreating))
                    {
                        Status = GameStatus.Lose;
                        StopAllMovement();
                        return;
                    }
                }
            }
        }

        #region CSS
        private string GetCreatureCss(List<PacEntity> entities)
        {
            var css = string.Empty;

            if (entities.Any(x => x.Creature == Creatures.RedGhost))
            {
                css = "ghost red-ghost";
            }
            else if (entities.Any(x => x.Creature == Creatures.PinkGhost))
            {
                css = "ghost pink-ghost";
            }
            else if (entities.Any(x => x.Creature == Creatures.BlueGhost))
            {
                css = "ghost blue-ghost";
            }
            else if (entities.Any(x => x.Creature == Creatures.OrangeGhost))
            {
                css = "ghost orange-ghost";
            }

            if (IsPoweredUp && entities.All(x => x.Creature != Creatures.Pacman))
            {
                css += " ghost-retreat";
            }

            if (entities.Any(x => x.Ghost != null && x.Ghost.GoingHome))
            {
                css = "ghost ghost-go-home";
            }

            return css;
        }

        private string GetPacmanCss(List<PacEntity> entities)
        {
            if (entities.Any(x => x.Creature == Creatures.Pacman))
            {
                if (IsPoweredUp)
                {
                    return "pacman powered-up";
                }

                return "pacman";
            }

            return string.Empty;
        }

        private string GetBlockerCss(Blockers blocker)
        {
            return blocker switch
            {
                Blockers.Top => "top-blocker",
                Blockers.Full => "full-blocker",
                Blockers.Left => "left-blocker",
                Blockers.Right => "right-blocker",
                Blockers.Bottom => "bottom-blocker",
                Blockers.TopLeftCorner => "top-left-blocker",
                Blockers.TopRightCorner => "top-right-blocker",
                Blockers.BottomLeftCorner => "bottom-left-blocker",
                Blockers.BottomRightCorner => "bottom-right-blocker",
                _ => string.Empty
            };
        }
        #endregion

        public enum MoveDir
        {
            None,
            Up,
            Down,
            Left,
            Right
        }

        private enum GameStatus
        {
            None,
            Win,
            Lose
        }
    }
}
