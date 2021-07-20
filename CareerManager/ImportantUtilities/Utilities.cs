using System;
using System.Diagnostics;
using UnityEngine;

namespace ImportantUtilities
{
	public static class Utilities
	{
		public static void GetConfigValue(this ConfigNode node, out bool outval, string key)
		{
			bool flag = bool.TryParse(node.GetValue(key), out outval);
			bool flag2 = !flag;
			if (flag2)
			{
				outval = false;
			}
		}

		public static void GetConfigValue(this ConfigNode node, out int outval, string key)
		{
			bool flag = int.TryParse(node.GetValue(key), out outval);
			bool flag2 = !flag;
			if (flag2)
			{
				outval = 0;
			}
		}

		public static void GetConfigValue(this ConfigNode node, out float outval, string key)
		{
			bool flag = float.TryParse(node.GetValue(key), out outval);
			bool flag2 = !flag;
			if (flag2)
			{
				outval = 0f;
			}
		}

		public static void GetConfigValue(this ConfigNode node, out double outval, string key)
		{
			bool flag = double.TryParse(node.GetValue(key), out outval);
			bool flag2 = !flag;
			if (flag2)
			{
				outval = 0.0;
			}
		}

		public static void GetConfigValue(this ConfigNode node, out string outval, string key)
		{
			outval = node.GetValue(key);
		}

        public static void Log(string message)
        {
#if DEBUG
			UnityEngine.Debug.Log("CareerManager: "+ message);
#endif
        }

        public static void Log(string pluginName, int instanceID, string message)
		{
#if DEBUG
			UnityEngine.Debug.Log(string.Concat(new string[]
			{
				"[",
				pluginName,
				"[",
				instanceID.ToString("X"),
				"][",
				Time.time.ToString("0.0000"),
				"]: ",
				message
			}));
#endif
		}
	}
}
