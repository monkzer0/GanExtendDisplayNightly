// ModOptionsIntegration.cs
// Soft-dependency integration with EvilMask's Mod Options.
//
// The TypeLoadException was caused by:
//   private static ModOptionController _controller;  ← static field on this class
//
// When the CLR loads ModOptionsIntegration, it resolves all field types.
// That forced ModOptions.dll to load, throwing TypeLoadException for users
// without the mod.
//
// Fix: no static fields of ModOptions types.  The controller is held as a
// local variable in Register() and passed to helpers as a parameter.
// Register() is [NoInlining] so the JIT will not compile it (and therefore
// will not need ModOptions.dll) until the method is actually called — which
// only happens after the presence guard confirms ModOptions is installed.
//
// Reference: https://github.com/EvilMask/Elin.ModOptions
// Required: Elin/Package/Mod_ModOptions/ModOptions.dll (compile-time reference only;
//           not copied to output — Private=False in .csproj).

using System;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using EvilMask.Elin.ModOptions;
using EvilMask.Elin.ModOptions.UI;

namespace GanExtendDisplay
{
	internal static class ModOptionsIntegration
	{
		private const string ModOptionsGuid = "evilmask.elinplugins.modoptions";

		// No static fields that reference ModOptions types — that was the bug.

		/// <summary>
		/// Call from Main.Start() after all config entries are bound.
		/// Safe to call when Mod Options is not installed — exits immediately.
		/// </summary>
		internal static void TryRegister(BaseUnityPlugin plugin)
		{
			// Use BepInEx's own plugin registry rather than [BepInDependency(SoftDependency)]
			// (which has a known issue where a soft dep with an unmet hard dep causes the
			// depending plugin to fail) or ModManager.ListPluginObject (which may not
			// contain BepInEx plugins, or may not be populated at Start() time).
			// Chainloader.PluginInfos is guaranteed to be fully populated before Start().
			if (!BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ModOptionsGuid))
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

		// [NoInlining] — the JIT will not compile this method (and therefore will
		// not need ModOptions types) until it is actually called, i.e. only after
		// the guard above has confirmed ModOptions is present.
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void Register(BaseUnityPlugin plugin)
		{
			var ctrl = ModOptionController.Register(plugin.Info.Metadata.GUID, "ExtDisplay.Tab");
			SetTranslations(ctrl);
			ctrl.SetPreBuildWithXml(BuildXml());
			ctrl.OnBuildUI += OnBuildUI;
			Main.Logger.LogInfo("[GanExtendDisplay] Registered with Mod Options.");
		}

		// -------------------------------------------------------------------------
		// Translations — SetTranslation(id, English, Japanese, Chinese Simplified)
		// -------------------------------------------------------------------------

		private static void SetTranslations(ModOptionController ctrl)
		{
			// Tab button tooltip
			T(ctrl, "ExtDisplay.Tab",
				"Extend Display",
				"拡張表示",
				"扩展显示");

			// Section headers
			T(ctrl, "ExtDisplay.Sec.Affected",
				"Affected Display",
				"表示機能スイッチ",
				"显示功能开关");
			T(ctrl, "ExtDisplay.Sec.Chara",
				"Character Display Lines",
				"キャラクター情報表示行",
				"角色信息显示行");

			// Dropdown option labels (shared across all Keep/Hide/Disable dropdowns)
			T(ctrl, "ExtDisplay.Keep",
				"Keep (always visible)",
				"Keep（常に表示）",
				"Keep（始终显示）");
			T(ctrl, "ExtDisplay.Hide",
				"Hide (Alt to reveal)",
				"Hide（Altキーで表示）",
				"Hide（Alt 键显示）");
			T(ctrl, "ExtDisplay.Disable",
				"Disable (never shown)",
				"Disable（常に非表示）",
				"Disable（永不显示）");

			// Main feature section labels
			T(ctrl, "ExtDisplay.F.Chara",
				"Character Display",
				"キャラクター表示",
				"角色悬停显示");
			T(ctrl, "ExtDisplay.F.Thing",
				"Thing Display (Ground Items)",
				"アイテム表示（地面）",
				"物品显示（地面物品）");
			T(ctrl, "ExtDisplay.F.Interact",
				"Interact Display (Harvesting)",
				"インタラクト表示（採集）",
				"交互显示（采集作业）");
			T(ctrl, "ExtDisplay.F.Notif",
				"Notification UI (Buff Bar)",
				"通知UI（バフバー）",
				"通知UI（增益栏）");
			T(ctrl, "ExtDisplay.F.Enchant",
				"Enchant Display (Equip / DNA)",
				"エンチャント表示（装備/DNA）",
				"附魔显示（装备 / DNA）");

			// Per-feature tooltip notes
			T(ctrl, "ExtDisplay.F.AffectedNote",
				"Note: enabling/disabling these features requires a game restart.",
				"注：これらの機能の有効/無効にはゲームの再起動が必要です。",
				"注意：启用/禁用这些功能需要重启游戏。");

			// Character line labels
			T(ctrl, "ExtDisplay.L.L1",
				"Line 1: Sex, Age, Race, Job, AI, Armor Skill, Attack Style",
				"行1：性別 / 年齢 / 種族 / 職業 / AI / 防具スキル / 攻撃スタイル",
				"第1行：性别 / 年龄 / 种族 / 职业 / AI / 护甲技能 / 攻击方式");
			T(ctrl, "ExtDisplay.L.L2",
				"Line 2: HP, DV, PV, Speed",
				"行2：HP / DV / PV / 速度",
				"第2行：生命值 / 回避值 / 防御值 / 速度");
			T(ctrl, "ExtDisplay.L.L3",
				"Line 3: SP, Hunger, Works / Hobbies",
				"行3：SP / 空腹度 / 仕事 / 趣味",
				"第3行：精力值 / 饥饿度 / 工作 / 爱好");
			T(ctrl, "ExtDisplay.L.L4",
				"Line 4: MP, Weight, EXP",
				"行4：MP / 重量 / 経験値",
				"第4行：魔法值 / 负重 / 经验值");
			T(ctrl, "ExtDisplay.L.Res",
				"Resist Line: Elemental Resistances",
				"耐性行：属性耐性",
				"抗性行：属性抗性");
			T(ctrl, "ExtDisplay.L.Att",
				"Attributes Line: STR CON DEX PER LRN WIL MAG CHR",
				"属性行：STR CON DEX PER LRN WIL MAG CHR",
				"属性行：力量 体质 灵巧 感知 学习 意志 魔力 魅力");
			T(ctrl, "ExtDisplay.L.Fav",
				"Favorite Gift Line",
				"好物行",
				"喜爱礼物行");
			T(ctrl, "ExtDisplay.L.Act",
				"Act Line: Active Abilities",
				"行動行：アクティブスキル",
				"行动行：主动技能");
			T(ctrl, "ExtDisplay.L.Fea",
				"Feat Line: Passive Traits",
				"特技行：パッシブ特性",
				"特技行：被动特质");

			// Per-line sub-setting labels
			T(ctrl, "ExtDisplay.PCFac",
				"PC Faction Only",
				"PC派閥のみ",
				"仅玩家阵营");
			T(ctrl, "ExtDisplay.Size.10", "10", "10", "10");
			T(ctrl, "ExtDisplay.Size.12", "12", "12", "12");
			T(ctrl, "ExtDisplay.Size.13", "13", "13", "13");
			T(ctrl, "ExtDisplay.Size.14", "14", "14", "14");
			T(ctrl, "ExtDisplay.Size.15", "15", "15", "15");
			T(ctrl, "ExtDisplay.Size.16", "16", "16", "16");
			T(ctrl, "ExtDisplay.Size.17", "17", "17", "17");
			T(ctrl, "ExtDisplay.Size.18", "18", "18", "18");
			T(ctrl, "ExtDisplay.Size.19", "19", "19", "19");
			T(ctrl, "ExtDisplay.Size.20", "20", "20", "20");
			T(ctrl, "ExtDisplay.Size.22", "22", "22", "22");
			T(ctrl, "ExtDisplay.Size.24", "24", "24", "24");
			T(ctrl, "ExtDisplay.IPL",
				"Items Per Line:",
				"1行の件数:",
				"每行条目数:");
			T(ctrl, "ExtDisplay.IPL.0",  "0  (no limit)", "0（無制限）", "0（不限制）");
			T(ctrl, "ExtDisplay.IPL.2",  "2",  "2",  "2");
			T(ctrl, "ExtDisplay.IPL.3",  "3",  "3",  "3");
			T(ctrl, "ExtDisplay.IPL.4",  "4",  "4",  "4");
			T(ctrl, "ExtDisplay.IPL.5",  "5",  "5",  "5");
			T(ctrl, "ExtDisplay.IPL.6",  "6",  "6",  "6");
			T(ctrl, "ExtDisplay.IPL.7",  "7",  "7",  "7");
			T(ctrl, "ExtDisplay.IPL.8",  "8",  "8",  "8");
			T(ctrl, "ExtDisplay.IPL.9",  "9",  "9",  "9");
			T(ctrl, "ExtDisplay.IPL.10", "10", "10", "10");

			// Character line change note
			T(ctrl, "ExtDisplay.CharaNote",
				"Character line changes take effect immediately.",
				"キャラクター行の変更はすぐに反映されます。",
				"角色行设置更改立即生效。");
		}

		private static void T(ModOptionController ctrl, string id, string en, string jp, string cn)
		{
			ctrl.SetTranslation(id, en, jp, cn);
		}

		// -------------------------------------------------------------------------
		// XML layout
		// -------------------------------------------------------------------------

		private static string BuildXml() => @"<config>
  <topic>ExtDisplay.Sec.Affected</topic>
  <text align=""left"">ExtDisplay.F.AffectedNote</text>
  <topic>ExtDisplay.F.Chara</topic>
  <one_choice id=""dd_charaDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>
  <topic>ExtDisplay.F.Thing</topic>
  <one_choice id=""dd_thingDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>
  <topic>ExtDisplay.F.Interact</topic>
  <one_choice id=""dd_interactDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>
  <topic>ExtDisplay.F.Notif</topic>
  <one_choice id=""dd_notifDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>
  <topic>ExtDisplay.F.Enchant</topic>
  <one_choice id=""dd_enchantDisp"" type=""dropdown"">
    <choice><contentId>ExtDisplay.Keep</contentId></choice>
    <choice><contentId>ExtDisplay.Hide</contentId></choice>
    <choice><contentId>ExtDisplay.Disable</contentId></choice>
  </one_choice>

  <topic>ExtDisplay.Sec.Chara</topic>
  <text align=""left"">ExtDisplay.CharaNote</text>

  <topic>ExtDisplay.L.L1</topic>
  <hlayout align=""left"">
    <one_choice id=""dd_l1"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_l1pcf"" width=""20%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <one_choice id=""dd_l1sz"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Size.10</contentId></choice>
      <choice><contentId>ExtDisplay.Size.12</contentId></choice>
      <choice><contentId>ExtDisplay.Size.13</contentId></choice>
      <choice><contentId>ExtDisplay.Size.14</contentId></choice>
      <choice><contentId>ExtDisplay.Size.15</contentId></choice>
      <choice><contentId>ExtDisplay.Size.16</contentId></choice>
      <choice><contentId>ExtDisplay.Size.17</contentId></choice>
      <choice><contentId>ExtDisplay.Size.18</contentId></choice>
      <choice><contentId>ExtDisplay.Size.19</contentId></choice>
      <choice><contentId>ExtDisplay.Size.20</contentId></choice>
      <choice><contentId>ExtDisplay.Size.22</contentId></choice>
      <choice><contentId>ExtDisplay.Size.24</contentId></choice>
    </one_choice>
  </hlayout>

  <topic>ExtDisplay.L.L2</topic>
  <hlayout align=""left"">
    <one_choice id=""dd_l2"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_l2pcf"" width=""20%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <one_choice id=""dd_l2sz"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Size.10</contentId></choice>
      <choice><contentId>ExtDisplay.Size.12</contentId></choice>
      <choice><contentId>ExtDisplay.Size.13</contentId></choice>
      <choice><contentId>ExtDisplay.Size.14</contentId></choice>
      <choice><contentId>ExtDisplay.Size.15</contentId></choice>
      <choice><contentId>ExtDisplay.Size.16</contentId></choice>
      <choice><contentId>ExtDisplay.Size.17</contentId></choice>
      <choice><contentId>ExtDisplay.Size.18</contentId></choice>
      <choice><contentId>ExtDisplay.Size.19</contentId></choice>
      <choice><contentId>ExtDisplay.Size.20</contentId></choice>
      <choice><contentId>ExtDisplay.Size.22</contentId></choice>
      <choice><contentId>ExtDisplay.Size.24</contentId></choice>
    </one_choice>
  </hlayout>

  <topic>ExtDisplay.L.L3</topic>
  <hlayout align=""left"">
    <one_choice id=""dd_l3"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_l3pcf"" width=""20%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <one_choice id=""dd_l3sz"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Size.10</contentId></choice>
      <choice><contentId>ExtDisplay.Size.12</contentId></choice>
      <choice><contentId>ExtDisplay.Size.13</contentId></choice>
      <choice><contentId>ExtDisplay.Size.14</contentId></choice>
      <choice><contentId>ExtDisplay.Size.15</contentId></choice>
      <choice><contentId>ExtDisplay.Size.16</contentId></choice>
      <choice><contentId>ExtDisplay.Size.17</contentId></choice>
      <choice><contentId>ExtDisplay.Size.18</contentId></choice>
      <choice><contentId>ExtDisplay.Size.19</contentId></choice>
      <choice><contentId>ExtDisplay.Size.20</contentId></choice>
      <choice><contentId>ExtDisplay.Size.22</contentId></choice>
      <choice><contentId>ExtDisplay.Size.24</contentId></choice>
    </one_choice>
  </hlayout>

  <topic>ExtDisplay.L.L4</topic>
  <hlayout align=""left"">
    <one_choice id=""dd_l4"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_l4pcf"" width=""20%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <one_choice id=""dd_l4sz"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Size.10</contentId></choice>
      <choice><contentId>ExtDisplay.Size.12</contentId></choice>
      <choice><contentId>ExtDisplay.Size.13</contentId></choice>
      <choice><contentId>ExtDisplay.Size.14</contentId></choice>
      <choice><contentId>ExtDisplay.Size.15</contentId></choice>
      <choice><contentId>ExtDisplay.Size.16</contentId></choice>
      <choice><contentId>ExtDisplay.Size.17</contentId></choice>
      <choice><contentId>ExtDisplay.Size.18</contentId></choice>
      <choice><contentId>ExtDisplay.Size.19</contentId></choice>
      <choice><contentId>ExtDisplay.Size.20</contentId></choice>
      <choice><contentId>ExtDisplay.Size.22</contentId></choice>
      <choice><contentId>ExtDisplay.Size.24</contentId></choice>
    </one_choice>
  </hlayout>

  <topic>ExtDisplay.L.Res</topic>
  <hlayout align=""left"">
    <one_choice id=""dd_lRes"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_lRespcf"" width=""20%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <one_choice id=""dd_lRessz"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Size.10</contentId></choice>
      <choice><contentId>ExtDisplay.Size.12</contentId></choice>
      <choice><contentId>ExtDisplay.Size.13</contentId></choice>
      <choice><contentId>ExtDisplay.Size.14</contentId></choice>
      <choice><contentId>ExtDisplay.Size.15</contentId></choice>
      <choice><contentId>ExtDisplay.Size.16</contentId></choice>
      <choice><contentId>ExtDisplay.Size.17</contentId></choice>
      <choice><contentId>ExtDisplay.Size.18</contentId></choice>
      <choice><contentId>ExtDisplay.Size.19</contentId></choice>
      <choice><contentId>ExtDisplay.Size.20</contentId></choice>
      <choice><contentId>ExtDisplay.Size.22</contentId></choice>
      <choice><contentId>ExtDisplay.Size.24</contentId></choice>
    </one_choice>
  </hlayout>

  <topic>ExtDisplay.L.Att</topic>
  <hlayout align=""left"">
    <one_choice id=""dd_lAtt"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_lAttpcf"" width=""20%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <one_choice id=""dd_lAttsz"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Size.10</contentId></choice>
      <choice><contentId>ExtDisplay.Size.12</contentId></choice>
      <choice><contentId>ExtDisplay.Size.13</contentId></choice>
      <choice><contentId>ExtDisplay.Size.14</contentId></choice>
      <choice><contentId>ExtDisplay.Size.15</contentId></choice>
      <choice><contentId>ExtDisplay.Size.16</contentId></choice>
      <choice><contentId>ExtDisplay.Size.17</contentId></choice>
      <choice><contentId>ExtDisplay.Size.18</contentId></choice>
      <choice><contentId>ExtDisplay.Size.19</contentId></choice>
      <choice><contentId>ExtDisplay.Size.20</contentId></choice>
      <choice><contentId>ExtDisplay.Size.22</contentId></choice>
      <choice><contentId>ExtDisplay.Size.24</contentId></choice>
    </one_choice>
  </hlayout>

  <topic>ExtDisplay.L.Fav</topic>
  <hlayout align=""left"">
    <one_choice id=""dd_lFav"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_lFavpcf"" width=""20%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <one_choice id=""dd_lFavsz"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Size.10</contentId></choice>
      <choice><contentId>ExtDisplay.Size.12</contentId></choice>
      <choice><contentId>ExtDisplay.Size.13</contentId></choice>
      <choice><contentId>ExtDisplay.Size.14</contentId></choice>
      <choice><contentId>ExtDisplay.Size.15</contentId></choice>
      <choice><contentId>ExtDisplay.Size.16</contentId></choice>
      <choice><contentId>ExtDisplay.Size.17</contentId></choice>
      <choice><contentId>ExtDisplay.Size.18</contentId></choice>
      <choice><contentId>ExtDisplay.Size.19</contentId></choice>
      <choice><contentId>ExtDisplay.Size.20</contentId></choice>
      <choice><contentId>ExtDisplay.Size.22</contentId></choice>
      <choice><contentId>ExtDisplay.Size.24</contentId></choice>
    </one_choice>
  </hlayout>

  <topic>ExtDisplay.L.Act</topic>
  <hlayout align=""left"">
    <one_choice id=""dd_lAct"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_lActpcf"" width=""20%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <one_choice id=""dd_lActsz"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Size.10</contentId></choice>
      <choice><contentId>ExtDisplay.Size.12</contentId></choice>
      <choice><contentId>ExtDisplay.Size.13</contentId></choice>
      <choice><contentId>ExtDisplay.Size.14</contentId></choice>
      <choice><contentId>ExtDisplay.Size.15</contentId></choice>
      <choice><contentId>ExtDisplay.Size.16</contentId></choice>
      <choice><contentId>ExtDisplay.Size.17</contentId></choice>
      <choice><contentId>ExtDisplay.Size.18</contentId></choice>
      <choice><contentId>ExtDisplay.Size.19</contentId></choice>
      <choice><contentId>ExtDisplay.Size.20</contentId></choice>
      <choice><contentId>ExtDisplay.Size.22</contentId></choice>
      <choice><contentId>ExtDisplay.Size.24</contentId></choice>
    </one_choice>
  </hlayout>
  <text align=""left"">ExtDisplay.IPL</text>
  <one_choice id=""dd_lActipl"" type=""dropdown"" width=""40%"">
    <choice><contentId>ExtDisplay.IPL.0</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.2</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.3</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.4</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.5</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.6</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.7</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.8</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.9</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.10</contentId></choice>
  </one_choice>

  <topic>ExtDisplay.L.Fea</topic>
  <hlayout align=""left"">
    <one_choice id=""dd_lFea"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Keep</contentId></choice>
      <choice><contentId>ExtDisplay.Hide</contentId></choice>
      <choice><contentId>ExtDisplay.Disable</contentId></choice>
    </one_choice>
    <toggle id=""tg_lFeapcf"" width=""20%"">
      <contentId>ExtDisplay.PCFac</contentId>
    </toggle>
    <one_choice id=""dd_lFeasz"" type=""dropdown"" width=""40%"">
      <choice><contentId>ExtDisplay.Size.10</contentId></choice>
      <choice><contentId>ExtDisplay.Size.12</contentId></choice>
      <choice><contentId>ExtDisplay.Size.13</contentId></choice>
      <choice><contentId>ExtDisplay.Size.14</contentId></choice>
      <choice><contentId>ExtDisplay.Size.15</contentId></choice>
      <choice><contentId>ExtDisplay.Size.16</contentId></choice>
      <choice><contentId>ExtDisplay.Size.17</contentId></choice>
      <choice><contentId>ExtDisplay.Size.18</contentId></choice>
      <choice><contentId>ExtDisplay.Size.19</contentId></choice>
      <choice><contentId>ExtDisplay.Size.20</contentId></choice>
      <choice><contentId>ExtDisplay.Size.22</contentId></choice>
      <choice><contentId>ExtDisplay.Size.24</contentId></choice>
    </one_choice>
  </hlayout>
  <text align=""left"">ExtDisplay.IPL</text>
  <one_choice id=""dd_lFeaipl"" type=""dropdown"" width=""40%"">
    <choice><contentId>ExtDisplay.IPL.0</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.2</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.3</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.4</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.5</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.6</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.7</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.8</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.9</contentId></choice>
    <choice><contentId>ExtDisplay.IPL.10</contentId></choice>
  </one_choice>

  <vlayout height=""30""/>
</config>";

		// -------------------------------------------------------------------------
		// OnBuildUI — wire each UI element to its ConfigEntry
		// -------------------------------------------------------------------------

		private static void OnBuildUI(OptionUIBuilder builder)
		{
			if (!builder.Valid) return;

			// --- Affected Display ---
			BindDD(builder, "dd_charaDisp",    PluginSettings.CharaDisplay);
			BindDD(builder, "dd_thingDisp",    PluginSettings.ThingDisplay);
			BindDD(builder, "dd_interactDisp", PluginSettings.InteractDisplay);
			BindDD(builder, "dd_notifDisp",    PluginSettings.NotificationUI);
			BindDD(builder, "dd_enchantDisp",  PluginSettings.EnchantDisplay);

			// --- Character Lines (all live — re-bake CharaConfigClass on any change) ---
			BindDDLive(builder, "dd_l1",      CharaSettings.CharaDisplayLine1);
			BindTGLive(builder, "tg_l1pcf",   CharaSettings.CharaDisplayLine1PCFactionOnly);
			BindDDSzLive(builder, "dd_l1sz",  CharaSettings.CharaDisplayLine1Size);

			BindDDLive(builder, "dd_l2",      CharaSettings.CharaDisplayLine2);
			BindTGLive(builder, "tg_l2pcf",   CharaSettings.CharaDisplayLine2PCFactionOnly);
			BindDDSzLive(builder, "dd_l2sz",  CharaSettings.CharaDisplayLine2Size);

			BindDDLive(builder, "dd_l3",      CharaSettings.CharaDisplayLine3);
			BindTGLive(builder, "tg_l3pcf",   CharaSettings.CharaDisplayLine3PCFactionOnly);
			BindDDSzLive(builder, "dd_l3sz",  CharaSettings.CharaDisplayLine3Size);

			BindDDLive(builder, "dd_l4",      CharaSettings.CharaDisplayLine4);
			BindTGLive(builder, "tg_l4pcf",   CharaSettings.CharaDisplayLine4PCFactionOnly);
			BindDDSzLive(builder, "dd_l4sz",  CharaSettings.CharaDisplayLine4Size);

			BindDDLive(builder, "dd_lRes",    CharaSettings.CharaDisplayLineResist);
			BindTGLive(builder, "tg_lRespcf", CharaSettings.CharaDisplayLineResistPCFactionOnly);
			BindDDSzLive(builder, "dd_lRessz",CharaSettings.CharaDisplayLineResistSize);

			BindDDLive(builder, "dd_lAtt",    CharaSettings.CharaDisplayLineAttributes);
			BindTGLive(builder, "tg_lAttpcf", CharaSettings.CharaDisplayLineAttributesPCFactionOnly);
			BindDDSzLive(builder, "dd_lAttsz",CharaSettings.CharaDisplayLineAttributesSize);

			BindDDLive(builder, "dd_lFav",    CharaSettings.CharaDisplayLineFavgift);
			BindTGLive(builder, "tg_lFavpcf", CharaSettings.CharaDisplayLineFavgiftPCFactionOnly);
			BindDDSzLive(builder, "dd_lFavsz",CharaSettings.CharaDisplayLineFavgiftSize);

			BindDDLive(builder, "dd_lAct",    CharaSettings.CharaDisplayLineAct);
			BindTGLive(builder, "tg_lActpcf", CharaSettings.CharaDisplayLineActPCFactionOnly);
			BindDDSzLive(builder, "dd_lActsz",CharaSettings.CharaDisplayLineActSize);
			BindDDIplLive(builder, "dd_lActipl", CharaSettings.CharaDisplayLineActItemsPerLine);

			BindDDLive(builder, "dd_lFea",    CharaSettings.CharaDisplayLineFeat);
			BindTGLive(builder, "tg_lFeapcf", CharaSettings.CharaDisplayLineFeatPCFactionOnly);
			BindDDSzLive(builder, "dd_lFeasz",CharaSettings.CharaDisplayLineFeatSize);
			BindDDIplLive(builder, "dd_lFeaipl", CharaSettings.CharaDisplayLineFeatItemsPerLine);
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
			tg.Checked = e.Value;
			tg.OnValueChanged += v => { e.Value = v; RefreshCharaSettings(); };
		}

		// -------------------------------------------------------------------------
		// Font-size dropdown helpers — values {10,12,13,14,15,16,17,18,19,20,22,24}
		// -------------------------------------------------------------------------

		private static readonly int[] SzValues = { 10, 12, 13, 14, 15, 16, 17, 18, 19, 20, 22, 24 };

		private static int ToSzIdx(int v)
		{
			int best = 0;
			for (int i = 1; i < SzValues.Length; i++)
				if (Math.Abs(SzValues[i] - v) < Math.Abs(SzValues[best] - v))
					best = i;
			return best;
		}

		private static int FromSzIdx(int i) =>
			SzValues[Math.Max(0, Math.Min(i, SzValues.Length - 1))];

		private static void BindDDSzLive(OptionUIBuilder b, string id, ConfigEntry<int> e)
		{
			var dd = b.GetPreBuild<OptDropdown>(id);
			if (dd == null) return;
			dd.Value = ToSzIdx(e.Value);
			dd.OnValueChanged += v => { e.Value = FromSzIdx(v); RefreshCharaSettings(); };
		}

		// -------------------------------------------------------------------------
		// IPL dropdown helpers — values are {0, 2, 3, 4, 5, 6, 7, 8, 9, 10}
		// (1 is intentionally skipped; 0 means no limit)
		// -------------------------------------------------------------------------

		private static readonly int[] IplValues = { 0, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

		private static int ToIplIdx(int v)
		{
			for (int i = 0; i < IplValues.Length; i++)
				if (IplValues[i] == v) return i;
			return 0; // unknown value → "no limit"
		}

		private static int FromIplIdx(int i) =>
			IplValues[Math.Max(0, Math.Min(i, IplValues.Length - 1))];

		private static void BindDDIplLive(OptionUIBuilder b, string id, ConfigEntry<int> e)
		{
			var dd = b.GetPreBuild<OptDropdown>(id);
			if (dd == null) return;
			dd.Value = ToIplIdx(e.Value);
			dd.OnValueChanged += v => { e.Value = FromIplIdx(v); RefreshCharaSettings(); };
		}
	}
}
