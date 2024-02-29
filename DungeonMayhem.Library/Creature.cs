using System.IO;
using System.Linq;
using System.Text.Json;

namespace DungeonMayhem.Library
{
    public class Creature
    {
        public int CreatureId { get; set; }
        public string CreatureName { get; set; }
        public string PlayerName { get; set; }
        public bool IsHuman { get; set; }
        public int MaxHitPoints { get; set; }
        public int CurrentHitPoints { get; set; }
        public Deck DiscardDeck { get; set; }
        public Deck InHandDeck { get; set; }
        public Deck DrawDeck { get; set; }
        public Deck ShieldDeck { get; set; }
        public Deck InPlayDeck { get; set; }
        public int NumberOfShields { get; set; }
        public ShapeshiftForm ShapeShiftForm { get; set; }
        public int PlayerNumber { get; set; }

        public Creature(int creatureId, string name, string jsonFile, bool isHuman = false)
        {
            CreatureId = creatureId;
            CreatureName = name;
            CurrentHitPoints = 10;
            DiscardDeck = new Deck();
            DrawDeck = JsonSerializer.Deserialize<Deck>(File.ReadAllText($"Creatures\\{jsonFile}"));
            InHandDeck = new Deck();
            IsHuman = isHuman;
            MaxHitPoints = 10;
            NumberOfShields = 0;
            PlayerName = string.Empty;
            PlayerNumber = 0;
            InPlayDeck = new Deck();
            ShapeShiftForm = ShapeshiftForm.None;
            ShieldDeck = new Deck();
        }

        public Card AddCardFromDrawDeckToInHandDeck()
        {
            while (true)
            {
                //Console.WriteLine($"----- Draw count: {DrawDeck.CardDeck.Count}, In-hand count: {InHandDeck.CardDeck.Count}, Discard count: {DiscardDeck.CardDeck.Count}");
                if (DrawDeck.CardDeck.Any())
                {
                    var cardToAdd = DrawDeck.CardDeck.First();
                    InHandDeck.CardDeck.Add(cardToAdd);
                    DrawDeck.CardDeck.Remove(cardToAdd);
                    return cardToAdd;
                }

                if (DiscardDeck.CardDeck.Any())
                {
                    DiscardDeck.CardDeck.Shuffle();
                    DrawDeck.CardDeck.AddRange(DiscardDeck.CardDeck);
                    DiscardDeck.CardDeck.Clear();
                }
                else
                {
                    return null;
                }
            }
        }

        public void MoveCardToDiscardDeck(Card card)
        {
            DiscardDeck.CardDeck.Add(card);
        }

        public void PlayShieldCard(Card card)
        {
            if (!ShieldDeck.CardDeck.Contains(card))
            {
                RemoveCardFromInPlayDeck(card);
                ShieldDeck.CardDeck.Add(card);
                card.NumShieldsRemaining = card.Actions.Count(x => x.ActionType == ActionType.Block);
            }
            NumberOfShields++;
        }

        public void DiscardShieldCard(Card card)
        {
            ShieldDeck.CardDeck.Remove(card);
            DiscardDeck.CardDeck.Add(card);
        }

        public void AttackShield()
        {
            var card = ShieldDeck.CardDeck.FirstOrDefault();
            if (card != null)
            {
                card.NumShieldsRemaining--;
                if (card.NumShieldsRemaining <= 0)
                {
                    DiscardShieldCard(card);
                }
            }

            NumberOfShields--;
        }

        public void Heal()
        {
            CurrentHitPoints++;
            if (CurrentHitPoints > MaxHitPoints)
            {
                CurrentHitPoints = MaxHitPoints;
            }
        }

        public void RemoveCardFromInPlayDeck(Card card)
        {
            InPlayDeck.CardDeck.Remove(card);
        }
    }
}
