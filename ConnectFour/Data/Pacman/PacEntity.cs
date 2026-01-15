using static ConnectFour.Data.Pacman.PacGridBox;

namespace ConnectFour.Data.Pacman
{
    public class PacEntity
    {
        public Creatures Creature { get; set; }
        public PacGhost Ghost { get; set; }
    }
}
