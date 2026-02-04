namespace ConnectFour.Data.Pacman
{
    public class RedGhost : PacGhost
    {
        // red ghost straight up chases you
        public RedGhost() { TickTime = .9f; }
        public override void Move(List<PacGridBox> gridBoxes)
        {
            
        }
    }
}
