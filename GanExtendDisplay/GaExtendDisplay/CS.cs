using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace GanExtendDisplay
{
	// Shim that shadows ClassExtension.lang() within our namespace.
	//
	// The game periodically changes the number of optional parameters on lang(),
	// which causes MissingMethodException in our compiled binary because C# bakes
	// the exact overload signature into the IL at compile time.
	//
	// By defining our own lang() extension in the same namespace, the C# compiler
	// always resolves to THIS method (local namespace wins over using-imported ones),
	// so our IL never references ClassExtension.lang() directly.  The actual call is
	// dispatched at runtime via a cached delegate that adapts to whatever overload
	// the current game DLL exposes.
	internal static class LangShim
	{
		private static readonly Func<string, string[], string> _fn = Build();

		private static Func<string, string[], string> Build()
		{
			try
			{
				var candidates = typeof(ClassExtension)
					.GetMethods(BindingFlags.Public | BindingFlags.Static)
					.Where(m => m.Name == "lang" && m.ReturnType == typeof(string))
					.ToList();

				// Prefer params-style: lang(string key, params string[] args)
				var paramsOverload = candidates.FirstOrDefault(m =>
				{
					var ps = m.GetParameters();
					return ps.Length == 2
						&& ps[0].ParameterType == typeof(string)
						&& ps[1].IsDefined(typeof(ParamArrayAttribute), false);
				});
				if (paramsOverload != null)
					return (key, args) =>
						(string)paramsOverload.Invoke(null, new object[] { key, args ?? Array.Empty<string>() }) ?? key;

				// Fall back to the overload with the most parameters (optional-param style)
				var best = candidates.OrderByDescending(m => m.GetParameters().Length).FirstOrDefault();
				if (best == null) return (key, _) => key;

				int n = best.GetParameters().Length;
				return (key, args) =>
				{
					var a = new object[n];
					a[0] = key;
					for (int i = 1; i < n; i++)
						a[i] = (args != null && i - 1 < args.Length) ? args[i - 1] : "";
					return (string)best.Invoke(null, a) ?? key;
				};
			}
			catch
			{
				return (key, _) => key;
			}
		}

		public static string lang(this string key, params string[] args) => _fn(key, args);
	}


	public static class CS
	{
		public static UnityEngine.Color Color_Crude = new UnityEngine.Color(0.44705883f, 0.21176471f, 0.21176471f);

		public static UnityEngine.Color Color_Normal = new UnityEngine.Color(0.4f, 0.4f, 0.4f);

		public static UnityEngine.Color Color_Superior = new UnityEngine.Color(0f, 0.6745098f, 1f);

		public static UnityEngine.Color Color_Legendary = new UnityEngine.Color(0.9137255f, 0.4745098f, 1f);

		public static UnityEngine.Color Color_Mythical = new UnityEngine.Color(1f, 0.89411765f, 0.3137255f);

		public static UnityEngine.Color Color_Artifact = new UnityEngine.Color(1f, 0.15686275f, 0.15686275f);
	}
	public static class resistCS
	{
		// 火焰抗性
		public static Color flameResistance = new Color(1f, 0.5f, 0.2f); // 橙色

		// 寒冷抗性
		public static Color coldResistance = new Color(0.2f, 0.5f, 1f); // 蓝色

		// 电击抗性
		public static Color electricResistance = new Color(1f, 1f, 0.2f); // 黄色

		// 黑暗抗性
		public static Color darkResistance = new Color(0.5f, 0.2f, 0.5f); // 黑色

		// 幻惑抗性
		public static Color mesmerizeResistance = new Color(0.5f, 0.2f, 1f); // 紫色

		// 毒抗性
		public static Color poisonResistance = new Color(0.2f, 1f, 0.2f); // 绿色

		// 地狱抗性
		public static Color hellResistance = new Color(1f, 0.2f, 0.2f); // 红色

		// 音抗性
		public static Color soundResistance = new Color(1f, 0.5f, 0.5f); // 粉色或浅橙色

		// 神经抗性
		public static Color nerveResistance = new Color(0.2f, 1f, 0.6f); // 靑绿色

		// 混沌抗性
		public static Color chaosResistance = new Color(0.6f, 0.3f, 0.2f); // 棕色

		// 神圣抗性
		public static Color divineResistance = new Color(1f, 1f, 0.5f); // 金色

		// 魔力抗性
		public static Color magicResistance = new Color(0.2f, 0.2f, 1f); // 蓝色

		// 以太抗性
		public static Color aetherResistance = new Color(0.2f, 0.2f, 1f); // 蓝色

		// 酸抗性
		public static Color acidResistance = new Color(0.2f, 1f, 0.2f); // 绿色

		// 出血抗性
		public static Color bleedingResistance = new Color(1f, 0.2f, 0.2f); // 红色

		// 冲击抗性
		public static Color impactResistance = new Color(0.2f, 0.2f, 1f); // 蓝色

		// 腐败抗性
		public static Color corruptionResistance = new Color(0.6f, 0.3f, 0.2f); // 棕色

		// 伤害抗性
		public static Color damageResistance = new Color(0.5f, 0.5f, 0.5f); // 灰色

		// 诅咒抗性
		public static Color curseResistance = new Color(0.2f, 0.2f, 0.2f); // 黑色



		public static string GetName(string res, int id) {
			//res = res.Substring(0, res.Length - 2);
			switch (id) {
				case 950:
					res = res.TagColor(flameResistance);
					break;
				case 951:
					res = res.TagColor(coldResistance);
					break;
				case 952:
					res = res.TagColor(electricResistance);
					break;
				case 953:
					res = res.TagColor(darkResistance);
					break;
				case 954:
					res = res.TagColor(mesmerizeResistance);
					break;
				case 955:
					res = res.TagColor(poisonResistance);
					break;
				case 956:
					res = res.TagColor(hellResistance);
					break;
				case 957:
					res = res.TagColor(soundResistance);
					break;
				case 958:
					res = res.TagColor(nerveResistance);
					break;
				case 959:
					res = res.TagColor(chaosResistance);
					break;
				case 960:
					res = res.TagColor(divineResistance);
					break;
				case 961:
					res = res.TagColor(magicResistance);
					break;
				case 962:
					res = res.TagColor(aetherResistance);
					break;
				case 963:
					res = res.TagColor(acidResistance);
					break;
				case 964:
					res = res.TagColor(bleedingResistance);
					break;
				case 965:
					res = res.TagColor(impactResistance);
					break;
				case 970:
					res = res.TagColor(corruptionResistance);
					break;
				case 971:
					res = res.TagColor(damageResistance);
					break;
				case 972:
					res = res.TagColor(curseResistance);
					break;
				default:
					// 当expression的值不匹配任何case时执行的代码块（如果存在）
					break;
			}
			return res;
		}
		public static string ShortOut(List<string> inList, int lineSize = 5) {
			if (inList.Count == 0) { return ""; }
			StringBuilder res = new StringBuilder();
			int count = 0;
			foreach (string s in inList) {
				count++;
				// 判断是否是每行的第一个元素，如果是则添加换行符
				if (count % lineSize == 1) {
					res.Append(Environment.NewLine).Append(s);
				} else {
					res.Append(" ").Append(s);
				}
			}
			return res.ToString();
		}
	}

}
