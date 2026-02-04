namespace ConnectFour.Data.Pacman
{
    public class BlueGhost : PacGhost
    {
        // chases you but when within 8 boxes of player, attempts to go to the lower left corner
        public BlueGhost() { TickTime = 1.1f; }
    }
}
