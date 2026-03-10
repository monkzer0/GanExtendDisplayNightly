using BepInEx;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

		// Appended after the original GetHoverText2 output — adds content the original does not
		// produce or produces in a less informative form.
		//
		// Conditions: the original writes basic conditions (name + numeric value). This method
		// replaces that section with an enhanced version that adds colour-coding by group
		// (Buff/Debuff/Disease), an area-debuff name fallback when GetPhaseStr() == "#", and
		// resistCon display. Before appending our enhanced output, the original's basic conditions
		// lines are stripped from originalResult via a regex so they are not shown twice.
		//
		// Acts / Feats: unique additions that the original never generates; appended as-is.
		public static string Chara_GetHoverText2_Additions(Chara __instance, string originalResult) {
			string result = originalResult;

			// Enhanced conditions — colour-coded by group, area-debuff fallback, resistCon
			// Wrapped in try-catch: if this section throws, the original's basic conditions
			// remain in place and acts/feats still render below.
			try {
				IEnumerable<BaseStats> condSources = __instance.conditions.Concat(
					(!__instance.IsPCFaction) ? new BaseStats[0] : new BaseStats[2] { __instance.hunger, __instance.stamina }
				);
				int condCount = 0;
				string condText = "<size=14>";
				var rawPhaseTexts = new System.Collections.Generic.List<string>();
				foreach (BaseStats item in condSources) {
					string text = item.GetPhaseStr();
					if (text.IsEmpty()) { continue; }
					if (text == "#") {
						text = item.source.GetName();
						if (text.IsEmpty()) { continue; }
					}
					rawPhaseTexts.Add(text);
					Color c = Color.white;
					switch (item.source.group) {
						case "Bad":
						case "Debuff":
						case "Disease":
							c = EClass.Colors.colorDebuff;
							break;
						case "Buff":
							c = EClass.Colors.colorBuff;
							break;
					}
					text = text + "(" + item.GetValue() + ")";
					if (__instance.resistCon != null && __instance.resistCon.ContainsKey(item.id)) {
						text = text + "{" + __instance.resistCon[item.id] + "}";
					}
					condText += text.TagColor(c) + ", ";
					condCount++;
				}
				if (condCount > 0) {
					condText = condText.TrimEnd().TrimEnd(',');
					condText += "</size>";
					// Remove the original's conditions block.
					// Conditions are the last thing GetHoverText2 appends, so find the
					// newline immediately before the earliest condition name and truncate
					// there. Plain string search -- no assumptions about tag format.
					int cutAt = result.Length;
					foreach (string ph in rawPhaseTexts) {
						int idx = result.LastIndexOf(ph);
						if (idx < 0) continue;
						int nl = result.LastIndexOf('\n', idx);
						int cutPos = nl >= 0 ? nl : 0; // no preceding newline = conditions start at position 0
						if (cutPos < cutAt) cutAt = cutPos;
					}
					if (cutAt < result.Length) result = result.Substring(0, cutAt);
					result += Environment.NewLine + condText;
				}
			} catch (Exception condEx) {
				Main.Logger.LogWarning($"[ExtendDisplay] conditions block threw: {condEx.Message}");
				// original's basic conditions remain; no enhanced section appended
			}

			// Favgift force-display: the game shows favgift only when knowFav=true.
			// When the config is set to Show and the player hasn't discovered it yet, add it ourselves.
			if (!__instance.knowFav &&
				CharaSettings.CharaDisplayLineFavgiftSettings != null &&
				CharaSettings.CharaDisplayLineFavgiftSettings.CharaDisplayLineOut &&
				(!CharaSettings.CharaDisplayLineFavgiftSettings.CharaDisplayPCFactionOnly || __instance.IsPCFaction)) {
				try {
					string favLine = "favgift".lang(__instance.GetFavCat().GetName().ToLower(), __instance.GetFavFood().GetName());
					if (!favLine.IsEmpty()) {
						result += Environment.NewLine + $"<size={CharaSettings.CharaDisplayLineFavgiftSettings.Size}>" + favLine + "</size>";
					}
				} catch (Exception favEx) {
					Main.Logger.LogWarning($"[ExtendDisplay] favgift force-display threw: {favEx.Message}");
				}
			}

			if (CharaSettings.CharaDisplayLineActSettings.CharaDisplayLineOut && (!CharaSettings.CharaDisplayLineActSettings.CharaDisplayPCFactionOnly || __instance.IsPCFaction)) {
				int actSize = CharaSettings.CharaDisplayLineActSettings.Size;
				int actItemsPerLine = CharaSettings.CharaDisplayLineActItemsPerLine.Value;
				int actCount = 0;
				string actSegment = "";
				foreach (ActList.Item item in __instance.ability.list.items) {
					if (actItemsPerLine > 0 && actCount > 0 && actCount % actItemsPerLine == 0) {
						result += Environment.NewLine + actSegment.TrimEnd().TrimEnd(',').TrimEnd().TagSize(actSize);
						actSegment = "";
					}
					string aliasParentName = null;
					if (!string.IsNullOrWhiteSpace(item.act.source.aliasParent)) {
						string aliasParentElement = Element.GetName(item.act.source.aliasParent);
						if (aliasParentElement != null) {
							aliasParentName = "(" + aliasParentElement + ")";
						}
					}
					actSegment += item.act.Name + aliasParentName + ", ";
					actCount++;
				}
				if (actCount > 0) {
					result += Environment.NewLine + actSegment.TrimEnd().TrimEnd(',').TrimEnd().TagSize(actSize);
				}
			}

			// Feat line — displayed separately from acts so each can be toggled independently
			// 特技行 — 与行动分开显示，可独立切换
			// 特技行 — 行動とは別に表示し、独立して切り替え可能
			if (CharaSettings.CharaDisplayLineFeatSettings.CharaDisplayLineOut && (!CharaSettings.CharaDisplayLineFeatSettings.CharaDisplayPCFactionOnly || __instance.IsPCFaction)) {
				var feats = __instance.elements.ListElements(x => x.source.category == "feat" && x.Value > 0);
				if (feats != null && feats.Any()) {
					int featSize = CharaSettings.CharaDisplayLineFeatSettings.Size;
					int featItemsPerLine = CharaSettings.CharaDisplayLineFeatItemsPerLine.Value;
					int featCount = 0;
					string featSegment = "";
					foreach (Element feat in feats) {
						if (featItemsPerLine > 0 && featCount > 0 && featCount % featItemsPerLine == 0) {
							result += Environment.NewLine + featSegment.TrimEnd().TrimEnd(',').TrimEnd().TagColor(EClass.Colors.colorBuff).TagSize(featSize);
							featSegment = "";
						}
						featSegment += feat.Name + ", ";
						featCount++;
					}
					if (featCount > 0) {
						result += Environment.NewLine + featSegment.TrimEnd().TrimEnd(',').TrimEnd().TagColor(EClass.Colors.colorBuff).TagSize(featSize);
					}
				}
			}

			return result;
		}

	}
}
