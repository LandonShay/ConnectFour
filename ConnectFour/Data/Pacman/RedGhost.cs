using static ConnectFour.Data.Pacman.PacGridBox;

namespace ConnectFour.Data.Pacman
{ // red straight up chases you
    public class RedGhost : PacGhost
    {
        public RedGhost() { TickTime = .35f; RetreatTickTime = .9f; }

        public override void Move(List<PacGridBox> gridBoxes)
        {
            if (!Retreating && !Recovering && !GoingHome)
            {
                if (CurrentBox.InGhostSpawn)
                {
                    ExitSpawn(gridBoxes);
                    return;
                }

                var pacmanBox = gridBoxes.Find(x => x.Entities.Any(x => x.Creature == Creatures.Pacman));

                if (pacmanBox != null)
                {
                    var dupeBoxes = gridBoxes.ToList();

                    var opposite = GetOppositeDirection(MoveDirection);
                    var behindBox = GetBoxInDirection(opposite, dupeBoxes);

                    var behindBoxOriginalBlocker = behindBox!.Blocker;
                    behindBox.Blocker = Blockers.Full;

                    var path = AStar.FindPath(CurrentBox, pacmanBox, gridBoxes);

                    behindBox.Blocker = behindBoxOriginalBlocker;

                    if (path.Count > 0)
                    {
                        var targetBox = path[0];
                        MoveBox(targetBox, gridBoxes);
                    }
                }
            }
            else
            {
                base.Move(gridBoxes);
            }
        }
    }
}
