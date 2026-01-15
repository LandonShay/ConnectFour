using static ConnectFour.Data.Pacman.PacGridBox;

namespace ConnectFour.Data.Pacman
{
    public abstract class PacGhost
    {
        public PacGridBox CurrentBox = new();
        public PacEntity Entity = new();

        public bool InSpawn { get; set; } = true;
        public bool Retreating { get; set; } // after getting eaten, retreat to spawn
        public bool Recovering { get; set; } // recovery in spawn

        public virtual void Move(List<PacGridBox> gridBoxes) { }

        public virtual bool TryMoveBox(List<PacGridBox> gridBoxes, List<Blockers> blockers, int moveIndex, bool horizontal)
        {
            if (!blockers.Contains(CurrentBox.Blocker))
            {
                PacGridBox? targetBox = null;

                if (horizontal)
                {
                    targetBox = gridBoxes.Find(x => x.Coordinates.x == CurrentBox.Coordinates.x + moveIndex &&
                                                    x.Coordinates.y == CurrentBox.Coordinates.y);
                }
                else
                {
                    targetBox = gridBoxes.Find(x => x.Coordinates.x == CurrentBox.Coordinates.x &&
                                                    x.Coordinates.y == CurrentBox.Coordinates.y + moveIndex);
                }

                if (targetBox != null && targetBox.Blocker != Blockers.Full)
                {
                    CurrentBox.Entities.Remove(Entity);
                    CurrentBox = targetBox;
                    CurrentBox.Entities.Add(Entity);

                    return true;
                }
            }

            return false;
        }
    }
}
