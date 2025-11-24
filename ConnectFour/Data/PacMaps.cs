using static ConnectFour.Data.PacGridBox;

namespace ConnectFour.Data
{
    public static class PacMap
    {
        public static List<PacGridBox> Grid = new()
        {
            // 1st row
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.TopLeftCorner },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Top },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Top },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Top },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Top },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Top },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Top },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Top },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Top },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Top },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Top },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Top },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Top },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.TopRightCorner },

            // 2nd row
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Left },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Blocker = Blockers.Full},
            new PacGridBox { Blocker = Blockers.Full},
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Right },

            // 3rd row
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Left },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Right },

            // 4th row
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Left },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Right },

            // 5th row
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Left },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet, Blocker = Blockers.Right },

            // 6th row
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Item = BoxItem.Pellet },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Blocker = Blockers.Full },
            new PacGridBox { Blocker = Blockers.Full },

            //7th row 

        };
    }
}
