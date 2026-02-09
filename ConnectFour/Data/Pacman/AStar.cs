using static ConnectFour.Data.Pacman.PacGridBox;
using static ConnectFour.Pages.Pacman;

namespace ConnectFour.Data.Pacman
{
    public static class AStar
    {
        private static int GetHeuristic(PacGridBox a, PacGridBox b)
        {
            return Math.Abs(a.Coordinates.x - b.Coordinates.x) + Math.Abs(a.Coordinates.y - b.Coordinates.y);
        }

        private static List<PacGridBox> GetNeighbors(PacGridBox box, List<PacGridBox> allBoxes)
        {
            var directions = new List<MoveDir> { MoveDir.Up, MoveDir.Down, MoveDir.Left, MoveDir.Right };
            var neighbors = new List<PacGridBox>();

            allBoxes = allBoxes.ToList();

            foreach (var direction in directions)
            {
                PacGridBox? targetBox = null;
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

                if (horizontal)
                {
                    targetBox = allBoxes.Find(x => x.Coordinates.x == box.Coordinates.x + index &&
                                                   x.Coordinates.y == box.Coordinates.y);
                }
                else
                {
                    targetBox = allBoxes.Find(x => x.Coordinates.x == box.Coordinates.x &&
                                                   x.Coordinates.y == box.Coordinates.y + index);
                }

                if (targetBox != null)
                {
                    neighbors.Add(targetBox);
                }
            }

            return neighbors;
        }

        public static List<PacGridBox> FindPath(PacGridBox startBox, PacGridBox targetBox, List<PacGridBox> allBoxes)
        {
            var openSet = new List<PacGridBox>() { startBox };
            var closedSet = new HashSet<PacGridBox>();

            while (openSet.Count > 0)
            {
                var currentBox = openSet[0];

                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentBox.FCost ||
                        openSet[i].FCost == currentBox.FCost &&
                        openSet[i].HCost < currentBox.HCost)
                    {
                        currentBox = openSet[i];
                    }
                }

                openSet.Remove(currentBox);
                closedSet.Add(currentBox);

                if (currentBox == targetBox)
                {
                    return RetracePath(startBox, targetBox);
                }

                foreach (var neighbor in GetNeighbors(currentBox, allBoxes))
                {
                    if (neighbor.Blocker == Blockers.Full || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newCostToNeighbor = currentBox.GCost + 1;

                    if (newCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newCostToNeighbor;
                        neighbor.HCost = GetHeuristic(neighbor, targetBox);
                        neighbor.Parent = currentBox;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            return new();
        }

        private static List<PacGridBox> RetracePath(PacGridBox startBox, PacGridBox endBox)
        {
            var path = new List<PacGridBox>();
            var currentBox = endBox;

            while (currentBox != startBox && currentBox != null)
            {
                path.Add(currentBox);
                currentBox = currentBox.Parent;
            }

            path.Reverse();
            return path;
        }
    }
}
