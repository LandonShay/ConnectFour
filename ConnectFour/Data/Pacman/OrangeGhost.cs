using static ConnectFour.Pages.Pacman;
using MoreLinq;

namespace ConnectFour.Data.Pacman
{
    public class OrangeGhost : PacGhost
    { // to do: orange should have a chance of changing direction when reaching an intersection
        public OrangeGhost() { TickTime = 1.2f; }

        public override void Move(List<PacGridBox> gridBoxes)
        {
            if (!InSpawn)
            {
                if (!Retreating && !Recovering)
                {
                    if (MoveDirection == MoveDir.None)
                    { // at entrance
                        var rnd = new Random();
                        var value = rnd.NextDouble();

                        MoveDirection = value > .5 ? MoveDir.Right : MoveDir.Left;
                        PreviousDirection = MoveDirection;
                        return;
                    }

                    var success = TryMoveBox(MoveDirection, gridBoxes);

                    if (!success)
                    { // pick new valid direction that isn't the way they just came from
                        var directions = new List<MoveDir> { MoveDir.Up, MoveDir.Down, MoveDir.Left, MoveDir.Right };
                        var oppositeDirection = GetOppositeDirection(PreviousDirection);

                        foreach (var direction in directions.Shuffle())
                        {
                            if (direction != PreviousDirection && direction != oppositeDirection)
                            {
                                var success2 = TryMoveBox(direction, gridBoxes);

                                if (success2)
                                {
                                    PreviousDirection = direction;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private MoveDir GetOppositeDirection(MoveDir dir)
        {
            return dir switch
            {
                MoveDir.Up => MoveDir.Down,
                MoveDir.Down => MoveDir.Up,
                MoveDir.Left => MoveDir.Right,
                _ => MoveDir.Left
            };
        }
    }
}
