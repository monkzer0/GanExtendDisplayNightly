using BepInEx;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using static GanExtendDisplay.CharaSettings;



namespace GanExtendDisplay
{
	[BepInPlugin("ExtendDisplay", "ExtendDisplay", "1.0.0")]
	public class Main : BaseUnityPlugin
	{
		internal static new ManualLogSource Logger;

		private void Awake() {
			Main.Logger = base.Logger;
			Main.Logger.LogInfo("Initializing configuration for [ Gan Extend Display ] plugin...");
			ConfigInit.InitializeConfiguration(base.Config);
			Main.Logger.LogInfo("Successfully initialized configuration for [ Gan Extend Display ] plugin.");
		}
		private void Start() {
			CharaSettings.CharaDisplayLine1Settings = new CharaConfigClass(CharaDisplayLine1.Value, CharaDisplayLine1PCFactionOnly.Value, CharaDisplayLine1Size.Value);
			CharaSettings.CharaDisplayLine2Settings = new CharaConfigClass(CharaDisplayLine2.Value, CharaDisplayLine2PCFactionOnly.Value, CharaDisplayLine2Size.Value);
			CharaSettings.CharaDisplayLine3Settings = new CharaConfigClass(CharaDisplayLine3.Value, CharaDisplayLine3PCFactionOnly.Value, CharaDisplayLine3Size.Value);
			CharaSettings.CharaDisplayLine4Settings = new CharaConfigClass(CharaDisplayLine4.Value, CharaDisplayLine4PCFactionOnly.Value, CharaDisplayLine4Size.Value);
			CharaSettings.CharaDisplayLineResistSettings = new CharaConfigClass(CharaDisplayLineResist.Value, CharaDisplayLineResistPCFactionOnly.Value, CharaDisplayLineResistSize.Value);
			CharaSettings.CharaDisplayLineAttributesSettings = new CharaConfigClass(CharaDisplayLineAttributes.Value, CharaDisplayLineAttributesPCFactionOnly.Value, CharaDisplayLineAttributesSize.Value);
			CharaSettings.CharaDisplayLineFavgiftSettings = new CharaConfigClass(CharaDisplayLineFavgift.Value, CharaDisplayLineFavgiftPCFactionOnly.Value, CharaDisplayLineFavgiftSize.Value);
			CharaSettings.CharaDisplayLineActSettings = new CharaConfigClass(CharaDisplayLineAct.Value, CharaDisplayLineActPCFactionOnly.Value, CharaDisplayLineActSize.Value);
			if (PluginSettings.CharaDisplay.Value != "Disable") {
				Harmony.CreateAndPatchAll(typeof(CharaDisplay), null);
			}
			if (PluginSettings.ThingDisplay.Value != "Disable") {
				Harmony.CreateAndPatchAll(typeof(ThingDisplay), null);
			}
			if (PluginSettings.ThingDisplay.Value != "Disable") {
				Harmony.CreateAndPatchAll(typeof(InteractDisplay), null);
			}
			if (PluginSettings.NotificationUI.Value != "Disable") {
				Harmony.CreateAndPatchAll(typeof(NotificationUI), null);
			}
			if (PluginSettings.NotificationUI.Value != "Disable") {
				Harmony.CreateAndPatchAll(typeof(EnchantDisplay), null);
			}
			Main.Logger.LogInfo("Successfully applied patches for [ Gan Extend Display ] plugin.");
		}

		public static bool ModEnable = true;
		public static bool isAltKeyDown = false;
		public static bool ChangeDisplay = false;
		public static bool ChangeDisplayKeep = false;



		private void Update() {
			// 检测LeftAlt键是否被按下
			if (Input.GetKeyDown(KeyCode.LeftAlt)) {
				isAltKeyDown = true;
			}

			// 检测LeftAlt键是否松开，如果松开了，将相关变量重置
			if (Input.GetKeyUp(KeyCode.LeftAlt)) {
				isAltKeyDown = false;
				if (!ChangeDisplayKeep) {
					ChangeDisplay = false;
				}
			}

			if (isAltKeyDown) {
				if (Input.GetKeyUp(KeyCode.CapsLock)) {
					ChangeDisplayKeep = !ChangeDisplayKeep;
				} else {
					ChangeDisplay = true;
				}

				if (Input.GetKeyUp(KeyCode.End)) {
					ModEnable = !ModEnable;
				}
				if (Input.GetKeyUp(KeyCode.Home)) {
					EClass.debug.showExtra = !EClass.debug.showExtra;
				}
			}

			//保持显示
			if (ChangeDisplayKeep) { ChangeDisplay = true; }




		}
		public Main() {
		}
	}


}
