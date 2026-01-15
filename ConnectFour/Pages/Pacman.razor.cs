using ConnectFour.Data;
using Microsoft.AspNetCore.Components.Web;
using static ConnectFour.Data.PacGridBox;

namespace ConnectFour.Pages
{
    public partial class Pacman
    {
        private GameStatus Status = GameStatus.None;
        private Move MoveDirection = Move.None;

        private List<PacGridBox> GridBoxes = new();
        private PacGridBox? CurrentPlayerBox;

        private int Score;

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
                y++;

                if (y == 15)
                {
                    x++;
                    y = 0;
                }

                GridBoxes.Add(gridItem);
            }

            //CurrentPlayerBox = GridBoxes.First(x => x.Item == BoxItem.Pacman);

            _ = MovePacman();
        }

        #region Move
        private async Task MovePacman()
        {
            while (true)
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

                CheckWin();
                StateHasChanged();

                if (Status == GameStatus.Win)
                {
                    return;
                }

                await Task.Delay(250);
            }
        }

        private void TryMoveBox(List<Blockers> stopBlockers, int moveIndex, bool horizontal)
        {
            if (CurrentPlayerBox != null && !stopBlockers.Contains(CurrentPlayerBox.Blocker))
            {
                PacGridBox? targetBox = null;

                if (horizontal)
                {
                    targetBox = GridBoxes.Find(x => x.Coordinates.Item1 == CurrentPlayerBox.Coordinates.Item1 &&
                                                    x.Coordinates.Item2 == CurrentPlayerBox.Coordinates.Item2 + moveIndex);
                }
                else
                {
                    targetBox = GridBoxes.Find(x => x.Coordinates.Item2 == CurrentPlayerBox.Coordinates.Item2 &&
                                                    x.Coordinates.Item1 == CurrentPlayerBox.Coordinates.Item1 + moveIndex);
                }

                if (targetBox != null && targetBox.Blocker != Blockers.Full)
                {
                    CurrentPlayerBox = targetBox;
                    CurrentPlayerBox.Entities.Add(Creatures.Pacman);

                    if (CurrentPlayerBox.Item == BoxItem.Pellet)
                    {
                        Score += 200;
                    }
                    else if (CurrentPlayerBox.Item == BoxItem.PowerPellet)
                    {
                        Score += 500;
                        // ghosts run, player can eat ghosts for x seconds
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

        private void CheckWin()
        {
            if (!GridBoxes.Any(x => x.Item == BoxItem.Pellet))
            {
                Status = GameStatus.Win;
            }
        }

        private string GetCreatureCss(List<Creatures> creatures)
        {
            if (creatures.Contains(Creatures.RedGhost))
            {
                return "ghost red-ghost";
            }
            else if (creatures.Contains(Creatures.BlueGhost))
            {
                return "ghost blue-ghost";
            }
            else if (creatures.Contains(Creatures.PinkGhost))
            {
                return "ghost pink-ghost";
            }
            else if (creatures.Contains(Creatures.OrangeGhost))
            {
                return "ghost orange-ghost";
            }

            return string.Empty;
        }

        private string GetPacmanCss(List<Creatures> creatures)
        {
            if (creatures.Contains(Creatures.Pacman))
            {
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
