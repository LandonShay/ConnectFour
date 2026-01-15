namespace ConnectFour.Data
{
    public class PacGridBox
    {
        public Blockers Blocker { get; set; } = Blockers.None;
        public BoxItem Item { get; set; } = BoxItem.None;
        public List<Creatures> Entities { get; set; } = new();
        public (int x, int y) Coordinates { get; set; }

        public enum BoxItem
        {
            None,
            Pellet,
            PowerPellet
        }

        public enum Creatures
        {
            Pacman,
            RedGhost,
            OrangeGhost,
            BlueGhost,
            PinkGhost
        }

        public enum Blockers
        {
            None,
            TopLeftCorner,
            TopRightCorner,
            BottomLeftCorner,
            BottomRightCorner,
            Top,
            Left,
            Right,
            Bottom,
            Full,
        }
    }
}
