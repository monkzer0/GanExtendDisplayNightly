// ModOptionsIntegration.cs
// Soft-dependency integration with EvilMask's Mod Options.
//
// This class is only JIT-compiled — and ModOptions types only resolved — when
// TryRegister() is called. TryRegister() is itself guarded by a runtime plugin
// presence check, so the mod works normally for users who do not have
// Mod Options installed.
//
// Reference: https://github.com/EvilMask/Elin.ModOptions
// Required: Elin/Package/Mod_ModOptions/ModOptions.dll (compile-time reference only;
//           not copied to output — Private=False in .csproj).

using System;
using BepInEx;
using BepInEx.Configuration;
using EvilMask.Elin.ModOptions;
using EvilMask.Elin.ModOptions.UI;

namespace GanExtendDisplay
{
	internal static class ModOptionsIntegration
	{
		private const string ModOptionsGuid = "evilmask.elinplugins.modoptions";
		private static ModOptionController _controller;

		/// <summary>
		/// Call from Main.Start() after all config entries are bound.
		/// Safe to call when Mod Options is not installed — exits immediately.
		/// </summary>
		internal static void TryRegister(BaseUnityPlugin plugin)
		{
			// Iterate loaded plugins rather than using [BepInDependency(SoftDependency)],
			// which has a known BepInEx issue where a soft dep with an unmet hard dep
			// causes the depending plugin to fail even though it is optional.
			bool found = false;
			foreach (var obj in ModManager.ListPluginObject)
			{
				if (obj is BaseUnityPlugin p && p.Info.Metadata.GUID == ModOptionsGuid)
				{
					found = true;
					break;
				}
			}

			if (!found)
				return;

			try
			{
				Register(plugin);
			}
			catch (Exception ex)
			{
				Main.Logger.LogWarning("[GanExtendDisplay] Mod Options registration failed: " + ex);

			}
		}

		// Separate method so ModOptions types are only JIT-resolved when actually called.
		private static void Register(BaseUnityPlugin plugin)
		{
			_controller = ModOptionController.Register(plugin.Info.Metadata.GUID, "ExtDisplay.Tab");
			SetTranslations();
			_controller.SetPreBuildWithXml(BuildXml());
			_controller.OnBuildUI += OnBuildUI;
			Main.Logger.LogInfo("[GanExtendDisplay] Registered with Mod Options.");
		}

		// -------------------------------------------------------------------------
		// Translations — SetTranslation(id, English, Japanese, Chinese Simplified)
		// -------------------------------------------------------------------------

		private static void SetTranslations()
		{
			// Tab button tooltip
			T("ExtDisplay.Tab",
				"Extend Display",
				"拡張表示",
				"扩展显示");

			// Section headers
			T("ExtDisplay.Sec.Affected",
				"Affected Display",
				"表示機能スイッチ",
				"显示功能开关");
			T("ExtDisplay.Sec.Chara",
				"Character Display Lines",
				"キャラクター情報表示行",
				"角色信息显示行");

			// Dropdown option labels (shared across all Keep/Hide/Disable dropdowns)
			T("ExtDisplay.Keep",
				"Keep (always visible)",
				"Keep（常に表示）",
				"Keep（始终显示）");
			T("ExtDisplay.Hide",
				"Hide (Alt to reveal)",
				"Hide（Altキーで表示）",
				"Hide（Alt 键显示）");
			T("ExtDisplay.Disable",
				"Disable (never shown)",
				"Disable（常に非表示）",
				"Disable（永不显示）");

			// Main feature section labels
			T("ExtDisplay.F.Chara",
				"Character Display",
				"キャラクター表示",
				"角色悬停显示");
			T("ExtDisplay.F.Thing",
				"Thing Display (Ground Items)",
				"アイテム表示（地面）",
				"物品显示（地面物品）");
			T("ExtDisplay.F.Interact",
				"Interact Display (Harvesting)",
				"インタラクト表示（採集）",
				"交互显示（采集作业）");
			T("ExtDisplay.F.Notif",
				"Notification UI (Buff Bar)",
				"通知UI（バフバー）",
				"通知UI（增益栏）");
			T("ExtDisplay.F.Enchant",
				"Enchant Display (Equip / DNA)",
				"エンチャント表示（装備/DNA）",
				"附魔显示（装备 / DNA）");

			// Per-feature tooltip notes
			T("ExtDisplay.F.AffectedNote",
				"Note: enabling/disabling these features requires a game restart.",
				"注：これらの機能の有効/無効にはゲームの再起動が必要です。",
				"注意：启用/禁用这些功能需要重启游戏。");

			// Character line labels
			T("ExtDisplay.L.L1",
				"Line 1: Sex, Age, Race, Job, AI, Armor Skill, Attack Style",
				"行1：性別 / 年齢 / 種族 / 職業 / AI / 防具スキル / 攻撃スタイル",
				"第1行：性别 / 年龄 / 种族 / 职业 / AI / 护甲技能 / 攻击方式");
			T("ExtDisplay.L.L2",
				"Line 2: HP, DV, PV, Speed",
				"行2：HP / DV / PV / 速度",
				"第2行：生命值 / 回避值 / 防御值 / 速度");
			T("ExtDisplay.L.L3",
				"Line 3: SP, Hunger, Works / Hobbies",
				"行3：SP / 空腹度 / 仕事 / 趣味",
				"第3行：精力值 / 饥饿度 / 工作 / 爱好");
			T("ExtDisplay.L.L4",
				"Line 4: MP, Weight, EXP",
				"行4：MP / 重量 / 経験値",
				"第4行：魔法值 / 负重 / 经验值");
			T("ExtDisplay.L.Res",
				"Resist Line: Elemental Resistances",
				"耐性行：属性耐性",
				"抗性行：属性抗性");
			T("ExtDisplay.L.Att",
				"Attributes Line: STR CON DEX PER LRN WIL MAG CHR",
				"属性行：STR CON DEX PER LRN WIL MAG CHR",
				"属性行：力量 体质 灵巧 感知 学习 意志 魔力 魅力");
			T("ExtDisplay.L.Fav",
				"Favorite Gift Line",
				"好物行",
				"喜爱礼物行");
			T("ExtDisplay.L.Act",
				"Act Line: Active Abilities",
				"行動行：アクティブスキル",
				"行动行：主动技能");
			T("ExtDisplay.L.Fea",
				"Feat Line: Passive Traits",
				"特技行：パッシブ特性",
				"特技行：被动特质");

			// Per-line sub-setting labels
			T("ExtDisplay.PCFac",
				"PC Faction Only",
				"PC派閥のみ",
				"仅玩家阵营");
			T("ExtDisplay.Size",
				"Font Size",
				"フォントサイズ",
				"字体大小");
			T("ExtDisplay.IPL",
				"Items Per Line  (0 = no limit)",
				"1行の件数（0 = 無制限）",
				"每行条目数（0 = 不限制）");

			// Character line change note
			T("ExtDisplay.CharaNote",
				"Character line changes take effect immediately.",
				"キャラクター行の変更はすぐに反映されます。",
				"角色行设置更改立即生效。");
		}

		// Convenience wrapper — matches SetTranslation(id, en, jp, cn)
		private static void T(string id, string en, string jp, string cn)
		{
			_controller.SetTranslation(id, en, jp, cn);
		}

		// -------------------------------------------------------------------------
		// XML layout
		// -------------------------------------------------------------------------

		private static string BuildXml() => @"<config>
  <topic>ExtDisplay.Sec.Affected</topic>
  <text>ExtDisplay.F.AffectedNote</text>
  <text>ExtDisplay.F.Chara</text>
  <one_choice id=""dd_charaDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>
  <text>ExtDisplay.F.Thing</text>
  <one_choice id=""dd_thingDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>
  <text>ExtDisplay.F.Interact</text>
  <one_choice id=""dd_interactDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>
  <text>ExtDisplay.F.Notif</text>
  <one_choice id=""dd_notifDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>
  <text>ExtDisplay.F.Enchant</text>
  <one_choice id=""dd_enchantDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>

  <topic>ExtDisplay.Sec.Chara</topic>
  <text>ExtDisplay.CharaNote</text>

  <text>ExtDisplay.L.L1</text>
  <hlayout>
    <one_choice id=""dd_l1"" type=""dropdown"" width=""50%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_l1pcf"" width=""25%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <slider id=""sl_l1sz"" min=""10"" max=""30"" step=""1"" width=""25%""/>
  </hlayout>

  <text>ExtDisplay.L.L2</text>
  <hlayout>
    <one_choice id=""dd_l2"" type=""dropdown"" width=""50%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_l2pcf"" width=""25%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <slider id=""sl_l2sz"" min=""10"" max=""30"" step=""1"" width=""25%""/>
  </hlayout>

  <text>ExtDisplay.L.L3</text>
  <hlayout>
    <one_choice id=""dd_l3"" type=""dropdown"" width=""50%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_l3pcf"" width=""25%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <slider id=""sl_l3sz"" min=""10"" max=""30"" step=""1"" width=""25%""/>
  </hlayout>

  <text>ExtDisplay.L.L4</text>
  <hlayout>
    <one_choice id=""dd_l4"" type=""dropdown"" width=""50%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_l4pcf"" width=""25%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <slider id=""sl_l4sz"" min=""10"" max=""30"" step=""1"" width=""25%""/>
  </hlayout>

  <text>ExtDisplay.L.Res</text>
  <hlayout>
    <one_choice id=""dd_lRes"" type=""dropdown"" width=""50%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_lRespcf"" width=""25%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <slider id=""sl_lRessz"" min=""10"" max=""30"" step=""1"" width=""25%""/>
  </hlayout>

  <text>ExtDisplay.L.Att</text>
  <hlayout>
    <one_choice id=""dd_lAtt"" type=""dropdown"" width=""50%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_lAttpcf"" width=""25%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <slider id=""sl_lAttsz"" min=""10"" max=""30"" step=""1"" width=""25%""/>
  </hlayout>

  <text>ExtDisplay.L.Fav</text>
  <hlayout>
    <one_choice id=""dd_lFav"" type=""dropdown"" width=""50%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_lFavpcf"" width=""25%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <slider id=""sl_lFavsz"" min=""10"" max=""30"" step=""1"" width=""25%""/>
  </hlayout>

  <text>ExtDisplay.L.Act</text>
  <hlayout>
    <one_choice id=""dd_lAct"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_lActpcf"" width=""20%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <slider id=""sl_lActsz"" min=""10"" max=""30"" step=""1"" width=""20%""/>
    <slider id=""sl_lActipl"" min=""0"" max=""20"" step=""1"" width=""20%""/>
  </hlayout>

  <text>ExtDisplay.L.Fea</text>
  <hlayout>
    <one_choice id=""dd_lFea"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_lFeapcf"" width=""20%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <slider id=""sl_lFeasz"" min=""10"" max=""30"" step=""1"" width=""20%""/>
    <slider id=""sl_lFeaipl"" min=""0"" max=""20"" step=""1"" width=""20%""/>
  </hlayout>
</config>";

		// -------------------------------------------------------------------------
		// OnBuildUI — wire each UI element to its ConfigEntry
		// -------------------------------------------------------------------------

		private static void OnBuildUI(OptionUIBuilder builder)
		{
			if (!builder.Valid) return;

			// --- Affected Display ---
			// Changing these enables/disables Harmony patches which are applied once
			// at startup, so changes here take effect on the next game restart.
			BindDD(builder, "dd_charaDisp",    PluginSettings.CharaDisplay);
			BindDD(builder, "dd_thingDisp",    PluginSettings.ThingDisplay);
			BindDD(builder, "dd_interactDisp", PluginSettings.InteractDisplay);
			BindDD(builder, "dd_notifDisp",    PluginSettings.NotificationUI);
			BindDD(builder, "dd_enchantDisp",  PluginSettings.EnchantDisplay);

			// --- Character Lines ---
			// Mode, PCFactionOnly, and Size all re-bake CharaConfigClass on change so
			// they take effect immediately without a game restart.
			// ItemsPerLine is already live (read directly from ConfigEntry.Value at
			// render time) and is bound without the extra refresh call.

			BindDDLive(builder, "dd_l1",     CharaSettings.CharaDisplayLine1);
			BindTGLive(builder, "tg_l1pcf",  CharaSettings.CharaDisplayLine1PCFactionOnly);
			BindSLLive(builder, "sl_l1sz",   "ExtDisplay.Size", CharaSettings.CharaDisplayLine1Size);

			BindDDLive(builder, "dd_l2",     CharaSettings.CharaDisplayLine2);
			BindTGLive(builder, "tg_l2pcf",  CharaSettings.CharaDisplayLine2PCFactionOnly);
			BindSLLive(builder, "sl_l2sz",   "ExtDisplay.Size", CharaSettings.CharaDisplayLine2Size);

			BindDDLive(builder, "dd_l3",     CharaSettings.CharaDisplayLine3);
			BindTGLive(builder, "tg_l3pcf",  CharaSettings.CharaDisplayLine3PCFactionOnly);
			BindSLLive(builder, "sl_l3sz",   "ExtDisplay.Size", CharaSettings.CharaDisplayLine3Size);

			BindDDLive(builder, "dd_l4",     CharaSettings.CharaDisplayLine4);
			BindTGLive(builder, "tg_l4pcf",  CharaSettings.CharaDisplayLine4PCFactionOnly);
			BindSLLive(builder, "sl_l4sz",   "ExtDisplay.Size", CharaSettings.CharaDisplayLine4Size);

			BindDDLive(builder, "dd_lRes",   CharaSettings.CharaDisplayLineResist);
			BindTGLive(builder, "tg_lRespcf",CharaSettings.CharaDisplayLineResistPCFactionOnly);
			BindSLLive(builder, "sl_lRessz", "ExtDisplay.Size", CharaSettings.CharaDisplayLineResistSize);

			BindDDLive(builder, "dd_lAtt",   CharaSettings.CharaDisplayLineAttributes);
			BindTGLive(builder, "tg_lAttpcf",CharaSettings.CharaDisplayLineAttributesPCFactionOnly);
			BindSLLive(builder, "sl_lAttsz", "ExtDisplay.Size", CharaSettings.CharaDisplayLineAttributesSize);

			BindDDLive(builder, "dd_lFav",   CharaSettings.CharaDisplayLineFavgift);
			BindTGLive(builder, "tg_lFavpcf",CharaSettings.CharaDisplayLineFavgiftPCFactionOnly);
			BindSLLive(builder, "sl_lFavsz", "ExtDisplay.Size", CharaSettings.CharaDisplayLineFavgiftSize);

			BindDDLive(builder, "dd_lAct",   CharaSettings.CharaDisplayLineAct);
			BindTGLive(builder, "tg_lActpcf",CharaSettings.CharaDisplayLineActPCFactionOnly);
			BindSLLive(builder, "sl_lActsz", "ExtDisplay.Size", CharaSettings.CharaDisplayLineActSize);
			BindSL(builder,     "sl_lActipl","ExtDisplay.IPL",  CharaSettings.CharaDisplayLineActItemsPerLine); // already live

			BindDDLive(builder, "dd_lFea",   CharaSettings.CharaDisplayLineFeat);
			BindTGLive(builder, "tg_lFeapcf",CharaSettings.CharaDisplayLineFeatPCFactionOnly);
			BindSLLive(builder, "sl_lFeasz", "ExtDisplay.Size", CharaSettings.CharaDisplayLineFeatSize);
			BindSL(builder,     "sl_lFeaipl","ExtDisplay.IPL",  CharaSettings.CharaDisplayLineFeatItemsPerLine); // already live
		}

		// -------------------------------------------------------------------------
		// Helpers — map "Keep"/"Hide"/"Disable" ↔ dropdown index 0/1/2
		// -------------------------------------------------------------------------

		private static int    ToIdx(string v)  => v == "Hide" ? 1 : v == "Disable" ? 2 : 0;
		private static string FromIdx(int v)   => v == 2 ? "Disable" : v == 1 ? "Hide" : "Keep";

		// Re-bake all CharaConfigClass instances from current ConfigEntry values.
		// Called after any character-line setting changes so they take effect
		// immediately without a game restart.
		private static void RefreshCharaSettings()
		{
			CharaSettings.CharaDisplayLine1Settings = new CharaSettings.CharaConfigClass(
				CharaSettings.CharaDisplayLine1.Value,
				CharaSettings.CharaDisplayLine1PCFactionOnly.Value,
				CharaSettings.CharaDisplayLine1Size.Value);
			CharaSettings.CharaDisplayLine2Settings = new CharaSettings.CharaConfigClass(
				CharaSettings.CharaDisplayLine2.Value,
				CharaSettings.CharaDisplayLine2PCFactionOnly.Value,
				CharaSettings.CharaDisplayLine2Size.Value);
			CharaSettings.CharaDisplayLine3Settings = new CharaSettings.CharaConfigClass(
				CharaSettings.CharaDisplayLine3.Value,
				CharaSettings.CharaDisplayLine3PCFactionOnly.Value,
				CharaSettings.CharaDisplayLine3Size.Value);
			CharaSettings.CharaDisplayLine4Settings = new CharaSettings.CharaConfigClass(
				CharaSettings.CharaDisplayLine4.Value,
				CharaSettings.CharaDisplayLine4PCFactionOnly.Value,
				CharaSettings.CharaDisplayLine4Size.Value);
			CharaSettings.CharaDisplayLineResistSettings = new CharaSettings.CharaConfigClass(
				CharaSettings.CharaDisplayLineResist.Value,
				CharaSettings.CharaDisplayLineResistPCFactionOnly.Value,
				CharaSettings.CharaDisplayLineResistSize.Value);
			CharaSettings.CharaDisplayLineAttributesSettings = new CharaSettings.CharaConfigClass(
				CharaSettings.CharaDisplayLineAttributes.Value,
				CharaSettings.CharaDisplayLineAttributesPCFactionOnly.Value,
				CharaSettings.CharaDisplayLineAttributesSize.Value);
			CharaSettings.CharaDisplayLineFavgiftSettings = new CharaSettings.CharaConfigClass(
				CharaSettings.CharaDisplayLineFavgift.Value,
				CharaSettings.CharaDisplayLineFavgiftPCFactionOnly.Value,
				CharaSettings.CharaDisplayLineFavgiftSize.Value);
			CharaSettings.CharaDisplayLineActSettings = new CharaSettings.CharaConfigClass(
				CharaSettings.CharaDisplayLineAct.Value,
				CharaSettings.CharaDisplayLineActPCFactionOnly.Value,
				CharaSettings.CharaDisplayLineActSize.Value);
			CharaSettings.CharaDisplayLineFeatSettings = new CharaSettings.CharaConfigClass(
				CharaSettings.CharaDisplayLineFeat.Value,
				CharaSettings.CharaDisplayLineFeatPCFactionOnly.Value,
				CharaSettings.CharaDisplayLineFeatSize.Value);
		}

		// Bind dropdown — updates ConfigEntry only (main feature toggles; no live rebake)
		private static void BindDD(OptionUIBuilder b, string id, ConfigEntry<string> e)
		{
			// <one_choice type="dropdown"> produces OptDropdown
			var dd = b.GetPreBuild<OptDropdown>(id);
			if (dd == null) return;
			dd.Value = ToIdx(e.Value);
			dd.OnValueChanged += v => e.Value = FromIdx(v);
		}

		// Bind dropdown and re-bake CharaSettings on change (character lines)
		private static void BindDDLive(OptionUIBuilder b, string id, ConfigEntry<string> e)
		{
			var dd = b.GetPreBuild<OptDropdown>(id);
			if (dd == null) return;
			dd.Value = ToIdx(e.Value);
			dd.OnValueChanged += v => { e.Value = FromIdx(v); RefreshCharaSettings(); };
		}

		// Bind toggle and re-bake CharaSettings on change
		private static void BindTGLive(OptionUIBuilder b, string id, ConfigEntry<bool> e)
		{
			var tg = b.GetPreBuild<OptToggle>(id);
			if (tg == null) return;
			tg.Checked = e.Value;                                     // OptToggle uses .Checked
			tg.OnValueChanged += v => { e.Value = v; RefreshCharaSettings(); };
		}

		// Bind slider and re-bake CharaSettings on change
		private static void BindSLLive(OptionUIBuilder b, string id, string titleId, ConfigEntry<int> e)
		{
			var sl = b.GetPreBuild<OptSlider>(id);
			if (sl == null) return;
			sl.Title = _controller.Tr(titleId);
			sl.Value = e.Value;
			sl.OnValueChanged += v => { e.Value = (int)v; RefreshCharaSettings(); };
		}

		// Bind slider without rebake (for already-live settings like ItemsPerLine)
		private static void BindSL(OptionUIBuilder b, string id, string titleId, ConfigEntry<int> e)
		{
			var sl = b.GetPreBuild<OptSlider>(id);
			if (sl == null) return;
			sl.Title = _controller.Tr(titleId);
			sl.Value = e.Value;
			sl.OnValueChanged += v => e.Value = (int)v;
		}
	}
}

