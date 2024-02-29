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
					return "\tMighty Power Not Found!";
			}
		}

		public static string GetAzzanVampiricTouchText()
		{
			return "\tSwap your hit points with an opponent's.";
		}

		public static string GetAzzanFireballText()
		{
			return "\tEach player (including you) takes 3 damage!";
		}

		public static string GetAzzanCharmText()
		{
			return "\tTake the shields that an opponent has in play - it protects you now!";
		}

		public static string GetBlorpHugsText()
		{
			return "\tDestroy a creature's shields and then heal for each shield destroyed.";
		}

		public static string GetBlorpBurpedUpBonesText()
		{
			return "\tAttack twice and gain 3 shields.";
		}

		public static string GetBlorpHereIComeText()
		{
			return "\tThis turn, your attack cards ignore shield cards.  Gain 3 attacks.";
		}

		public static string GetDelilahPraiseMeText()
		{
			return "\tDraw 3 cards, then, each opponent can choose to praise your greatness.  Double attack those who do not (not-implemented).";
		}

		public static string GetDelilahDeathRayText()
		{
			return "\tDouble attack each opponent with no shield cards in play.  Then destroy all shield cards - including yours!";
		}

		public static string GetDelilahCharmRayText()
		{
			return "\tUntil your next turn, choose the target of all attack cards.";
		}

		public static string GetDrTMindBlastText()
		{
			return "\tAttack an opponent once for each card they have in their hand.";
		}

		public static string GetDrTMindGamesText()
		{
			return "\tSwap your hand with an opponent's hand.";
		}

		public static string GetDrTTellMeAboutYourMotherText()
		{
			return "\tAdd the top card of each opponent's discard pile to your hand.";
		}

		public static string GetHootsForMyNextTrickText()
		{
			return "\tUntil your next turn, your attacks hit all opponents.";
		}

		public static string GetHootsToTheFaceText()
		{
			return "\tDestroy a shield card and then attack for each starting shield on that card.";
		}

		public static string GetHootsOwlbearBoogieText()
		{
			return $"\tEach player does a little dance and draws a card.  You then draw a card for each player who danced.";
		}

		public static string GetJaheiraPrimalStrikeText()
		{
			return "\tYou make an animal noise and attack each opponent.";
		}

		public static string GetJaheiraCommuneWithNatureText()
		{
			return "\tYou may play a Form card for free.";
		}

		public static string GetLiaDivineInspirationText()
		{
			return "\tChoose any card in your discard pile and put it into your hand, then heal twice";
		}

		public static string GetLiaBanishingSmiteText()
		{
			return "\tDestroy all shield cards in play (including yours), then go again.";
		}

		public static string GetLordCinderpuffLiquidateAssetsText()
		{
			return "\tDiscard your hand and attack equal to the number of cards discarded.";
		}

		public static string GetLordCinderpuffHostileTakeoverText()
		{
			return "\tAttack all opponents, double attack one opponent, then attack a different opponent.";
		}

		public static string GetLordCinderpuffMurdersAndAcquisitionsText()
		{
			return "\tEach player must attack, heal, or draw.  Start with you and go right.  You repeat all choices.";
		}

		public static string GetMimiABookCannotBiteText()
		{
			return "\tUse the top-listed Mighty Power of the player to your left or right";
		}

		public static string GetMimiItsNotATrapText()
		{
			return "\tMake one player's hit points equal to another player's hit points";
		}

		public static string GetMimiDefinitelyJustAMirrorText()
		{
			return "\tPlay this card as a copy of any other shield card in play";
		}

		public static string GetMinscAndBooSwapportunityText()
		{
			return "\tEach player gives their hit point total to the player on their right";
		}

		public static string GetMinscAndBooScoutingOutingText()
		{
			return "\tDraw a card from the top of each opponent's deck";
		}

		public static string GetMinscAndBooFavoredFrienemiesText()
		{
			return "\tYour attack cards deal one bonus damage this turn";
		}

		public static string GetOriaxCleverDisguiseText()
		{
			return "\tNone of your opponents' cards affect you or your shield cards until your next turn.";
		}

		public static string GetOriaxSneakAttackText()
		{
			return "\tDestroy one shield card in play, then play again.";
		}

		public static string GetOriaxPickPocketText()
		{
			return "\tSteal the top card of any player's deck and play it.";
		}

		public static string GetSuthaWhirlingAxesText()
		{
			return "\tYou heal once per opponent, then attack each opponent.";
		}

		public static string GetSuthaBattleRoarText()
		{
			return "\tEach player (including you) discards their hand, then draws three cards.  Then play again.";
		}

		public static string GetSuthaMightyTossText()
		{
			return "\tDestroy one shield card in play, then draw a card.";
		}
	}
}
