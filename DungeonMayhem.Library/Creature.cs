using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DungeonMayhem.Library
{
    public class Creature
    {
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

        public Creature()
        {
            
        }

        public Creature(string name, string jsonFile)
        {
            CreatureName = name;
            CurrentHitPoints = 10;
            DiscardDeck = new Deck();
            DrawDeck = JsonSerializer.Deserialize<Deck>(File.ReadAllText($"Creatures\\{jsonFile}"));
            InHandDeck = new Deck();
            IsHuman = false;
            MaxHitPoints = 10;
            NumberOfShields = 0;
            PlayerName = string.Empty;
            PlayerNumber = 0;
            InPlayDeck = new Deck();
            ShapeShiftForm = ShapeshiftForm.None;
            ShieldDeck = new Deck();
        }

        public void AddCardFromDrawDeckToInHandDeck()
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

                    AddCardFromDrawDeckToInHandDeck();
                }
                else
                {
                    Console.WriteLine("**** There are no draw cards or discard cards to draw from. ****");
                }
            }
        }

        public Card PlayCardFromHand()
        {
            if (InHandDeck.CardDeck.Count <= 0)
            {
                AddCardFromDrawDeckToInHandDeck();
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
            if (!ShieldDeck.CardDeck.Contains(card))
            {
                ShieldDeck.CardDeck.Add(card);
            }
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
