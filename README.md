# GanExtendDisplayNightly

A community-maintained fork of [GanExtendDisplay](https://steamcommunity.com/sharedfiles/filedetails/?id=3358329014) by [Gan@伪物](https://github.com/qaz13e/GanExtendDisplay), kept working on Elin's Nightly build.

---

## Can I use this alongside the original mod?

**No.** Both mods patch the same game methods. Running them together will produce unpredictable results — whichever loads last will override the other, and the tooltip output may be garbled or duplicated. **Unsubscribe from the original GanExtendDisplay before installing this one.**

---

## What does this do?

When you hover your mouse over a character, an item on the ground, or equipment in your inventory, the game normally shows only basic information. This mod adds configurable extra detail to those tooltips — stats, resistances, buffs, feats, enchantment values — without you having to open any menus.

---

## What you'll see

### Characters
Hover over any character to see (depending on your config):

- **Level and threat indicator** — color-coded relative to your own level; a ☠ marks anything severely dangerous
- **Rarity badge** — ★ ☆ ◇ △ symbols for each rarity tier
- **Affinity** — a ♥ / ♡ showing how they feel about you
- **Line 1:** Sex, age, race, job, AI behavior, armor skill, attack style
- **Line 2:** HP, DV (dodge), PV (protection), Speed
- **Line 3:** SP (stamina), hunger, current works and hobbies
- **Line 4:** MP, carried weight vs. limit, EXP to next level
- **Resists:** All non-zero elemental resistances
- **Attributes:** The eight core stats (STR, CON, DEX, PER, LRN, WIL, MAG, CHR) with values
- **Favorite gift:** The category and food item this character prefers as a gift
- **Conditions:** Active buffs and debuffs, color-coded green (buff) or red (debuff/disease), with phase and numeric value — including area-effect debuffs that the vanilla UI doesn't show
- **Acts:** Active combat abilities
- **Feats:** Passive traits and talents, shown on their own line in a distinct color (separately from acts, so each can be toggled independently)

### Ground items
Hover over anything on the ground to see its level, rarity, and material.

### Inventory and equipment
Hover over gear to see the numeric value of every enchantment entry. Works on gene-type items too.

### Buff / notification bar
The UI widget that shows status effects displays the same extra detail on hover.

---

## Controls

| Input | Effect |
|---|---|
| Hold **Alt** | Reveal lines set to `Hide` |
| **Alt + Caps Lock** | Toggle "always show hidden lines" on/off |
| **Alt + End** | Toggle the entire mod on/off |
| **Alt + Home** | Toggle the game's own debug display |

---

## Configuration

The config file is at `Elin\BepInEx\config\ExtendDisplay.cfg`. If you have the BepInEx config manager installed, you can also edit settings in-game.

Each display section accepts three values:

| Value | Meaning |
|---|---|
| `Keep` | Always visible |
| `Hide` | Hidden by default; hold Alt to reveal |
| `Disable` | Never shown |

Each character stat line also has:

| Setting | Meaning |
|---|---|
| **PCFactionOnly** | When `true`, only shows for characters in your faction |
| **Size** | Font size for that line |
| **Items Per Line** *(acts and feats only)* | Wrap to a new line after this many entries; `0` means no limit |

Config descriptions appear in **English, Simplified Chinese, and Japanese** in the config manager.

---

## What's different from the original mod?

### Nightly compatibility fixes

The original mod was written for Elin's stable branch. The Nightly build periodically changes internal APIs. This fork patches those breaks as they appear:

- **DNA.WriteNote / CanRemove** — the gene inspection method gained a required `Chara` parameter in a Nightly update; patch signature and call sites updated to match
- **Gene display** — numeric values are now only shown for standard element categories, not for slots, feats, or abilities, avoiding redundant output
- **Miko feats not showing** — feats from the Miko class tree (and similar character-type feats stored in the elements system rather than the ability list) were silently absent; fixed with a second element scan
- **Area debuffs not showing** — debuffs from area effects that carry no phase text (e.g. slow-field effects) were silently dropped; they now fall back to the source name

### New features added in this fork

- **Feats on their own line** — feats are now independently toggled and displayed separately from acts, so you can show one without the other
- **Configurable line wrapping** — characters with many feats or acts used to stretch the tooltip to full screen width; set *Items Per Line* to a positive number to wrap after that many entries (default `0` = no limit, matching original behavior)
- **Trilingual config descriptions** — every config entry shows its description in English, Simplified Chinese, and Japanese in the config manager

---

## This is a fork

This project began as a compatibility patch but has grown to include new features. It is effectively a fork of GanExtendDisplay. Gan@伪物 has explicitly welcomed secondary development and modification of the original mod — that invitation extends here.

**Contributions are welcome.** Open a pull request or issue on [this repository](https://github.com/monkzer0/GanExtendDisplayNightly/).

---

## Credits

Original mod by **Gan@伪物**
[Original source](https://github.com/qaz13e/GanExtendDisplay) · [Original Steam Workshop page](https://steamcommunity.com/sharedfiles/filedetails/?id=3358329014)

This fork is uploaded and maintained with permission from the original author.

---

*Targets the Nightly build of Elin. Provided as-is — it may break when the game updates.*

---

由Claude翻译。

# GanExtendDisplayNightly

这是由社区维护的 [GanExtendDisplay](https://steamcommunity.com/sharedfiles/filedetails/?id=3358329014)（原作者：[Gan@伪物](https://github.com/qaz13e/GanExtendDisplay)）的分支版本，专为 Elin Nightly 版本保持兼容性。

---

## 能否与原版模组同时使用？

**不能。** 两个模组会对相同的游戏方法打补丁。同时运行会产生不可预测的结果——后加载的模组会覆盖另一个，提示框内容可能混乱或重复。**请在安装本模组前，先取消订阅原版 GanExtendDisplay。**

---

## 这个模组有什么用？

当你将鼠标悬停在角色、地面物品或背包中的装备上时，游戏通常只显示基本信息。本模组在这些提示框中添加了可配置的额外详情——属性、抗性、增益、特技、附魔数值——无需打开任何菜单即可查看。

---

## 你能看到什么

### 角色
将鼠标悬停在任意角色上，根据你的配置可以显示：

- **等级与威胁指示**——相对于你自身等级的颜色标记；☠ 表示极度危险
- **稀有度标识**——★ ☆ ◇ △ 对应各稀有度等级
- **亲密度**——♥ / ♡ 显示对方对你的态度
- **第1行：** 性别、年龄、种族、职业、AI行为、护甲技能、攻击方式
- **第2行：** 生命值、回避值（DV）、防御值（PV）、速度
- **第3行：** 精力值（SP）、饥饿度、当前工作与爱好
- **第4行：** 魔法值（MP）、负重与上限、距下一级所需经验
- **抗性行：** 所有非零属性抗性
- **属性行：** 八项核心属性（力量、体质、灵巧、感知、学习、意志、魔力、魅力）及其数值
- **喜爱礼物：** 该角色偏好的礼物类别与食物
- **状态行：** 当前增益与减益，绿色（增益）/红色（减益/疾病）颜色标记，显示状态阶段与数值——包括原版UI不显示的区域效果减益
- **行动行：** 主动战斗技能
- **特技行：** 被动特质与天赋，以独立颜色单独一行显示（与行动分开，可各自独立切换）

### 地面物品
悬停在地面物品上可查看其等级、稀有度与材质。

### 背包与装备
悬停在装备上可查看所有附魔词条的数值，包括基因类物品。

### 增益/通知栏
状态效果显示控件悬停时同样显示额外详情。

---

## 操作方式

| 输入 | 效果 |
|---|---|
| 按住 **Alt** | 显示设为`Hide`的行 |
| **Alt + Caps Lock** | 切换"始终显示隐藏行"模式 |
| **Alt + End** | 切换整个模组的开关 |
| **Alt + Home** | 切换游戏自带的调试显示 |

---

## 配置

配置文件位于 `Elin\BepInEx\config\ExtendDisplay.cfg`。若已安装 BepInEx 配置管理器，也可在游戏内直接编辑。

每个显示项支持三个值：

| 值 | 含义 |
|---|---|
| `Keep` | 始终显示 |
| `Hide` | 默认隐藏；按住 Alt 可显示 |
| `Disable` | 禁用，永不显示 |

每个角色属性行还有：

| 设置 | 含义 |
|---|---|
| **PCFactionOnly** | 为 `true` 时仅对玩家阵营成员显示 |
| **Size** | 该行字体大小 |
| **Items Per Line** *（仅行动/特技行）* | 每行显示条目数上限，超出后换行；`0` 表示不限制 |

配置描述在配置管理器中以**英语、简体中文和日语**同时显示。

---

## 与原版模组的区别

### Nightly 兼容性修复

原版模组基于 Elin 稳定版开发，而 Nightly 版本会定期变更内部 API。本分支会随之修复这些兼容性问题：

- **DNA.WriteNote / CanRemove** — 基因检视方法新增了必要的 `Chara` 参数；已更新补丁签名与调用处
- **基因显示** — 数值现在仅对标准元素类别显示，不再对槽位、特技或技能显示，避免冗余输出
- **巫女特技未显示** — 巫女职业树（及类似）的特技存储在元素系统中而非技能列表，原版无法显示；已通过额外的元素扫描修复
- **区域减益未显示** — 无相位文本的区域效果减益（如减速场地效果）被静默忽略；现在回退显示来源名称

### 本分支新增功能

- **特技独立成行** — 特技现在可独立切换，与行动分开显示
- **可配置换行** — 特技或行动过多时提示框会横向延伸至全屏；设置 *Items Per Line* 为正整数可在达到数量后换行（默认 `0` = 不限制）
- **三语配置说明** — 配置管理器中每个配置项均以英语、简体中文和日语显示说明

---

## 关于这个分支

本项目起初只是兼容性补丁，但已发展为包含新功能的分支版本。Gan@伪物 明确欢迎对原版模组进行二次创作与修改——这一邀请同样适用于本项目。

**欢迎贡献代码。** 请在[本仓库](https://github.com/monkzer0/GanExtendDisplayNightly/)提交 Pull Request 或 Issue。

---

*针对 Elin Nightly 版本，按原样提供——游戏更新时可能失效。*
