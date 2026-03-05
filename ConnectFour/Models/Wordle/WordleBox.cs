namespace ConnectFour.Models.Wordle
{
    public class WordleBox
    {
        public string Letter { get; set; } = string.Empty;
        public WordleIndicator Status { get; set; }

        public byte Row { get; set; }
        public byte Column { get; set; }

        public bool HasLetter => !Letter.IsWhiteSpace();
    }

    public enum WordleIndicator
    {
        None,
        WrongSpot,
        Correct
    }
}
