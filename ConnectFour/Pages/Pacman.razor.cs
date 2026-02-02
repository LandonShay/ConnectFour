using ConnectFour.Data.Pacman;
using Microsoft.AspNetCore.Components.Web;
using static ConnectFour.Data.Pacman.PacGridBox;

namespace ConnectFour.Pages
{
    public partial class Pacman
    {
        private readonly float _tickDuration = .25f;
        private readonly float _poweredUpDuration = 5f;

        private GameStatus Status = GameStatus.None;
        private MoveDir MoveDirection = MoveDir.None;

        private List<PacGridBox> GridBoxes = new();
        private PacGridBox? CurrentPlayerBox;
        private PacMap Map = new PacMap();

        private int Score { get; set; }

        private bool IsPoweredUp { get; set; }
        private float poweredUpElasped { get; set; }

        private PacEntity OrangeGhost { get; set; } = new();
        private PacEntity BlueGhost { get; set; } = new();
        private PacEntity PinkGhost { get; set; } = new();
        private PacEntity RedGhost { get; set; } = new();

        protected override void OnInitialized()
        {
            ResetMap();
        }

        private void ResetMap()
        {
            GridBoxes.Clear();
            Status = GameStatus.None;

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

            CurrentPlayerBox = GridBoxes.First(x => x.Entities.Any(y => y.Creature == Creatures.Pacman));

            GhostCountdown();

            _ = PlayerTick();
        }

        #region Ticks
        private async Task PlayerTick()
        {
            while (true)
            {
                HandlePoweredUp();
                MovePacman();

                CheckWin();
                StateHasChanged();

                if (Status != GameStatus.None)
                {
                    break;
                }

                await Task.Delay((int)(_tickDuration * 1000));
            }
        }

        private async Task GhostTick(PacGhost ghost)
        {
            var isFirstMove = true;

            while (true)
            {
                if (!isFirstMove)
                {
                    await Task.Delay((int)(ghost.TickTime * 1000));
                }
                else
                {
                    isFirstMove = false;
                }

                ghost.Move(GridBoxes);
                await InvokeAsync(StateHasChanged);
            }
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
            var reference = new Dictionary<Creatures, float>()
            {
                { Creatures.OrangeGhost, 3 },
                { Creatures.BlueGhost, 3 },
                { Creatures.PinkGhost, 3 },
                { Creatures.RedGhost, 3 },
            };

            foreach (var item in reference)
            {
                await Task.Delay((int)(item.Value * 1000));

                var ghostBox = GridBoxes.First(x => x.Entities.Any(x => x.Creature == item.Key));
                var entrance = GridBoxes.First(x => x.IsEntrance);

                var ghost = ghostBox.Entities.First();
                ghost.Ghost.InSpawn = false;

                ghostBox.Entities.Remove(ghost);
                entrance.Entities.Add(ghost);

                var entity = ghost.Creature switch
                {
                    Creatures.OrangeGhost => OrangeGhost,
                    Creatures.BlueGhost => BlueGhost,
                    Creatures.PinkGhost => PinkGhost,
                    _ => RedGhost
                };

                entity = ghost;
                entity.Ghost.Entity = entity;
                entity.Ghost.CurrentBox = entrance;
                _ = GhostTick(entity.Ghost);
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
                }
                else
                {
                    IsPoweredUp = false;
                    poweredUpElasped = 0;
                }
            }
        }

        private void CheckWin()
        {
            if (!GridBoxes.Any(x => x.Item == BoxItem.Pellet))
            {
                Status = GameStatus.Win;
                return;
            }

            if (!IsPoweredUp && GridBoxes.Any(x => x.Entities.Count > 1 && x.Entities.Any(x => x.Creature == Creatures.Pacman)))
            { // if pacman and a ghost are in the same box
                Status = GameStatus.Lose;
                return;
            }
        }

        #region CSS
        private string GetCreatureCss(List<PacEntity> entities)
        {
            if (entities.Any(x => x.Creature == Creatures.RedGhost))
            {
                return "ghost red-ghost";
            }
            else if (entities.Any(x => x.Creature == Creatures.PinkGhost))
            {
                return "ghost pink-ghost";
            }
            else if (entities.Any(x => x.Creature == Creatures.BlueGhost))
            {
                return "ghost blue-ghost";
            }
            else if (entities.Any(x => x.Creature == Creatures.OrangeGhost))
            {
                return "ghost orange-ghost";
            }

            return string.Empty;
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
