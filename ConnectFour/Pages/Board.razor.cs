using ConnectFour.Data;
using MoreLinq;

namespace ConnectFour.Pages
{
    public partial class Board
    {
        public List<BoardBox> Boxes = new List<BoardBox>();
        public List<BoardBox> HeaderBoxes = new List<BoardBox>();
        public List<WinConditions> WinConditions = new List<WinConditions>();

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
            WinConditions.Add(new WinConditions { SpacesAway = 1, Direction = addAction });
            WinConditions.Add(new WinConditions { SpacesAway = 6, Direction = addAction });
            WinConditions.Add(new WinConditions { SpacesAway = 7, Direction = addAction });
            WinConditions.Add(new WinConditions { SpacesAway = 8, Direction = addAction });
            WinConditions.Add(new WinConditions { SpacesAway = 1, Direction = subtractAction });
            WinConditions.Add(new WinConditions { SpacesAway = 6, Direction = subtractAction });
            WinConditions.Add(new WinConditions { SpacesAway = 7, Direction = subtractAction });
            WinConditions.Add(new WinConditions { SpacesAway = 8, Direction = subtractAction });

            ResetBoard();
        }

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

        private async Task PiecePlay(BoardBox headerBox, string player)
        {
            var finalPlayBox = new BoardBox();
            PieceFalling = true;

            var firstRowPlayBox = Boxes.First(x => x.Index == headerBox.Index);
            var secondRowPlayBox = Boxes.First(x => x.Index == firstRowPlayBox.Index + 7);

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

                    var nextBox = Boxes.First(x => x.Index == box.Index + 7);

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

        private void CheckWin(string player)
        {
            var success = false;

            foreach (var box in Boxes.Where(x => x.OccupiedBy == player))
            {
                foreach (var condition in WinConditions)
                {
                    success = CheckDirections(box, player, condition.SpacesAway, condition.Direction);

                    if (success)
                    {
                        EndGame(player);
                        break;
                    }
                }

                if (success)
                {
                    break;
                }
            }
        }

        private bool CheckDirections(BoardBox box, string player, int checkAmount, string checkDirection)
        {
            var success = false;
            checkAmount = checkDirection == addAction ? checkAmount : checkAmount * -1;

            if (box.OccupiedBy != null && box.OccupiedBy == player)
            {
                for (int i = 0; i < 3; i++)
                {
                    var directionBox = Boxes.FirstOrDefault(x => x.Index == box.Index + checkAmount);

                    if (directionBox != null && directionBox.OccupiedBy == player && i < 2)
                    {
                        box = directionBox;
                    }
                    else if (i == 2 && directionBox != null && directionBox.OccupiedBy == player)
                    {
                        success = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return success;
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

            for (int i = 1; i < 8; i++)
            {
                HeaderBoxes.Add(new BoardBox { Index = i });
            }

            for (byte i = 1; i < 43; i++)
            {
                Boxes.Add(new BoardBox { Index = i });
            }
        }

        private async Task FallAnimation(BoardBox box, string player)
        {
            var occupiedBy = player == User ? User : CPU;

            box.OccupiedBy = occupiedBy;
            StateHasChanged();
            await Task.Delay(150);
            box.OccupiedBy = null;
            StateHasChanged();
        }
    }
}
