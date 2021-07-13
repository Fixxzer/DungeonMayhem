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
        private bool _specialAttackBonusDamageOverride;
        private Creature _currentTurn;
        private Creature _specialAttackSpecificOverride;
        private bool _isFirstTurn;

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
                var message = creature.IsHuman ? $"\t{creature.CreatureName} - played by {creature.PlayerName}" : $"\t{creature.CreatureName}";
                LogLine(message);
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
                        LogLine();
                        var optionalPlayerName = !string.IsNullOrWhiteSpace(creature.PlayerName) ? $" ({creature.PlayerName})" : string.Empty;
                        LogLine($"--- {creature.CreatureName}{optionalPlayerName}'s turn.");
                        LogLine();
                     
                        // Draw a card, 3 cards for round 1, only 1 card for every round after
                        for (int i = 0; i < (round == 1 ? 3 : 1); i++)
                        {
                            DrawCard(creature);
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
            if (creature.IsHuman)
            {
                Console.WriteLine();
                Console.WriteLine("=============================================");
                Console.WriteLine($"It's now your turn {creature.PlayerName}");
                if (specificCard != null)
                {
                    Console.WriteLine($"You must play {DisplayCard(specificCard)}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
                else
                {
                    int optionNum = 1;
                    foreach (var handCard in creature.InHandDeck.CardDeck)
                    {
                        Console.WriteLine($"{optionNum++} - {DisplayCard(handCard)}");
                    }

                    int? selectedNum = null;
                    while (selectedNum == null)
                    {
                        Console.WriteLine("Please select a card to play, type the number and press the <enter> key to choose");
                        string option = Console.ReadLine();
                        selectedNum = int.Parse(option);
                    }

                    var selectedCard = creature.InHandDeck.CardDeck[selectedNum.Value - 1];
                    specificCard = selectedCard;
                }
            }

            if (_specialAttackAllOverride && _currentTurn != creature)
            {
                _specialAttackAllOverride = false;
            }

            if (_specialAttackBonusDamageOverride && _currentTurn != creature)
            {
                _specialAttackBonusDamageOverride = false;
            }

            if (_specialAttackSpecificOverride != null && _currentTurn != creature)
            {
                _isFirstTurn = false;
            }

            if (!_isFirstTurn && _specialAttackSpecificOverride != null && _currentTurn == creature)
            {
                _specialAttackSpecificOverride = null;
            }

            var card = specificCard ?? RetrieveCardFromHand(creature);
            creature.InHandDeck.CardDeck.Remove(card);
            creature.InPlayDeck.CardDeck.Add(card);

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
                        DrawCard(creature);
                        break;
                    case ActionType.Block:
                        LogLine("*Add 1 shield");
                        PlayShieldCard(creature, card);
                        break;
                    case ActionType.Damage:
                        LogLine("*Deal 1 damage");
                        Attack(creature, AttackType.Random);
                        break;
                    case ActionType.Heal:
                        LogLine("*Recover 1 hit point");
                        Heal(creature);
                        break;
                    case ActionType.PlayExtraCard:
                        LogLine("*Play an extra card");
                        PlayCard(creature);
                        break;
                    case ActionType.MightyPower:
                        LogLine("*Mighty Power!");
                        MightyPower(creature, card);
                        break;
                    case ActionType.DamageIgnoreShields:
                        Attack(creature, AttackType.Random, null, true);
                        break;
                    case ActionType.Shapeshift:
                        switch (creature.ShapeShiftForm)
                        {
                            case ShapeshiftForm.Bear:
                                LogLine("*Bear Form: Heal");
                                Heal(creature);
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

            if (card.Actions.All(x => x.ActionType != ActionType.Block))
            {
                creature.RemoveCardFromInPlayDeck(card);
                creature.MoveCardToDiscardDeck(card);
            }

            if (card.Actions.Any(x => x.ActionType == ActionType.Block))
            {
                LogLine();
                LogLine($"\t{creature.CreatureName} now has {creature.NumberOfShields} shield(s)");
            }

            if (card.Actions.Any(x => x.ActionType == ActionType.Heal))
            {
                LogLine();
                LogLine($"\t{creature.CreatureName} now has {creature.CurrentHitPoints} hit point(s)");
            }

            // Check in-hand, if 0 - draw 2
            if (!creature.InHandDeck.CardDeck.Any())
            {
                DrawCard(creature);
                DrawCard(creature);
            }
        }

        private Card RetrieveCardFromHand(Creature creature)
        {
            // If out of cards, draw twice
            if (creature.InHandDeck.CardDeck.Count <= 0)
            {
                DrawCard(creature);
                DrawCard(creature);
            }

            // Prioritize play again cards
            var playAgainCard = GetCardWithMostActionItems(creature, ActionType.PlayExtraCard);
            if (playAgainCard != null)
            {
                creature.InHandDeck.CardDeck.Remove(playAgainCard);
                return playAgainCard;
            }

            // If health is low, prioritize healing or shields
            if (creature.CurrentHitPoints <= 5)
            {
                int shieldCount = 0;
                int healingCount = 0;

                var shieldCard = GetCardWithMostActionItems(creature, ActionType.Block);
                if (shieldCard != null)
                {
                    shieldCount = shieldCard.Actions.Count(x => x.ActionType == ActionType.Block);
                }

                var healthCard = GetCardWithMostActionItems(creature, ActionType.Heal);
                if (healthCard != null)
                {
                    healingCount = healthCard.Actions.Count(x => x.ActionType == ActionType.Heal);
                }

                if (healingCount != 0 || shieldCount != 0)
                {
                    if (healingCount >= shieldCount)
                    {
                        creature.InHandDeck.CardDeck.Remove(healthCard);
                        return healthCard;
                    }

                    creature.InHandDeck.CardDeck.Remove(shieldCard);
                    return shieldCard;
                }
            }

            // Otherwise randomize
            creature.InHandDeck.CardDeck.Shuffle();

            var card = creature.InHandDeck.CardDeck.First();
            return card;
        }

        private static Card GetCardWithMostActionItems(Creature creature, ActionType actionType)
        {
            List<Card> cards = new List<Card>();
            foreach (var card in creature.InHandDeck.CardDeck)
            {
                foreach (var action in card.Actions)
                {
                    if (action.ActionType == actionType)
                    {
                        cards.Add(card);
                    }
                }
            }

            int max = 0;
            Card maxCard = null;
            foreach (var card in cards)
            {
                var count = card.Actions.Count(x => x.ActionType == actionType);
                if (count > max)
                {
                    max = count;
                    maxCard = card;
                }
            }

            return maxCard;
        }

        private static string DisplayCard(Card specificCard)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(specificCard.Name);
            foreach (var action in specificCard.Actions)
            {
                switch (action.ActionType)
                {
                    case ActionType.Block:
                        sb.AppendLine("Block");
                        break;
                    case ActionType.Draw:
                        sb.AppendLine("Draw");
                        break;
                    case ActionType.Damage:
                        sb.AppendLine("Attack");
                        break;
                    case ActionType.PlayExtraCard:
                        sb.AppendLine("Lightning");
                        break;
                    case ActionType.Heal:
                        sb.AppendLine("Heal");
                        break;
                    case ActionType.MightyPower:
                        sb.AppendLine("Mighty Power");
                        break;
                    case ActionType.Shapeshift:
                        sb.AppendLine("Shapeshift");
                        break;
                    case ActionType.DamageIgnoreShields:
                        sb.AppendLine("Attack, ignoring shields");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return sb.ToString();
        }

        private static void PlayShapeshift(Creature creature, Card card)
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
            var attackTargets = new List<Creature>();
            
            if (_specialAttackAllOverride)
            {
                attackType = AttackType.Opponents;
            }

            if (_specialAttackSpecificOverride != null)
            {
                attackType = AttackType.Specific;
                target = _specialAttackSpecificOverride;
            }

            switch (attackType)
            {
                case AttackType.Random:
                    var attackList = GetOpponents(attacker).ToList();
                    if (!attackList.Any())
                    {
                        break;
                    }
                    //attackList.Shuffle();
                    attackTargets.Add(attackList.OrderByDescending(x => x.CurrentHitPoints).First());
                    break;
                case AttackType.All:
                    attackTargets.AddRange(GetAliveCreatures());
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

            if (_specialAttackBonusDamageOverride)
            {
                attackTargets.Double();
            }

            foreach (var attackedCreature in attackTargets.OrderBy(x => x.CreatureName))
            {
                LogMessage($"\t{attacker.CreatureName} attacks {attackedCreature.CreatureName}");
                if (attackedCreature.CurrentHitPoints <= 0)
                {
                    LogLine($" - {attackedCreature.CreatureName} has been defeated, and can no longer be attacked.");
                    break;
                }

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

        public void Heal(Creature creature)
        {
            creature.Heal();
        }

        public void DrawCard(Creature creature)
        {
            var card = creature.AddCardFromDrawDeckToInHandDeck();
            if (card == null)
            {
                LogLine("**** There are no draw cards or discard cards to draw from. ****");
                return;
            }

            LogLine($"\t{creature.CreatureName} draws {card.Name}.");
        }

        public void PlayShieldCard(Creature creature, Card card)
        {
            creature.PlayShieldCard(card);
        }

        private void MightyPower(Creature creature, Card card)
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
                    foreach (var targetCreature in _creatures.Where(x => x != creature))
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
                    var ops1 = GetOpponents(creature).Where(x => x.NumberOfShields > 0);
                    var creatureWithMostShields = ops1.OrderByDescending(x => x.NumberOfShields).FirstOrDefault();
                    if (creatureWithMostShields == null)
                    {
                        LogLine("\tThere are not any shields in play");
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

                            creature.ShieldDeck.CardDeck.AddRange(creatureWithMostShields.ShieldDeck.CardDeck);
                            creatureWithMostShields.ShieldDeck.CardDeck.Clear();

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
                            creatureWithMostShields.ShieldDeck.CardDeck.Clear();

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
                    LogLine("\tDraw 3 cards, then, each opponent can choose to praise your greatness.");
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
                        c1.ShieldDeck.CardDeck.Clear();
                    }
                    break;
                case "Charm Ray": //Delilah
                    LogLine("\tUntil your next turn, choose the target of all attack cards.");
                    var t = GetOpponents(creature).FirstOrDefault();
                    if (t == null)
                    {
                        LogLine("\tNo opponents remaining");
                        break;
                    }
                    LogLine($"\t{t.CreatureName} will be the target of all attack cards until your next turn.");
                    _specialAttackSpecificOverride = t;
                    _currentTurn = creature;
                    _isFirstTurn = true;
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
                            opponent.DiscardDeck.CardDeck.Remove(lastOrDefault);
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
                    var (highestShieldCharacter, highestShieldCard) = GetMaxShieldCardFromOpponents(creature);
                    if (highestShieldCharacter == null)
                    {
                        LogLine("\tThere are not any shields in play");
                        break;
                    }
                    var numOfShieldsOnCard = highestShieldCard.Actions.Count(x => x.ActionType == ActionType.Block);
                    highestShieldCharacter.NumberOfShields -= numOfShieldsOnCard;
                    if (highestShieldCharacter.NumberOfShields < 0)
                    {
                        highestShieldCharacter.NumberOfShields = 0;
                    }
                    LogLine($"\t{highestShieldCharacter.CreatureName} gets {numOfShieldsOnCard} shield(s) destroyed, and gets attacked for each one.");
                    highestShieldCharacter.DiscardShieldCard(highestShieldCard);
                    for (int i = 0; i < numOfShieldsOnCard; i++)
                    {
                        Attack(creature, AttackType.Specific, highestShieldCharacter);
                    }

                    break;
                case "Owlbear Boogie": //Hoots
                    LogLine($"\tEach player does a little dance and draws a card.  You then draw a card for each player who danced.");
                    int count = 0;
                    foreach (var c in GetOpponents(creature))
                    {
                        DrawCard(c);
                        count++;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        DrawCard(creature);
                    }
                    LogLine($"\tYou draw {count} card(s).");
                    break;
                case "Primal Strike": //Jaheira
                    LogLine("\tYou make an animal noise and attack each opponent.");
                    Attack(creature, AttackType.Opponents);
                    break;
                case "Commune With Nature": //Jaheira
                    LogLine("\tYou may play a Form card for free.");
                    var shapeshiftCardInHand = creature.InHandDeck.CardDeck.FirstOrDefault(cardInHand => cardInHand.Name.StartsWith("Shapeshift"));
                    
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
                case "Liquidate Assets": //Lord C
                    LogLine("\tDiscard your hand and attack equal to the number of cards discarded.");
                    
                    int numAttacks = creature.InHandDeck.CardDeck.Count;
                    LogLine($"\tYou have {numAttacks} card(s) in your hand, and get to attack {numAttacks} time(s).");
                    creature.DiscardDeck.CardDeck.AddRange(creature.InHandDeck.CardDeck);
                    creature.InHandDeck.CardDeck.Clear();
                    for (int i = 0; i < numAttacks; i++)
                    {
                        Attack(creature, AttackType.Random);
                    }
                    break;
                case "Hostile Takeover": //Lord C
                    LogLine("\tAttack all opponents, double attack one opponent, then attack a different opponent.");
                    
                    // Attack all
                    LogLine();
                    LogLine("\tAttacking all opponents");
                    Attack(creature, AttackType.Opponents);
                    
                    // Attack one twice
                    var ops = GetOpponents(creature).ToList();
                    if (!ops.Any())
                    {
                        LogLine($"\tNo opponents remaining.");
                        break;
                    }

                    ops.Shuffle();
                    LogLine();
                    LogLine($"\tAttacking {ops.First().CreatureName} twice.");
                    var t2 = ops.First();
                    for (int i = 0; i < 2; i++)
                    {
                        if (t2.CurrentHitPoints <= 0)
                        {
                            LogLine($"\t{t2.CreatureName} has been defeated and cannot be attacked.");
                            break;
                        }
                        Attack(creature, AttackType.Specific, t2);
                    }

                    // Attack a different
                    var op = ops.FirstOrDefault(x => x != ops.First());
                    if (op == null)
                    {
                        LogLine("\tNo target available to attack");
                        break;
                    }
                    
                    LogLine();
                    LogLine($"\tAttacking someone different, attacking {op.CreatureName}");
                    
                    Attack(creature, AttackType.Specific, op);

                    break;
                case "Murders and Acquisitions": //Lord C
                    LogLine("\tEach player must attack, heal, or draw.  Start with you and go right.  You repeat all choices.");

                    // Start with me
                    int r = new Random().Next(0, 3);
                    switch (r)
                    {
                        case 0: //attack
                            Attack(creature, AttackType.Random);
                            break;
                        case 1: //heal
                            Heal(creature);
                            break;
                        case 2: //draw
                            DrawCard(creature);
                            break;
                    }

                    // Go through rest of opponents and repeat their choice
                    foreach (var opponent in GetOpponents(creature))
                    {
                        r = new Random().Next(0, 3);
                        switch (r)
                        {
                            case 0: //attack
                                Attack(opponent, AttackType.Random);
                                Attack(creature, AttackType.Random);
                                break;
                            case 1: //heal
                                Heal(opponent);
                                Heal(creature);
                                break;
                            case 2: //draw
                                DrawCard(opponent);
                                DrawCard(creature);
                                break;
                        }
                    }
                    break;
                case "A Book (Cannot Bite)": //Mimi
                    LogLine("\tUse the top-listed Mighty Power of the player to your left or right");
                    var indexOf = _creatures.IndexOf(creature);
                    if (indexOf == 0)
                    {
                        indexOf = _creatures.Count;
                    }

                    var previousCreature = _creatures[indexOf - 1];

                    Card mightyPowerCard = previousCreature.DrawDeck.CardDeck.FirstOrDefault(card1 => card1.Actions.Any(action => action.ActionType == ActionType.MightyPower));
                    if (mightyPowerCard != null)
                    {
                        previousCreature.DrawDeck.CardDeck.Remove(mightyPowerCard);
                        PlayCard(creature, mightyPowerCard);
                        break;
                    }

                    mightyPowerCard = previousCreature.DiscardDeck.CardDeck.FirstOrDefault(card1 => card1.Actions.Any(action => action.ActionType == ActionType.MightyPower));
                    if (mightyPowerCard != null)
                    {
                        previousCreature.DiscardDeck.CardDeck.Remove(mightyPowerCard);
                        PlayCard(creature, mightyPowerCard);
                        break;
                    }

                    mightyPowerCard = previousCreature.InHandDeck.CardDeck.FirstOrDefault(card1 => card1.Actions.Any(action => action.ActionType == ActionType.MightyPower));
                    if (mightyPowerCard != null)
                    {
                        previousCreature.InHandDeck.CardDeck.Remove(mightyPowerCard);
                        PlayCard(creature, mightyPowerCard);
                        break;
                    }

                    break;
                case "It's Not a Trap": //Mimi
                    LogLine("\tMake one player's hit points equal to another player's hit points");
                    var aliveCreatures = _creatures.Where(x => x.CurrentHitPoints > 0 && x != creature).OrderBy(x => x.CurrentHitPoints).ToList();
                    var lowest = aliveCreatures.FirstOrDefault();
                    var highest = aliveCreatures.LastOrDefault();

                    if (lowest != null && highest != null)
                    {
                        LogLine($"\tMaking {highest.CreatureName}'s hit points ({highest.CurrentHitPoints}) equal to {lowest.CreatureName}'s hit points ({lowest.CurrentHitPoints}).");
                        highest.CurrentHitPoints = lowest.CurrentHitPoints;
                    }
                    break;
                case "Definitely Just a Mirror": //Mimi
                    LogLine("\tPlay this card as a copy of any other shield card in play");
                    var (_, c3) = GetMaxShieldCardFromOpponents(creature);
                    if (c3!= null)
                    {
                        PlayCard(creature, c3);
                    }
                    break;
                case "Swapportunity": //M&B
                    LogLine("\tEach player gives their hit point total to the player on their right");
                    int tmpHitPoints = 0;
                    var players = _creatures.Where(x => x.CurrentHitPoints > 0).ToList();
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (i == 0)
                        {
                            tmpHitPoints = players[i].CurrentHitPoints;
                        }

                        if (i < players.Count - 1)
                        {
                            players[i].CurrentHitPoints = players[i + 1].CurrentHitPoints;
                        }
                        else
                        {
                            players[i].CurrentHitPoints = tmpHitPoints;
                        }
                    }
                    break;
                case "Scouting Outing": //M&B
                    LogLine("\tDraw a card from the top of each opponent's deck");
                    foreach (var creature1 in _creatures.Where(x => x.CurrentHitPoints > 0))
                    {
                        var card2 = creature1.DrawDeck.CardDeck.FirstOrDefault();
                        if (card2 == null)
                        {
                            if (creature1.DiscardDeck.CardDeck.Any())
                            {
                                creature1.DiscardDeck.CardDeck.Shuffle();
                                card2 = creature1.DiscardDeck.CardDeck.FirstOrDefault();
                            }
                            else
                            {
                                creature1.InHandDeck.CardDeck.Shuffle();
                                card2 = creature1.InHandDeck.CardDeck.FirstOrDefault();
                            }
                        }

                        if (card2 == null)
                        {
                            LogLine($"\t{creature1.CreatureName} does not have any cards to draw from");
                        }
                        else
                        {
                            creature.InHandDeck.CardDeck.Add(card2);
                        }
                    }
                    break;
                case "Favored Frienemies": //M&B
                    LogLine("\tYour attack cards deal one bonus damage this turn");
                    _specialAttackBonusDamageOverride = true;
                    _currentTurn = creature;
                    break;
                default:
                    LogLine("\tMighty Power Not Found!");
                    break;
            }
        }

        private (Creature, Card) GetMaxShieldCardFromOpponents(Creature creature)
        {
            var charsWithShields = GetOpponents(creature).Where(x => x.NumberOfShields > 0).ToList();

            if (!charsWithShields.Any())
            {
                LogLine("\tNo shields in play.");
                return (null, null);
            }

            int maxShields = 0;
            Creature maxCreature = null;
            Card maxCard = null;

            foreach (var charWithShield in charsWithShields)
            {
                foreach (var card in charWithShield.ShieldDeck.CardDeck)
                {
                    int numShields = card.Actions.Count(action => action.ActionType == ActionType.Block);

                    if (numShields > maxShields)
                    {
                        maxShields = numShields;
                        maxCreature = charWithShield;
                        maxCard = card;
                    }
                }
            }

            return (maxCreature, maxCard);
        }

        private IEnumerable<Creature> GetOpponents(Creature creature)
        {
            return _creatures.Where(x => x != creature && x.CurrentHitPoints > 0);
        }

        private IEnumerable<Creature> GetAliveCreatures()
        {
            return _creatures.Where(x => x.CurrentHitPoints > 0);
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
}
