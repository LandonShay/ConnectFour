namespace ConnectFour.Models.WordSearch
{
    public class WordSearchWord
    {
        public string Word { get; set; } = string.Empty;
        public bool Found { get; set; }
        public WordOrientation Orientation { get; set; }
    }

    public enum WordOrientation
    {
        Horizontal,
        Vertical,
        DiagonalUp,
        DiagonalDown
    }
}
