using BepInEx;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Unity.Jobs;
using UnityEngine;
using static GanExtendDisplay.DisplayConfigBaseClass;
using static TextureReplace;
using static Weather;

namespace GanExtendDisplay

{
	internal class CharaDisplayClass {
		
		public static string Chara_GetHoverText_Postfix(Chara __instance, string __result) {

			//顶行
			__result = CharaDisplayElementsClass.Show_Affinity(__instance, __result);//亲密度
			__result = CharaDisplayElementsClass.Show_Rarity(__instance, __result);//稀有度
			__result = CharaDisplayElementsClass.Show_Lv(__instance) + __result;//威胁标志

			//第1行
			if (CharaSettings.CharaDisplayLine1Settings.CharaDisplayLineOut) {
				if (!CharaSettings.CharaDisplayLine1Settings.CharaDisplayPCFactionOnly || __instance.IsPCFaction) {
					__result += Environment.NewLine;
					__result += (CharaDisplayElementsClass.Show_RaceJob(__instance) + CharaDisplayElementsClass.StyleShow(__instance)).TagSize(CharaSettings.CharaDisplayLine1Settings.Size) ; //种族职业模式
				}
			}

			//第2行
			if (CharaSettings.CharaDisplayLine2Settings.CharaDisplayLineOut) {
				if (!CharaSettings.CharaDisplayLine2Settings.CharaDisplayPCFactionOnly || __instance.IsPCFaction) {
					__result += Environment.NewLine;
					__result += (CharaDisplayElementsClass.Show_HP(__instance)+ CharaDisplayElementsClass.DVPV(__instance) + CharaDisplayElementsClass.Show_Speed(__instance)).TagSize(CharaSettings.CharaDisplayLine2Settings.Size);
				}
			}

			//第3行
			if (CharaSettings.CharaDisplayLine3Settings.CharaDisplayLineOut) {
				if (!CharaSettings.CharaDisplayLine3Settings.CharaDisplayPCFactionOnly || __instance.IsPCFaction) {
					__result += Environment.NewLine;
					__result += (CharaDisplayElementsClass.Show_SP(__instance) + CharaDisplayElementsClass.Show_Hunger(__instance) + CharaDisplayElementsClass.Show_Works(__instance)).TagSize(CharaSettings.CharaDisplayLine3Settings.Size);
				}
			}


			//第4行
			if (CharaSettings.CharaDisplayLine4Settings.CharaDisplayLineOut) {
				if (!CharaSettings.CharaDisplayLine4Settings.CharaDisplayPCFactionOnly || __instance.IsPCFaction) {
					__result += Environment.NewLine;
					__result += (CharaDisplayElementsClass.Show_MP(__instance) + CharaDisplayElementsClass.Show_Weight(__instance) + CharaDisplayElementsClass.Show_EXP(__instance)).TagSize(CharaSettings.CharaDisplayLine4Settings.Size);
				}
			}

			//抗性行
			if (CharaSettings.CharaDisplayLineResistSettings.CharaDisplayLineOut) {
				if (!CharaSettings.CharaDisplayLineResistSettings.CharaDisplayPCFactionOnly || __instance.IsPCFaction) {
					__result += CharaDisplayElementsClass.Show_Resist(__instance).TagSize(CharaSettings.CharaDisplayLineResistSettings.Size);
				}
			}


			//属性行,按下显示
			if (CharaSettings.CharaDisplayLineAttributesSettings.CharaDisplayLineOut) {
				if (!CharaSettings.CharaDisplayLineAttributesSettings.CharaDisplayPCFactionOnly || __instance.IsPCFaction) {
					__result += Environment.NewLine;
					__result += CharaDisplayElementsClass.Show_Attributes(__instance).TagSize(CharaSettings.CharaDisplayLineAttributesSettings.Size);
				}
			}

			////喜好礼物行
			//if (__instance.knowFav) {
			//	__result += Environment.NewLine;
			//	__result = __result + "<size=14>" + "favgift".lang(__instance.GetFavCat().GetName().ToLower(), __instance.GetFavFood().GetName()) + "</size>";
			//}

			return __result;
		}

		// Appended after the original GetHoverText2 output — adds only content the original does
		// not produce (acts line, feats line). Favgift, conditions, debug, and whip-works text
		// are handled by the original and intentionally not reimplemented here.
		public static string Chara_GetHoverText2_Additions(Chara __instance) {
			string result = "";

			if (CharaSettings.CharaDisplayLineActSettings.CharaDisplayLineOut && (!CharaSettings.CharaDisplayLineActSettings.CharaDisplayPCFactionOnly || __instance.IsPCFaction)) {
				result += Environment.NewLine;
				int actItemsPerLine = CharaSettings.CharaDisplayLineActItemsPerLine.Value;
				int actCount = 0;
				foreach (ActList.Item item in __instance.ability.list.items) {
					if (actItemsPerLine > 0 && actCount > 0 && actCount % actItemsPerLine == 0) {
						result += Environment.NewLine;
					}
					string aliasParentName = null;
					if (!string.IsNullOrWhiteSpace(item.act.source.aliasParent)) {
						string aliasParentElement = Element.GetName(item.act.source.aliasParent);
						if (aliasParentElement != null) {
							aliasParentName = "(" + aliasParentElement + ")";
						}
					}
					result += (item.act.Name + aliasParentName + ", ").TagSize(CharaSettings.CharaDisplayLineActSettings.Size);
					actCount++;
				}
				result = result.TrimEnd(" ".ToCharArray()).TrimEnd(",".ToCharArray());
			}

			// Feat line — displayed separately from acts so each can be toggled independently
			// 特技行 — 与行动分开显示，可独立切换
			// 特技行 — 行動とは別に表示し、独立して切り替え可能
			if (CharaSettings.CharaDisplayLineFeatSettings.CharaDisplayLineOut && (!CharaSettings.CharaDisplayLineFeatSettings.CharaDisplayPCFactionOnly || __instance.IsPCFaction)) {
				var feats = __instance.elements.ListElements(x => x.source.category == "feat" && x.Value > 0);
				if (feats != null && feats.Any()) {
					result += Environment.NewLine;
					int featItemsPerLine = CharaSettings.CharaDisplayLineFeatItemsPerLine.Value;
					int featCount = 0;
					foreach (Element feat in feats) {
						if (featItemsPerLine > 0 && featCount > 0 && featCount % featItemsPerLine == 0) {
							result += Environment.NewLine;
						}
						result += (feat.Name + ", ").TagColor(EClass.Colors.colorBuff).TagSize(CharaSettings.CharaDisplayLineFeatSettings.Size);
						featCount++;
					}
					result = result.TrimEnd(" ".ToCharArray()).TrimEnd(",".ToCharArray());
				}
			}

			return result;
		}

	}
}
