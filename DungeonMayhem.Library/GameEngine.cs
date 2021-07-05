using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DungeonMayhem.Library
{
    public class GameEngine
    {
        private readonly List<Creature> _creatures;
        private readonly Stack<Creature> _defeated;
        private readonly bool _useMightyPowers;
        private readonly bool _useConsoleLogs;
        private bool _specialAttackAllOverride;
        private Creature _currentTurn;

        public GameEngine(List<Creature> creatures, bool useMightyPowers = true, bool useConsoleLogs = true)
        {
            _creatures = creatures;
            _defeated = new Stack<Creature>(creatures.Count);
            _useMightyPowers = useMightyPowers;
            _useConsoleLogs = useConsoleLogs;
        }

        public List<Creature> GameLoop()
        {
            _creatures.Shuffle();

            LogLine("Starting a new game");
            LogLine("Play order will be:");
            foreach (var creature in _creatures)
            {
                LogLine($"\t{creature.CreatureName}");
            }

            // Shuffle Decks
            foreach (var creature in _creatures)
            {
                creature.DrawDeck.CardDeck.Shuffle();
            }

            int round = 1;
            while (true)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"------ Round {round}:-----");
                foreach (var creature in _creatures.OrderByDescending(x => x.CurrentHitPoints).ThenByDescending(x => x.NumberOfShields))
                {
                    sb.AppendLine($"\t{creature.CreatureName} with {creature.CurrentHitPoints} hit points and {creature.NumberOfShields} shield(s)");
                }

                LogLine();
                LogLine(sb.ToString());

                foreach (var creature in _creatures)
                {
                    // Determine if isAlive
                    if (creature.CurrentHitPoints > 0)
                    {
                        // Draw a card (up to 3 cards in hand for round 1, only 1 card for every round after)
                        while (creature.InHandDeck.CardDeck.Count < (round == 1 ? 3 : 1))
                        {
                            creature.DrawCard();
                        }

                        // Play a card
                        PlayCard(creature);
                    }

                    // Evaluate Game End scenario
                    var aliveCreatures = _creatures.Where(x => x.CurrentHitPoints > 0).ToList();
                    if (aliveCreatures.Count == 1)
                    {
                        return Winner(aliveCreatures.FirstOrDefault());
                    }

                    if (!aliveCreatures.Any())
                    {
                        return NoWinner();
                    }
                }

                round++;
            }
        }

        private List<Creature> NoWinner()
        {
            List<Creature> winnerList = new List<Creature>();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("********************************************************************************************************************");
            sb.AppendLine("Ha ha ha!  You were all defeated, there were no victors today!");
            sb.AppendLine();
            sb.AppendLine("The following places:");
            int count = 1;
            foreach (var loser in _defeated)
            {
                sb.AppendLine($"{count++} - {loser.CreatureName}");
                winnerList.Add(loser);
            }
            sb.AppendLine("********************************************************************************************************************");
            sb.AppendLine();

            LogLine(sb.ToString());

            return winnerList;
        }

        private List<Creature> Winner(Creature creature)
        {
            List<Creature> winnerList = new List<Creature> { creature };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("********************************************************************************************************************");
            sb.AppendLine($"Congratulations {creature.CreatureName}, you are the winner, with {creature.CurrentHitPoints} hit points and {creature.NumberOfShields} shield(s) remaining!");
            sb.AppendLine();
            sb.AppendLine("The following places:");
            sb.AppendLine($"{1} - {creature.CreatureName}");
            int count = 2;
            foreach (var loser in _defeated)
            {
                sb.AppendLine($"{count++} - {loser.CreatureName}");
                winnerList.Add(loser);
            }
            sb.AppendLine("********************************************************************************************************************");
            sb.AppendLine();

            LogLine(sb.ToString());

            return winnerList;
        }

        private void PlayCard(Creature creature, Card specificCard = null)
        {
            if (_specialAttackAllOverride && _currentTurn != creature)
            {
                _specialAttackAllOverride = false;
            }

            var card = specificCard ?? creature.DrawCardFromHand();

            LogLine();
            LogLine($"{creature.CreatureName} plays '{card.Name}'");

            if (card.Name.Contains("Shapeshift:"))
            {
                PlayShapeshift(creature, card);
            }

            foreach (var action in card.Actions)
            {
                // Evaluate action
                switch (action.ActionType)
                {
                    case ActionType.Draw:
                        LogLine("*Draw a card");
                        creature.DrawCard();
                        break;
                    case ActionType.Block:
                        LogLine("*Block 1 damage");
                        creature.PlayShieldCard(card);
                        LogLine($"\t{creature.CreatureName} now has {creature.NumberOfShields} shield(s)");
                        break;
                    case ActionType.Damage:
                        LogLine("*Deal 1 damage");
                        Attack(creature, AttackType.Random);
                        break;
                    case ActionType.Heal:
                        LogLine("*Recover 1 hit point");
                        creature.Heal();
                        LogLine($"\t{creature.CreatureName} now has {creature.CurrentHitPoints} hit points");
                        break;
                    case ActionType.PlayExtraCard:
                        LogLine("*Play an extra card");
                        PlayCard(creature);
                        break;
                    case ActionType.MightyPower:
                        LogLine("*Mighty Power!");
                        MightyPower(card, creature);
                        break;
                    case ActionType.DamageIgnoreShields:
                        Attack(creature, AttackType.Random, null, true);
                        break;
                    case ActionType.Shapeshift:
                        switch (creature.ShapeShiftForm)
                        {
                            case ShapeshiftForm.Bear:
                                LogLine("*Bear Form: Heal");
                                creature.Heal();
                                break;
                            case ShapeshiftForm.Wolf:
                                LogLine("*Wolf Form: Attack");
                                Attack(creature, AttackType.Random);
                                break;
                            case ShapeshiftForm.None:
                                LogLine("*No Shapeshift Form");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            creature.MoveCardToDiscardDeck(card);
        }

        private void PlayShapeshift(Creature creature, Card card)
        {
            if (card.Name.Contains("Bear"))
            {
                creature.ShapeShiftForm = ShapeshiftForm.Bear;
            }
            else if (card.Name.Contains("Wolf"))
            {
                creature.ShapeShiftForm = ShapeshiftForm.Wolf;
            }
        }

        public void Attack(Creature attacker, AttackType attackType, Creature target = null, bool bypassShields = false)
        {
            List<Creature> attackTargets = new List<Creature>();
            if (_specialAttackAllOverride)
            {
                attackType = AttackType.Opponents;
            }

            switch (attackType)
            {
                case AttackType.Random:
                    var attackList = new List<Creature>(_creatures.Count - 1);
                    attackList.AddRange(from c in _creatures
                                        where c != attacker && c.CurrentHitPoints > 0
                                        select c);

                    // All opponents have been defeated
                    if (!attackList.Any())
                    {
                        break;
                    }

                    attackList.Shuffle();
                    attackTargets.Add(attackList.First());
                    break;
                case AttackType.All:
                    attackTargets.AddRange(_creatures.Where(x => x.CurrentHitPoints > 0));
                    break;
                case AttackType.Specific:
                    attackTargets.Add(target);
                    break;
                case AttackType.Opponents:
                    attackTargets.AddRange(GetOpponents(attacker));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attackType), attackType, null);
            }

            foreach (var attackedCreature in attackTargets)
            {
                LogMessage($"\t{attacker.CreatureName} attacks {attackedCreature.CreatureName}");

                if (bypassShields)
                {
                    attackedCreature.CurrentHitPoints--;
                    LogLine($" - {attackedCreature.CreatureName} loses 1 hit point");
                }
                else
                {
                    if (attackedCreature.NumberOfShields > 0)
                    {
                        attackedCreature.AttackShield();
                        LogLine($" - {attackedCreature.CreatureName} defends with a shield");
                    }
                    else
                    {
                        attackedCreature.CurrentHitPoints--;
                        LogLine($" - {attackedCreature.CreatureName} loses 1 hit point");
                    }
                }

                LogLine($"\t\t{attackedCreature.CreatureName} now has {attackedCreature.CurrentHitPoints} current hit points and {attackedCreature.NumberOfShields} shield(s)");

                if (attackedCreature.CurrentHitPoints <= 0)
                {
                    LogLine();
                    LogLine($"===== {attackedCreature.CreatureName} has been defeated by {attacker.CreatureName}! ======");

                    _defeated.Push(attackedCreature);
                }
            }
        }

        private void MightyPower(Card card, Creature creature)
        {
            if (!_useMightyPowers)
            {
                return;
            }

            switch (card.Name)
            {
                case "Vampiric Touch": //Azzan
                    LogLine("\tSwap your hit points with an opponent's");
                    int maxHp = creature.CurrentHitPoints;
                    Creature maxHpCreature = creature;
                    foreach (var targetCreature in _creatures)
                    {
                        if (targetCreature.CurrentHitPoints > maxHp)
                        {
                            maxHpCreature = targetCreature;
                            maxHp = targetCreature.CurrentHitPoints;
                        }
                    }

                    int tmp = creature.CurrentHitPoints;
                    creature.CurrentHitPoints = maxHpCreature.CurrentHitPoints;
                    maxHpCreature.CurrentHitPoints = tmp;

                    LogLine($"\t{creature.CreatureName} swapped hit points with {maxHpCreature.CreatureName}");
                    LogLine($"\t{creature.CreatureName} has {creature.CurrentHitPoints} hit points, {maxHpCreature.CreatureName} has {maxHpCreature.CurrentHitPoints} hit points.");
                    break;
                case "Fireball": //Azzan
                    LogLine("\tEach player (including you) takes 3 damage!");

                    foreach (var c in _creatures.Where(x => x.CurrentHitPoints > 0))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (c.CurrentHitPoints > 0)
                            {
                                Attack(creature, AttackType.Specific, c);

                                if (creature == c && c.CurrentHitPoints <= 0)
                                {
                                    LogLine("Ha ha ha, you just killed yourself!");
                                    return;
                                }
                            }
                        }

                        LogLine();
                    }
                    break;
                case "Charm": //Azzan
                    LogLine("\tTake the shields that an opponent has in play - it protects you now!");

                    // find the creature with the most shields
                    var creatureWithMostShields = _creatures.OrderByDescending(x => x.NumberOfShields).FirstOrDefault();
                    if (creatureWithMostShields == null)
                    {
                        LogLine($"\tThere are not any shields in play");
                    }
                    else
                    {
                        int shields = creatureWithMostShields.NumberOfShields;
                        if (shields == 0)
                        {
                            LogLine($"\tThere are not any shields in play");
                        }
                        else
                        {
                            creature.NumberOfShields += shields;
                            creatureWithMostShields.NumberOfShields -= shields;

                            LogLine($"\t{creature.CreatureName} charms {shields} shield(s) from {creatureWithMostShields.CreatureName}");
                        }
                    }

                    break;
                case "Hugs!": //Blorp
                    LogLine("\tDestroy a creature's shields and then heal for each shield destroyed");

                    creatureWithMostShields = _creatures.OrderByDescending(x => x.NumberOfShields).FirstOrDefault();
                    if (creatureWithMostShields == null)
                    {
                        LogLine($"\tThere are not any shields in play");
                    }
                    else
                    {
                        int shields = creatureWithMostShields.NumberOfShields;
                        if (shields == 0)
                        {
                            LogLine($"\tThere are not any shields in play");
                        }
                        else
                        {
                            creatureWithMostShields.NumberOfShields -= shields;
                            creature.CurrentHitPoints += shields;
                            if (creature.CurrentHitPoints > creature.MaxHitPoints)
                            {
                                creature.CurrentHitPoints = creature.MaxHitPoints;
                            }

                            LogLine($"\t{creature.CreatureName} destroys {shields} shield(s) from {creatureWithMostShields.CreatureName}, and now has {creature.CurrentHitPoints} hit points");
                        }
                    }

                    break;
                case "Burped-Up Bones": //Blorp
                    LogLine("\tAttack twice and gain 3 shields");
                    break;
                case "Here I Come!": //Blorp
                    LogLine("\tThis turn, your attack cards ignore shield cards.  Gain 3 attacks.");
                    break;
                case "Praise Me": //Delilah
                    LogLine("\tDraw 3 cards, then double attack.");
                    break;
                case "Death Ray": //Delilah
                    LogLine("\tDouble attack each opponent with no shield cards in play.  Then destroy all shield cards - including yours!");

                    var noShieldChars = _creatures.Where(x => x.NumberOfShields == 0 && x.CurrentHitPoints > 0 && x != creature).ToList();

                    foreach (var c in noShieldChars)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Attack(creature, AttackType.Specific, c);
                        }
                    }

                    foreach (var c1 in _creatures.Where(x => x.NumberOfShields > 0))
                    {
                        LogLine($"\tDestroying {c1.NumberOfShields} of {c1.CreatureName}'s shield(s).");
                        c1.NumberOfShields = 0;
                    }
                    break;
                case "Charm Ray": //Delilah
                    LogLine("\tUntil your next turn, choose the target of all attack cards (attacks are random, so doesn't really do much, sorry).");
                    break;
                case "Mind Blast": //Dr. T
                    LogLine("\tAttack an opponent once for each card they have in their hand.");
                    var chars = GetOpponents(creature).ToList();
                    foreach (var target in chars)
                    {
                        for (int i = 0; i < target.InHandDeck.CardDeck.Count; i++)
                        {
                            Attack(creature, AttackType.Specific, target);
                        }
                    }
                    break;
                case "Mind Games": //Dr. T
                    LogLine("\tSwap your hand with an opponent's hand.");
                    var targetList = GetOpponents(creature).ToList();
                    targetList.Shuffle();
                    var t1 = targetList.FirstOrDefault();
                    if (t1 == null)
                    {
                        LogLine("\tNo target found");
                        break;
                    }

                    var tmpInHandDeck = creature.InHandDeck;
                    creature.InHandDeck = t1.InHandDeck;
                    t1.InHandDeck = tmpInHandDeck;
                    LogLine($"\tSwapping hands with {t1.CreatureName}");

                    break;
                case "Tell Me About Your Mother": // Dr. T
                    LogLine("\tAdd the top card of each opponent's discard pile to your hand.");
                    foreach (var opponent in GetOpponents(creature))
                    {
                        var lastOrDefault = opponent.DiscardDeck.CardDeck.LastOrDefault();
                        if (lastOrDefault == null)
                        {
                            LogLine($"\t{opponent.CreatureName} does not have any cards in their discard pile");
                        }
                        else
                        {
                            creature.InHandDeck.CardDeck.Add(lastOrDefault);
                            LogLine($"\tAdding {lastOrDefault.Name} from {opponent.CreatureName} to hand.");
                        }
                    }
                    break; 
                case "For My Next Trick...": //Hoots
                    LogLine("\tUntil your next turn, your attacks hit all opponents.");
                    _specialAttackAllOverride = true;
                    _currentTurn = creature;
                    break;
                case "To The Face!": //Hoots
                    LogLine("\tDestroy a shield card and then attack for each starting shield on that card.");
                    // Find the character with the most shields on a card
                    var charsWithShields = _creatures.Where(x => x.CurrentHitPoints > 0 && x.NumberOfShields > 0 && x != creature).ToList();
                    Creature charToAttack = charsWithShields.FirstOrDefault();
                    Card highestShieldCard = charToAttack?.ShieldDeck.CardDeck.FirstOrDefault();
                    int? highestShieldCount = highestShieldCard?.Actions.Count(x => x.ActionType == ActionType.Block);

                    if (highestShieldCount == null)
                    {
                        LogLine("\tNo Shields in Play!");
                        break;
                    }

                    foreach (var ch in charsWithShields)
                    {
                        foreach (var shieldCard in ch.ShieldDeck.CardDeck)
                        {
                            var numOfShieldsOnCard = shieldCard.Actions.Count(x => x.ActionType == ActionType.Block);
                            if (numOfShieldsOnCard > highestShieldCount)
                            {
                                charToAttack = ch;
                                highestShieldCard = shieldCard;
                                highestShieldCount = numOfShieldsOnCard;
                            }
                        }
                    }

                    LogLine($"\t{charToAttack.CreatureName} has the highest shield card with {highestShieldCount} shield(s).");

                    // Reduce number of shields from character
                    charToAttack.NumberOfShields -= highestShieldCount.Value;
                    if (charToAttack.NumberOfShields < 0)
                    {
                        charToAttack.NumberOfShields = 0;
                    }
                    LogLine($"\t{charToAttack.CreatureName} gets {highestShieldCount} shield(s) destroyed, and gets attacked for each one.");

                    // Move shield card to discard pile
                    charToAttack.DiscardDeck.CardDeck.Add(highestShieldCard);
                    charToAttack.ShieldDeck.CardDeck.Remove(highestShieldCard);

                    // Attack character for number of shields
                    for (int i = 0; i < highestShieldCount; i++)
                    {
                        Attack(creature, AttackType.Specific, charToAttack);
                    }

                    break;
                case "Owlbear Boogie": //Hoots
                    LogLine($"\tEach player does a little dance and draws a card.  You then draw a card for each player who danced.");
                    int count = 0;
                    foreach (var c in GetOpponents(creature))
                    {
                        c.DrawCard();
                        count++;
                    }
                    LogLine($"\tYou draw {count} card(s).");
                    for (int i = 0; i < count; i++)
                    {
                        creature.DrawCard();
                    }
                    break;
                case "Primal Strike": //Jaheira
                    LogLine("\tYou make an animal noise and attack each opponent.");
                    Attack(creature, AttackType.Opponents);
                    break;
                case "Commune With Nature": //Jaheira
                    LogLine("\tYou may play a Form card for free.");
                    var shapeshiftCardInHand = creature.InHandDeck.CardDeck.FirstOrDefault(x => x.Name.Contains("Shapeshift"));
                    if (shapeshiftCardInHand == null)
                    {
                        LogLine("\tNo shapeshift cards in hand");
                    }
                    else
                    {
                        PlayCard(creature, shapeshiftCardInHand);
                    }
                    break;
                case "Divine Inspiration": //Lia
                    LogLine("\tChoose any card in your discard pile and put it into your hand, then heal twice");

                    if (creature.DiscardDeck.CardDeck.Count == 0)
                    {
                        LogLine("\tNo cards in discard pile to choose from");
                        break;
                    }
                    int rand = new Random().Next(0, creature.DiscardDeck.CardDeck.Count);
                    var randomDiscard = creature.DiscardDeck.CardDeck[rand];
                    creature.InHandDeck.CardDeck.Add(randomDiscard);
                    creature.DiscardDeck.CardDeck.Remove(randomDiscard);
                    LogLine($"\tMoving {randomDiscard.Name} to hand.");
                    break;
                case "Banishing Smite": //Lia
                    LogLine("\tDestroy all shield cards in play (including yours), then go again.");
                    foreach (var creature1 in _creatures.Where(x => x.CurrentHitPoints > 0 && x.NumberOfShields > 0))
                    {
                        LogLine($"\tDestroying {creature1.NumberOfShields} of {creature1.CreatureName}'s shields.");
                        creature1.DiscardDeck.CardDeck.AddRange(creature1.ShieldDeck.CardDeck);
                        creature1.ShieldDeck.CardDeck.Clear();
                        creature1.NumberOfShields = 0;
                    }
                    break;
                default:
                    LogLine("\tMighty Power Not Found!");
                    break;
            }
        }

        private IEnumerable<Creature> GetOpponents(Creature creature)
        {
            return _creatures.Where(x => x != creature && x.CurrentHitPoints > 0);
        }

        private void LogLine()
        {
            if (_useConsoleLogs)
            {
                Console.WriteLine();
            }
        }

        private void LogLine(string message)
        {
            if (_useConsoleLogs)
            {
                Console.WriteLine(message);
            }
        }

        private void LogMessage(string message)
        {
            if (_useConsoleLogs)
            {
                Console.Write(message);
            }
        }
    }

    public enum AttackType
    {
        Random, All, Specific, Opponents
    }
}
