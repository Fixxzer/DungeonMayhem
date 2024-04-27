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
		private bool _isGameOver = false;

		private Creature _currentCreature;
		private Card _currentCard;
		private int _round;
		private int _turn;

		public Form1()
		{
			InitializeComponent();

			listBoxCharacterName.DisplayMember = "CreatureName";
			listBoxHealth.DisplayMember = "CurrentHitPoints";
			listBoxShields.DisplayMember = "NumberOfShields";
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			//ShowOptions();
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

		public void StartGame()
		{
			_creatures.Shuffle();

			// Shuffle Draw deck
			foreach (var creature in _creatures)
			{
				creature.DrawDeck.CardDeck.Shuffle();
			}

			_round = 0;
			_turn = 0;

			IncrementRound();
		}

		private void IncrementRound()
		{
			DisplayCharacterStatus();

			if (_currentCreature == null || _creatures.IndexOf(_currentCreature) == _creatures.Count - 1)
			{
				_currentCreature = _creatures.First();
				_round++;
			}
			else
			{
				_currentCreature = _creatures[_creatures.IndexOf(_currentCreature) + 1];
			}

			_turn++;
			labelRoundCounter.Text = $"Round {_round}, Turn {_turn}";
			
			PlayTurn();
		}

		private void PlayTurn()
		{
			if (_currentCreature.CurrentHitPoints > 0)
			{
				string message = $"It's {_currentCreature.CreatureName}'s turn";
				labelPlayersTurnName.Text = message;
				MessageBox.Show(message);

				DrawCards();

				// We need to end the call on this, because it waits for the UI.  Note - also plays card for non-human players.
				SelectCard();
			}
			else
			{
				// This is needed to toggle off the special abilities of the defeated players
				SelectCard();
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

			_isGameOver = true;

			return winnerList;
		}

		private List<Creature> Winner(Creature winner)
		{
			List<Creature> winnerList = new List<Creature> { winner };

			StringBuilder sb = new StringBuilder();
			sb.AppendLine();
			sb.AppendLine("********************************************************************************************************************");
			sb.AppendLine($"Congratulations {winner.CreatureName}, you are the winner, with {winner.CurrentHitPoints} hit points and {winner.NumberOfShields} shield(s) remaining!");
			sb.AppendLine();
			sb.AppendLine("The following places:");
			sb.AppendLine($"{1} - {winner.CreatureName}");
			int count = 2;
			foreach (var loser in _defeated)
			{
				sb.AppendLine($"{count++} - {loser.CreatureName}");
				winnerList.Add(loser);
			}
			sb.AppendLine("********************************************************************************************************************");
			sb.AppendLine();

			LogLine(sb.ToString());

			_isGameOver = true;

			return winnerList;
		}

		private void DrawCards()
		{
			// Draw a card, 3 cards for round 1, only 1 card for every round after
			for (int i = 0; i < (_round == 1 ? 3 : 1); i++)
			{
				var card = _currentCreature.AddCardFromDrawDeckToInHandDeck();
				if (card == null)
					return;
			}
		}

		private void SelectCard()
		{
			DisplayHand();
			if (_currentCreature.IsHuman)
			{

				LogLine("Please select a card to play, select the line and press the play card button");
			}
			else
			{
				_currentCard = RetrieveCardFromHand();
				PlayCard(_currentCard);

				// Play next round
				IncrementRound();
			}
		}

		private void DisplayHand()
		{
			listBoxHand.Items.Clear();
			//int optionNum = 1;
			foreach (var handCard in _currentCreature.InHandDeck.CardDeck)
			{
				//LogLine($"{optionNum++} - {handCard}");
				listBoxHand.Items.Add(handCard);
			}
		}

		private void buttonPlayCard_Click(object sender, EventArgs e)
		{
			//int? selectedNum = null;
			//while (selectedNum == null)
			if (listBoxHand.SelectedItem == null)
			{
				//LogLine("Please select a card to play, type the number and press the <enter> key to choose");
				LogLine("Please select a card to play, select the line and press the play card button");
				return;
			}

			_currentCard = (Card)listBoxHand.SelectedItem;

			PlayCard(_currentCard);

			// Play next round
			IncrementRound();


			//string option = Console.ReadLine();
			//selectedNum = int.Parse(option);

			//if (selectedNum <= 0 || selectedNum > _currentCreature.InHandDeck.CardDeck.Count)
			//{
			//    LogLine();
			//    LogLine("Invalid selection, please try again.");
			//    selectedNum = null;
			//}

			//var selectedCard = _currentCreature.InHandDeck.CardDeck[selectedNum.Value - 1];
			//specificCard = selectedCard;

			//PlayTurn(NextCreature());
		}

		private void PlayCard(Card? specificCard = null)
		{
			if (_specialAttackAllOverride && _currentTurn != _currentCreature)
			{
				_specialAttackAllOverride = false;
			}

			if (_specialAttackBonusDamageOverride && _currentTurn != _currentCreature)
			{
				_specialAttackBonusDamageOverride = false;
			}

			if (_specialAttackSpecificOverride != null && _currentTurn != _currentCreature)
			{
				_isFirstTurn = false;
			}

			if (!_isFirstTurn && _specialAttackSpecificOverride != null && _currentTurn == _currentCreature)
			{
				_specialAttackSpecificOverride = null;
			}

			if (_specialUntargetable && _untargetableCreature == _currentCreature)
			{
				_specialUntargetable = false;
				_untargetableCreature = null;
			}

			// This is needed to let the special attacks reset
			if (_currentCreature.CurrentHitPoints <= 0)
			{
				return;
			}

			var card = specificCard ?? RetrieveCardFromHand();
			_currentCreature.InHandDeck.CardDeck.Remove(card);
			_currentCreature.InPlayDeck.CardDeck.Add(card);

			LogLine();
			LogLine($"{_currentCreature.CreatureName} plays '{card.Name}'");

			if (card.Name.Contains("Shapeshift:"))
			{
				PlayShapeshift(card);
			}

			foreach (var action in card.Actions)
			{
				// Evaluate action
				switch (action.ActionType)
				{
					case ActionType.Draw:
						LogLine("*Draw a card");
						DrawCard();
						break;
					case ActionType.Block:
						LogLine("*Add 1 shield");
						PlayShieldCard(card);
						break;
					case ActionType.Damage:
						LogLine("*Deal 1 damage");
						Attack(AttackType.Random);
						break;
					case ActionType.Heal:
						LogLine("*Recover 1 hit point");
						Heal();
						break;
					case ActionType.PlayExtraCard:
						LogLine("*Play an extra card");
						SelectCard();
						break;
					case ActionType.MightyPower:
						LogMessage("*Mighty Power! ");
						MightyPower(card);
						break;
					case ActionType.DamageIgnoreShields:
						Attack(AttackType.Random, null, true);
						break;
					case ActionType.Shapeshift:
						switch (_currentCreature.ShapeShiftForm)
						{
							case ShapeshiftForm.Bear:
								LogLine("*Bear Form: Heal");
								Heal();
								break;
							case ShapeshiftForm.Wolf:
								LogLine("*Wolf Form: Attack");
								Attack(AttackType.Random);
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
				_currentCreature.RemoveCardFromInPlayDeck(card);
				_currentCreature.MoveCardToDiscardDeck(card);
			}

			if (card.Actions.Any(x => x.ActionType == ActionType.Block))
			{
				LogLine();
				LogLine($"\t{_currentCreature.CreatureName} now has {_currentCreature.NumberOfShields} shield(s)");
			}

			if (card.Actions.Any(x => x.ActionType == ActionType.Heal))
			{
				LogLine();
				LogLine($"\t{_currentCreature.CreatureName} now has {_currentCreature.CurrentHitPoints} hit point(s)");
			}

			// Check in-hand, if 0 - draw 2
			if (!_currentCreature.InHandDeck.CardDeck.Any())
			{
				DrawCard();
				DrawCard();
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

		private Card RetrieveCardFromHand()
		{
			// If out of cards, draw twice
			if (_currentCreature.InHandDeck.CardDeck.Count <= 0)
			{
				DrawCard();
				DrawCard();
			}

			// Prioritize play again cards
			var playAgainCard = GetCardWithMostActionItems(ActionType.PlayExtraCard);
			if (playAgainCard != null)
			{
				_currentCreature.InHandDeck.CardDeck.Remove(playAgainCard);
				return playAgainCard;
			}

			// If health is low, prioritize healing or shields
			if (_currentCreature.CurrentHitPoints <= 5)
			{
				int shieldCount = 0;
				int healingCount = 0;

				var shieldCard = GetCardWithMostActionItems(ActionType.Block);
				if (shieldCard != null)
				{
					shieldCount = shieldCard.Actions.Count(x => x.ActionType == ActionType.Block);
				}

				var healthCard = GetCardWithMostActionItems(ActionType.Heal);
				if (healthCard != null)
				{
					healingCount = healthCard.Actions.Count(x => x.ActionType == ActionType.Heal);
				}

				if (healingCount != 0 || shieldCount != 0)
				{
					if (healingCount >= shieldCount)
					{
						_currentCreature.InHandDeck.CardDeck.Remove(healthCard);
						return healthCard;
					}

					_currentCreature.InHandDeck.CardDeck.Remove(shieldCard);
					return shieldCard;
				}
			}

			// Check again if out of cards, draw twice
			if (_currentCreature.InHandDeck.CardDeck.Count <= 0)
			{
				DrawCard();
				DrawCard();
			}

			// Otherwise randomize
			_currentCreature.InHandDeck.CardDeck.Shuffle();
			var card = _currentCreature.InHandDeck.CardDeck.FirstOrDefault();
			if (card == null)
			{
				Console.WriteLine("Error getting cards!");
			}

			return card;
		}

		private Card GetCardWithMostActionItems(ActionType actionType)
		{
			List<Card> cards = new List<Card>();
			foreach (var card in _currentCreature.InHandDeck.CardDeck)
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

		private void PlayShapeshift(Card card)
		{
			if (card.Name.Contains("Bear"))
			{
				_currentCreature.ShapeShiftForm = ShapeshiftForm.Bear;
			}
			else if (card.Name.Contains("Wolf"))
			{
				_currentCreature.ShapeShiftForm = ShapeshiftForm.Wolf;
			}
		}

		public void Attack(AttackType attackType, Creature? target = null, bool bypassShields = false, bool isMightyPower = false, Creature? specificCreature = null)
		{
			specificCreature ??= _currentCreature;

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
					var attackList = GetOpponents().ToList();
					if (!attackList.Any())
					{
						break;
					}

					if (specificCreature.IsHuman)
					{
						DisplayCreatureList(attackList);

						while (listBoxCharacterName.SelectedItem == null)
						{
							LogLine($"{specificCreature.CreatureName} who would you like to attack? Please select a character from the character list");
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
					attackTargets.AddRange(GetOpponents());
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
				LogMessage($"\t{specificCreature.CreatureName} attacks {attackedCreature.CreatureName}");
				if (attackedCreature.CurrentHitPoints <= 0)
				{
					//LogLine($" - {attacked_currentCreature.CreatureName} has been defeated, and can no longer be attacked.");
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
					LogLine($"===== {attackedCreature.CreatureName} has been defeated by {specificCreature.CreatureName}! ======");

					_defeated.Push(attackedCreature);
				}
			}
		}

		public void Heal(Creature? specificCreature = null)
		{
			specificCreature ??= _currentCreature;
			specificCreature.Heal();
		}

		public void DrawCard(Creature? specificCreature = null)
		{
			specificCreature ??= _currentCreature;

			var card = specificCreature.AddCardFromDrawDeckToInHandDeck();
			if (card == null)
			{
				LogLine("**** There are no draw cards or discard cards to draw from. ****");
				return;
			}

			LogLine($"\t{specificCreature.CreatureName} draws {card.Name}.");
		}

		public void PlayShieldCard(Card card)
		{
			_currentCreature.PlayShieldCard(card);
		}

		#region MightyPowers

		private void MightyPower(Card card)
		{
			if (!_useMightyPowers)
			{
				return;
			}

			switch (card.Name)
			{
				case "Vampiric Touch": //Azzan
					AzzanVampiricTouch();
					break;
				case "Fireball": //Azzan
					AzzanFireball();
					break;
				case "Charm": //Azzan
					AzzanCharm();
					break;
				case "Hugs!": //Blorp
					BlorpHugs();
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
					DelilahDeathRay();
					break;
				case "Charm Ray": //Delilah
					DelilahCharmRay();
					break;
				case "Mind Blast": //Dr. T
					DrTMindBlast();
					break;
				case "Mind Games": //Dr. T
					DrTMindGames();
					break;
				case "Tell Me About Your Mother": // Dr. T
					DrTTellMeAboutYourMother();
					break;
				case "For My Next Trick...": //Hoots
					HootsForMyNextTrick();
					break;
				case "To The Face!": //Hoots
					HootsToTheFace();
					break;
				case "Owlbear Boogie": //Hoots
					HootsOwlbearBoogie();
					break;
				case "Primal Strike": //Jaheira
					JaheiraPrimalStrike();
					break;
				case "Commune With Nature": //Jaheira
					JaheiraCommuneWithNature();
					break;
				case "Divine Inspiration": //Lia
					LiaDivineInspiration();
					break;
				case "Banishing Smite": //Lia
					LiaBanishingSmite();
					break;
				case "Liquidate Assets": //Lord C
					LordCinderpuffLiquidateAssets();
					break;
				case "Hostile Takeover": //Lord C
					LordCinderpuffHostileTakeover();
					break;
				case "Murders and Acquisitions": //Lord C
					LordCinderpuffMurdersAndAcquisitions();
					break;
				case "A Book (Cannot Bite)": //Mimi
					MimiABookCannotBite();
					break;
				case "It's Not a Trap": //Mimi
					MimiItsNotATrap();
					break;
				case "Definitely Just a Mirror": //Mimi
					MimiDefinitelyJustAMirror();
					break;
				case "Swapportunity": //M&B
					MinscAndBooSwapportunity();
					break;
				case "Scouting Outing": //M&B
					MinscAndBooScoutingOuting();
					break;
				case "Favored Frienemies": //M&B
					MinscAndBooFavoredFrienemies();
					break;
				case "Clever Disguise": //Oriax
					OriaxCleverDisguise();
					break;
				case "Sneak Attack!": //Oriax
					OriaxSneakAttack();
					break;
				case "Pick Pocket": //Oriax
					OriaxPickPocket();
					break;
				case "Whirling Axes": //Sutha
					SuthaWhirlingAxes();
					break;
				case "Battle Roar": //Sutha
					SuthaBattleRoar();
					break;
				case "Mighty Toss": //Sutha
					SuthaMightyToss();
					break;
				default:
					LogLine("\tMighty Power Not Found!");
					break;
			}
		}

		private void AzzanVampiricTouch()
		{
			LogLine(Abilities.GetAzzanVampiricTouchText());

			var ops = GetOpponents();
			var maxHpOps = ops.OrderByDescending(x => x.CurrentHitPoints).FirstOrDefault();
			if (maxHpOps == null)
			{
				LogLine("\tTarget does not exist");
				return;
			}

			int tmp = _currentCreature.CurrentHitPoints;
			_currentCreature.CurrentHitPoints = maxHpOps.CurrentHitPoints;
			maxHpOps.CurrentHitPoints = tmp;

			LogLine($"\t{_currentCreature.CreatureName} swapped hit points with {maxHpOps.CreatureName}");
			LogLine($"\t{_currentCreature.CreatureName} has {_currentCreature.CurrentHitPoints} hit points, {maxHpOps.CreatureName} has {maxHpOps.CurrentHitPoints} hit points.");
		}

		private void AzzanFireball()
		{
			LogLine(Abilities.GetAzzanFireballText());

			foreach (var c in _creatures.Where(x => x.CurrentHitPoints > 0))
			{
				for (int i = 0; i < 3; i++)
				{
					if (c.CurrentHitPoints > 0)
					{
						Attack(AttackType.Specific, c, false, true);

						if (_currentCreature == c && c.CurrentHitPoints <= 0)
						{
							LogLine("\tHa ha ha, you just killed yourself!");
						}
					}
				}

				LogLine();
			}
		}

		private void AzzanCharm()
		{
			LogLine(Abilities.GetAzzanCharmText());

			// find the creature with the most shields
			var ops1 = GetOpponents().Where(x => x.NumberOfShields > 0);
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
					_currentCreature.NumberOfShields += shields;
					creatureWithMostShields.NumberOfShields -= shields;

					_currentCreature.ShieldDeck.CardDeck.AddRange(creatureWithMostShields.ShieldDeck.CardDeck);
					creatureWithMostShields.ShieldDeck.CardDeck.Clear();

					LogLine($"\t{_currentCreature.CreatureName} charms {shields} shield(s) from {creatureWithMostShields.CreatureName}");
				}
			}
		}

		private void BlorpHugs()
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

					_currentCreature.CurrentHitPoints += shields;
					if (_currentCreature.CurrentHitPoints > _currentCreature.MaxHitPoints)
					{
						_currentCreature.CurrentHitPoints = _currentCreature.MaxHitPoints;
					}

					LogLine($"\t{_currentCreature.CreatureName} destroys {shields} shield(s) from {creatureWithMostShields.CreatureName}, and now has {_currentCreature.CurrentHitPoints} hit points");
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

		private void DelilahDeathRay()
		{
			LogLine(Abilities.GetDelilahDeathRayText());

			var noShieldChars = _creatures.Where(x => x.NumberOfShields == 0 && x.CurrentHitPoints > 0 && x != _currentCreature).ToList();

			foreach (var c in noShieldChars)
			{
				for (int i = 0; i < 2; i++)
				{
					Attack(AttackType.Specific, c);
				}
			}

			foreach (var c1 in _creatures.Where(x => x.NumberOfShields > 0))
			{
				LogLine($"\tDestroying {c1.NumberOfShields} of {c1.CreatureName}'s shield(s).");
				c1.NumberOfShields = 0;
				c1.ShieldDeck.CardDeck.Clear();
			}
		}

		private void DelilahCharmRay()
		{
			LogLine(Abilities.GetDelilahCharmRayText());

			var firstOpponent = GetOpponents().FirstOrDefault();
			if (firstOpponent == null)
			{
				LogLine("\tNo opponents remaining");
				return;
			}

			LogLine($"\t{firstOpponent.CreatureName} will be the target of all attack cards until your next turn.");
			_specialAttackSpecificOverride = firstOpponent;
			_currentTurn = _currentCreature;
			//_isFirstTurn = true;
		}

		private void DrTMindBlast()
		{
			LogLine(Abilities.GetDrTMindBlastText());

			var chars = GetOpponents().ToList();
			foreach (var target in chars)
			{
				for (int i = 0; i < target.InHandDeck.CardDeck.Count; i++)
				{
					Attack(AttackType.Specific, target);
				}
			}
		}

		private void DrTMindGames()
		{
			LogLine(Abilities.GetDrTMindGamesText());

			var targetList = GetOpponents().ToList();
			targetList.Shuffle();
			var t1 = targetList.FirstOrDefault();
			if (t1 == null)
			{
				LogLine("\tNo target found");
				return;
			}

			(_currentCreature.InHandDeck, t1.InHandDeck) = (t1.InHandDeck, _currentCreature.InHandDeck);
			LogLine($"\tSwapping hands with {t1.CreatureName}");
		}

		private void DrTTellMeAboutYourMother()
		{
			LogLine(Abilities.GetDrTTellMeAboutYourMotherText());

			foreach (var opponent in GetOpponents())
			{
				var lastOrDefault = opponent.DiscardDeck.CardDeck.LastOrDefault();

				if (lastOrDefault == null)
				{
					LogLine($"\t{opponent.CreatureName} does not have any cards in their discard pile");
				}
				else
				{
					opponent.DiscardDeck.CardDeck.Remove(lastOrDefault);
					_currentCreature.InHandDeck.CardDeck.Add(lastOrDefault);
					LogLine($"\tAdding {lastOrDefault.Name} from {opponent.CreatureName} to hand.");
				}
			}
		}

		private void HootsForMyNextTrick()
		{
			LogLine(Abilities.GetHootsForMyNextTrickText());

			_specialAttackAllOverride = true;
			_currentTurn = _currentCreature;
		}

		private void HootsToTheFace()
		{
			LogLine(Abilities.GetHootsToTheFaceText());

			var (numOfShieldsOnCard, highestShieldCharacter) = DestroyShieldCardInPlay();

			// Attack for each starting shield
			for (int i = 0; i < numOfShieldsOnCard; i++)
			{
				Attack(AttackType.Specific, highestShieldCharacter);
			}
		}

		private void HootsOwlbearBoogie()
		{
			LogLine(Abilities.GetHootsOwlbearBoogieText());

			int count = 0;
			foreach (var opponent in GetOpponents())
			{
				DrawCard(opponent); //@todo: Revisit DrawCard to take a specific character
				count++;
			}

			for (int i = 0; i < count; i++)
			{
				DrawCard();
			}

			LogLine($"\tYou draw {count} card(s).");
		}

		private void JaheiraPrimalStrike()
		{
			LogLine(Abilities.GetJaheiraPrimalStrikeText());

			Attack(AttackType.Opponents, null, false, true);
		}

		private void JaheiraCommuneWithNature()
		{
			LogLine(Abilities.GetJaheiraCommuneWithNatureText());

			var shapeshiftCardInHand = _currentCreature.InHandDeck.CardDeck.FirstOrDefault(cardInHand => cardInHand.Name.StartsWith("Shapeshift"));

			if (shapeshiftCardInHand == null)
			{
				LogLine("\tNo shapeshift cards in hand");
			}
			else
			{
				PlayCard(shapeshiftCardInHand);
			}
		}

		private void LiaDivineInspiration()
		{
			LogLine(Abilities.GetLiaDivineInspirationText());

			if (_currentCreature.DiscardDeck.CardDeck.Count == 0)
			{
				LogLine("\tNo cards in discard pile to choose from");
				return;
			}

			int rand = new Random().Next(0, _currentCreature.DiscardDeck.CardDeck.Count);
			var randomDiscard = _currentCreature.DiscardDeck.CardDeck[rand];
			_currentCreature.InHandDeck.CardDeck.Add(randomDiscard);
			_currentCreature.DiscardDeck.CardDeck.Remove(randomDiscard);
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

		private void LordCinderpuffLiquidateAssets()
		{
			LogLine(Abilities.GetLordCinderpuffLiquidateAssetsText());

			int numAttacks = _currentCreature.InHandDeck.CardDeck.Count;
			LogLine($"\tYou have {numAttacks} card(s) in your hand, and get to attack {numAttacks} time(s).");
			_currentCreature.DiscardDeck.CardDeck.AddRange(_currentCreature.InHandDeck.CardDeck);
			_currentCreature.InHandDeck.CardDeck.Clear();
			for (int i = 0; i < numAttacks; i++)
			{
				Attack(AttackType.Random);
			}
		}

		private void LordCinderpuffHostileTakeover()
		{
			LogLine(Abilities.GetLordCinderpuffHostileTakeoverText());

			// Attack all
			LogLine();
			LogLine("\tAttacking all opponents");
			Attack(AttackType.Opponents, null, false, true);

			// Attack one twice
			var ops = GetOpponents().ToList();
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

				Attack(AttackType.Specific, t2);
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

			Attack(AttackType.Specific, op);
		}

		private void LordCinderpuffMurdersAndAcquisitions()
		{
			LogLine(Abilities.GetLordCinderpuffMurdersAndAcquisitionsText());

			// Start with me
			int r = new Random().Next(0, 3);
			switch (r)
			{
				case 0: //attack
					Attack(AttackType.Random);
					break;
				case 1: //heal
					Heal();
					break;
				case 2: //draw
					DrawCard();
					break;
			}

			// Go through rest of opponents and repeat their choice
			foreach (var opponent in GetOpponents())
			{
				r = new Random().Next(0, 3);
				switch (r)
				{
					case 0: //attack
						Attack(AttackType.Random, opponent);
						Attack(AttackType.Random);
						break;
					case 1: //heal
						Heal(opponent);
						Heal();
						break;
					case 2: //draw
						DrawCard(opponent);
						DrawCard();
						break;
				}
			}
		}

		private void MimiABookCannotBite()
		{
			LogLine(Abilities.GetMimiABookCannotBiteText());

			var indexOf = _creatures.IndexOf(_currentCreature);
			if (indexOf == 0)
			{
				indexOf = _creatures.Count;
			}

			var previousCreature = _creatures[indexOf - 1];

			Card mightyPowerCard = previousCreature.DrawDeck.CardDeck.FirstOrDefault(card1 => card1.Actions.Any(action => action.ActionType == ActionType.MightyPower));
			if (mightyPowerCard != null)
			{
				previousCreature.DrawDeck.CardDeck.Remove(mightyPowerCard);
				PlayCard(mightyPowerCard);
				return;
			}

			mightyPowerCard = previousCreature.DiscardDeck.CardDeck.FirstOrDefault(card1 => card1.Actions.Any(action => action.ActionType == ActionType.MightyPower));
			if (mightyPowerCard != null)
			{
				previousCreature.DiscardDeck.CardDeck.Remove(mightyPowerCard);
				PlayCard(mightyPowerCard);
				return;
			}

			mightyPowerCard = previousCreature.InHandDeck.CardDeck.FirstOrDefault(card1 => card1.Actions.Any(action => action.ActionType == ActionType.MightyPower));
			if (mightyPowerCard != null)
			{
				previousCreature.InHandDeck.CardDeck.Remove(mightyPowerCard);
				PlayCard(mightyPowerCard);
				return;
			}
		}

		private void MimiItsNotATrap()
		{
			LogLine(Abilities.GetMimiItsNotATrapText());

			var aliveCreatures = _creatures.Where(x => x.CurrentHitPoints > 0 && x != _currentCreature).OrderBy(x => x.CurrentHitPoints).ToList();
			var lowest = aliveCreatures.FirstOrDefault();
			var highest = aliveCreatures.LastOrDefault();

			if (lowest != null && highest != null)
			{
				LogLine($"\tMaking {highest.CreatureName}'s hit points ({highest.CurrentHitPoints}) equal to {lowest.CreatureName}'s hit points ({lowest.CurrentHitPoints}).");
				highest.CurrentHitPoints = lowest.CurrentHitPoints;
			}
		}

		private void MimiDefinitelyJustAMirror()
		{
			LogLine(Abilities.GetMimiDefinitelyJustAMirrorText());

			var (_, c3) = GetMaxShieldCardFromOpponents();
			if (c3 != null)
			{
				PlayCard(c3);
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

		private void MinscAndBooScoutingOuting()
		{
			LogLine(Abilities.GetMinscAndBooScoutingOutingText());

			foreach (var creature1 in GetOpponents())
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
					_currentCreature.InHandDeck.CardDeck.Add(card2);
					LogLine($"{_currentCreature.CreatureName} draws {card2.Name} from {creature1.CreatureName}");
				}
			}
		}

		private void MinscAndBooFavoredFrienemies()
		{
			LogLine(Abilities.GetMinscAndBooFavoredFrienemiesText());

			_specialAttackBonusDamageOverride = true;
			_currentTurn = _currentCreature;
		}

		private void OriaxCleverDisguise()
		{
			LogLine(Abilities.GetOriaxCleverDisguiseText());

			_specialUntargetable = true;
			_untargetableCreature = _currentCreature;
			_currentTurn = _currentCreature;
		}

		private void OriaxSneakAttack()
		{
			LogLine(Abilities.GetOriaxSneakAttackText());

			DestroyShieldCardInPlay();

			// Play again action is on the card
		}

		private void OriaxPickPocket()
		{
			LogLine(Abilities.GetOriaxPickPocketText());

			var ops2 = GetOpponents().ToList();
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

			LogLine($"\t{_currentCreature.CreatureName} drew {cardToPlay.Name} from {creatureToTarget.CreatureName}.");

			PlayCard(cardToPlay);
		}

		private void SuthaWhirlingAxes()
		{
			LogLine(Abilities.GetSuthaWhirlingAxesText());

			foreach (var opponent in GetOpponents())
			{
				Heal();
				Attack(AttackType.Specific, opponent, false, true);
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

		private void SuthaMightyToss()
		{
			LogLine(Abilities.GetSuthaMightyTossText());

			DestroyShieldCardInPlay();
			// Draw action is on the card
		}

		#endregion

		private (int? numOfShieldsOnCard, Creature? highestShieldCharacter) DestroyShieldCardInPlay()
		{
			var (highestShieldCharacter, highestShieldCard) = GetMaxShieldCardFromOpponents();
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

		private (Creature?, Card?) GetMaxShieldCardFromOpponents()
		{
			int maxShields = 0;
			Creature? maxCreature = null;
			Card? maxCard = null;

			var charsWithShields = GetOpponents().Where(x => x.NumberOfShields > 0).ToList();

			if (!charsWithShields.Any())
			{
				LogLine("\tNo shields in play.");
				return (maxCreature, maxCard);
			}

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

		private IEnumerable<Creature> GetOpponents()
		{
			return _specialUntargetable ? _creatures.Where(x => x != _currentCreature && x != _untargetableCreature && x.CurrentHitPoints > 0) : _creatures.Where(x => x != _currentCreature && x.CurrentHitPoints > 0);
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
				textBoxMessages.Text += message + Environment.NewLine;
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
