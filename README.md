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

## In-game configuration (optional)

Three companion mods give you in-game access to settings and help. All are optional — the mod runs fine without any of them.

### Mod Options — by EvilMask

Install [Mod Options](https://steamcommunity.com/workshop/filedetails/?id=3381182341) and a tab labelled **Extend Display** appears in its panel. This mod has native integration with Mod Options — settings are grouped and labelled, with live-update support for character line settings.

- Changes to character display lines (mode, font size, faction filter, items per line) take effect **immediately** without restarting.
- Changes to the five main feature toggles (Character Display, Thing Display, etc.) take effect on the **next game restart**, because those control whether Harmony patches are applied at startup.

### Mod Config GUI — by xTracr

[Mod Config GUI](https://steamcommunity.com/sharedfiles/filedetails/?id=3379819704) is a general-purpose in-game config editor for BepInEx mods. This mod uses a standard `.cfg` file, so Mod Config GUI works with it automatically — no special integration needed.

### Mod Help — by Drakeny

[Mod Help](https://steamcommunity.com/sharedfiles/filedetails/?id=3406542368) is an in-game help viewer. This mod ships built-in help pages in English, Simplified Chinese, and Japanese — if Mod Help is installed, the full documentation is readable from within the game.

---

## Configuration

The config file is at `Elin\BepInEx\config\ExtendDisplay.cfg`. With [Mod Options](https://steamcommunity.com/workshop/filedetails/?id=3381182341) or [Mod Config GUI](https://steamcommunity.com/sharedfiles/filedetails/?id=3379819704) installed, you can also edit settings in-game.

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

Config descriptions are provided in **English, Simplified Chinese, and Japanese**.

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
- **Trilingual config descriptions** — every config entry ships with a description in English, Simplified Chinese, and Japanese
- **Native [Mod Options](https://steamcommunity.com/workshop/filedetails/?id=3381182341) panel** — a dedicated **Extend Display** tab with grouped settings, live updates for character line changes, and free-text number inputs for font size and items-per-line
- **Built-in [Mod Help](https://steamcommunity.com/sharedfiles/filedetails/?id=3406542368) pages** — full in-game documentation in English, Simplified Chinese, and Japanese, readable without leaving the game

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

*Primarily targets the Nightly build of Elin; tested on stable. Provided as-is — it may break when the game updates.*

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

## 游戏内配置（可选）

以下三款配套模组可让你在游戏内访问设置和帮助文档。全部为可选项——不安装任何一款，本模组均可正常运行。

### Mod Options — 作者：EvilMask

安装 [Mod Options](https://steamcommunity.com/workshop/filedetails/?id=3381182341) 后，其面板中将出现名为 **Extend Display** 的标签页。本模组已与 Mod Options 原生集成——设置已分组标注，角色显示行设置支持实时更新。

- 角色显示行的更改（显示模式、字体大小、阵营过滤、每行条目数）**立即生效**，无需重启游戏。
- 五项主要功能开关（角色显示、物品显示等）的更改在**下次启动游戏**后生效，因为这些开关控制 Harmony 补丁是否在启动时加载。

### Mod Config GUI — 作者：xTracr

[Mod Config GUI](https://steamcommunity.com/sharedfiles/filedetails/?id=3379819704) 是一款适用于 BepInEx 模组的通用游戏内配置编辑器。由于本模组使用标准 `.cfg` 文件，Mod Config GUI 无需任何特殊集成即可自动兼容。

### Mod Help — 作者：Drakeny

[Mod Help](https://steamcommunity.com/sharedfiles/filedetails/?id=3406542368) 是一款游戏内帮助查看器。本模组内置了英语、简体中文和日语三种语言的帮助页面——安装 Mod Help 后，即可在游戏内直接阅读完整文档。

---

## 配置

配置文件位于 `Elin\BepInEx\config\ExtendDisplay.cfg`。若已安装 [Mod Options](https://steamcommunity.com/workshop/filedetails/?id=3381182341) 或 [Mod Config GUI](https://steamcommunity.com/sharedfiles/filedetails/?id=3379819704)，也可在游戏内直接编辑。

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

配置说明以**英语、简体中文和日语**提供。

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
- **三语配置说明** — 每个配置项均附有英语、简体中文和日语说明
- **原生 [Mod Options](https://steamcommunity.com/workshop/filedetails/?id=3381182341) 面板** — 专属的 **Extend Display** 标签页，支持分组设置、角色行实时更新，以及字体大小与每行条目数的自由文本输入
- **内置 [Mod Help](https://steamcommunity.com/sharedfiles/filedetails/?id=3406542368) 页面** — 英语、简体中文和日语全文档，可在游戏内直接阅读

---

## 关于这个分支

本项目起初只是兼容性补丁，但已发展为包含新功能的分支版本。Gan@伪物 明确欢迎对原版模组进行二次创作与修改——这一邀请同样适用于本项目。

**欢迎贡献代码。** 请在[本仓库](https://github.com/monkzer0/GanExtendDisplayNightly/)提交 Pull Request 或 Issue。

---

*主要针对 Elin Nightly 版本，同时兼容稳定版。按原样提供——游戏更新时可能失效。*

---

由Claude翻訳。

# GanExtendDisplayNightly

Elin Nightly ビルド向けに維持されている、[GanExtendDisplay](https://steamcommunity.com/sharedfiles/filedetails/?id=3358329014)（原作者：[Gan@伪物](https://github.com/qaz13e/GanExtendDisplay)）のコミュニティフォークです。

---

## 元のMODと同時に使用できますか？

**いいえ。** 両方が同じゲームメソッドにパッチを当てます。同時に実行すると予測不能な結果が生じます — 後にロードされた方が前のものを上書きし、ツールチップの出力が乱れたり重複したりすることがあります。**このMODをインストールする前に、元のGanExtendDisplayの購読を解除してください。**

---

## 何ができるの？

マウスをキャラクター、地面のアイテム、またはインベントリの装備にホバーすると、通常は基本情報のみが表示されます。このMODはそれらのツールチップに設定可能な詳細情報を追加します — ステータス、耐性、バフ、特技、エンチャント値 — メニューを開かずに確認できます。

---

## 表示内容

### キャラクター
任意のキャラクターにホバーすると、設定に応じて以下を確認できます：

- **レベルと脅威表示** — 自分のレベルと比較した色分け表示；☠ は極めて危険なものを示す
- **レアリティバッジ** — 各レアリティ段階の ★ ☆ ◇ △ シンボル
- **好感度** — 相手があなたに対してどう感じているかを示す ♥ / ♡
- **行1:** 性別、年齢、種族、職業、AI行動、防具スキル、攻撃スタイル
- **行2:** HP、DV（回避）、PV（防御）、速度
- **行3:** SP（スタミナ）、空腹度、現在の仕事と趣味
- **行4:** MP、所持重量と上限、次のレベルまでの経験値
- **耐性:** すべての非ゼロ属性耐性
- **属性値:** 8つの基本ステータス（STR、CON、DEX、PER、LRN、WIL、MAG、CHR）と数値
- **好物:** このキャラクターが好むギフトのカテゴリと食べ物
- **状態:** アクティブなバフとデバフ、緑（バフ）または赤（デバフ/病気）でカラー表示、フェーズと数値付き — バニラUIに表示されないエリア効果デバフを含む
- **行動:** アクティブな戦闘スキル
- **特技:** パッシブな特性と才能、独自の行に特徴的な色で表示（行動とは別に、それぞれ独立して切り替え可能）

### 地面のアイテム
地面の任意のアイテムにホバーすると、そのレベル、レアリティ、素材を確認できます。

### インベントリと装備
装備にホバーすると、すべてのエンチャント項目の数値を確認できます。遺伝子タイプのアイテムにも対応しています。

### バフ/通知バー
ステータス効果を表示するUIウィジェットが、ホバー時に同じ追加詳細を表示します。

---

## 操作方法

| 入力 | 効果 |
|---|---|
| **Alt** を押しながら | `Hide` に設定した行を表示 |
| **Alt + Caps Lock** | 「常時非表示行を表示」をオン/オフ切り替え |
| **Alt + End** | MOD全体のオン/オフ切り替え |
| **Alt + Home** | ゲームのデバッグ表示を切り替え |

---

## ゲーム内設定（オプション）

3つの補助MODでゲーム内から設定やヘルプにアクセスできます。すべてオプションです — どれもインストールしなくても本MODは正常に動作します。

### Mod Options — EvilMask 作

[Mod Options](https://steamcommunity.com/workshop/filedetails/?id=3381182341) をインストールすると、そのパネルに **Extend Display** タブが表示されます。このMODはMod Optionsとネイティブに統合されています — 設定はグループ化・ラベル付けされており、キャラクター行の設定はリアルタイム更新に対応しています。

- キャラクター表示行への変更（モード、フォントサイズ、派閥フィルター、1行の件数）は、再起動なしで**即時反映**されます。
- 5つの主要機能スイッチ（キャラクター表示、アイテム表示など）への変更は、**次回ゲーム起動後**に反映されます（起動時のパッチ適用を制御するため）。

### Mod Config GUI — xTracr 作

[Mod Config GUI](https://steamcommunity.com/sharedfiles/filedetails/?id=3379819704) は、BepInEx MOD向けの汎用ゲーム内コンフィグエディタです。このMODは標準的な `.cfg` ファイルを使用しているため、Mod Config GUI は特別な統合なしに自動的に対応します。

### Mod Help — Drakeny 作

[Mod Help](https://steamcommunity.com/sharedfiles/filedetails/?id=3406542368) はゲーム内ヘルプビューアです。このMODには英語・簡体字中国語・日本語の内蔵ヘルプページが付属しています — Mod Help がインストールされていれば、ゲーム内から完全なドキュメントを閲覧できます。

---

## 設定

設定ファイルは `Elin\BepInEx\config\ExtendDisplay.cfg` にあります。[Mod Options](https://steamcommunity.com/workshop/filedetails/?id=3381182341) または [Mod Config GUI](https://steamcommunity.com/sharedfiles/filedetails/?id=3379819704) がインストールされていれば、ゲーム内でも設定を変更できます。

各表示セクションは3つの値を受け付けます：

| 値 | 意味 |
|---|---|
| `Keep` | 常に表示 |
| `Hide` | デフォルトは非表示；Alt を押しながらで表示可能 |
| `Disable` | 表示しない |

各キャラクターステータス行にはさらに：

| 設定 | 意味 |
|---|---|
| **PCFactionOnly** | `true` の場合、自分の派閥のキャラクターにのみ表示 |
| **Size** | その行のフォントサイズ |
| **Items Per Line** *（行動・特技のみ）* | N件ごとに改行；`0` は無制限 |

設定の説明は**英語・簡体字中国語・日本語**で提供されています。

---

## 元のMODとの違い

### Nightlyとの互換性修正

元のMODはElinの安定版向けに書かれています。Nightlyビルドは定期的に内部APIを変更します。このフォークはそれらの変更に対応してパッチを当てています：

- **DNA.WriteNote / CanRemove** — 遺伝子検査メソッドがNightlyの更新で必須の `Chara` パラメータを追加；パッチシグネチャとコールサイトを更新
- **遺伝子表示** — 数値は標準要素カテゴリのみ表示、スロット・特技・スキルには表示しない（冗長な出力を回避）
- **巫女の特技が表示されない** — 巫女クラスツリーの特技（スキルリストではなく要素システムに格納）が表示されていなかった；2回目の要素スキャンで修正
- **エリアデバフが表示されない** — フェーズテキストがないエリア効果デバフ（例：鈍足フィールド効果）が表示されなかった；ソース名にフォールバックするよう修正

### このフォークで追加された新機能

- **特技を独立した行に** — 特技が独立して切り替え可能になり、行動とは別に表示
- **設定可能な行折り返し** — 特技や行動が多い場合にツールチップが全画面幅に広がっていた；*Items Per Line* を正の数に設定することで、その件数ごとに折り返し（デフォルト `0` = 無制限、元の動作と同じ）
- **三言語設定の説明** — すべての設定項目に英語・簡体字中国語・日本語の説明を同梱
- **[Mod Options](https://steamcommunity.com/workshop/filedetails/?id=3381182341) ネイティブパネル** — **Extend Display** 専用タブ。設定をグループ化・ラベル付けし、キャラクター行設定のリアルタイム更新と、フォントサイズ・1行の件数の自由入力に対応
- **[Mod Help](https://steamcommunity.com/sharedfiles/filedetails/?id=3406542368) 内蔵ヘルプページ** — 英語・簡体字中国語・日本語の完全なドキュメントをゲーム内から直接閲覧可能

---

## これはフォークです

このプロジェクトは互換性パッチとして始まりましたが、新機能を含むフォークに成長しました。Gan@伪物は元のMODの二次開発や改変を明示的に歓迎しています — その招待はここにも適用されます。

**貢献を歓迎します。** [このリポジトリ](https://github.com/monkzer0/GanExtendDisplayNightly/)でプルリクエストまたはIssueを開いてください。

---

## クレジット

元のMODは **Gan@伪物** による作品です。
[元のソース](https://github.com/qaz13e/GanExtendDisplay) · [元のSteam Workshopページ](https://steamcommunity.com/sharedfiles/filedetails/?id=3358329014)

このフォークは元の作者の許可を得て公開・維持されています。

---

*主にElin Nightlyビルドを対象としています。安定版でも動作確認済み。現状のまま提供 — ゲームが更新された際に動作しなくなる可能性があります。*
