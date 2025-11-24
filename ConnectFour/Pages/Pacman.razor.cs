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

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ResetMap();
        }

        private void ResetMap()
        {
            GridBoxes.Clear();
            Status = GameStatus.None;

            // 1st row
            GridBoxes.Add(new PacGridBox { Coordinates = (0, 0), Item = BoxItem.Pellet, Blocker = Blockers.TopLeftCorner });
            GridBoxes.Add(new PacGridBox { Coordinates = (0, 1), Item = BoxItem.Pellet, Blocker = Blockers.Top });
            GridBoxes.Add(new PacGridBox { Coordinates = (0, 2), Item = BoxItem.Pellet, Blocker = Blockers.Top });
            GridBoxes.Add(new PacGridBox { Coordinates = (0, 3), Item = BoxItem.Pellet, Blocker = Blockers.Top });
            GridBoxes.Add(new PacGridBox { Coordinates = (0, 4), Item = BoxItem.Pellet, Blocker = Blockers.Top });
            GridBoxes.Add(new PacGridBox { Coordinates = (0, 5), Item = BoxItem.Pellet, Blocker = Blockers.Top });
            GridBoxes.Add(new PacGridBox { Coordinates = (0, 6), Item = BoxItem.Pellet, Blocker = Blockers.TopRightCorner });

            // 2nd row
            GridBoxes.Add(new PacGridBox { Coordinates = (1, 0), Item = BoxItem.Pellet, Blocker = Blockers.Left });
            GridBoxes.Add(new PacGridBox { Coordinates = (1, 1), Blocker = Blockers.Full });
            GridBoxes.Add(new PacGridBox { Coordinates = (1, 2), Blocker = Blockers.Full });
            GridBoxes.Add(new PacGridBox { Coordinates = (1, 3), Blocker = Blockers.Full });
            GridBoxes.Add(new PacGridBox { Coordinates = (1, 4), Blocker = Blockers.Full });
            GridBoxes.Add(new PacGridBox { Coordinates = (1, 5), Blocker = Blockers.Full });
            GridBoxes.Add(new PacGridBox { Coordinates = (1, 6), Item = BoxItem.Pellet, Blocker = Blockers.Right });

            // 3rd row
            GridBoxes.Add(new PacGridBox { Coordinates = (2, 0), Item = BoxItem.Pacman, Blocker = Blockers.BottomLeftCorner });
            GridBoxes.Add(new PacGridBox { Coordinates = (2, 1), Item = BoxItem.Pellet, Blocker = Blockers.Bottom });
            GridBoxes.Add(new PacGridBox { Coordinates = (2, 2), Item = BoxItem.Pellet, Blocker = Blockers.Bottom });
            GridBoxes.Add(new PacGridBox { Coordinates = (2, 3), Item = BoxItem.Pellet, Blocker = Blockers.Bottom });
            GridBoxes.Add(new PacGridBox { Coordinates = (2, 4), Item = BoxItem.Pellet, Blocker = Blockers.Bottom });
            GridBoxes.Add(new PacGridBox { Coordinates = (2, 5), Item = BoxItem.Pellet, Blocker = Blockers.Bottom });
            GridBoxes.Add(new PacGridBox { Coordinates = (2, 6), Item = BoxItem.Pellet, Blocker = Blockers.BottomRightCorner });

            CurrentPlayerBox = GridBoxes.First(x => x.Item == BoxItem.Pacman);

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
                    CurrentPlayerBox.Item = BoxItem.None;
                    CurrentPlayerBox = targetBox;
                    CurrentPlayerBox.Item = BoxItem.Pacman;
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
