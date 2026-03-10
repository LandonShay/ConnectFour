namespace ConnectFour.Models.WordSearch
{
    public class WordSearchBox
    {
        public WordSearchWord Word { get; set; } = new();
        public string Letter { get; set; } = string.Empty;
        public bool Checked { get; set; }
        public bool WordFound { get; set; }
        public (int x, int y) Coor { get; set; }
    }
}
