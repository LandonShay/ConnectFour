using static ConnectFour.Data.Pacman.PacGridBox;
using static ConnectFour.Pages.Pacman;

namespace ConnectFour.Data.Pacman
{ // pink ghost tries to move towards the box 2 boxes ahead of your position.
    public class PinkGhost : PacGhost
    {
        public PinkGhost() { TickTime = .4f; }

        public override void Move(List<PacGridBox> gridBoxes, MoveDir playerMoveDir)
        {
            if (!Retreating && !Recovering && !GoingHome)
            {
                if (CurrentBox.InGhostSpawn)
                {
                    ExitSpawn(gridBoxes);
                    return;
                }

                var pacmanBox = gridBoxes.Find(x => x.Entities.Any(e => e.Creature == Creatures.Pacman));

                if (pacmanBox != null)
                {
                    var dupeBoxes = gridBoxes.ToList();
                    var targetBox = GetBoxNTilesInDirection(pacmanBox, playerMoveDir, 2, dupeBoxes);

                    if (targetBox == null || targetBox.Blocker == Blockers.Full)
                    {
                        targetBox = pacmanBox;
                    }

                    var opposite = GetOppositeDirection(MoveDirection);
                    var behindBox = GetBoxInDirection(opposite, dupeBoxes);

                    var behindBoxOriginalBlocker = behindBox!.Blocker;
                    behindBox.Blocker = Blockers.Full;

                    var path = AStar.FindPath(CurrentBox, targetBox, gridBoxes);
                    behindBox.Blocker = behindBoxOriginalBlocker;

                    if (path.Count > 0)
                    {
                        MoveBox(path[0], gridBoxes);
                    }
                }
            }
            else
            {
                base.Move(gridBoxes, playerMoveDir);
            }
        }

    }
}
