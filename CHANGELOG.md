# Changelog

Developer-focused record of all changes from the original [GanExtendDisplay](https://github.com/qaz13e/GanExtendDisplay) (base version last synced: v0.23.108.9).

---

## [fork] — EA 23.284 compatibility: GetHoverText2 patch restructured

### Changed — `MainExtendDisplay.cs`

**`GetHoverText2` changed from a prefix-replace to a postfix**

The original patch used `[HarmonyPrefix]` returning `false`, which completely replaced the game's `GetHoverText2` method. This architecture has two failure modes:

1. **Runtime exception in our body** — Harmony skips the original because the prefix already signalled "don't run it", so the original's side effects (Talk context menu population, favgift display, conditions, whip-works text) are also lost. This produced the EA 23.284 symptoms: per-frame flicker, blank tooltip, and missing right-click Talk option.

2. **Future side-effect drift** — any new logic Noa adds to `GetHoverText2` is silently suppressed as long as our prefix returns `false`, even when our code is exception-free.

The patch is now `[HarmonyPostfix]` (`void`, no return value). The original runs unconditionally first; our postfix then appends. Any exception in our postfix is caught and logged to `BepInEx\LogOutput.log` (tag `[ExtendDisplay] GetHoverText2 threw: ...`) but does not affect game functionality.

### Changed — `CharaDisplayClass.cs`

**`Chara_GetHoverText2_Prefix` renamed to `Chara_GetHoverText2_Additions`; conditions section restored with replacement strategy**

With the postfix approach the original runs first and writes its own output. `Chara_GetHoverText2_Additions` then appends what it uniquely contributes:

- **Enhanced conditions line** — re-implemented with colour-coding by group (`Bad`/`Debuff`/`Disease` → `colorDebuff`; `Buff` → `colorBuff`; otherwise white), area-debuff name fallback (`GetPhaseStr() == "#"` → `item.source.GetName()`), and `resistCon` value display. To avoid showing conditions twice, the original's basic conditions lines are stripped from `originalResult` via a regex (`\n<size=\d+>[^<]*\(\d+\)[^<]*</size>`) before the enhanced version is appended. The conditions block is wrapped in a separate try-catch: if it throws, the original's basic output remains in place and acts/feats continue to render.
- **Acts line** — active combat abilities, with optional per-line item wrapping
- **Feats line** — passive traits/talents in a distinct colour, independently toggled

Favgift, debug text, and whip-works text are handled by the original and are not reimplemented here. Method signature updated to `(Chara __instance, ref string originalResult)` to allow the conditions block to mutate the accumulated result before appending.

---

## [fork] — Bugfix: bare number shown in vampire status widget

### Fixed — `MainExtendDisplay.cs`

**`NotificationCondition_OnRefresh` and `NotificationBuff_OnRefresh` always appended a numeric value**

Both patches unconditionally concatenated the condition's numeric value onto the label text:

```csharp
// before
__instance.text = __instance.condition.GetText() + (" " + __instance.condition.value.ToString());
```

When `GetText()` returns an empty string — as it does for certain vampire blood/thirst conditions at specific blood levels — the result was `"" + " 10"` = `" 10"`, rendering as a bare `"10"` in the player status widget.

`NotificationStats_OnRefresh` already applied the correct guard. The same pattern is now used in both affected patches:

```csharp
// after
string condText = __instance.condition.GetText();
__instance.text = condText.IsEmpty() ? condText : condText + " " + __instance.condition.value.ToString();
```

The value suffix is now only appended when `GetText()` produces a non-empty string.

---

## [fork] — Mod Options: free-text integer inputs for font size and IPL

### Changed — `ModOptionsIntegration.cs`

Font size and Items Per Line fields replaced from constrained dropdowns to free-text integer inputs (`<input>` XML element / `OptInput` C# class). Users can now type any font size ≥ 1 or any IPL value ≥ 0 directly, rather than being limited to 12 preset size values or 10 preset IPL values.

The size value translation keys (`ExtDisplay.Size.10` … `ExtDisplay.Size.24`) and IPL value translation keys (`ExtDisplay.IPL.0` … `ExtDisplay.IPL.10`) are removed — they were only needed as dropdown choice labels. The `ExtDisplay.IPL` label translation is updated to include "0 = no limit" inline.

A new `BindInputIntLive` helper replaces `BindDDSzLive` and `BindDDIplLive`. It binds an `OptInput` to a `ConfigEntry<int>`, populates the field with the current config value, restricts keyboard input to integers via `InputField.ContentType.IntegerNumber`, and validates `value >= min` before writing to the entry and calling `RefreshCharaSettings()`. The old size/IPL index-mapping functions (`ToSzIdx`, `FromSzIdx`, `SzValues`, `ToIplIdx`, `FromIplIdx`, `IplValues`) are removed.

---

## [fork] — Pre-existing patch-guard and config-entry bug fixes

Three copy-paste bugs in the original port; all three caused the wrong config entry to control a feature's behaviour.

### Fixed — `Mod_ExtendDisplay.cs`

**`InteractDisplay` patch guarded by the wrong config entry**

`Harmony.CreateAndPatchAll(typeof(InteractDisplay))` was wrapped in `if (PluginSettings.ThingDisplay.Value != "Disable")`. Should be `InteractDisplay`. Effect: disabling `ThingDisplay` silently killed `InteractDisplay`; setting `InteractDisplay = Disable` had no effect on whether the patches loaded.

**`EnchantDisplay` patch guarded by the wrong config entry**

`Harmony.CreateAndPatchAll(typeof(EnchantDisplay))` was wrapped in `if (PluginSettings.NotificationUI.Value != "Disable")`. Should be `EnchantDisplay`. Same class of bug.

### Fixed — `MainExtendDisplay.cs`

**`NotificationUI.NotificationUiConfig` initialised from the wrong config entry**

```csharp
// before
public static ConfigClass NotificationUiConfig = new ConfigClass(PluginSettings.CharaDisplay.Value);
// after
public static ConfigClass NotificationUiConfig = new ConfigClass(PluginSettings.NotificationUI.Value);
```

`NotificationUiConfig.OptionStatus` (and therefore its `CheckStatus` property) was derived from the `CharaDisplay` config value, not `NotificationUI`. Result: the Hide / Keep mode of all three notification patches followed `CharaDisplay` instead of their own setting.

---

## [fork] — Companion mod documentation

### Documented — Mod Help support

This mod ships built-in help pages (`LangMod/*/Text/Help/help.txt`) readable through [Mod Help](https://steamcommunity.com/sharedfiles/filedetails/?id=3406542368) by Drakeny. Help pages are available in English, Simplified Chinese, and Japanese. No code changes or special integration are required — Mod Help reads the help files automatically if it is installed.

### Documented — Mod Config GUI compatibility

[Mod Config GUI](https://steamcommunity.com/sharedfiles/filedetails/?id=3379819704) by xTracr works with this mod's standard BepInEx `.cfg` file without any special integration.

### Changed — Documentation

- **README.md** — Expanded "Mod Options (optional)" into "In-game configuration (optional)", covering all three companion mods: Mod Options (EvilMask), Mod Config GUI (xTracr), and Mod Help (Drakeny). Added full Japanese translation of the README.
- **package.xml** — Updated description to mention Mod Options, Mod Config GUI, and Mod Help.
- **Help files (EN/CN/JP)** — Updated all three language help files to document Mod Config GUI and Mod Help alongside the existing Mod Options section.

---

## [fork] — Mod Options integration

### Added — `ModOptionsIntegration.cs` (new file)

Soft-dependency integration with EvilMask's [Mod Options](https://steamcommunity.com/workshop/filedetails/?id=3381182341) (GUID `evilmask.elinplugins.modoptions`).

**Detection strategy:** Uses a runtime loop over `ModManager.ListPluginObject` rather than a `[BepInDependency(SoftDependency)]` attribute. This avoids a known BepInEx issue where a soft dependency that itself has an unmet hard dependency causes the depending plugin to fail, even though the dependency is declared optional.

**Registration:** `ModOptionsIntegration.TryRegister(this)` is called at the end of `Main.Start()`. It exits immediately when Mod Options is absent. When Mod Options is present it calls `ModOptionController.Register(guid, tooltipId)`, sets all translations, loads the XML layout, and subscribes to `OnBuildUI`.

**UI layout:** Defined in XML via `SetPreBuildWithXml()`. Two sections:
- *Affected Display* — five `<one_choice type="dropdown">` elements (Keep / Hide / Disable) for CharaDisplay, ThingDisplay, InteractDisplay, NotificationUI, EnchantDisplay.
- *Character Display Lines* — nine groups, each with a dropdown (mode), toggle (PCFactionOnly), and slider (font size). Act and Feat lines add a second slider for Items Per Line.

**Live updates for character lines:** Each `OnValueChanged` handler updates its `ConfigEntry<T>` then calls `RefreshCharaSettings()`, which re-instantiates all nine `CharaConfigClass` objects from the current `ConfigEntry` values. This gives immediate in-game effect for mode, PCFactionOnly, and font size changes without a game restart.

**Restart required for main feature toggles:** The five Affected Display dropdowns update their `ConfigEntry` values (which are persisted to the `.cfg` file) but Harmony patches are applied once at startup, so those changes take effect on the next launch.

**ItemsPerLine sliders** do not call `RefreshCharaSettings()` — those values are already read live directly from `ConfigEntry<int>.Value` at render time.

**Translations:** All labels registered in English, Japanese, and Simplified Chinese via `controller.SetTranslation(id, en, jp, cn)`.

### Changed — `Mod_ExtendDisplay.cs`

Added `ModOptionsIntegration.TryRegister(this)` call at the end of `Start()`.

### Changed — `GanExtendDisplay.csproj`

- Added `<Compile Include="ModOptionsIntegration.cs" />`.
- Added `<Reference Include="ModOptions">` pointing to `Elin\Package\Mod_ModOptions\ModOptions.dll` with `<Private>False</Private>` (compile-time reference only; not copied to the mod's output folder).

---

## [0.23.273.1] — Nightly Patch 2

**Target:** Elin 23.273 Nightly Patch 2

### Changed
- `package.xml`: version bump to `0.23.273.1` to reflect the new game target.

---

## [0.23.267.3] — Miko feats and area debuffs

### Fixed — `CharaDisplayClass.cs`

**Miko feats (and similar) not displayed**

The original act loop only iterated `Chara.ability.list.items` (type `ActList.Item`). Feats from the Miko class tree — and any other feats the game stores in `Chara.elements` rather than the ability list — were never reached. Added a second pass after the act loop:

```csharp
var feats = __instance.elements.ListElements(
    x => x.source.category == "feat" && x.Value > 0);
```

This surfaces all active feat-category elements regardless of how the game stores them internally.

**Area debuffs silently dropped**

Conditions with `GetPhaseStr() == "#"` represent area-effect debuffs that carry no localised phase string (e.g. slow-field effects). The original code checked for an empty string and skipped the entry entirely. Added a fallback:

```csharp
if (text4 == "#") {
    text4 = item3.source.GetName();
    if (text4.IsEmpty()) { continue; }
}
```

---

## [0.23.267.1] — Initial Nightly port

Base: original GanExtendDisplay v0.23.108.9

### Fixed — `EnchantDisplayClass.cs`

**`DNA.WriteNote` signature change**

Upstream Elin added a required `Chara tg` parameter to `DNA.WriteNote`. Updated `DNA_WriteNote_Prefix` to declare and forward this parameter to match the new game method signature.

**`DNA.CanRemove()` call site**

Changed `__instance.CanRemove()` to `__instance.CanRemove(tg)`. The old call no longer compiled after the API change.

**Gene value display — redundant output on special categories**

In the gene element loop, `element.Value` was appended unconditionally. Added a `bool` flag that is only set in the `default` branch of the category switch (i.e. when the element is not a `"slot"`, `"feat"`, or `"ability"`). Numeric values are now only rendered when that flag is true, preventing duplicate or semantically incorrect output on special-category gene entries.

### Fixed — `MainExtendDisplay.cs`

Updated the `DNA_WriteNote_Prefix` Harmony patch declaration to accept and forward the new `Chara tg` parameter, keeping the patch in sync with the game method.

### Changed — `package.xml`

Version: `0.23.108.9` → `0.23.267.1`

---

## [fork] — Post-port additions

These changes are not present in the original mod. They were added after the initial Nightly port.

### `PluginSettings.cs`

**Trilingual config descriptions**

All `ConfigDescription` strings now include English, Simplified Chinese, and Japanese on consecutive lines. The existing descriptions also had two copy-paste bugs that were fixed at the same time:

- `LineAttributes PCFactionOnly`: description incorrectly read `Default: "true", "false"` instead of `Options:`
- `LineAct Size`: description read `LineAct: Favgift(s)` instead of `Act(s)`

Note: `ConfigDefinition` section and key names were intentionally left unchanged (including the pre-existing `"Extend Charater Display"` section typo) to avoid resetting user config values on upgrade. BepInEx uses section+key to locate saved values; descriptions are only ever written as comments.

**New config group: `CharaDisplayLineFeat`**

Added four new entries under `"Extend Charater Display"`:

| Key | Type | Default | Purpose |
|---|---|---|---|
| `Display Line Feat` | `string` | `"Hide"` | Keep / Hide / Disable |
| `Display Line Feat PCFactionOnly` | `bool` | `false` | Restrict to PC faction |
| `Display Line Feat Size` | `int` | `14` | Font size |
| `Display Line Feat Items Per Line` | `int` | `0` | Wrap after N entries; 0 = no limit |

**New config entry: `CharaDisplayLineActItemsPerLine`**

Added `Display Line Act Items Per Line` (`int`, default `0`) to the existing act group, enabling the same wrapping behaviour for acts.

**New static field:** `CharaDisplayLineFeatSettings` (`CharaConfigClass`) instantiated in `Start()`.

### `CharaDisplayClass.cs`

**Feat/act block split**

The original single block guarded by `CharaDisplayLineActSettings` is now two independent blocks. Each checks its own settings instance and emits its own `Environment.NewLine`. The feat block only fires when `CharaDisplayLineFeatSettings.CharaDisplayLineOut` is true. This allows acts and feats to be toggled entirely independently.

**Items-per-line wrapping**

Both act and feat loops now read their respective `ItemsPerLine` config value (`CharaSettings.CharaDisplayLineActItemsPerLine.Value` / `CharaSettings.CharaDisplayLineFeatItemsPerLine.Value`) at render time. When the value is greater than zero, an additional `Environment.NewLine` is inserted before every Nth item (counter reset per block). Default `0` preserves the original single-line behavior.

The config values are read directly from `ConfigEntry<int>.Value` rather than being baked into `CharaConfigClass` at startup, so live changes via the config manager take effect without a restart.

**Trilingual block comment** on the feat section (EN / ZH / JP), consistent with the style of existing section comments in the file.

### `Mod_ExtendDisplay.cs`

Added instantiation of `CharaSettings.CharaDisplayLineFeatSettings` in `Start()`:

```csharp
CharaSettings.CharaDisplayLineFeatSettings = new CharaConfigClass(
    CharaDisplayLineFeat.Value,
    CharaDisplayLineFeatPCFactionOnly.Value,
    CharaDisplayLineFeatSize.Value);
```

---

## Known issues / future work

**`TrimEnd` calls are no-ops**

The `TrimEnd` calls at the end of both the act and feat loops (intended to strip a trailing comma) are functionally inert. Each item is individually wrapped in `TagSize`/`TagColor`, so the full string always ends with a closing tag (`</size>`), never a visible character. The trailing comma after the last item therefore remains visible in game. This is pre-existing behavior carried over from the original mod; left as-is until a broader rewrite is warranted.

**`CharaConfigClass.Size` live-update scope**

`Size` is baked into the `CharaConfigClass` instance at construction time. When changed through Mod Options, `BindInputIntLive` calls `RefreshCharaSettings()`, which re-constructs all nine `CharaConfigClass` instances — so size changes via Mod Options take effect immediately. Changes made by editing the `.cfg` file directly (without Mod Options) still require a game restart.
