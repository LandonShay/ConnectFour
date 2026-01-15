using ConnectFour.Data;
using Microsoft.AspNetCore.Components.Web;
using static ConnectFour.Data.PacGridBox;

namespace ConnectFour.Pages
{
    public partial class Pacman
    {
        private readonly float _tickDuration = .25f;
        private readonly float _poweredUpDuration = 5f;

        private GameStatus Status = GameStatus.None;
        private Move MoveDirection = Move.None;

        private List<PacGridBox> GridBoxes = new();
        private PacGridBox? CurrentPlayerBox;

        private int Score { get; set; }

        private bool IsPoweredUp { get; set; }
        private float poweredUpElasped { get; set; }

        private bool OrangeCanMove { get; set; }
        private bool BlueCanMove { get; set; }
        private bool PinkCanMove { get; set; }
        private bool RedCanMove { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ResetMap();
        }

        private void ResetMap()
        {
            GridBoxes.Clear();
            Status = GameStatus.None;

            var x = 0;
            var y = 0;

            foreach (var gridItem in PacMap.Grid)
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
            _ = Tick();
        }

        private async Task Tick()
        {
            while (true)
            {
                HandlePoweredUp();

                // move ghosts here
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

        #region Move
        private void MovePacman()
        {
            if (MoveDirection == Move.Up)
            {
                var stopBlockers = new List<Blockers>() { Blockers.Top, Blockers.TopLeftCorner, Blockers.TopRightCorner };
                TryMoveBox(stopBlockers, -1, false);
            }
            else if (MoveDirection == Move.Down)
            {
                var stopBlockers = new List<Blockers>() { Blockers.Bottom, Blockers.BottomLeftCorner, Blockers.BottomRightCorner };
                TryMoveBox(stopBlockers, 1, false);
            }
            else if (MoveDirection == Move.Right)
            {
                var stopBlockers = new List<Blockers>() { Blockers.Right, Blockers.TopRightCorner, Blockers.BottomRightCorner };
                TryMoveBox(stopBlockers, 1, true);
            }
            else if (MoveDirection == Move.Left)
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
                    MoveDirection = Move.Up;
                    break;
                case "a":
                    MoveDirection = Move.Left;
                    break;
                case "s":
                    MoveDirection = Move.Down;
                    break;
                case "d":
                    MoveDirection = Move.Right;
                    break;
            }
        }
        #endregion

        #region Ghosts
        private void GhostCountdown()
        {
            var reference = new Dictionary<Creatures, float>()
            {
                { Creatures.OrangeGhost, 3 },
                { Creatures.BlueGhost, 6 },
                { Creatures.PinkGhost, 9 },
                { Creatures.RedGhost, 12 },
            };

            foreach (var item in reference)
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay((int)(item.Value * 1000));

                    var ghostBox = GridBoxes.First(x => x.Entities.Any(x => x.Creature == item.Key));
                    var entrance = GridBoxes.First(x => x.IsEntrance);

                    var ghost = ghostBox.Entities.First();
                    ghost.CanMove = true;

                    ghostBox.Entities.Remove(ghost);
                    entrance.Entities.Add(ghost);

                });
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

        private enum Move
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
