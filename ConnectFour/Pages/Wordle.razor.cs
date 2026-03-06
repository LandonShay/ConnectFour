using ConnectFour.Models.Wordle;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace ConnectFour.Pages
{
    public partial class Wordle
    {
        private List<string> TargetWord { get; set; } = new();
        private List<WordleBox> Boxes { get; set; } = new();

        private WordleGameStatus GameStatus { get; set; } = WordleGameStatus.None;

        private WordleBox ActiveBox { get; set; } = new();
        private ElementReference InputRef;

        private enum WordleGameStatus
        {
            None,
            Win,
            Loss
        }

        #region Init
        protected override void OnInitialized()
        {
            RestartGame();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            FocusInput();
        }

        private void RestartGame()
        {
            Boxes.Clear();
            TargetWord.Clear();
            GameStatus = WordleGameStatus.None;

            PickWord();
            GenerateBoxes();
        }

        private void PickWord()
        {
            var wordFilePath = "Data\\wordle-words.json";
            var wordsString = File.ReadAllText(wordFilePath);

            var words = JsonSerializer.Deserialize<List<WordleWord>>(wordsString);

            if (words == null)
            {
                return;
            }

            var validWords = words.FindAll(x => x.Type == "target");
            var targetWord = validWords.Shuffle().First().Word;

            foreach (var letter in targetWord)
            {
                TargetWord.Add(letter.ToString());
            }
        }

        private void GenerateBoxes()
        {
            byte row = 1;
            byte column = 1;

            for (int i = 1; i <= 25; i++)
            {
                var box = new WordleBox { Row = row, Column = column };
                Boxes.Add(box);

                column++;

                if (column == 6)
                {
                    column = 1;
                }

                if (i % 5 == 0)
                {
                    row++;
                }
            }

            ActiveBox = Boxes.First();
        }
        #endregion

        private void SubmitWord()
        {
            var boxes = Boxes.FindAll(x => x.Row == ActiveBox.Row).OrderBy(x => x.Column).ToList();

            if (boxes.Any(x => x.Letter == string.Empty))
            {
                return;
            }
            
            for (int i = 0; i < 5; i++)
            {
                var targetLetter = TargetWord[i];
                var box = boxes[i];

                if (box.Letter.ToLower() == targetLetter.ToLower())
                {
                    box.Status = WordleIndicator.Correct;
                }
                else
                {
                    if (TargetWord.Any(x => x.ToLower() == box.Letter.ToLower()))
                    {
                        box.Status = WordleIndicator.WrongSpot;
                    }
                    else
                    {
                        box.Status = WordleIndicator.Wrong;
                    }
                }
            }

            if (Boxes.All(x => x.Status == WordleIndicator.Correct))
            {
                GameStatus = WordleGameStatus.Win;
            }
            else if (ActiveBox.Row < 5)
            {
                ActiveBox = Boxes.First(x => x.Row == ActiveBox.Row + 1 && x.Column == 1);
                FocusInput();
            }
            else
            {
                GameStatus = WordleGameStatus.Loss;
            }
        }

        private async void CheckWord(KeyboardEventArgs e)
        {
            var key = e.Key.ToLower();

            if (key == "backspace")
            {
                if (ActiveBox.Letter != string.Empty)
                {
                    ActiveBox.Letter = string.Empty;
                }
                else
                {
                    var previousBox = Boxes.FirstOrDefault(x => x.Row == ActiveBox.Row && x.Column == ActiveBox.Column - 1);

                    if (previousBox != null)
                    {
                        ActiveBox = previousBox;
                        ActiveBox.Letter = string.Empty;
                    }
                }
            }
            else if (Regex.IsMatch(key, "^[a-z]$")) // if a single lowercase letter
            {
                if (ActiveBox.Letter != string.Empty && ActiveBox.Column == 5)
                {
                    return;
                }

                ActiveBox.Letter = key;

                if (ActiveBox.Column < 5)
                {
                    ActiveBox = Boxes.First(x => x.Row == ActiveBox.Row && x.Column == ActiveBox.Column + 1);
                }
            }
        }

        private string BuildTargetWord()
        {
            var word = string.Empty;

            foreach (var letter in TargetWord)
            {
                word += letter;
            }

            return word;
        }

        private async void FocusInput()
        {
            await InputRef.FocusAsync();
        }

        private string GetBoxClass(WordleBox box)
        {
            return box.Status switch 
            {
                WordleIndicator.WrongSpot => "almost-box",
                WordleIndicator.Correct => "correct-box",
                WordleIndicator.Wrong => "incorrect-box",
                _ => string.Empty
            };
        }
    }
}
