using static ConnectFour.Pages.Pacman;
using MoreLinq;

namespace ConnectFour.Data.Pacman
{ // orange moves randomly and can change directions at intersections
    public class OrangeGhost : PacGhost
    {
        public OrangeGhost() { TickTime = .9f; }

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

                        var newDirection = value > .5 ? MoveDir.Right : MoveDir.Left;
                        ChangeDirection(newDirection);

                        return;
                    }

                    HandleIntersection(gridBoxes);

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
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void HandleIntersection(List<PacGridBox> gridBoxes)
        {
            var validDirections = new List<MoveDir>();
            var directions = new List<MoveDir>() { MoveDir.Up, MoveDir.Left, MoveDir.Right, MoveDir.Down };

            if (CurrentBox.IsEntrance)
            {
                return;
            }
            
            var opposite = GetOppositeDirection(MoveDirection);

            foreach (var dir in directions)
            {
                if (dir == opposite)
                {
                    continue;
                }

                var canMove = TryMoveBox(dir, gridBoxes, false);

                if (canMove)
                {
                    validDirections.Add(dir);
                }
            }

            if (validDirections.Count >= 2) // if it is an intersection
            {
                var rnd = new Random();

                if (rnd.NextDouble() >= .5f) // 50% chance of changing direction when at an intersection
                {
                    var newDirection = validDirections.Shuffle().First();
                    ChangeDirection(newDirection);
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
