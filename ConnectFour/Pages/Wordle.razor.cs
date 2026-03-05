using ConnectFour.Models.Wordle;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace ConnectFour.Pages
{
    public partial class Wordle
    {
        // todo: add a submit button (or on enter). check each letter in the target word and the current row and determine... stuff
        private List<string> TargetWord { get; set; } = new();
        private List<WordleBox> Boxes { get; set; } = new();

        private WordleBox ActiveBox { get; set; } = new();
        private ElementReference InputRef;

        #region Init
        protected override void OnInitialized()
        {
            PickWord();
            GenerateBoxes();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            FocusInput();
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

        private async void CheckWord(KeyboardEventArgs e)
        {
            var key = e.Key.ToLower();

            if (key == "backspace")
            {
                var previousBox = Boxes.FirstOrDefault(x => x.Row == ActiveBox.Row && x.Column == ActiveBox.Column - 1);

                ActiveBox.Letter = string.Empty;

                if (previousBox != null)
                {
                    ActiveBox = previousBox;
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

        private async void FocusInput()
        {
            await InputRef.FocusAsync();
        }

        private string GetBoxClass(WordleBox box)
        {
            if (box.HasLetter)
            {

            }

            return "empty";
        }
    }
}
