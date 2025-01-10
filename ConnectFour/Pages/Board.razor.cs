using ConnectFour.Data;
using MoreLinq;

namespace ConnectFour.Pages
{
    public partial class Board
    {
        public List<BoardBox> Boxes = new List<BoardBox>();
        public List<BoardBox> HeaderBoxes = new List<BoardBox>();

        private const string User = "user";
        private const string CPU = "cpu";

        private const string addAction = "Add";
        private const string subtractAction = "Subtract";

        private int Wins { get; set; }
        private int Losses { get; set; }
        private int Draws { get; set; }

        public bool IsPlayerTurn { get; set; } = true;
        public bool PieceFalling { get; set; }
        public string? Winner { get; set; }

        protected override void OnInitialized()
        {
            ResetBoard();
        }

        #region Game
        public async void PlayTurn(BoardBox headerBox)
        {
            if (!PieceFalling && IsPlayerTurn && Winner == null)
            {
                await PiecePlay(headerBox, User);
                CheckWin(User);

                if (Winner == null)
                {
                    ComputerTurn();
                }
            }
        }

        private async Task PiecePlay(BoardBox headerBox, string player)
        {
            var finalPlayBox = new BoardBox();
            PieceFalling = true;

            var firstRowPlayBox = Boxes.First(x => x.Coordinate.column == headerBox.Coordinate.column);
            var secondRowPlayBox = Boxes.First(x => x.Coordinate.row == firstRowPlayBox.Coordinate.row + 1 && x.Coordinate.column == firstRowPlayBox.Coordinate.column);

            await FallAnimation(firstRowPlayBox, player);

            if (secondRowPlayBox.OccupiedBy != null)
            {
                finalPlayBox = firstRowPlayBox;
            }
            else
            {
                var box = secondRowPlayBox;

                for (int i = 0; i < 4; i++)
                {
                    await FallAnimation(box, player);

                    var nextBox = Boxes.First(x => x.Coordinate.row == box.Coordinate.row + 1 && x.Coordinate.column == box.Coordinate.column);

                    if (nextBox.OccupiedBy != null)
                    {
                        finalPlayBox = box;
                        break;
                    }
                    else
                    {
                        if (i < 3)
                        {
                            box = nextBox;
                        }
                        else
                        {
                            finalPlayBox = nextBox;
                        }
                    }
                }
            }

            finalPlayBox.OccupiedBy = player;
            StateHasChanged();
            PieceFalling = false;
        }

        private void EndGame(string player)
        {
            Winner = player;

            if (player == User)
            {
                Wins++;
            }
            else
            {
                Losses++;
            }

            IsPlayerTurn = false;
            StateHasChanged();
        }

        private void ResetBoard()
        {
            HeaderBoxes.Clear();
            Boxes.Clear();

            Winner = null;
            PieceFalling = false;
            IsPlayerTurn = true;

            var row = 1;
            var column = 1;

            for (int i = 1; i < 8; i++)
            {
                HeaderBoxes.Add(new BoardBox { Coordinate = (0, i) });
            }

            for (byte i = 1; i < 43; i++)
            {
                Boxes.Add(new BoardBox { Coordinate = (row, column) });
                column++;

                if (i % 7 == 0)
                {
                    row++;
                    column = 1;
                }
            }
        }
        #endregion

        #region NPC Turn
        private async void ComputerTurn()
        {
            if (Winner == null)
            {
                IsPlayerTurn = false;

                var chosenHeader = HeaderBoxes.Shuffle().First();

                await PiecePlay(chosenHeader, CPU);
                CheckWin(CPU);

                IsPlayerTurn = true;
            }
        }
        #endregion

        #region Check Conditions
        private void CheckWin(string player)
        {
            var directions = new (int rowOffset, int colOffset)[]
            {
                (-1, 0), (1, 0), (0, -1), (0, 1),
                (-1, -1), (-1, 1), (1, -1), (1, 1)
            };

            foreach (var box in Boxes.Where(x => x.OccupiedBy == player))
            {
                foreach (var direction in directions)
                {
                    if (CheckDirection(box.Coordinate.row, box.Coordinate.column, direction, player))
                    {
                        EndGame(player);
                        return;
                    }
                }
            }
        }

        private bool CheckDirection(int startRow, int startCol, (int rowOffset, int colOffset) direction, string player)
        {
            for (int i = 1; i <= 3; i++)
            {
                var newRow = startRow + direction.rowOffset * i;
                var newCol = startCol + direction.colOffset * i;

                if (!IsValidCoordinate(newRow, newCol) || Boxes.FirstOrDefault(b => b.Coordinate == (newRow, newCol))?.OccupiedBy != player)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Misc
        private async Task FallAnimation(BoardBox box, string player)
        {
            var occupiedBy = player == User ? User : CPU;

            box.OccupiedBy = occupiedBy;
            StateHasChanged();
            await Task.Delay(150);
            box.OccupiedBy = null;
            StateHasChanged();
        }

        private bool IsValidCoordinate(int row, int col)
        {
            return row > 0 && row < 7 && col > 0 && col < 8;
        }
        #endregion
    }
}
