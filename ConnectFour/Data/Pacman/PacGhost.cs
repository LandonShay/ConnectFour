using MoreLinq;
using static ConnectFour.Data.Pacman.PacGridBox;
using static ConnectFour.Pages.Pacman;

namespace ConnectFour.Data.Pacman
{
    public abstract class PacGhost
    {
        // to do: when player is powered up, each ghost attempts to flee to a designated area and can be eaten when contact is made with player
        protected MoveDir PreviousDirection { get; private set; }
        protected MoveDir MoveDirection { get; private set; }
        protected readonly List<MoveDir> Directions = new() { MoveDir.Up, MoveDir.Down, MoveDir.Left, MoveDir.Right };

        public PacGridBox CurrentBox = new();
        public PacEntity Entity = new();
        public float TickTime = 1;
        public float RetreatTickTime = 2;

        public bool InSpawn { get; set; } = true;
        public bool Retreating { get; set; } // after getting eaten, retreat to spawn
        public bool Recovering { get; set; } // recovery in spawn

        protected PacGridBox? RetreatDestination { get; set; }

        protected float recoverElapsed = 0;
        protected readonly float _recoveryTime = 2.5f;

        public virtual void Move(List<PacGridBox> gridBoxes) { }

        public virtual bool TryMoveBox(MoveDir direction, List<PacGridBox> gridBoxes, bool actuallyMove = true)
        {
            var blockers = new List<Blockers>();
            var horizontal = false;
            var index = 1;

            if (direction == MoveDir.Up)
            {
                blockers = [Blockers.Top, Blockers.TopLeftCorner, Blockers.TopRightCorner];
                index = -1;
            }
            else if (direction == MoveDir.Right)
            {
                blockers = [Blockers.Right, Blockers.TopRightCorner, Blockers.BottomRightCorner];
                horizontal = true;
            }
            else if (direction == MoveDir.Left)
            {
                blockers = [Blockers.Left, Blockers.TopLeftCorner, Blockers.BottomLeftCorner];
                horizontal = true;
                index = -1;
            }
            else
            {
                // Down is the default so change nothing
            }

            if (!blockers.Contains(CurrentBox.Blocker))
            {
                PacGridBox? targetBox = null;

                if (horizontal)
                {
                    targetBox = gridBoxes.Find(x => x.Coordinates.x == CurrentBox.Coordinates.x + index &&
                                                    x.Coordinates.y == CurrentBox.Coordinates.y);
                }
                else
                {
                    targetBox = gridBoxes.Find(x => x.Coordinates.x == CurrentBox.Coordinates.x &&
                                                    x.Coordinates.y == CurrentBox.Coordinates.y + index);
                }

                if ((targetBox != null && targetBox.Blocker != Blockers.Full) || (targetBox == null && CurrentBox.Teleport))
                {
                    if (targetBox == null)
                    {
                        targetBox = gridBoxes.First(x => x.Teleport && x != CurrentBox);
                    }

                    if (actuallyMove)
                    {
                        MoveBox(targetBox, gridBoxes);
                        ChangeDirection(direction);
                    }

                    return true;
                }
            }

            return false;
        }

        public void MoveBox(PacGridBox targetBox, List<PacGridBox> gridBoxes)
        {
            var currentNeighbors = AStar.GetNeighbors(CurrentBox, gridBoxes);

            if (currentNeighbors.Contains(targetBox))
            {
                foreach (var direction in Directions)
                {
                    var dirBox = GetBoxInDirection(direction, gridBoxes);

                    if (dirBox == targetBox)
                    {
                        MoveDirection = direction;
                    }
                }
            }
            else
            {
                MoveDirection = MoveDir.None;
            }

            CurrentBox.Entities.Remove(Entity);
            CurrentBox = targetBox;
            CurrentBox.Entities.Add(Entity);
        }

        public void ChangeDirection(MoveDir dir)
        {
            PreviousDirection = MoveDirection;
            MoveDirection = dir;
        }

        public void Retreat(List<PacGridBox> gridBoxes)
        {
            if (RetreatDestination == null)
            {
                GetRetreatDestination(gridBoxes);
            }

            var path = AStar.FindPath(CurrentBox, RetreatDestination!, gridBoxes);

            if (path.Count > 0)
            {
                var targetBox = path[0];
                MoveBox(targetBox, gridBoxes);

                if (CurrentBox == RetreatDestination)
                {
                    RetreatDestination = null;
                }
            }
        }

        public void GetRetreatDestination(List<PacGridBox> gridBoxes)
        {
            var pacman = gridBoxes.Find(x => x.Entities.Any(x => x.Creature == Creatures.Pacman));

            if (pacman != null)
            {
                var validBoxes = new List<PacGridBox>();

                if (pacman.Coordinates.x <= 8 && pacman.Coordinates.y <= 10)
                { // 1st quadrant
                    validBoxes = gridBoxes.FindAll(x => x.Coordinates.x > 8 && x.Coordinates.y > 10 && x.Blocker != Blockers.Full);
                }
                else if (pacman.Coordinates.x > 8 && pacman.Coordinates.y <= 10)
                { // 2nd quadrant
                    validBoxes = gridBoxes.FindAll(x => x.Coordinates.x <= 8 && x.Coordinates.y > 10 && x.Blocker != Blockers.Full);
                }
                else if (pacman.Coordinates.x <= 8 && pacman.Coordinates.y > 10)
                { // 3rd quadrant
                    validBoxes = gridBoxes.FindAll(x => x.Coordinates.x > 8 && x.Coordinates.y <= 10 && x.Blocker != Blockers.Full);
                }
                else
                { // 4th quadrant
                    validBoxes = gridBoxes.FindAll(x => x.Coordinates.x <= 8 && x.Coordinates.y <= 10 && x.Blocker != Blockers.Full);
                }

                RetreatDestination = validBoxes.Shuffle().First();
            }
        }

        public void EndRetreat()
        {
            RetreatDestination = null;
            Retreating = false;
        }

        #region Helper
        protected PacGridBox? GetBoxInDirection(MoveDir direction, List<PacGridBox> gridBoxes)
        {
            var horizontal = false;
            var index = 1;

            if (direction == MoveDir.Up)
            {
                index = -1;
            }
            else if (direction == MoveDir.Right)
            {
                horizontal = true;
            }
            else if (direction == MoveDir.Left)
            {
                horizontal = true;
                index = -1;
            }
            else
            {
                // Down is the default so change nothing
            }

            PacGridBox? targetBox = null;

            if (horizontal)
            {
                targetBox = gridBoxes.Find(x => x.Coordinates.x == CurrentBox.Coordinates.x + index &&
                                                x.Coordinates.y == CurrentBox.Coordinates.y);
            }
            else
            {
                targetBox = gridBoxes.Find(x => x.Coordinates.x == CurrentBox.Coordinates.x &&
                                                x.Coordinates.y == CurrentBox.Coordinates.y + index);
            }

            return targetBox;
        }

        protected MoveDir GetOppositeDirection(MoveDir dir)
        {
            return dir switch
            {
                MoveDir.Up => MoveDir.Down,
                MoveDir.Down => MoveDir.Up,
                MoveDir.Left => MoveDir.Right,
                _ => MoveDir.Left
            };
        }
        #endregion
    }
}
