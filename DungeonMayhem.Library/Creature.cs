using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonMayhem.Library
{
    public class Creature
    {
        public string CreatureName { get; set; }
        public string PlayerName { get; set; }
        public bool IsCpu { get; set; }
        public int MaxHitPoints { get; set; }
        public int CurrentHitPoints { get; set; }
        public Deck DiscardDeck { get; set; }
        public Deck InHandDeck { get; set; }
        public Deck DrawDeck { get; set; }
        public Deck ShieldDeck { get; set; }
        public int NumberOfShields { get; set; }
        public ShapeshiftForm ShapeShiftForm { get; set; }

        public Creature()
        {
            DiscardDeck = new Deck();
            InHandDeck = new Deck();
            DrawDeck = new Deck();
            ShieldDeck = new Deck();
        }

        public void DrawCard()
        {
            //Console.WriteLine($"----- Draw count: {DrawDeck.CardDeck.Count}, In-hand count: {InHandDeck.CardDeck.Count}, Discard count: {DiscardDeck.CardDeck.Count}");

            if (DrawDeck.CardDeck.Any())
            {
                InHandDeck.CardDeck.Add(DrawDeck.CardDeck[0]);
                DrawDeck.CardDeck.RemoveAt(0);
            }
            else
            {
                if (DiscardDeck.CardDeck.Any())
                {
                    DiscardDeck.CardDeck.Shuffle();
                    DrawDeck.CardDeck.AddRange(DiscardDeck.CardDeck);
                    DiscardDeck.CardDeck.Clear();

                    DrawCard();
                }
                else
                {
                    Console.WriteLine($"**** There are no draw cards or discard cards to draw from. ****");
                }
            }
        }

        public Card DrawCardFromHand()
        {
            if (InHandDeck.CardDeck.Count <= 0)
            {
                DrawCard();
            }
            InHandDeck.CardDeck.Shuffle();
            var card = InHandDeck.CardDeck.First();
            InHandDeck.CardDeck.Remove(card);
            return card;
        }

        public void MoveCardToDiscardDeck(Card card)
        {
            DiscardDeck.CardDeck.Add(card);
            InHandDeck.CardDeck.Remove(card);
        }

        public void PlayShieldCard(Card card)
        {
            //@todo: track shield cards - used for some specials
            ShieldDeck.CardDeck.Add(card);
            //foreach (var block in card.Actions.Where(x => x.ActionType == ActionType.Block))
            //{
            //    NumberOfShields++;
            //}
            NumberOfShields++;
        }

        public void DiscardShieldCard(Card card)
        {
            DiscardDeck.CardDeck.Add(card);
            ShieldDeck.CardDeck.Remove(card);
        }

        public void AttackShield()
        {
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
    }

    public class Deck
    {
        public List<Card> CardDeck { get; set; }

        public Deck()
        {
            CardDeck = new List<Card>();
        }
    }

    public class Card
    {
        public string Name { get; set; }
        public List<CreatureAction> Actions { get; set; }

        public Card()
        {
            Actions = new List<CreatureAction>();
        }
    }

    public class CreatureAction
    {
        public ActionType ActionType { get; set; }
    }

    public enum ActionType
    {
        Block, Draw, Damage, PlayExtraCard, Heal, MightyPower, Shapeshift, DamageIgnoreShields
    }

    public enum ShapeshiftForm
    {
        None, Bear, Wolf
    }
}
