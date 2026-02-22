using BepInEx.Configuration;
using System.Runtime.CompilerServices;
using System;

using static GanExtendDisplay.DisplayConfigBaseClass;
using static Layer;

namespace GanExtendDisplay
{
	public class DisplayConfigBaseClass
	{
		public enum DisplayOption
		{
			Keep,
			Hide,
			Disable
		}
		public static DisplayOption StringToDisplayOption(string value) {
			if (Enum.TryParse(value, out DisplayOption result)) {
				return result;
			}
			// 如果解析失败，返回默认值 Keep
			return DisplayOption.Keep;
		}
		//通过获取配置得到本地变量缓存
		public class ConfigClass
		{
			//父类
			public DisplayOption OptionStatus { get; set; }
			public ConfigClass(String option) {
				// 初始化 DisplayOption 属性
				OptionStatus = StringToDisplayOption(option);
			}
			public bool CheckStatus {
				get {
					// 默认显示内容
					if (OptionStatus == DisplayOption.Keep) {
						return true;
					}
					//按下按键，显示隐藏内容
					if (Main.ChangeDisplay) {
						if (OptionStatus == DisplayOption.Hide) {
							return true;
						}
					}
					return false;
				}

			}
			public bool getCheckStatus() {
				return this.CheckStatus;
			}
		}


	}

	public class PluginSettings {

		public static ConfigEntry<String> CharaDisplay;
		public static ConfigEntry<String> ThingDisplay;
		public static ConfigEntry<String> InteractDisplay;
		public static ConfigEntry<String> NotificationUI;
		public static ConfigEntry<String> EnchantDisplay;
		public static void CharaDisplayConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Affected Display", "Character Display");
			var acceptableValues = new AcceptableValueList<string>("Keep", "Hide", "Disable");
			ConfigDescription configDescription = new ConfigDescription(
				"Affects how additional information is displayed when hovering over a character. Options: \"Keep\", \"Hide\", \"Disable\".\n" +
				"控制鼠标悬停在角色上时附加信息的显示方式。选项：\"Keep\"（保持）、\"Hide\"（隐藏）、\"Disable\"（禁用）。\n" +
				"キャラクターにマウスを重ねた際の追加情報の表示方法を設定します。オプション：\"Keep\"（維持）、\"Hide\"（非表示）、\"Disable\"（無効）。",
				acceptableValues, Array.Empty<object>());
			PluginSettings.CharaDisplay = config.Bind<String>(configDefinition, "Keep", configDescription);
		}
		public static void ThingDisplayConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Affected Display", "Thing Display");
			var acceptableValues = new AcceptableValueList<string>("Keep", "Hide", "Disable");
			ConfigDescription configDescription = new ConfigDescription(
				"Affects the display of additional information (level, rarity, material) when hovering over items on the ground. Options: \"Keep\", \"Hide\", \"Disable\".\n" +
				"控制鼠标悬停在地面物品上时附加信息（等级、稀有度、材质）的显示方式。选项：\"Keep\"（保持）、\"Hide\"（隐藏）、\"Disable\"（禁用）。\n" +
				"地面のアイテムにマウスを重ねた際の追加情報（レベル、レアリティ、素材）の表示方法を設定します。オプション：\"Keep\"（維持）、\"Hide\"（非表示）、\"Disable\"（無効）。",
				acceptableValues, Array.Empty<object>());
			PluginSettings.ThingDisplay = config.Bind<String>(configDefinition, "Keep", configDescription);
		}
		public static void EnchantDisplayConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Affected Display", "Enchant Display");
			var acceptableValues = new AcceptableValueList<string>("Keep", "Hide", "Disable");
			ConfigDescription configDescription = new ConfigDescription(
				"Affects the display of additional information (enchantment entries) when hovering over equipment and DNA. Options: \"Keep\", \"Hide\", \"Disable\".\n" +
				"控制鼠标悬停在装备和DNA上时附加信息（附魔词条）的显示方式。选项：\"Keep\"（保持）、\"Hide\"（隐藏）、\"Disable\"（禁用）。\n" +
				"装備とDNAにマウスを重ねた際の追加情報（エンチャント項目）の表示方法を設定します。オプション：\"Keep\"（維持）、\"Hide\"（非表示）、\"Disable\"（無効）。",
				acceptableValues, Array.Empty<object>());
			PluginSettings.EnchantDisplay = config.Bind<String>(configDefinition, "Keep", configDescription);
		}
		public static void InteractDisplayConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Affected Display", "Interact Display");
			var acceptableValues = new AcceptableValueList<string>("Keep", "Hide", "Disable");
			ConfigDescription configDescription = new ConfigDescription(
				"Affects how additional information (enchantment entries) is displayed when hovering over an item in the backpack. Options: \"Keep\", \"Hide\", \"Disable\".\n" +
				"控制鼠标悬停在背包物品上时附加信息（附魔词条）的显示方式。选项：\"Keep\"（保持）、\"Hide\"（隐藏）、\"Disable\"（禁用）。\n" +
				"バックパック内のアイテムにマウスを重ねた際の追加情報（エンチャント項目）の表示方法を設定します。オプション：\"Keep\"（維持）、\"Hide\"（非表示）、\"Disable\"（無効）。",
				acceptableValues, Array.Empty<object>());
			PluginSettings.InteractDisplay = config.Bind<String>(configDefinition, "Keep", configDescription);
		}
		public static void NotificationUiDisplayConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Affected Display", "Notification UI Display");
			var acceptableValues = new AcceptableValueList<string>("Keep", "Hide", "Disable");
			ConfigDescription configDescription = new ConfigDescription(
				"Affects how additional information is displayed when interacting with character status and buff notification controls in the UI. Options: \"Keep\", \"Hide\", \"Disable\".\n" +
				"控制与UI中角色状态和增益通知控件交互时附加信息的显示方式。选项：\"Keep\"（保持）、\"Hide\"（隐藏）、\"Disable\"（禁用）。\n" +
				"UIのキャラクターステータスおよびバフ通知コントロールを操作する際の追加情報の表示方法を設定します。オプション：\"Keep\"（維持）、\"Hide\"（非表示）、\"Disable\"（無効）。",
				acceptableValues, Array.Empty<object>());
			PluginSettings.NotificationUI = config.Bind<String>(configDefinition, "Keep", configDescription);
		}
	}

	public class CharaSettings
	{
		public class CharaConfigClass
		{
			private ConfigClass _charaDisplayLine;
			private bool _charaDisplayPCFactionOnly;
			private int _size;

			// 构造函数，用于初始化父类属性和新增属性
			public CharaConfigClass(string charaDisplayLine, bool charaDisplayLine1PCFactionOnly, int size) {
				Main.Logger.LogInfo("checkPont1.1.1");
				_charaDisplayLine = new ConfigClass(charaDisplayLine);
				_charaDisplayPCFactionOnly = charaDisplayLine1PCFactionOnly;
				_size = size;
			}

			// 新增属性访问器 CharaDisplayLine1PCFactionOnly
			public bool CharaDisplayLineOut {
				get { return _charaDisplayLine.CheckStatus;  }
			}

			// 新增属性访问器 CharaDisplayLine1PCFactionOnly
			public bool CharaDisplayPCFactionOnly {
				get { return _charaDisplayPCFactionOnly; }
			}

			// 新增属性访问器 Size
			public int Size {
				get { return _size; }
			}
		}

		public static ConfigEntry<String> CharaDisplayLine1;
		public static ConfigEntry<bool> CharaDisplayLine1PCFactionOnly;
		public static ConfigEntry<int> CharaDisplayLine1Size;

		public static ConfigEntry<String> CharaDisplayLine2;
		public static ConfigEntry<bool> CharaDisplayLine2PCFactionOnly;
		public static ConfigEntry<int> CharaDisplayLine2Size;

		public static ConfigEntry<String> CharaDisplayLine3;
		public static ConfigEntry<bool> CharaDisplayLine3PCFactionOnly;
		public static ConfigEntry<int> CharaDisplayLine3Size;

		public static ConfigEntry<String> CharaDisplayLine4;
		public static ConfigEntry<bool> CharaDisplayLine4PCFactionOnly;
		public static ConfigEntry<int> CharaDisplayLine4Size;

		public static ConfigEntry<String> CharaDisplayLineResist;
		public static ConfigEntry<bool> CharaDisplayLineResistPCFactionOnly;
		public static ConfigEntry<int> CharaDisplayLineResistSize;

		public static ConfigEntry<String> CharaDisplayLineAttributes;
		public static ConfigEntry<bool> CharaDisplayLineAttributesPCFactionOnly;
		public static ConfigEntry<int> CharaDisplayLineAttributesSize;

		public static ConfigEntry<String> CharaDisplayLineFavgift;
		public static ConfigEntry<bool> CharaDisplayLineFavgiftPCFactionOnly;
		public static ConfigEntry<int> CharaDisplayLineFavgiftSize;

		public static ConfigEntry<String> CharaDisplayLineAct;
		public static ConfigEntry<bool> CharaDisplayLineActPCFactionOnly;
		//public static ConfigEntry<bool> CharaDisplayLineActAliasParent;
		public static ConfigEntry<int> CharaDisplayLineActSize;
		public static ConfigEntry<int> CharaDisplayLineActItemsPerLine;

		public static ConfigEntry<String> CharaDisplayLineFeat;
		public static ConfigEntry<bool> CharaDisplayLineFeatPCFactionOnly;
		public static ConfigEntry<int> CharaDisplayLineFeatSize;
		public static ConfigEntry<int> CharaDisplayLineFeatItemsPerLine;

		//line1
		public static void CharaDisplayLine1Config(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line1");
			var acceptableValues = new AcceptableValueList<string>("Keep", "Hide", "Disable");
			ConfigDescription configDescription = new ConfigDescription(
				"Line 1: Sex, Age, [Race, Job, AI], Armor Skill, Attack Style. Options: \"Keep\", \"Hide\", \"Disable\".\n" +
				"第1行：性别、年龄、[种族、职业、AI]、护甲技能、攻击方式。选项：\"Keep\"（保持）、\"Hide\"（隐藏）、\"Disable\"（禁用）。\n" +
				"行1：性別、年齢、[種族、職業、AI]、防具スキル、攻撃スタイル。オプション：\"Keep\"（維持）、\"Hide\"（非表示）、\"Disable\"（無効）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLine1 = config.Bind<String>(configDefinition, "Keep", configDescription);

		}
		public static void CharaDisplayLine1PCFactionOnlyConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line1 PCFactionOnly");
			ConfigDescription configDescription = new ConfigDescription(
				"Line 1: Sex, Age, [Race, Job, AI], Armor Skill, Attack Style. Show only for PC faction members when enabled. Options: \"true\", \"false\".\n" +
				"第1行：性别、年龄、[种族、职业、AI]、护甲技能、攻击方式。启用后仅对玩家阵营成员显示。选项：\"true\"（是）、\"false\"（否）。\n" +
				"行1：性別、年齢、[種族、職業、AI]、防具スキル、攻撃スタイル。有効時はPCの派閥メンバーのみに表示。オプション：\"true\"（はい）、\"false\"（いいえ）。",
				null, Array.Empty<object>());
			CharaDisplayLine1PCFactionOnly = config.Bind<bool>(configDefinition, false, configDescription);
		}
		public static void CharaDisplayLine1SizeConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line1 Size");
			ConfigDescription configDescription = new ConfigDescription(
				"Line 1: Sex, Age, [Race, Job, AI], Armor Skill, Attack Style. Font size. Default: 18.\n" +
				"第1行：性别、年龄、[种族、职业、AI]、护甲技能、攻击方式。字体大小。默认值：18。\n" +
				"行1：性別、年齢、[種族、職業、AI]、防具スキル、攻撃スタイル。フォントサイズ。デフォルト：18。",
				null, Array.Empty<object>());
			CharaDisplayLine1Size = config.Bind<int>(configDefinition, 18, configDescription);
		}

		//line2
		public static void CharaDisplayLine2Config(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line2");
			var acceptableValues = new AcceptableValueList<string>("Keep", "Hide", "Disable");
			ConfigDescription configDescription = new ConfigDescription(
				"Line 2: HP, DV, PV, Speed. Options: \"Keep\", \"Hide\", \"Disable\".\n" +
				"第2行：生命值、回避值、防御值、速度。选项：\"Keep\"（保持）、\"Hide\"（隐藏）、\"Disable\"（禁用）。\n" +
				"行2：HP、DV、PV、速度。オプション：\"Keep\"（維持）、\"Hide\"（非表示）、\"Disable\"（無効）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLine2 = config.Bind<String>(configDefinition, "Keep", configDescription);
		}
		public static void CharaDisplayLine2PCFactionOnlyConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line2 PCFactionOnly");
			var acceptableValues = new AcceptableValueList<bool>(true, false);
			ConfigDescription configDescription = new ConfigDescription(
				"Line 2: HP, DV, PV, Speed. Show only for PC faction members when enabled. Options: \"true\", \"false\".\n" +
				"第2行：生命值、回避值、防御值、速度。启用后仅对玩家阵营成员显示。选项：\"true\"（是）、\"false\"（否）。\n" +
				"行2：HP、DV、PV、速度。有効時はPCの派閥メンバーのみに表示。オプション：\"true\"（はい）、\"false\"（いいえ）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLine2PCFactionOnly = config.Bind<bool>(configDefinition, false, configDescription);
		}
		public static void CharaDisplayLine2SizeConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line2 Size");
			ConfigDescription configDescription = new ConfigDescription(
				"Line 2: HP, DV, PV, Speed. Font size. Default: 18.\n" +
				"第2行：生命值、回避值、防御值、速度。字体大小。默认值：18。\n" +
				"行2：HP、DV、PV、速度。フォントサイズ。デフォルト：18。",
				null, Array.Empty<object>());
			CharaDisplayLine2Size = config.Bind<int>(configDefinition, 18, configDescription);
		}

		//line3
		public static void CharaDisplayLine3Config(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line3");
			var acceptableValues = new AcceptableValueList<string>("Keep", "Hide", "Disable");
			ConfigDescription configDescription = new ConfigDescription(
				"Line 3: SP, Hunger, Hobby(s). Options: \"Keep\", \"Hide\", \"Disable\".\n" +
				"第3行：精力值、饥饿值、爱好。选项：\"Keep\"（保持）、\"Hide\"（隐藏）、\"Disable\"（禁用）。\n" +
				"行3：SP、空腹度、趣味（複数可）。オプション：\"Keep\"（維持）、\"Hide\"（非表示）、\"Disable\"（無効）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLine3 = config.Bind<String>(configDefinition, "Hide", configDescription);
		}
		public static void CharaDisplayLine3PCFactionOnlyConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line3 PCFactionOnly");
			var acceptableValues = new AcceptableValueList<bool>(true, false);
			ConfigDescription configDescription = new ConfigDescription(
				"Line 3: SP, Hunger, Hobby(s). Show only for PC faction members when enabled. Options: \"true\", \"false\".\n" +
				"第3行：精力值、饥饿值、爱好。启用后仅对玩家阵营成员显示。选项：\"true\"（是）、\"false\"（否）。\n" +
				"行3：SP、空腹度、趣味（複数可）。有効時はPCの派閥メンバーのみに表示。オプション：\"true\"（はい）、\"false\"（いいえ）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLine3PCFactionOnly = config.Bind<bool>(configDefinition, false, configDescription);
		}
		public static void CharaDisplayLine3SizeConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line3 Size");
			ConfigDescription configDescription = new ConfigDescription(
				"Line 3: SP, Hunger, Hobby(s). Font size. Default: 18.\n" +
				"第3行：精力值、饥饿值、爱好。字体大小。默认值：18。\n" +
				"行3：SP、空腹度、趣味（複数可）。フォントサイズ。デフォルト：18。",
				null, Array.Empty<object>());
			CharaDisplayLine3Size = config.Bind<int>(configDefinition, 18, configDescription);
		}

		//line4
		public static void CharaDisplayLine4Config(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line4");
			var acceptableValues = new AcceptableValueList<string>("Keep", "Hide", "Disable");
			ConfigDescription configDescription = new ConfigDescription(
				"Line 4: MP, Weight, EXP. Options: \"Keep\", \"Hide\", \"Disable\".\n" +
				"第4行：魔法值、负重、经验值。选项：\"Keep\"（保持）、\"Hide\"（隐藏）、\"Disable\"（禁用）。\n" +
				"行4：MP、重量、経験値。オプション：\"Keep\"（維持）、\"Hide\"（非表示）、\"Disable\"（無効）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLine4 = config.Bind<String>(configDefinition, "Hide", configDescription);
		}
		public static void CharaDisplayLine4PCFactionOnlyConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line4 PCFactionOnly");
			var acceptableValues = new AcceptableValueList<bool>(true, false);
			ConfigDescription configDescription = new ConfigDescription(
				"Line 4: MP, Weight, EXP. Show only for PC faction members when enabled. Options: \"true\", \"false\".\n" +
				"第4行：魔法值、负重、经验值。启用后仅对玩家阵营成员显示。选项：\"true\"（是）、\"false\"（否）。\n" +
				"行4：MP、重量、経験値。有効時はPCの派閥メンバーのみに表示。オプション：\"true\"（はい）、\"false\"（いいえ）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLine4PCFactionOnly = config.Bind<bool>(configDefinition, false, configDescription);
		}
		public static void CharaDisplayLine4SizeConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line4 Size");
			ConfigDescription configDescription = new ConfigDescription(
				"Line 4: MP, Weight, EXP. Font size. Default: 18.\n" +
				"第4行：魔法值、负重、经验值。字体大小。默认值：18。\n" +
				"行4：MP、重量、経験値。フォントサイズ。デフォルト：18。",
				null, Array.Empty<object>());
			CharaDisplayLine4Size = config.Bind<int>(configDefinition, 18, configDescription);
		}

		//LineResist
		public static void CharaDisplayLineResistConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Resist");
			var acceptableValues = new AcceptableValueList<string>("Keep", "Hide", "Disable");
			ConfigDescription configDescription = new ConfigDescription(
				"Resist Line: Resistance(s). Options: \"Keep\", \"Hide\", \"Disable\".\n" +
				"抗性行：各项抗性。选项：\"Keep\"（保持）、\"Hide\"（隐藏）、\"Disable\"（禁用）。\n" +
				"耐性行：耐性値（複数可）。オプション：\"Keep\"（維持）、\"Hide\"（非表示）、\"Disable\"（無効）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLineResist = config.Bind<String>(configDefinition, "Hide", configDescription);
		}
		public static void CharaDisplayLineResistPCFactionOnlyConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Resist PCFactionOnly");
			var acceptableValues = new AcceptableValueList<bool>(true, false);
			ConfigDescription configDescription = new ConfigDescription(
				"Resist Line: Resistance(s). Show only for PC faction members when enabled. Options: \"true\", \"false\".\n" +
				"抗性行：各项抗性。启用后仅对玩家阵营成员显示。选项：\"true\"（是）、\"false\"（否）。\n" +
				"耐性行：耐性値（複数可）。有効時はPCの派閥メンバーのみに表示。オプション：\"true\"（はい）、\"false\"（いいえ）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLineResistPCFactionOnly = config.Bind<bool>(configDefinition, false, configDescription);
		}
		public static void CharaDisplayLineResistSizeConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Resist Size");
			ConfigDescription configDescription = new ConfigDescription(
				"Resist Line: Resistance(s). Font size. Default: 14.\n" +
				"抗性行：各项抗性。字体大小。默认值：14。\n" +
				"耐性行：耐性値（複数可）。フォントサイズ。デフォルト：14。",
				null, Array.Empty<object>());
			CharaDisplayLineResistSize = config.Bind<int>(configDefinition, 14, configDescription);
		}

		//LineAttributes
		public static void CharaDisplayLineAttributesConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Attributes");
			var acceptableValues = new AcceptableValueList<string>("Keep", "Hide", "Disable");
			ConfigDescription configDescription = new ConfigDescription(
				"Attributes Line: Attributes. Options: \"Keep\", \"Hide\", \"Disable\".\n" +
				"属性行：各项属性。选项：\"Keep\"（保持）、\"Hide\"（隐藏）、\"Disable\"（禁用）。\n" +
				"属性行：各属性値。オプション：\"Keep\"（維持）、\"Hide\"（非表示）、\"Disable\"（無効）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLineAttributes = config.Bind<String>(configDefinition, "Hide", configDescription);
		}
		public static void CharaDisplayLineAttributesPCFactionOnlyConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Attributes PCFactionOnly");
			var acceptableValues = new AcceptableValueList<bool>(true, false);
			ConfigDescription configDescription = new ConfigDescription(
				"Attributes Line: Attributes. Show only for PC faction members when enabled. Options: \"true\", \"false\".\n" +
				"属性行：各项属性。启用后仅对玩家阵营成员显示。选项：\"true\"（是）、\"false\"（否）。\n" +
				"属性行：各属性値。有効時はPCの派閥メンバーのみに表示。オプション：\"true\"（はい）、\"false\"（いいえ）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLineAttributesPCFactionOnly = config.Bind<bool>(configDefinition, false, configDescription);
		}
		public static void CharaDisplayLineAttributesSizeConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Attributes Size");
			ConfigDescription configDescription = new ConfigDescription(
				"Attributes Line: Attributes. Font size. Default: 14.\n" +
				"属性行：各项属性。字体大小。默认值：14。\n" +
				"属性行：各属性値。フォントサイズ。デフォルト：14。",
				null, Array.Empty<object>());
			CharaDisplayLineAttributesSize = config.Bind<int>(configDefinition, 14, configDescription);
		}

		//LineFavgift
		public static void CharaDisplayLineFavgiftConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Favgift");
			var acceptableValues = new AcceptableValueList<string>("Keep", "Hide", "Disable");
			ConfigDescription configDescription = new ConfigDescription(
				"Favorite Gift Line: Favorite Gift(s). Even if disabled, known NPC preferences will still be shown as per vanilla behavior. Options: \"Keep\", \"Hide\", \"Disable\".\n" +
				"礼物偏好行：喜爱礼物。即使禁用，已知的NPC偏好仍将按原版逻辑显示。选项：\"Keep\"（保持）、\"Hide\"（隐藏）、\"Disable\"（禁用）。\n" +
				"好物行：好物（複数可）。無効にしても、既知のNPCの好みは元のゲームの動作に従い表示されます。オプション：\"Keep\"（維持）、\"Hide\"（非表示）、\"Disable\"（無効）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLineFavgift = config.Bind<String>(configDefinition, "Hide", configDescription);
		}
		public static void CharaDisplayLineFavgiftPCFactionOnlyConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Favgift PCFactionOnly");
			var acceptableValues = new AcceptableValueList<bool>(true, false);
			ConfigDescription configDescription = new ConfigDescription(
				"Favorite Gift Line: Favorite Gift(s). Even if disabled, known NPC preferences will still be shown as per vanilla behavior. Show only for PC faction members when enabled. Options: \"true\", \"false\".\n" +
				"礼物偏好行：喜爱礼物。即使禁用，已知的NPC偏好仍将按原版逻辑显示。启用后仅对玩家阵营成员显示。选项：\"true\"（是）、\"false\"（否）。\n" +
				"好物行：好物（複数可）。無効にしても、既知のNPCの好みは元のゲームの動作に従い表示されます。有効時はPCの派閥メンバーのみに表示。オプション：\"true\"（はい）、\"false\"（いいえ）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLineFavgiftPCFactionOnly = config.Bind<bool>(configDefinition, false, configDescription);
		}
		public static void CharaDisplayLineFavgiftSizeConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Favgift Size");
			ConfigDescription configDescription = new ConfigDescription(
				"Favorite Gift Line: Favorite Gift(s). Font size. Default: 14.\n" +
				"礼物偏好行：喜爱礼物。字体大小。默认值：14。\n" +
				"好物行：好物（複数可）。フォントサイズ。デフォルト：14。",
				null, Array.Empty<object>());
			CharaDisplayLineFavgiftSize = config.Bind<int>(configDefinition, 14, configDescription);
		}

		//LineAct
		public static void CharaDisplayLineActConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Act");
			var acceptableValues = new AcceptableValueList<string>("Keep", "Hide", "Disable");
			ConfigDescription configDescription = new ConfigDescription(
				"Act Line: Act(s). Options: \"Keep\", \"Hide\", \"Disable\".\n" +
				"行动行：各项行动。选项：\"Keep\"（保持）、\"Hide\"（隐藏）、\"Disable\"（禁用）。\n" +
				"行動行：行動（複数可）。オプション：\"Keep\"（維持）、\"Hide\"（非表示）、\"Disable\"（無効）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLineAct = config.Bind<String>(configDefinition, "Hide", configDescription);
		}
		public static void CharaDisplayLineActPCFactionOnlyConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Act PCFactionOnly");
			var acceptableValues = new AcceptableValueList<bool>(true, false);
			ConfigDescription configDescription = new ConfigDescription(
				"Act Line: Act(s). Show only for PC faction members when enabled. Options: \"true\", \"false\".\n" +
				"行动行：各项行动。启用后仅对玩家阵营成员显示。选项：\"true\"（是）、\"false\"（否）。\n" +
				"行動行：行動（複数可）。有効時はPCの派閥メンバーのみに表示。オプション：\"true\"（はい）、\"false\"（いいえ）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLineActPCFactionOnly = config.Bind<bool>(configDefinition, false, configDescription);
		}
		//public static void CharaDisplayLineActAliasParentConfig(ConfigFile config) {
		//	ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Act PCFactionOnly");
		//	var acceptableValues = new AcceptableValueList<bool>(true, false);
		//	ConfigDescription configDescription = new ConfigDescription("LineAct: Act(s)(Contain alias Parent Attribute). Options: \"true\", \"false\".", acceptableValues, Array.Empty<object>());
		//	CharaDisplayLineActPCFactionOnly = config.Bind<bool>(configDefinition, true, configDescription);
		//}
		public static void CharaDisplayLineActSizeConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Act Size");
			ConfigDescription configDescription = new ConfigDescription(
				"Act Line: Act(s). Font size. Default: 14.\n" +
				"行动行：各项行动。字体大小。默认值：14。\n" +
				"行動行：行動（複数可）。フォントサイズ。デフォルト：14。",
				null, Array.Empty<object>());
			CharaDisplayLineActSize = config.Bind<int>(configDefinition, 14, configDescription);
		}
		public static void CharaDisplayLineActItemsPerLineConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Act Items Per Line");
			ConfigDescription configDescription = new ConfigDescription(
				"Act Line: Act(s). Maximum acts shown per line before wrapping to the next line. Set to 0 to disable wrapping. Default: 0.\n" +
				"行动行：各项行动。每行显示的最大行动数量，超出后自动换行。设为0禁用换行。默认值：0。\n" +
				"行動行：行動（複数可）。折り返す前に1行に表示する最大行動数。0に設定すると折り返しを無効化。デフォルト：0。",
				null, Array.Empty<object>());
			CharaDisplayLineActItemsPerLine = config.Bind<int>(configDefinition, 0, configDescription);
		}

		//LineFeat
		public static void CharaDisplayLineFeatConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Feat");
			var acceptableValues = new AcceptableValueList<string>("Keep", "Hide", "Disable");
			ConfigDescription configDescription = new ConfigDescription(
				"Feat Line: Feat(s). Displayed separately from acts. Options: \"Keep\", \"Hide\", \"Disable\".\n" +
				"特技行：各项特技。与行动分开显示。选项：\"Keep\"（保持）、\"Hide\"（隐藏）、\"Disable\"（禁用）。\n" +
				"特技行：特技（複数可）。行動とは別に表示されます。オプション：\"Keep\"（維持）、\"Hide\"（非表示）、\"Disable\"（無効）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLineFeat = config.Bind<String>(configDefinition, "Hide", configDescription);
		}
		public static void CharaDisplayLineFeatPCFactionOnlyConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Feat PCFactionOnly");
			var acceptableValues = new AcceptableValueList<bool>(true, false);
			ConfigDescription configDescription = new ConfigDescription(
				"Feat Line: Feat(s). Show only for PC faction members when enabled. Options: \"true\", \"false\".\n" +
				"特技行：各项特技。启用后仅对玩家阵营成员显示。选项：\"true\"（是）、\"false\"（否）。\n" +
				"特技行：特技（複数可）。有効時はPCの派閥メンバーのみに表示。オプション：\"true\"（はい）、\"false\"（いいえ）。",
				acceptableValues, Array.Empty<object>());
			CharaDisplayLineFeatPCFactionOnly = config.Bind<bool>(configDefinition, false, configDescription);
		}
		public static void CharaDisplayLineFeatSizeConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Feat Size");
			ConfigDescription configDescription = new ConfigDescription(
				"Feat Line: Feat(s). Font size. Default: 14.\n" +
				"特技行：各项特技。字体大小。默认值：14。\n" +
				"特技行：特技（複数可）。フォントサイズ。デフォルト：14。",
				null, Array.Empty<object>());
			CharaDisplayLineFeatSize = config.Bind<int>(configDefinition, 14, configDescription);
		}
		public static void CharaDisplayLineFeatItemsPerLineConfig(ConfigFile config) {
			ConfigDefinition configDefinition = new ConfigDefinition("Extend Charater Display", "Display Line Feat Items Per Line");
			ConfigDescription configDescription = new ConfigDescription(
				"Feat Line: Feat(s). Maximum feats shown per line before wrapping to the next line. Set to 0 to disable wrapping. Default: 0.\n" +
				"特技行：各项特技。每行显示的最大特技数量，超出后自动换行。设为0禁用换行。默认值：0。\n" +
				"特技行：特技（複数可）。折り返す前に1行に表示する最大特技数。0に設定すると折り返しを無効化。デフォルト：0。",
				null, Array.Empty<object>());
			CharaDisplayLineFeatItemsPerLine = config.Bind<int>(configDefinition, 0, configDescription);
		}


		public static CharaConfigClass CharaDisplayLine1Settings = null;
		public static CharaConfigClass CharaDisplayLine2Settings = null;
		public static CharaConfigClass CharaDisplayLine3Settings = null;
		public static CharaConfigClass CharaDisplayLine4Settings = null;
		public static CharaConfigClass CharaDisplayLineResistSettings = null;
		public static CharaConfigClass CharaDisplayLineAttributesSettings = null;
		public static CharaConfigClass CharaDisplayLineFavgiftSettings = null;
		public static CharaConfigClass CharaDisplayLineActSettings = null;
		public static CharaConfigClass CharaDisplayLineFeatSettings = null;

	}

	public class ConfigInit {
		public static void InitializeConfiguration(ConfigFile config) {
			PluginSettings.CharaDisplayConfig(config);
			PluginSettings.InteractDisplayConfig(config);
			PluginSettings.NotificationUiDisplayConfig(config);
			PluginSettings.ThingDisplayConfig(config);
			PluginSettings.EnchantDisplayConfig(config);
			CharaSettings.CharaDisplayLine1Config(config);

			CharaSettings.CharaDisplayLine1PCFactionOnlyConfig(config);

			CharaSettings.CharaDisplayLine1SizeConfig(config);

			CharaSettings.CharaDisplayLine2Config(config);
			CharaSettings.CharaDisplayLine2PCFactionOnlyConfig(config);
			CharaSettings.CharaDisplayLine2SizeConfig(config);

			CharaSettings.CharaDisplayLine3Config(config);
			CharaSettings.CharaDisplayLine3PCFactionOnlyConfig(config);
			CharaSettings.CharaDisplayLine3SizeConfig(config);

			CharaSettings.CharaDisplayLine4Config(config);
			CharaSettings.CharaDisplayLine4PCFactionOnlyConfig(config);
			CharaSettings.CharaDisplayLine4SizeConfig(config);

			CharaSettings.CharaDisplayLineResistConfig(config);
			CharaSettings.CharaDisplayLineResistPCFactionOnlyConfig(config);
			CharaSettings.CharaDisplayLineResistSizeConfig(config);

			CharaSettings.CharaDisplayLineAttributesConfig(config);
			CharaSettings.CharaDisplayLineAttributesPCFactionOnlyConfig(config);
			CharaSettings.CharaDisplayLineAttributesSizeConfig(config);

			CharaSettings.CharaDisplayLineFavgiftConfig(config);
			CharaSettings.CharaDisplayLineFavgiftPCFactionOnlyConfig(config);
			CharaSettings.CharaDisplayLineFavgiftSizeConfig(config);

			CharaSettings.CharaDisplayLineActConfig(config);
			CharaSettings.CharaDisplayLineActPCFactionOnlyConfig(config);
			CharaSettings.CharaDisplayLineActSizeConfig(config);
			CharaSettings.CharaDisplayLineActItemsPerLineConfig(config);

			CharaSettings.CharaDisplayLineFeatConfig(config);
			CharaSettings.CharaDisplayLineFeatPCFactionOnlyConfig(config);
			CharaSettings.CharaDisplayLineFeatSizeConfig(config);
			CharaSettings.CharaDisplayLineFeatItemsPerLineConfig(config);


		}
	}


}
