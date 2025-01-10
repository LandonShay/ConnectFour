namespace ConnectFour.Data
{
    public class BoardBox
    {
        public (int row, int column) Coordinate { get; set; }
        public string? OccupiedBy { get; set; }
    }
}
