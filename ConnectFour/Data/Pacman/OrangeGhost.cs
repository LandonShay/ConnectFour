using static ConnectFour.Data.Pacman.PacGridBox;
using static ConnectFour.Pages.Pacman;

namespace ConnectFour.Data.Pacman
{
    public class OrangeGhost : PacGhost
    {
        public OrangeGhost() { TickTime = 1.2f; }

        private MoveDir PreviousDirection;
        private MoveDir MoveDirection;

        public override void Move(List<PacGridBox> gridBoxes)
        {
            if (!InSpawn)
            {
                if (!Retreating && !Recovering)
                {
                    var stopBlockers = new List<Blockers>();
                    var horizontal = false;
                    var index = 1;

                    if (MoveDirection == MoveDir.Up)
                    {
                        stopBlockers = [Blockers.Top, Blockers.TopLeftCorner, Blockers.TopRightCorner];
                        index = -1;
                    }
                    else if (MoveDirection == MoveDir.Right)
                    {
                        stopBlockers = [Blockers.Right, Blockers.TopRightCorner, Blockers.BottomRightCorner];
                        horizontal = true;
                    }
                    else if (MoveDirection == MoveDir.Left)
                    {
                        stopBlockers = [Blockers.Left, Blockers.TopLeftCorner, Blockers.BottomLeftCorner];
                        horizontal = true;
                        index = -1;
                    }
                    else if (MoveDirection == MoveDir.None)
                    { // at entrance
                        var rnd = new Random();
                        var value = rnd.NextDouble();

                        MoveDirection = value > .5 ? MoveDir.Right : MoveDir.Left;
                        PreviousDirection = MoveDirection;
                        return;
                    }

                    var success = TryMoveBox(gridBoxes, stopBlockers, index, horizontal);

                    if (!success)
                    {

                        // pick new valid direction that isn't the way they just came from
                    }
                }
            }
        }
    }
}
