// ModOptionsIntegration.cs
// Soft-dependency integration with EvilMask's Mod Options.
//
// Split into two classes to avoid TypeLoadException when ModOptions is absent:
//
//   ModOptionsIntegration — thin shell, no ModOptions type references.
//     TryRegister() checks whether the plugin is loaded, then calls
//     CallImpl() through a [NoInlining] bridge so the JIT does not try to
//     resolve ModOptionsImpl's types until AFTER the guard has passed.
//
//   ModOptionsImpl — all ModOptions types live here.
//     Only JIT-compiled when CallImpl() is actually invoked.
//
// Reference: https://github.com/EvilMask/Elin.ModOptions
// Required: Elin/Package/Mod_ModOptions/ModOptions.dll (compile-time reference only;
//           not copied to output — Private=False in .csproj).

using System;
using System.Runtime.CompilerServices;
using BepInEx;

namespace GanExtendDisplay
{
	// ---------------------------------------------------------------------------
	// Outer shell — zero ModOptions type references; safe to load without the mod
	// ---------------------------------------------------------------------------
	internal static class ModOptionsIntegration
	{
		private const string ModOptionsGuid = "evilmask.elinplugins.modoptions";

		/// <summary>
		/// Call from Main.Start() after all config entries are bound.
		/// Safe to call when Mod Options is not installed — exits immediately.
		/// </summary>
		internal static void TryRegister(BaseUnityPlugin plugin)
		{
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
				CallImpl(plugin);
			}
			catch (Exception ex)
			{
				Main.Logger.LogWarning("[GanExtendDisplay] Mod Options registration failed: " + ex);
			}
		}

		// NoInlining ensures the JIT does not compile (and resolve ModOptionsImpl's
		// types) until this method is actually called — i.e. after the guard above.
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void CallImpl(BaseUnityPlugin plugin) => ModOptionsImpl.Register(plugin);
	}
}
