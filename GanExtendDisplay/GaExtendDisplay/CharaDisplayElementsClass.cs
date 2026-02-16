using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace GanExtendDisplay
{
	public class CharaDisplayElementsClass
	{
		public static string Show_Affinity(Chara __instance, string __result) {
			int affinity = __instance._affinity;
			if (affinity > 0) {
				string text = (affinity > 74) ? "♥" : "♡";
				text = text.TagSize(UnityEngine.Mathf.Clamp(affinity / 10 + 10, 12, 20)).TagColor(new UnityEngine.Color(0.5f, System.Math.Min(1f, 0.5f + (float)affinity * 1f / 100f), 0.5f));
				__result = " " + text + " " + __result;
			}
			return __result;
		}
		public static string Show_Rarity(Chara __instance, string __result) {
			string text2 = "";
			UnityEngine.Color c = UnityEngine.Color.black;
			switch (__instance.rarity) {
				case Rarity.Crude:
					text2 = "x";
					c = CS.Color_Crude;
					break;
				case Rarity.Normal:
					text2 = "";
					c = CS.Color_Normal;
					break;
				case Rarity.Superior:
					text2 = "△";
					c = CS.Color_Superior;
					break;
				case Rarity.Legendary:
					text2 = "◇";
					c = CS.Color_Legendary;
					break;
				case Rarity.Mythical:
					text2 = "☆";
					c = CS.Color_Mythical;
					break;
				case Rarity.Artifact:
					text2 = "★";
					c = CS.Color_Artifact;
					break;
			}
			text2 = text2.TagColor(c);
			__result = text2 + " " + __result;
			return __result;
		}
		public static string Show_Lv(Chara __instance) {
			int num = 2;
			int lv = EClass.pc.LV;
			bool flag2 = __instance.LV >= EClass.pc.LV * 5;
			if (flag2) {
				num = 0;
			} else {
				bool flag3 = __instance.LV >= EClass.pc.LV * 2;
				if (flag3) {
					num = 1;
				} else {
					bool flag4 = __instance.LV <= EClass.pc.LV / 4;
					if (flag4) {
						num = 4;
					} else {
						bool flag5 = __instance.LV <= EClass.pc.LV / 2;
						if (flag5) {
							num = 3;
						}
					}
				}
			}
			string s = "";
			bool flag6 = num == 0;
			if (flag6) {
				s = " ☠ ";
			}
			return (" Lv." + __instance.LV.ToString()).TagColor(EClass.Colors.gradientLVComparison.Evaluate(0.25f * (float)num)) + s.TagSize(30).TagColor(EClass.Colors.gradientLVComparison.Evaluate(0.25f * (float)num));
		}
		public static string Show_HP(Chara __instance) {
			return "HP:".TagColor(Color.red) + (__instance.hp.ToString() + "/" + __instance.MaxHP.ToString()).TagColor(((float)__instance.hp > (float)__instance.MaxHP * 0.2f) ? new UnityEngine.Color(0.73f, 1f, 0.82f) : new UnityEngine.Color(1f, 0.67f, 0.67f));
		}
		public static string Show_MP(Chara __instance) {
			return " MP:".TagColor(Color.blue) + (__instance.mana.value.ToString() + "/" + __instance.mana.max.ToString()).TagColor(((float)__instance.mana.value > (float)__instance.mana.max * 0.2f) ? new UnityEngine.Color(0.73f, 1f, 0.82f) : new UnityEngine.Color(1f, 0.67f, 0.67f));
		}
		public static string Show_SP(Chara __instance) {
			return " SP: ".TagColor(Color.green) + (__instance.stamina.value.ToString() + "/" + __instance.stamina.max.ToString()).TagColor(((float)__instance.stamina.value > (float)__instance.stamina.max * 0.2f) ? new UnityEngine.Color(0.73f, 1f, 0.82f) : new UnityEngine.Color(1f, 0.67f, 0.67f));
		}
		public static string Show_RaceJob(Chara __instance) {
			string res = $" {Lang._gender(__instance.bio.gender)} {__instance.bio.TextAge(__instance)} [{__instance.race.GetName()} {__instance.job.GetName()} {__instance.tactics.source.GetName()}]";
			return res;
		}
		public static string Show_Hunger(Chara __instance) {
			return $" {__instance.hunger.name}:{__instance.hunger.value}/{__instance.hunger.max}";
		}
		public static string DVPV(Chara __instance) {
			return  $" DV:{__instance.DV} PV:{__instance.PV}";
		}
		public static string StyleShow(Chara __instance) {

			return $" {__instance.elements.GetOrCreateElement(__instance.GetArmorSkill()).Name} {("style" + __instance.body.GetAttackStyle().ToString()).lang()}";
		}
		public static string Show_Works(Chara __instance) {
			string res = "";
			foreach (Hobby work in __instance.ListWorks()) {
				res += $" {work.Name}";
			}
			foreach (Hobby Hobbies in __instance.ListHobbies()) {
				res += $" {Hobbies.Name}";
			}
			return res;
		}
		public static string Show_Attributes(Chara __instance) {
			string res = "";
				res += $" {__instance.elements.GetElement(70).Name}:{__instance.elements.GetElement(70).Value}";//力量
				res += $" {__instance.elements.GetElement(71).Name}:{__instance.elements.GetElement(71).Value}";//体质
				res += $" {__instance.elements.GetElement(72).Name}:{__instance.elements.GetElement(72).Value}";//灵巧
				res += $" {__instance.elements.GetElement(73).Name}:{__instance.elements.GetElement(73).Value}";//感知
				res += $" {__instance.elements.GetElement(74).Name}:{__instance.elements.GetElement(74).Value}";//学习
				res += $" {__instance.elements.GetElement(75).Name}:{__instance.elements.GetElement(75).Value}";//意志
				res += $" {__instance.elements.GetElement(76).Name}:{__instance.elements.GetElement(76).Value}";//魔力
				res += $" {__instance.elements.GetElement(77).Name}:{__instance.elements.GetElement(77).Value}";//魅力
			return res;
		}
		public static string Show_Debug(Chara __instance) {
			string res = "";
			res += "Global:" + __instance.IsGlobal + "  AI:" + __instance.ai?.ToString() + " " + __instance.ai.Current?.ToString() + "" + __instance.source.tactics.IsEmpty(EClass.sources.tactics.map.TryGetValue(__instance.id)?.id ?? EClass.sources.tactics.map.TryGetValue(__instance.job.id)?.id ?? "predator");
			res += "\n" + __instance.uid + __instance.IsMinion + "/" + __instance.c_uidMaster + "/" + __instance.master;
			return res;
		}
		public static string Show_Weight(Chara __instance) {
			float num2 = (float)__instance.ChildrenWeight * 1f / 1000f;
			float num3 = (float)__instance.WeightLimit * 1f / 1000f;
			string result = $" {Element.Get(11).GetName()}:" + (((int)(__instance.ChildrenWeight * 1f / 1000f)).ToString("F0") + "s/" + ((int)(__instance.WeightLimit * 1f / 1000f)).ToString("F0") + "s").TagColor((num2 < num3 * 0.8f) ? new UnityEngine.Color(0.86f, 1f, 0.89f) : new UnityEngine.Color(1f, 0.8f, 0.8f));
			return result;
		}
		public static string Show_EXP(Chara __instance) {
			return $" EXP:{__instance.exp + "/" + __instance.ExpToNext}";
		}
		public static string Show_Speed(Chara __instance) {
			return $" {__instance.elements.GetElement(79).Name}:{__instance.Speed}";
		}
		public static string Show_Resist(Chara __instance) {
			List<string> eleList = __instance.elements.ListElements(x => x.source.category == "resist" && x.Value != 0).Select(x => $"{resistCS.GetName(x.Name, x.id)}:{x.Value}").ToList();
			string resist = resistCS.ShortOut(eleList);
			return resist;
		}
	}

}
