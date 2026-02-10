using static ConnectFour.Data.Pacman.PacGridBox;

namespace ConnectFour.Data.Pacman
{
    public class PacEntity
    {
        public Creatures Creature { get; set; }
#pragma warning disable CS8618
        public PacGhost Ghost { get; set; }
#pragma warning restore CS8618

        public void ConfigureGhost(List<PacGridBox> gridBoxes, Creatures creature)
        {
            var ghostBox = gridBoxes.First(x => x.Entities.Any(x => x.Creature == creature));
            Ghost = ghostBox.Entities.First().Ghost;
            Ghost.CurrentBox = ghostBox;
            Ghost.StartBox = ghostBox;
            Ghost.Entity = this;
            Creature = creature;
        }
    }
}
