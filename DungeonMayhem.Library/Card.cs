using System.Collections.Generic;

namespace DungeonMayhem.Library
{
    public class Card
    {
        public string Name { get; set; }
        public List<CreatureAction> Actions { get; set; }
        public int NumShieldsRemaining { get; set; }
        public int NumTotalShields { get; set; }

        public Card()
        {
            Actions = new List<CreatureAction>();
        }
    }
}