using System;
using System.Collections.Generic;
using System.Text;

namespace DungeonMayhem.Library
{
    public class Card
    {
        public string Name { get; set; }
        public List<CreatureAction> Actions { get; set; } = [];
        public int NumShieldsRemaining { get; set; }
        public int NumTotalShields { get; set; }

        public override string ToString()
        {
	        StringBuilder sb = new StringBuilder();
			sb.Append(Name);
			foreach (var action in Actions)
			{
				sb.Append(' ');
				switch (action.ActionType)
				{
					case ActionType.Block:
						sb.Append("[Block]");
						break;
					case ActionType.Draw:
						sb.Append("[Draw]");
						break;
					case ActionType.Damage:
						sb.Append("[Attack]");
						break;
					case ActionType.PlayExtraCard:
						sb.Append("[Lightning]");
						break;
					case ActionType.Heal:
						sb.Append("[Heal]");
						break;
					case ActionType.MightyPower:
						sb.Append($"[Mighty Power - {Abilities.LookupMightyPowerText(Name)}]");
						break;
					case ActionType.Shapeshift:
						sb.Append("[Shapeshift]");
						break;
					case ActionType.DamageIgnoreShields:
						sb.Append("[Attack, ignoring shields]");
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			return sb.ToString();
		}
	}
}