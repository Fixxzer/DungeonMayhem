using System.Text;
using DungeonMayhem.Library;

namespace Game.Winforms
{
	public partial class Form1 : Form
	{
		private List<Creature> _creatures;
		private Stack<Creature> _defeated;
		private bool _useMightyPowers;
		private bool _specialAttackAllOverride;
		private bool _specialAttackBonusDamageOverride;
		private Creature _currentTurn;
		private Creature _specialAttackSpecificOverride;
		private bool _isFirstTurn;
		private bool _specialUntargetable = false;
		private Creature _untargetableCreature;

		private Creature _currentCreature;
		private Card _currentCard;

		public Form1()
		{
			InitializeComponent();

			listBoxCharacterName.DisplayMember = "CreatureName";
			listBoxHealth.DisplayMember = "CurrentHitPoints";
			listBoxShields.DisplayMember = "NumberOfShields";
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			ShowOptions();
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowOptions();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void ShowOptions()
		{
			var form = new FormNewGame();
			if (form.ShowDialog() == DialogResult.OK)
			{
				_creatures = form.Creatures;
				_defeated = new Stack<Creature>(_creatures.Count);
				_useMightyPowers = form.UseMightyPowers;

				StartGame();
			}
		}

		private void StartGame()
		{
			_creatures.Shuffle();
			DisplayCharacterStatus();

			// Shuffle Draw deck
			foreach (var creature in _creatures)
			{
				creature.DrawDeck.CardDeck.Shuffle();
			}

			var round = 1;
			labelRoundCounter.Text = $"Round {round}";

			//while (true)
			{
				foreach (var creature in _creatures)
				{
					if (creature.CurrentHitPoints > 0)
					{
						labelPlayersTurnName.Text = $"It's {creature.CreatureName}'s turn";

						DrawCards(creature, round);

						// Play a card
						SelectCard(creature);
					}
					else
					{
						// This is needed to toggle off the special abilities of the defeated players
						SelectCard(creature);
					}


				}

				round++;
			}
		}

		private void DisplayCharacterStatus()
		{
			listBoxCharacterName.Items.Clear();
			listBoxHealth.Items.Clear();
			listBoxShields.Items.Clear();

			foreach (var creature in _creatures)
			{
				listBoxCharacterName.Items.Add(creature);
				listBoxHealth.Items.Add(creature);
				listBoxShields.Items.Add(creature);
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

		private void DrawCards(Creature creature, int round)
		{
			// Draw a card, 3 cards for round 1, only 1 card for every round after
			for (int i = 0; i < (round == 1 ? 3 : 1); i++)
			{
				var card = creature.AddCardFromDrawDeckToInHandDeck();
				if (card == null)
					return;

				creature.InHandDeck.CardDeck.Add(card);
			}
		}

		private void SelectCard(Creature creature, Card specificCard = null)
		{
			if (creature.CurrentHitPoints > 0)
			{
				labelPlayersTurnName.Text = creature.CreatureName;
				_currentCreature = creature;
				_currentCard = specificCard;

				DisplayHand(creature);

				if (specificCard != null)
				{
					LogLine($"You must play {specificCard}");
					//LogLine("Press any key to continue...");
					//Console.ReadKey();
					listBoxHand.Items.Clear();
					listBoxHand.Items.Add(specificCard);
				}
				else
				{
					LogLine("Please select a card to play, select the line and press the play card button");
				}

				if (!creature.IsHuman)
				{
					PlayCard(creature, specificCard);
				}
			}
		}

		private void DisplayHand(Creature creature)
		{
			listBoxHand.Items.Clear();
			//int optionNum = 1;
			foreach (var handCard in creature.InHandDeck.CardDeck)
			{
				//LogLine($"{optionNum++} - {handCard}");
				listBoxHand.Items.Add(handCard);
			}
		}

		private void buttonPlayCard_Click(object sender, EventArgs e)
		{
			var creature = _currentCreature;
			var specificCard = _currentCard;

			//int? selectedNum = null;
			//while (selectedNum == null)
			if (listBoxHand.SelectedItem == null)
			{
				//LogLine("Please select a card to play, type the number and press the <enter> key to choose");
				LogLine("Please select a card to play, select the line and press the play card button");
				return;
			}

			specificCard = (Card)listBoxHand.SelectedItem;

			PlayCard(creature, specificCard);
			//string option = Console.ReadLine();
			//selectedNum = int.Parse(option);

			//if (selectedNum <= 0 || selectedNum > creature.InHandDeck.CardDeck.Count)
			//{
			//    LogLine();
			//    LogLine("Invalid selection, please try again.");
			//    selectedNum = null;
			//}

			//var selectedCard = creature.InHandDeck.CardDeck[selectedNum.Value - 1];
			//specificCard = selectedCard;
		}

		private void PlayCard(Creature creature, Card specificCard = null)
		{
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

			if (_specialUntargetable && _untargetableCreature == creature)
			{
				_specialUntargetable = false;
				_untargetableCreature = null;
			}

			// This is needed to let the special attacks reset
			if (creature.CurrentHitPoints <= 0)
			{
				return;
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
						SelectCard(creature);
						break;
					case ActionType.MightyPower:
						LogMessage("*Mighty Power! ");
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

			// Evaluate Game End scenario
			var aliveCreatures = _creatures.Where(x => x.CurrentHitPoints > 0).ToList();
			if (aliveCreatures.Count == 1)
			{
				Winner(aliveCreatures.FirstOrDefault());
			}

			if (!aliveCreatures.Any())
			{
				NoWinner();
			}

			DisplayCharacterStatus();
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

			// Check again if out of cards, draw twice
			if (creature.InHandDeck.CardDeck.Count <= 0)
			{
				DrawCard(creature);
				DrawCard(creature);
			}

			// Otherwise randomize
			creature.InHandDeck.CardDeck.Shuffle();
			var card = creature.InHandDeck.CardDeck.FirstOrDefault();
			if (card == null)
			{
				Console.WriteLine();
			}

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

		public void Attack(Creature attacker, AttackType attackType, Creature target = null, bool bypassShields = false, bool isMightyPower = false)
		{
			var attackTargets = new List<Creature>();

			if (_specialAttackAllOverride && !isMightyPower)
			{
				attackType = AttackType.Opponents;
			}

			if (_specialAttackSpecificOverride != null && !isMightyPower)
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

					if (attacker.IsHuman)
					{
						DisplayCreatureList(attackList);

						while (listBoxCharacterName.SelectedItem == null)
						{
							LogLine($"{attacker.CreatureName} who would you like to attack? Please select a character from the character list");
							Thread.Sleep(1000);
						}

						//while (true)
						//{
						//    LogLine($"{attacker.CreatureName} who would you like to attack?");
						//    var choice = Console.ReadLine();

						//    if (int.TryParse(choice, out var result))
						//    {
						//        if (result > 0 && result <= attackList.Count)
						//        {
						//            attackTargets.Add(attackList[result - 1]);
						//            break;
						//        }
						//    }

						//    LogLine("Unable to determine target, please try again.");
						//}

						attackTargets.Add((Creature)listBoxCharacterName.SelectedItem);
					}
					else
					{
						attackTargets.Add(attackList.OrderByDescending(x => x.CurrentHitPoints).First());
					}
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

			if (_specialAttackBonusDamageOverride && !isMightyPower)
			{
				attackTargets.Double();
			}

			foreach (var attackedCreature in attackTargets.OrderBy(x => x.CreatureName))
			{
				LogMessage($"\t{attacker.CreatureName} attacks {attackedCreature.CreatureName}");
				if (attackedCreature.CurrentHitPoints <= 0)
				{
					//LogLine($" - {attackedCreature.CreatureName} has been defeated, and can no longer be attacked.");
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

		#region MightyPowers

		private void MightyPower(Creature creature, Card card)
		{
			if (!_useMightyPowers)
			{
				return;
			}

			switch (card.Name)
			{
				case "Vampiric Touch": //Azzan
					AzzanVampiricTouch(creature);
					break;
				case "Fireball": //Azzan
					AzzanFireball(creature);
					break;
				case "Charm": //Azzan
					AzzanCharm(creature);
					break;
				case "Hugs!": //Blorp
					BlorpHugs(creature);
					break;
				case "Burped-Up Bones": //Blorp
					BlorpBurpedUpBones();
					break;
				case "Here I Come!": //Blorp
					BlorpHereICome();
					break;
				case "Praise Me": //Delilah
					DelilahPraiseMe();
					break;
				case "Death Ray": //Delilah
					DelilahDeathRay(creature);
					break;
				case "Charm Ray": //Delilah
					DelilahCharmRay(creature);
					break;
				case "Mind Blast": //Dr. T
					DrTMindBlast(creature);
					break;
				case "Mind Games": //Dr. T
					DrTMindGames(creature);
					break;
				case "Tell Me About Your Mother": // Dr. T
					DrTTellMeAboutYourMother(creature);
					break;
				case "For My Next Trick...": //Hoots
					HootsForMyNextTrick(creature);
					break;
				case "To The Face!": //Hoots
					HootsToTheFace(creature);
					break;
				case "Owlbear Boogie": //Hoots
					HootsOwlbearBoogie(creature);
					break;
				case "Primal Strike": //Jaheira
					JaheiraPrimalStrike(creature);
					break;
				case "Commune With Nature": //Jaheira
					JaheiraCommuneWithNature(creature);
					break;
				case "Divine Inspiration": //Lia
					LiaDivineInspiration(creature);
					break;
				case "Banishing Smite": //Lia
					LiaBanishingSmite();
					break;
				case "Liquidate Assets": //Lord C
					LordCinderpuffLiquidateAssets(creature);
					break;
				case "Hostile Takeover": //Lord C
					LordCinderpuffHostileTakeover(creature);
					break;
				case "Murders and Acquisitions": //Lord C
					LordCinderpuffMurdersAndAcquisitions(creature);
					break;
				case "A Book (Cannot Bite)": //Mimi
					MimiABookCannotBite(creature);
					break;
				case "It's Not a Trap": //Mimi
					MimiItsNotATrap(creature);
					break;
				case "Definitely Just a Mirror": //Mimi
					MimiDefinitelyJustAMirror(creature);
					break;
				case "Swapportunity": //M&B
					MinscAndBooSwapportunity();
					break;
				case "Scouting Outing": //M&B
					MinscAndBooScoutingOuting(creature);
					break;
				case "Favored Frienemies": //M&B
					MinscAndBooFavoredFrienemies(creature);
					break;
				case "Clever Disguise": //Oriax
					OriaxCleverDisguise(creature);
					break;
				case "Sneak Attack!": //Oriax
					OriaxSneakAttack(creature);
					break;
				case "Pick Pocket": //Oriax
					OriaxPickPocket(creature);
					break;
				case "Whirling Axes": //Sutha
					SuthaWhirlingAxes(creature);
					break;
				case "Battle Roar": //Sutha
					SuthaBattleRoar();
					break;
				case "Mighty Toss": //Sutha
					SuthaMightyToss(creature);
					break;
				default:
					LogLine("\tMighty Power Not Found!");
					break;
			}
		}

		private void AzzanVampiricTouch(Creature creature)
		{
			LogLine(Abilities.GetAzzanVampiricTouchText());

			var ops = GetOpponents(creature);
			var maxHpOps = ops.OrderByDescending(x => x.CurrentHitPoints).FirstOrDefault();
			if (maxHpOps == null)
			{
				LogLine("\tTarget does not exist");
				return;
			}

			int tmp = creature.CurrentHitPoints;
			creature.CurrentHitPoints = maxHpOps.CurrentHitPoints;
			maxHpOps.CurrentHitPoints = tmp;

			LogLine($"\t{creature.CreatureName} swapped hit points with {maxHpOps.CreatureName}");
			LogLine($"\t{creature.CreatureName} has {creature.CurrentHitPoints} hit points, {maxHpOps.CreatureName} has {maxHpOps.CurrentHitPoints} hit points.");
		}

		private void AzzanFireball(Creature creature)
		{
			LogLine(Abilities.GetAzzanFireballText());

			foreach (var c in _creatures.Where(x => x.CurrentHitPoints > 0))
			{
				for (int i = 0; i < 3; i++)
				{
					if (c.CurrentHitPoints > 0)
					{
						Attack(creature, AttackType.Specific, c, false, true);

						if (creature == c && c.CurrentHitPoints <= 0)
						{
							LogLine("\tHa ha ha, you just killed yourself!");
						}
					}
				}

				LogLine();
			}
		}

		private void AzzanCharm(Creature creature)
		{
			LogLine(Abilities.GetAzzanCharmText());

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
		}

		private void BlorpHugs(Creature creature)
		{
			LogLine(Abilities.GetBlorpHugsText());

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
		}

		private void BlorpBurpedUpBones()
		{
			LogLine(Abilities.GetBlorpBurpedUpBonesText());

			// Actions are on the card
		}

		private void BlorpHereICome()
		{
			LogLine(Abilities.GetBlorpHereIComeText());

			// Actions are on the card
		}

		private void DelilahPraiseMe()
		{
			LogLine(Abilities.GetDelilahPraiseMeText());

			// Actions are on the card
		}

		private void DelilahDeathRay(Creature creature)
		{
			LogLine(Abilities.GetDelilahDeathRayText());

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
		}

		private void DelilahCharmRay(Creature creature)
		{
			LogLine(Abilities.GetDelilahCharmRayText());

			var t = GetOpponents(creature).FirstOrDefault();
			if (t == null)
			{
				LogLine("\tNo opponents remaining");
				return;
			}

			LogLine($"\t{t.CreatureName} will be the target of all attack cards until your next turn.");
			_specialAttackSpecificOverride = t;
			_currentTurn = creature;
			_isFirstTurn = true;
		}

		private void DrTMindBlast(Creature creature)
		{
			LogLine(Abilities.GetDrTMindBlastText());

			var chars = GetOpponents(creature).ToList();
			foreach (var target in chars)
			{
				for (int i = 0; i < target.InHandDeck.CardDeck.Count; i++)
				{
					Attack(creature, AttackType.Specific, target);
				}
			}
		}

		private void DrTMindGames(Creature creature)
		{
			LogLine(Abilities.GetDrTMindGamesText());

			var targetList = GetOpponents(creature).ToList();
			targetList.Shuffle();
			var t1 = targetList.FirstOrDefault();
			if (t1 == null)
			{
				LogLine("\tNo target found");
				return;
			}

			var tmpInHandDeck = creature.InHandDeck;
			creature.InHandDeck = t1.InHandDeck;
			t1.InHandDeck = tmpInHandDeck;
			LogLine($"\tSwapping hands with {t1.CreatureName}");
		}

		private void DrTTellMeAboutYourMother(Creature creature)
		{
			LogLine(Abilities.GetDrTTellMeAboutYourMotherText());

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
		}

		private void HootsForMyNextTrick(Creature creature)
		{
			LogLine(Abilities.GetHootsForMyNextTrickText());

			_specialAttackAllOverride = true;
			_currentTurn = creature;
		}

		private void HootsToTheFace(Creature creature)
		{
			LogLine(Abilities.GetHootsToTheFaceText());

			var (numOfShieldsOnCard, highestShieldCharacter) = DestroyShieldCardInPlay(creature);

			// Attack for each starting shield
			for (int i = 0; i < numOfShieldsOnCard; i++)
			{
				Attack(creature, AttackType.Specific, highestShieldCharacter);
			}
		}

		private void HootsOwlbearBoogie(Creature creature)
		{
			LogLine(Abilities.GetHootsOwlbearBoogieText());

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
		}

		private void JaheiraPrimalStrike(Creature creature)
		{
			LogLine(Abilities.GetJaheiraPrimalStrikeText());

			Attack(creature, AttackType.Opponents, null, false, true);
		}

		private void JaheiraCommuneWithNature(Creature creature)
		{
			LogLine(Abilities.GetJaheiraCommuneWithNatureText());

			var shapeshiftCardInHand = creature.InHandDeck.CardDeck.FirstOrDefault(cardInHand => cardInHand.Name.StartsWith("Shapeshift"));

			if (shapeshiftCardInHand == null)
			{
				LogLine("\tNo shapeshift cards in hand");
			}
			else
			{
				SelectCard(creature, shapeshiftCardInHand);
			}
		}

		private void LiaDivineInspiration(Creature creature)
		{
			LogLine(Abilities.GetLiaDivineInspirationText());

			if (creature.DiscardDeck.CardDeck.Count == 0)
			{
				LogLine("\tNo cards in discard pile to choose from");
				return;
			}

			int rand = new Random().Next(0, creature.DiscardDeck.CardDeck.Count);
			var randomDiscard = creature.DiscardDeck.CardDeck[rand];
			creature.InHandDeck.CardDeck.Add(randomDiscard);
			creature.DiscardDeck.CardDeck.Remove(randomDiscard);
			LogLine($"\tMoving {randomDiscard.Name} to hand.");
		}

		private void LiaBanishingSmite()
		{
			LogLine(Abilities.GetLiaBanishingSmiteText());

			foreach (var creature1 in _creatures.Where(x => x.CurrentHitPoints > 0 && x.NumberOfShields > 0))
			{
				LogLine($"\tDestroying {creature1.NumberOfShields} of {creature1.CreatureName}'s shields.");
				creature1.DiscardDeck.CardDeck.AddRange(creature1.ShieldDeck.CardDeck);
				creature1.ShieldDeck.CardDeck.Clear();
				creature1.NumberOfShields = 0;
			}
		}

		private void LordCinderpuffLiquidateAssets(Creature creature)
		{
			LogLine(Abilities.GetLordCinderpuffLiquidateAssetsText());

			int numAttacks = creature.InHandDeck.CardDeck.Count;
			LogLine($"\tYou have {numAttacks} card(s) in your hand, and get to attack {numAttacks} time(s).");
			creature.DiscardDeck.CardDeck.AddRange(creature.InHandDeck.CardDeck);
			creature.InHandDeck.CardDeck.Clear();
			for (int i = 0; i < numAttacks; i++)
			{
				Attack(creature, AttackType.Random);
			}
		}

		private void LordCinderpuffHostileTakeover(Creature creature)
		{
			LogLine(Abilities.GetLordCinderpuffHostileTakeoverText());

			// Attack all
			LogLine();
			LogLine("\tAttacking all opponents");
			Attack(creature, AttackType.Opponents, null, false, true);

			// Attack one twice
			var ops = GetOpponents(creature).ToList();
			if (!ops.Any())
			{
				LogLine($"\tNo opponents remaining.");
				return;
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
				return;
			}

			LogLine();
			LogLine($"\tAttacking someone different, attacking {op.CreatureName}");

			Attack(creature, AttackType.Specific, op);
		}

		private void LordCinderpuffMurdersAndAcquisitions(Creature creature)
		{
			LogLine(Abilities.GetLordCinderpuffMurdersAndAcquisitionsText());

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
		}

		private void MimiABookCannotBite(Creature creature)
		{
			LogLine(Abilities.GetMimiABookCannotBiteText());

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
				SelectCard(creature, mightyPowerCard);
				return;
			}

			mightyPowerCard = previousCreature.DiscardDeck.CardDeck.FirstOrDefault(card1 => card1.Actions.Any(action => action.ActionType == ActionType.MightyPower));
			if (mightyPowerCard != null)
			{
				previousCreature.DiscardDeck.CardDeck.Remove(mightyPowerCard);
				SelectCard(creature, mightyPowerCard);
				return;
			}

			mightyPowerCard = previousCreature.InHandDeck.CardDeck.FirstOrDefault(card1 => card1.Actions.Any(action => action.ActionType == ActionType.MightyPower));
			if (mightyPowerCard != null)
			{
				previousCreature.InHandDeck.CardDeck.Remove(mightyPowerCard);
				SelectCard(creature, mightyPowerCard);
				return;
			}
		}

		private void MimiItsNotATrap(Creature creature)
		{
			LogLine(Abilities.GetMimiItsNotATrapText());

			var aliveCreatures = _creatures.Where(x => x.CurrentHitPoints > 0 && x != creature).OrderBy(x => x.CurrentHitPoints).ToList();
			var lowest = aliveCreatures.FirstOrDefault();
			var highest = aliveCreatures.LastOrDefault();

			if (lowest != null && highest != null)
			{
				LogLine($"\tMaking {highest.CreatureName}'s hit points ({highest.CurrentHitPoints}) equal to {lowest.CreatureName}'s hit points ({lowest.CurrentHitPoints}).");
				highest.CurrentHitPoints = lowest.CurrentHitPoints;
			}
		}

		private void MimiDefinitelyJustAMirror(Creature creature)
		{
			LogLine(Abilities.GetMimiDefinitelyJustAMirrorText());

			var (_, c3) = GetMaxShieldCardFromOpponents(creature);
			if (c3 != null)
			{
				SelectCard(creature, c3);
			}
		}

		private void MinscAndBooSwapportunity()
		{
			LogLine(Abilities.GetMinscAndBooSwapportunityText());

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
		}

		private void MinscAndBooScoutingOuting(Creature creature)
		{
			LogLine(Abilities.GetMinscAndBooScoutingOutingText());

			foreach (var creature1 in GetOpponents(creature))
			{
				var card2 = creature1.DrawDeck.CardDeck.FirstOrDefault();
				if (card2 != null)
				{
					creature1.DrawDeck.CardDeck.Remove(card2);
				}

				if (card2 == null)
				{
					if (creature1.DiscardDeck.CardDeck.Any())
					{
						creature1.DiscardDeck.CardDeck.Shuffle();
						card2 = creature1.DiscardDeck.CardDeck.FirstOrDefault();

						if (card2 != null)
						{
							creature1.DiscardDeck.CardDeck.Remove(card2);
						}
					}
					else
					{
						creature1.InHandDeck.CardDeck.Shuffle();
						card2 = creature1.InHandDeck.CardDeck.FirstOrDefault();

						if (card2 != null)
						{
							creature1.InHandDeck.CardDeck.Remove(card2);
						}
					}
				}

				if (card2 == null)
				{
					LogLine($"\t{creature1.CreatureName} does not have any cards to draw from");
				}
				else
				{
					creature.InHandDeck.CardDeck.Add(card2);
					LogLine($"{creature.CreatureName} draws {card2.Name} from {creature1.CreatureName}");
				}
			}
		}

		private void MinscAndBooFavoredFrienemies(Creature creature)
		{
			LogLine(Abilities.GetMinscAndBooFavoredFrienemiesText());

			_specialAttackBonusDamageOverride = true;
			_currentTurn = creature;
		}

		private void OriaxCleverDisguise(Creature creature)
		{
			LogLine(Abilities.GetOriaxCleverDisguiseText());

			_specialUntargetable = true;
			_untargetableCreature = creature;
			_currentTurn = creature;
		}

		private void OriaxSneakAttack(Creature creature)
		{
			LogLine(Abilities.GetOriaxSneakAttackText());

			DestroyShieldCardInPlay(creature);

			// Play again action is on the card
		}

		private void OriaxPickPocket(Creature creature)
		{
			LogLine(Abilities.GetOriaxPickPocketText());

			var ops2 = GetOpponents(creature).ToList();
			ops2.Shuffle();
			var creatureToTarget = ops2.FirstOrDefault(x => x.DrawDeck.CardDeck.Count > 0);
			if (creatureToTarget == null)
			{
				LogLine("\tNo targets available");
				return;
			}

			var cardToPlay = creatureToTarget.DrawDeck.CardDeck.FirstOrDefault();
			if (cardToPlay == null)
			{
				LogLine("\tNo cards available");
				return;
			}

			creatureToTarget.DrawDeck.CardDeck.Remove(cardToPlay);

			LogLine($"\t{creature.CreatureName} drew {cardToPlay.Name} from {creatureToTarget.CreatureName}.");

			SelectCard(creature, cardToPlay);
		}

		private void SuthaWhirlingAxes(Creature creature)
		{
			LogLine(Abilities.GetSuthaWhirlingAxesText());

			foreach (var opponent in GetOpponents(creature))
			{
				Heal(creature);
				Attack(creature, AttackType.Specific, opponent, false, true);
			}
		}

		private void SuthaBattleRoar()
		{
			LogLine(Abilities.GetSuthaBattleRoarText());

			foreach (var c in _creatures)
			{
				c.DiscardDeck.CardDeck.AddRange(c.InHandDeck.CardDeck);
				c.InHandDeck.CardDeck.Clear();

				for (int i = 0; i < 3; i++)
				{
					DrawCard(c);
				}
			}
			// Play again action is on the card
		}

		private void SuthaMightyToss(Creature creature)
		{
			LogLine(Abilities.GetSuthaMightyTossText());

			DestroyShieldCardInPlay(creature);
			// Draw action is on the card
		}

		#endregion

		private (int? numOfShieldsOnCard, Creature highestShieldCharacter) DestroyShieldCardInPlay(Creature creature)
		{
			var (highestShieldCharacter, highestShieldCard) = GetMaxShieldCardFromOpponents(creature);
			if (highestShieldCharacter == null)
			{
				LogLine("\tThere are not any shields in play");
				return (null, null);
			}

			var numOfShieldsOnCard = highestShieldCard.Actions.Count(x => x.ActionType == ActionType.Block);
			highestShieldCharacter.NumberOfShields -= numOfShieldsOnCard;
			if (highestShieldCharacter.NumberOfShields < 0)
			{
				highestShieldCharacter.NumberOfShields = 0;
			}

			LogLine($"\t{highestShieldCharacter.CreatureName} gets {numOfShieldsOnCard} shield(s) destroyed.");
			highestShieldCharacter.DiscardShieldCard(highestShieldCard);

			return (numOfShieldsOnCard, highestShieldCharacter);
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
			return _specialUntargetable ? _creatures.Where(x => x != creature && x != _untargetableCreature && x.CurrentHitPoints > 0) : _creatures.Where(x => x != creature && x.CurrentHitPoints > 0);
		}

		private IEnumerable<Creature> GetAliveCreatures()
		{
			return _creatures.Where(x => x.CurrentHitPoints > 0);
		}

		private void LogLine()
		{
			//if (_useConsoleLogs)
			{
				//Console.WriteLine();
				textBoxMessages.Text += Environment.NewLine;
			}
		}

		private void LogLine(string message)
		{
			//if (_useConsoleLogs)
			{
				//Console.WriteLine(message);
				textBoxMessages.Text = message + Environment.NewLine;
			}
		}

		private void LogMessage(string message)
		{
			//if (_useConsoleLogs)
			{
				//Console.Write(message);
				textBoxMessages.Text += message;
			}
		}

		private void DisplayCreatureList(IEnumerable<Creature> creatureList)
		{
			LogLine();
			int count = 1;
			foreach (var creature in creatureList)
			{
				LogLine($"{count++} - {creature.CreatureName}");
			}

			LogLine();
		}
	}
}
