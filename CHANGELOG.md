# Changelog

Developer-focused record of changes since forking from [GanExtendDisplay](https://github.com/qaz13e/GanExtendDisplay) (base synced: v0.23.108.9). Maintained continuously against Elin Nightly; stable-branch regressions are patched as reported.

---

## Recent bug fixes

### Stable: blank tooltips on items (Thing.GetHoverText)

`Thing_GetHoverText_Postfix` lacked a try-catch. When `GetPrice()` threw on stable — its 4-parameter signature differs from nightly — the exception propagated through Harmony and blanked the entire tooltip, including vanilla text.

- **`MainExtendDisplay.cs`** — wrapped in try-catch (same pattern already used for `GetHoverText2`); worst case on stable is our additions are skipped, vanilla tooltip is preserved.
- **`ThingDisplayClass.cs`** — isolated `GetPrice()` call specifically; price display is omitted silently rather than crashing the whole patch.

### lang() MissingMethodException after game patch

`ClassExtension.lang()` had its optional-parameter count changed in a nightly update. Because C# bakes exact overload signatures into IL at compile time, the compiled binary hard-referenced the old 7-parameter form, producing `MissingMethodException` on load.

**`CS.cs` — `LangShim` added**

A `lang()` extension method in our own namespace shadows `ClassExtension.lang()` at compile time (local namespace wins over using-imported ones). At runtime a cached delegate finds whatever overload the current game DLL exposes via reflection:

- Prefers `params string[]` style if present (2-parameter overload)
- Falls back to the overload with the most parameters (optional-param style), filling unused slots with `""`
- On complete failure, returns the raw key string — untranslated but no crash

This makes our compiled binary immune to further parameter-count changes on `lang()`.

### C# 7.3 interpolated-string-handler build error

Removed use of `$""` with `DefaultInterpolatedStringHandler` introduced in C# 10 / .NET 6; replaced with explicit `string.Format` or concatenation compatible with C# 7.3 / .NET Framework 4.x (BepInEx's target).

---

## Features

### Mod Options integration

Soft-dependency integration with EvilMask's [Mod Options](https://steamcommunity.com/workshop/filedetails/?id=3381182341) (`evilmask.elinplugins.modoptions`).

- Detected at runtime via `ModManager.ListPluginObject` (avoids BepInEx soft-dependency attribute pitfalls)
- **`ModOptionsIntegration.cs`** — `TryRegister(this)` called at end of `Start()`; exits immediately when Mod Options is absent
- UI layout defined in XML: two sections — *Affected Display* (five Keep/Hide/Disable dropdowns) and *Character Display Lines* (nine groups of mode + PCFactionOnly + font size, with IPL inputs for act and feat lines)
- Character line changes (mode, size, faction filter, IPL) are **live** — `RefreshCharaSettings()` re-instantiates all nine `CharaConfigClass` objects on value change
- Main feature toggles require restart (Harmony patches are applied once at startup)
- Free-text integer inputs for font size and IPL (replaced constrained dropdowns)
- All labels in English, Japanese, and Simplified Chinese

### EA 23.284 compatibility: GetHoverText2 restructured from prefix to postfix

The original patch used `[HarmonyPrefix]` returning `false`, which completely replaced the game's `GetHoverText2`. This caused two failure modes:

1. **Runtime exception in our body** — Harmony skips the original because the prefix already returned `false`, so Talk context menu population, favgift display, and conditions are also lost. This produced EA 23.284 symptoms: per-frame flicker, blank tooltip, missing right-click Talk.
2. **Future side-effect drift** — any new logic added to `GetHoverText2` by the game is silently suppressed.

The patch is now `[HarmonyPostfix]`. The original runs unconditionally first; our postfix appends. Exceptions are caught and logged (`[ExtendDisplay] GetHoverText2 threw: ...`) without affecting game functionality.

The conditions section was re-implemented with colour-coding by group and area-debuff fallback. To avoid showing conditions twice, the original's basic conditions lines are stripped from `originalResult` via regex before the enhanced version is appended.

### Feats on their own configurable line

The original single act loop is split into two independent blocks — one for acts, one for feats — each with its own Keep/Hide/Disable toggle, PCFactionOnly flag, font size, and Items Per Line setting. Feats are sourced from `chara.elements.ListElements(x => x.source.category == "feat" && x.Value > 0)` to catch feats stored in the elements system (e.g. Miko class tree) rather than the ability list.

### Companion mod documentation

- **Mod Help** ([Drakeny](https://steamcommunity.com/sharedfiles/filedetails/?id=3406542368)) — built-in help pages in EN/ZH/JP under `LangMod/*/Text/Help/help.txt`, readable in-game with no extra integration
- **Mod Config GUI** ([xTracr](https://steamcommunity.com/sharedfiles/filedetails/?id=3379819704)) — works automatically from the standard `.cfg` file; documented in README

---

## Compatibility fixes (ported from original)

### DNA.WriteNote / CanRemove signature change

`DNA.WriteNote` gained a required `Chara tg` parameter in a nightly update. Patch signature and call sites updated. `CanRemove()` updated to `CanRemove(tg)`.

### Gene value display — redundant output on special categories

`element.Value` was appended unconditionally in the gene element loop. A flag now restricts numeric output to the `default` branch of the category switch; `"slot"`, `"feat"`, and `"ability"` entries no longer show a redundant value.

### Area debuffs silently dropped

Conditions with `GetPhaseStr() == "#"` (area-effect debuffs with no localised phase string) were skipped. They now fall back to `item.source.GetName()`.

### Miko feats not displayed

The act loop only covered `Chara.ability.list.items`. A second pass over `chara.elements.ListElements` surfaces feats stored in the element system.

---

## Bug fixes (original mod)

### NotificationCondition / NotificationBuff: bare number in vampire status widget

Both patches unconditionally appended the numeric value even when `GetText()` returned empty, producing `" 10"` as a bare number in the player status widget. Fixed with the same guard already present in `NotificationStats_OnRefresh`:

```csharp
string condText = __instance.condition.GetText();
__instance.text = condText.IsEmpty() ? condText : condText + " " + __instance.condition.value.ToString();
```

### Patch-guard and config-entry copy-paste bugs

Three copy-paste bugs in the original port caused wrong config entries to control feature behaviour:

- `InteractDisplay` patches guarded by `ThingDisplay` config
- `EnchantDisplay` patches guarded by `NotificationUI` config
- `NotificationUiConfig` initialised from `CharaDisplay` config entry

---

## Known issues / future work

**Trailing comma after last act/feat entry**

`TrimEnd` calls at the end of both loops are inert — items are wrapped in `TagSize`/`TagColor` tags, so the string always ends with `</size>`, never a bare comma. The trailing comma remains visible. Pre-existing behaviour; left until a broader rewrite is warranted.

**`CharaConfigClass.Size` live-update scope**

`Size` is baked into the `CharaConfigClass` instance at construction. Changes via Mod Options trigger `RefreshCharaSettings()` and take effect immediately. Direct `.cfg` file edits still require a game restart.

**Price display omitted on stable**

`GetPrice(CurrencyType, bool, PriceType, null)` is silently skipped on stable due to signature mismatch. The ¤ price value does not appear in item hover text on the stable branch. No known workaround without a stable-specific build.
