using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMayhem.Library
{
	public static class Abilities
	{
		public static string LookupMightyPowerText(string name)
		{
			switch (name)
			{
				case "Vampiric Touch": //Azzan
					return GetAzzanVampiricTouchText();
				case "Fireball": //Azzan
					return GetAzzanFireballText();
				case "Charm": //Azzan
					return GetAzzanCharmText();
				case "Hugs!": //Blorp
					return GetBlorpHugsText();
				case "Burped-Up Bones": //Blorp
					return GetBlorpBurpedUpBonesText();
				case "Here I Come!": //Blorp
					return GetBlorpHereIComeText();
				case "Praise Me": //Delilah
					return GetDelilahPraiseMeText();
				case "Death Ray": //Delilah
					return GetDelilahDeathRayText();
				case "Charm Ray": //Delilah
					return GetDelilahCharmRayText();
				case "Mind Blast": //Dr. T
					return GetDrTMindBlastText();
				case "Mind Games": //Dr. T
					return GetDrTMindGamesText();
				case "Tell Me About Your Mother": // Dr. T
					return GetDrTTellMeAboutYourMotherText();
				case "For My Next Trick...": //Hoots
					return GetHootsForMyNextTrickText();
				case "To The Face!": //Hoots
					return GetHootsToTheFaceText();
				case "Owlbear Boogie": //Hoots
					return GetHootsOwlbearBoogieText();
				case "Primal Strike": //Jaheira
					return GetJaheiraPrimalStrikeText();
				case "Commune With Nature": //Jaheira
					return GetJaheiraCommuneWithNatureText();
				case "Divine Inspiration": //Lia
					return GetLiaDivineInspirationText();
				case "Banishing Smite": //Lia
					return GetLiaBanishingSmiteText();
				case "Liquidate Assets": //Lord C
					return GetLordCinderpuffLiquidateAssetsText();
				case "Hostile Takeover": //Lord C
					return GetLordCinderpuffHostileTakeoverText();
				case "Murders and Acquisitions": //Lord C
					return GetLordCinderpuffMurdersAndAcquisitionsText();
				case "A Book (Cannot Bite)": //Mimi
					return GetMimiABookCannotBiteText();
				case "It's Not a Trap": //Mimi
					return GetMimiItsNotATrapText();
				case "Definitely Just a Mirror": //Mimi
					return GetMimiDefinitelyJustAMirrorText();
				case "Swapportunity": //M&B
					return GetMinscAndBooSwapportunityText();
				case "Scouting Outing": //M&B
					return GetMinscAndBooScoutingOutingText();
				case "Favored Frienemies": //M&B
					return GetMinscAndBooFavoredFrienemiesText();
				case "Clever Disguise": //Oriax
					return GetOriaxCleverDisguiseText();
				case "Sneak Attack!": //Oriax
					return GetOriaxSneakAttackText();
				case "Pick Pocket": //Oriax
					return GetOriaxPickPocketText();
				case "Whirling Axes": //Sutha
					return GetSuthaWhirlingAxesText();
				case "Battle Roar": //Sutha
					return GetSuthaBattleRoarText();
				case "Mighty Toss": //Sutha
					return GetSuthaMightyTossText();
				default:
					return "Mighty Power Not Found!";
			}
		}

		public static string GetAzzanVampiricTouchText()
		{
			return "Swap your hit points with an opponent's.";
		}

		public static string GetAzzanFireballText()
		{
			return "Each player (including you) takes 3 damage!";
		}

		public static string GetAzzanCharmText()
		{
			return "Take the shields that an opponent has in play - it protects you now!";
		}

		public static string GetBlorpHugsText()
		{
			return "Destroy a creature's shields and then heal for each shield destroyed.";
		}

		public static string GetBlorpBurpedUpBonesText()
		{
			return "Attack twice and gain 3 shields.";
		}

		public static string GetBlorpHereIComeText()
		{
			return "This turn, your attack cards ignore shield cards.  Gain 3 attacks.";
		}

		public static string GetDelilahPraiseMeText()
		{
			return "Draw 3 cards, then, each opponent can choose to praise your greatness.  Double attack those who do not (not-implemented).";
		}

		public static string GetDelilahDeathRayText()
		{
			return "Double attack each opponent with no shield cards in play.  Then destroy all shield cards - including yours!";
		}

		public static string GetDelilahCharmRayText()
		{
			return "Until your next turn, choose the target of all attack cards.";
		}

		public static string GetDrTMindBlastText()
		{
			return "Attack an opponent once for each card they have in their hand.";
		}

		public static string GetDrTMindGamesText()
		{
			return "Swap your hand with an opponent's hand.";
		}

		public static string GetDrTTellMeAboutYourMotherText()
		{
			return "Add the top card of each opponent's discard pile to your hand.";
		}

		public static string GetHootsForMyNextTrickText()
		{
			return "Until your next turn, your attacks hit all opponents.";
		}

		public static string GetHootsToTheFaceText()
		{
			return "Destroy a shield card and then attack for each starting shield on that card.";
		}

		public static string GetHootsOwlbearBoogieText()
		{
			return $"Each player does a little dance and draws a card.  You then draw a card for each player who danced.";
		}

		public static string GetJaheiraPrimalStrikeText()
		{
			return "You make an animal noise and attack each opponent.";
		}

		public static string GetJaheiraCommuneWithNatureText()
		{
			return "You may play a Form card for free.";
		}

		public static string GetLiaDivineInspirationText()
		{
			return "Choose any card in your discard pile and put it into your hand, then heal twice";
		}

		public static string GetLiaBanishingSmiteText()
		{
			return "Destroy all shield cards in play (including yours), then go again.";
		}

		public static string GetLordCinderpuffLiquidateAssetsText()
		{
			return "Discard your hand and attack equal to the number of cards discarded.";
		}

		public static string GetLordCinderpuffHostileTakeoverText()
		{
			return "Attack all opponents, double attack one opponent, then attack a different opponent.";
		}

		public static string GetLordCinderpuffMurdersAndAcquisitionsText()
		{
			return "Each player must attack, heal, or draw.  Start with you and go right.  You repeat all choices.";
		}

		public static string GetMimiABookCannotBiteText()
		{
			return "Use the top-listed Mighty Power of the player to your left or right";
		}

		public static string GetMimiItsNotATrapText()
		{
			return "Make one player's hit points equal to another player's hit points";
		}

		public static string GetMimiDefinitelyJustAMirrorText()
		{
			return "Play this card as a copy of any other shield card in play";
		}

		public static string GetMinscAndBooSwapportunityText()
		{
			return "Each player gives their hit point total to the player on their right";
		}

		public static string GetMinscAndBooScoutingOutingText()
		{
			return "Draw a card from the top of each opponent's deck";
		}

		public static string GetMinscAndBooFavoredFrienemiesText()
		{
			return "Your attack cards deal one bonus damage this turn";
		}

		public static string GetOriaxCleverDisguiseText()
		{
			return "None of your opponents' cards affect you or your shield cards until your next turn.";
		}

		public static string GetOriaxSneakAttackText()
		{
			return "Destroy one shield card in play, then play again.";
		}

		public static string GetOriaxPickPocketText()
		{
			return "Steal the top card of any player's deck and play it.";
		}

		public static string GetSuthaWhirlingAxesText()
		{
			return "You heal once per opponent, then attack each opponent.";
		}

		public static string GetSuthaBattleRoarText()
		{
			return "Each player (including you) discards their hand, then draws three cards.  Then play again.";
		}

		public static string GetSuthaMightyTossText()
		{
			return "Destroy one shield card in play, then draw a card.";
		}
	}
}
