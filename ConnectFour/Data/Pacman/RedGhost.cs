using static ConnectFour.Data.Pacman.PacGridBox;

namespace ConnectFour.Data.Pacman
{ // red straight up chases you
    public class RedGhost : PacGhost
    {
        public RedGhost() { TickTime = .35f; }

        public override void Move(List<PacGridBox> gridBoxes)
        {
            var pacmanBox = gridBoxes.Find(x => x.Entities.Any(x => x.Creature == Creatures.Pacman));

            if (pacmanBox != null)
            {
                var path = AStar.FindPath(CurrentBox, pacmanBox, gridBoxes);

                if (path.Count > 0)
                {
                    var targetBox = path[0];
                    MoveBox(targetBox);
                }
            }
        }
    }
}
