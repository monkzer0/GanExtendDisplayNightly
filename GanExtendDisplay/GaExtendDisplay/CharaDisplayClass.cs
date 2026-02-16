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

		public static string Chara_GetHoverText2_Prefix(Chara __instance, string __result) {
			string text = "";
			if (__instance.knowFav || (CharaSettings.CharaDisplayLineFavgiftSettings.CharaDisplayLineOut && (!CharaSettings.CharaDisplayLineAttributesSettings.CharaDisplayPCFactionOnly || __instance.IsPCFaction))) {
				text += Environment.NewLine;
				text = text + $"<size={CharaSettings.CharaDisplayLineFavgiftSettings.Size}>" + "favgift".lang(__instance.GetFavCat().GetName().ToLower(), __instance.GetFavFood().GetName()) + "</size>";
			}

			string text2 = "";
			if (EClass.debug.showExtra) {
				text2 += Environment.NewLine;
				//text2 = text2 + "Lv:" + __instance.LV + "  HP:" + __instance.hp + "/" + __instance.MaxHP + "  MP:" + __instance.mana.value + "/" + __instance.mana.max + "  DV:" + __instance.DV + "  PV:" + __instance.PV + "  Hunger:" + __instance.hunger.value;
				text2 += Environment.NewLine;
				text2 = text2 + "Global:" + __instance.IsGlobal + "  AI:" + __instance.ai?.ToString() + " " + __instance.source.tactics.IsEmpty(EClass.sources.tactics.map.TryGetValue(__instance.id)?.id ?? EClass.sources.tactics.map.TryGetValue(__instance.job.id)?.id ?? "predator");
				text2 += Environment.NewLine;
				text2 = text2 + __instance.uid + __instance.IsMinion + "/" + __instance.c_uidMaster + "/" + __instance.master;
				text2 = text2 + " dir:" + __instance.dir + " skin:" + __instance.idSkin;
			}

			if (EClass.pc.held?.trait is TraitWhipLove && __instance.IsPCFaction) {
				text2 += Environment.NewLine;
				text2 += "<size=14>";
				foreach (Hobby item in __instance.ListWorks()) {
					text2 = text2 + item.Name + ", ";
				}

				foreach (Hobby item2 in __instance.ListHobbies()) {
					text2 = text2 + item2.Name + ", ";
				}

				text2 = text2.TrimEnd(", ".ToCharArray()) + "</size>";
			}

			string text3 = "";
			IEnumerable<BaseStats> enumerable = __instance.conditions.Concat((!__instance.IsPCFaction) ? new BaseStats[0] : new BaseStats[2] { __instance.hunger, __instance.stamina });
			if (enumerable.Count() > 0) {
				text3 += Environment.NewLine;
				text3 += "<size=14>";
				int num = 0;
				foreach (BaseStats item3 in enumerable) {
					string text4 = item3.GetPhaseStr();
					if (text4.IsEmpty() || text4 == "#") {
						continue;
					}

					Color c = Color.white;
					switch (item3.source.group) {
						case "Bad":
						case "Debuff":
						case "Disease":
							c = EClass.Colors.colorDebuff;
							break;
						case "Buff":
							c = EClass.Colors.colorBuff;
							break;
					}


					text4 = text4 + "(" + item3.GetValue() + ")";
					if (__instance.resistCon != null && __instance.resistCon.ContainsKey(item3.id)) {
						text4 = text4 + "{" + __instance.resistCon[item3.id] + "}";
					}
				

					num++;
					text3 = text3 + text4.TagColor(c) + ", ";
				}

				if (num == 0) {
					text3 = "";
				} else {
					text = "";
					text3 = text3.TrimEnd(", ".ToCharArray()) + "</size>";
				}
			}

			if (CharaSettings.CharaDisplayLineActSettings.CharaDisplayLineOut && (!CharaSettings.CharaDisplayLineActSettings.CharaDisplayPCFactionOnly || __instance.IsPCFaction)) {
				text3 += Environment.NewLine;
				foreach (ActList.Item item4 in __instance.ability.list.items) {
					string aliasParentName = null;
					if (!string.IsNullOrWhiteSpace(item4.act.source.aliasParent)) {
						string aliasParentElement = Element.GetName(item4.act.source.aliasParent);
						if (aliasParentElement != null) {
							aliasParentName = "(" + aliasParentElement + ")";
						}
					}
					
					text3 = text3 + (item4.act.Name + aliasParentName + ", ").TagSize(CharaSettings.CharaDisplayLineActSettings.Size);
				}

				text3 = text3.TrimEnd(" ".ToCharArray());
			}

			return text + text2 + text3;
		}

	}
}
