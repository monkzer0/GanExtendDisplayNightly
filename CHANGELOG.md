# Changelog

Developer-focused record of all changes from the original [GanExtendDisplay](https://github.com/qaz13e/GanExtendDisplay) (base version last synced: v0.23.108.9).

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

**`CharaConfigClass.Size` is set at startup**

`Size` (font size) is baked into the `CharaConfigClass` instance during `Start()`. Unlike `ItemsPerLine`, a live change to a size setting via the config manager does not take effect until the next game restart. This is also pre-existing behavior.
