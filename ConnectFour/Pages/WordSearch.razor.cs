using ConnectFour.Models.WordSearch;

namespace ConnectFour.Pages
{
    public partial class WordSearch
    {
        // pick words from a dictionary
        // determine how each word will be generated (horizontal, vertical, diagonal up, diagonal down)
        // generate an empty grid, place the words on the grid with their orientation, ensuring no invalid placements
        // fill in the remaining empty spots with random letters

        // each letter will know if it's part of a word or not and its index in the word
        // click each letter individually in sequence. if it's part of the word and the click sequence is correct, it is valid. otherwise it will empty your selection

        private DifficultyLevel Difficulty { get; set; }
        private List<WordSearchBox> Boxes { get; set; } = new();
        private List<WordSearchWord> Words { get; set; } = new();
        private WordSearchGameStatus GameStatus { get; set; } = WordSearchGameStatus.ChooseDifficulty;
        private List<WordOrientation> Orientations { get; set; } = [WordOrientation.Horizontal, WordOrientation.Vertical, WordOrientation.DiagonalUp, WordOrientation.DiagonalDown];

        protected override void OnInitialized()
        {

        }

        private void SelectDifficulty(DifficultyLevel difficulty)
        {
            var filePath = "Data\\word-search-words.txt";
            var words = File.ReadAllLines(filePath).ToList();

            if (difficulty == DifficultyLevel.Easy)
            {
                PickWordsByLength(words, 2, 5);
                PickWordsByLength(words, 2, 4);
                PickWordsByLength(words, 1, 3);
                GenerateGrid(15);
            }

        }

        private void GenerateGrid(int gridSize)
        {
            var row = 1;
            var column = 1;

            for (int i = 0; i < gridSize * 2; i++)
            {
                var box = new WordSearchBox { Coor = (row, column) };
                Boxes.Add(box);

                column++;

                if (column == gridSize + 1)
                {
                    column = 1;
                    row++;
                }
            }

            try
            {
                foreach (var word in Words)
                {
                    // pick a random open box on the grid
                    // using the orientation, attempt to place each letter in the orientation
                    // if the entire word is placed without issue, continue
                    // if a letter is attempted to be placed in an invalid spot, undo placement and start over. keep a record of the boxes tried and do not try the same box twice
                    // attempt this up to the maximum number of open grid spaces. if all fail, change the word's orientation and try again, never resuing an already used orientation
                    // if all orientations fail, throw error

                    var orientationsAttempted = new List<WordOrientation>();
                    var attemptedBoxes = new List<WordSearchBox>();

                    while (true)
                    {
                        var startBox = Boxes.Where(x => !attemptedBoxes.Contains(x) && x.Letter == string.Empty).Shuffle().FirstOrDefault();

                        if (startBox != null)
                        {
                            attemptedBoxes.Add(startBox);

                            var nextBox = startBox;
                            var wordBoxes = new List<WordSearchBox>();

                            foreach (var letter in word.Word)
                            {
                                if (nextBox.Letter != string.Empty)
                                {
                                    nextBox = null;
                                }
                                else
                                {
                                    nextBox.Word = word;
                                    nextBox.Letter = letter.ToString();

                                    wordBoxes.Add(nextBox);

                                    nextBox = word.Orientation switch
                                    {
                                        WordOrientation.Vertical => Boxes.FirstOrDefault(x => x.Coor.x == nextBox.Coor.x + 1 && x.Coor.y == nextBox.Coor.y),
                                        WordOrientation.DiagonalUp => Boxes.FirstOrDefault(x => x.Coor.x == nextBox.Coor.x + 1 && x.Coor.y == nextBox.Coor.y - 1),
                                        WordOrientation.DiagonalDown => Boxes.FirstOrDefault(x => x.Coor.x == nextBox.Coor.x + 1 && x.Coor.y == nextBox.Coor.y + 1),
                                        _ => Boxes.FirstOrDefault(x => x.Coor.x == nextBox.Coor.x && x.Coor.y == nextBox.Coor.y + 1), // horizontal
                                    };
                                }

                                if (nextBox == null)
                                {
                                    foreach (var box in wordBoxes)
                                    {
                                        box.Word = new();
                                        box.Letter = string.Empty;
                                    }

                                    break;
                                }
                            }

                            break;
                        }
                        else
                        {
                            attemptedBoxes.Clear();
                            orientationsAttempted.Add(word.Orientation);

                            var unusedOrientations = Orientations.FindAll(x => !orientationsAttempted.Contains(x));

                            if (unusedOrientations.Count > 0)
                            {
                                word.Orientation = unusedOrientations.Shuffle().First();
                            }
                            else
                            {
                                throw new Exception("No possible way to place the chosen words on the grid");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void ResetGame()
        {
            Words.Clear();
            GameStatus = WordSearchGameStatus.ChooseDifficulty;
        }

        private void PickWordsByLength(List<string> words, byte wordCount, byte wordLength)
        {
            for (int i = 0; i < wordCount; i++)
            {
                var word = new WordSearchWord
                {
                    Word = words.Where(x => x.Length == wordLength).Shuffle().First(),
                    Orientation = Orientations.Shuffle().First()
                };

                Words.Add(word);
            }
        }

        private string GetGridSizeCss()
        {
            return Difficulty switch
            {
                DifficultyLevel.Easy => "easy-grid",
                DifficultyLevel.Medium => "medium-grid",
                DifficultyLevel.Hard => "hard-grid",
                DifficultyLevel.VeryHard => "very-hard-grid",
                _ => string.Empty
            };
        }

        #region Enums
        private enum WordSearchGameStatus
        {
            ChooseDifficulty,
            Ongoing,
            Win
        }

        private enum DifficultyLevel
        {
            Easy,
            Medium,
            Hard,
            VeryHard
        }
        #endregion
    }
}
