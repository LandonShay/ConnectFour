using static ConnectFour.Data.Pacman.PacGridBox;
using static ConnectFour.Pages.Pacman;

namespace ConnectFour.Data.Pacman
{
    public class BlueGhost : PacGhost
    {
        public BlueGhost() { TickTime = .5f; }

        public override void Move(List<PacGridBox> gridBoxes, MoveDir playerMoveDir)
        {
            if (!Retreating && !Recovering && !GoingHome)
            {
                if (CurrentBox.InGhostSpawn)
                {
                    ExitSpawn(gridBoxes);
                    return;
                }

                var dupeBoxes = gridBoxes.ToList();

                var opposite = GetOppositeDirection(MoveDirection);
                var behindBox = GetBoxInDirection(opposite, dupeBoxes);

                var behindBoxOriginalBlocker = behindBox!.Blocker;
                behindBox.Blocker = Blockers.Full;

                var pacmanBox = dupeBoxes.Find(x => x.Entities.Any(e => e.Creature == Creatures.Pacman));
                var redGhostBox = dupeBoxes.Find(x => x.Entities.Any(e => e.Creature == Creatures.RedGhost));

                if (pacmanBox != null && redGhostBox != null)
                {
                    var twoAhead = GetBoxNTilesInDirection(pacmanBox, playerMoveDir, 2, dupeBoxes) ?? pacmanBox;

                    var xCor = twoAhead.Coordinates.x + (twoAhead.Coordinates.x - redGhostBox.Coordinates.x);
                    var yCor = twoAhead.Coordinates.y + (twoAhead.Coordinates.y - redGhostBox.Coordinates.y);

                    var targetBox = dupeBoxes.Find(x => x.Coordinates.x == xCor && x.Coordinates.y == yCor && x.Blocker != Blockers.Full) ?? twoAhead;
                    var path = AStar.FindPath(CurrentBox, targetBox, dupeBoxes);

                    if (path.Count > 0)
                    {
                        MoveBox(path[0], dupeBoxes);
                    }
                }

                behindBox.Blocker = behindBoxOriginalBlocker;
            }
            else
            {
                base.Move(gridBoxes, playerMoveDir);
            }
        }
    }
}
