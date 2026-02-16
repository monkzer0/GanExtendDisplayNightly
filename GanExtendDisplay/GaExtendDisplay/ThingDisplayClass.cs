using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using UnityEngine;
using UnityEngine.UI;

using static TextureReplace;

namespace GanExtendDisplay
{
	internal class ThingDisplayClass
	{
		public static string Thing_GetHoverText_Postfix(Thing __instance, string __result) {
			string text = "";
			if (__instance.isNPCProperty)
				text += "(x)";
			if (!__instance.isChara) {
				bool isNPCProperty = __instance.isNPCProperty;
				if (isNPCProperty) {
					__result += "(x)";
				}
				Color c = Color.black;
				switch (__instance.rarity) {
					case Rarity.Crude:
						text = "x";
						c = CS.Color_Crude;
						break;
					case Rarity.Normal:
						text = "";
						c = CS.Color_Normal;
						break;
					case Rarity.Superior:
						text = "△";
						c = CS.Color_Superior;
						break;
					case Rarity.Legendary:
						text = "◇";
						c = CS.Color_Legendary;
						break;
					case Rarity.Mythical:
						text = "☆";
						c = CS.Color_Mythical;
						break;
					case Rarity.Artifact:
						text = "★";
						c = CS.Color_Artifact;
						break;
				}
				text = text.TagColor(c);
				__result = $"{text} Lv.{__instance.LV.ToString()} ".TagColor(c) + $"{__instance.material.GetName()} {__result}";
				string lockLv = (__instance.c_lockLv > 0) ? ($" Lock.Lv.{__instance.c_lockLv.ToString().TagColor(Color.yellow)}") : "";
				string Scales = $"¤ {__instance.GetPrice(CurrencyType.Money, false, PriceType.Default, null).ToString()}".TagSize(14);
				__result = $"{__result} {Scales} {lockLv}";
			}
			return __result;
		}
	}
}
