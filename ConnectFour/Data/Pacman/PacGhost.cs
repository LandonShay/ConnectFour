using static ConnectFour.Data.Pacman.PacGridBox;
using static ConnectFour.Pages.Pacman;

namespace ConnectFour.Data.Pacman
{
    public abstract class PacGhost
    {
        // to do: when player is powered up, each ghost attempts to flee to a designated area and can be eaten when contact is made with player
        protected MoveDir PreviousDirection { get; private set; }
        protected MoveDir MoveDirection { get; private set; }

        public PacGridBox CurrentBox = new();
        public PacEntity Entity = new();
        public float TickTime = 1;

        public bool InSpawn { get; set; } = true;
        public bool Retreating { get; set; } // after getting eaten, retreat to spawn
        public bool Recovering { get; set; } // recovery in spawn

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
                        CurrentBox.Entities.Remove(Entity);
                        CurrentBox = targetBox;
                        CurrentBox.Entities.Add(Entity);

                        ChangeDirection(direction);
                    }

                    return true;
                }
            }

            return false;
        }

        public void ChangeDirection(MoveDir dir)
        {
            PreviousDirection = MoveDirection;
            MoveDirection = dir;
        }
    }
}
