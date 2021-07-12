using System.Collections.Generic;

namespace DungeonMayhem.Library
{
    public class Deck
    {
        public List<Card> CardDeck { get; set; }

        public Deck()
        {
            CardDeck = new List<Card>();
        }
    }
}