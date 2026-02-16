using HarmonyLib;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using UnityEngine;

using static LayerFaith;

namespace GanExtendDisplay
{
	internal class EnchantDisplayClass {
		public static void DNA_WriteNote_Prefix(DNA __instance, UINote n, Chara tg) {
                if (__instance.slot >= 1)
                {
                    n.AddText("isGeneReqSlots".lang(__instance.slot.ToString() ?? ""), FontColor.Warning);
                }

                if (!__instance.CanRemove(tg))
                {
                    n.AddText("isPermaGene".lang(), FontColor.Warning);
                }

                n.Space(4);
                if (__instance.type == DNA.Type.Brain)
                {
                    SourceChara.Row row = EClass.sources.charas.map.TryGetValue(__instance.id);
                    if (row != null)
                    {
                        string key = row.tactics.IsEmpty(EClass.sources.tactics.map.TryGetValue(row.id)?.id ?? EClass.sources.tactics.map.TryGetValue(row.job)?.id ?? "predator");
                        n.AddText("gene_info".lang(EClass.sources.tactics.map[key].GetName().ToTitleCase(), ""), FontColor.ButtonGeneral);
                    }

                    for (int i = 0; i < __instance.vals.Count; i += 2)
                    {
                        int num = __instance.vals[i];
                        int num2 = __instance.vals[i + 1];
                        FontColor color = ((num2 >= 0) ? FontColor.Good : FontColor.Bad);
                        string @ref = (num + 1).ToString() ?? "";
                        string text = "";
                        num2 = Mathf.Abs(num2 / 20) + 1;
                        text = text + "[" + "*".Repeat(Mathf.Clamp(num2, 1, 5)) + ((num2 > 5) ? "+" : "") + "]";
                        n.AddText("gene_info_brain".lang(@ref, text), color);
                    }

                    return;
                }

                for (int j = 0; j < __instance.vals.Count; j += 2)
                {
                    Element element = Element.Create(__instance.vals[j], __instance.vals[j + 1]);
                    string text2 = "";
                    int num3 = element.Value / 10;
                    FontColor color2 = FontColor.Good;
                    bool flag = false;
                    switch (element.source.category)
                    {
                        case "slot":
                            color2 = FontColor.Myth;
                            num3 = -1;
                            break;
                        case "feat":
                            color2 = FontColor.FoodMisc;
                            num3 = -1;
                            break;
                        case "ability":
                            color2 = FontColor.Topic2;
                            num3 = -1;
                            break;
                        default:
                            flag = true;
                            break;
                    }

                    if (num3 >= 0)
                    {
                        text2 = text2 + "[" + "*".Repeat(Mathf.Clamp(num3, 1, 5)) + ((num3 > 5) ? "+" : "") + "]";
                    }
                    if (flag)
                    {
                        text2 = text2 + " (" + element.Value + ")";
                    }
                    

                    n.AddText("gene_info".lang(element.Name.ToTitleCase(wholeText: true), text2), color2);
                }
            }


        public static void Enchant_AddEncNote_Prefix(Element __instance, UINote n, Card Card, ElementContainer.NoteMode mode = ElementContainer.NoteMode.Default, Func<Element, string, string> funcText = null, Action<UINote, Element> onAddNote = null) {

			string text = "";
			switch (mode) {
				case ElementContainer.NoteMode.Domain:
					n.AddText(__instance.Name, FontColor.Default);
					return;
				case ElementContainer.NoteMode.Default:
				case ElementContainer.NoteMode.BonusTrait: {
						bool flag = __instance.source.tag.Contains("common");
						string categorySub = __instance.source.categorySub;
						bool flag2 = false;
						bool flag3 = (__instance.source.tag.Contains("neg") ? (__instance.Value > 0) : (__instance.Value < 0));
						int num = Mathf.Abs(__instance.Value);
						bool flag4 = Card?.ShowFoodEnc ?? false;
						bool flag5 = Card != null && Card.IsWeapon && __instance is Ability;
						if (__instance.IsTrait || (flag4 && __instance.IsFoodTrait)) {
							string[] textArray = __instance.source.GetTextArray("textAlt");
							int num2 = Mathf.Clamp(__instance.Value / 10 + 1, (__instance.Value < 0 || textArray.Length <= 2) ? 1 : 2, textArray.Length - 1);
							text = "altEnc".lang(textArray[0].IsEmpty(__instance.Name), textArray[num2], EClass.debug.showExtra ? (__instance.Value + " " + __instance.Name) : "");
							flag3 = num2 <= 1 || textArray.Length <= 2;
							flag2 = true;
						} else if (flag5) {
							text = "isProc".lang(__instance.Name);
							flag3 = false;
						} else if (categorySub == "resist") {
							text = ("isResist" + (flag3 ? "Neg" : "")).lang(__instance.Name);
						} else if (categorySub == "eleAttack") {
							text = "isEleAttack".lang(__instance.Name);
						} else if (!__instance.source.textPhase.IsEmpty() && __instance.Value > 0) {
							text = __instance.source.GetText("textPhase");
						} else {
							string name = __instance.Name;
							bool flag6 = __instance.source.category == "skill" || (__instance.source.category == "attribute" && !__instance.source.textPhase.IsEmpty());
							bool flag7 = __instance.source.category == "enchant";
							if (__instance.source.tag.Contains("multiplier")) {
								flag6 = (flag7 = false);
								name = EClass.sources.elements.alias[__instance.source.aliasRef].GetName();
							}

							flag2 = !(flag6 || flag7);
							text = (flag6 ? "textEncSkill" : (flag7 ? "textEncEnc" : "textEnc")).lang(name, num + (__instance.source.tag.Contains("ratio") ? "%" : ""), ((__instance.Value > 0) ? "encIncrease" : "encDecrease").lang());
						}

						int num3 = ((!(__instance is Resistance)) ? 1 : 0);
						int num4 = 5;
						if (__instance.id == 484) {
							num3 = 0;
							num4 = 1;
						}

						if (!flag && !flag2 && !__instance.source.tag.Contains("flag")) {
							text = text + " [" + "*".Repeat(Mathf.Clamp(num * __instance.source.mtp / num4 + num3, 1, 5)) + ((num * __instance.source.mtp / num4 + num3 > 5) ? "+" : "") + "]" + $"({num})";
						}

						if (__instance.HasTag("hidden") && mode != ElementContainer.NoteMode.BonusTrait) {
							text = "(debug)" + text;
						}

						FontColor color = (flag ? FontColor.Default : (flag3 ? FontColor.Bad : FontColor.Good));
						if (__instance.IsGlobalElement) {
							text = text + " " + (__instance.IsFactionWideElement ? "_factionWide" : "_partyWide").lang();
							if (!__instance.IsActive(Card)) {
								return;
							}

							color = FontColor.Myth;
						}

						if (flag4 && __instance.IsFoodTrait && !__instance.IsFoodTraitMain) {
							color = FontColor.FoodMisc;
						}

						if (__instance.id == 2 && __instance.Value >= 0) {
							color = FontColor.FoodQuality;
						}

						if (funcText != null) {
							text = funcText(__instance, text);
						}

						UIItem uIItem = n.AddText("NoteText_enc", text, color);
						Sprite sprite = EClass.core.refs.icons.enc.enc;
						Thing thing = Card?.Thing;
						if (thing != null) {
							if (thing.material.HasEnc(__instance.id)) {
								sprite = EClass.core.refs.icons.enc.mat;
							}

							foreach (int key in thing.source.elementMap.Keys) {
								if (key == __instance.id) {
									sprite = EClass.core.refs.icons.enc.card;
								}
							}

							if (thing.IsFood && __instance.IsFoodTrait) {
								sprite = EClass.core.refs.icons.enc.traitFood;
							}

							if (__instance.id == thing.GetInt(107)) {
								sprite = EClass.core.refs.icons.enc.cat;
							}

							if (thing.GetRuneEnc(__instance.id) != null) {
								sprite = EClass.core.refs.icons.enc.rune;
							}
						}

						if ((bool)sprite) {
							uIItem.image1.SetActive(enable: true);
							uIItem.image1.sprite = sprite;
						}

						uIItem.image2.SetActive(__instance.source.IsWeaponEnc);
						onAddNote?.Invoke(n, __instance);
						return;
					}


			}
			UIItem uIItem2 = n.AddTopic("TopicAttribute", __instance.Name, "".TagColor((__instance.ValueWithoutLink > 0) ? SkinManager.CurrentColors.textGood : SkinManager.CurrentColors.textBad, __instance.ValueWithoutLink.ToString() ?? ""));
			if ((bool)uIItem2.button1) {
				uIItem2.button1.tooltip.onShowTooltip = delegate (UITooltip t) {
					__instance.WriteNote(t.note, EClass.pc.elements);
				};
			}
			__instance.SetImage(uIItem2.image1);
			UnityEngine.UI.Image image = uIItem2.image2;
			int value = (__instance.Potential - 80) / 20;
			image.enabled = __instance.Potential != 80;
			image.sprite = EClass.core.refs.spritesPotential[Mathf.Clamp(Mathf.Abs(value), 0, EClass.core.refs.spritesPotential.Count - 1)];
			image.color = ((__instance.Potential - 80 >= 0) ? Color.white : new Color(1f, 0.7f, 0.7f));

		}
	}
}
