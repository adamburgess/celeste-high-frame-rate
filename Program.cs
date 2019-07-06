using Harmony;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Reflection;

namespace CelesteHFR
{
    class Program
    {
        static void Main(string[] args)
        {
            // install patches
            var harmony = HarmonyInstance.Create("celeste.hfr");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // place a steam_appid.txt file in the current dir
            File.WriteAllText("steam_appid.txt", "504230");

            // run celeste
            typeof(Celeste.Celeste).GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { new string[] { } });
        }

        // Main patch: Ensure XNA's FixedTimeStep is false.

        [HarmonyPatch(typeof(Game))]
        [HarmonyPatch("set_IsFixedTimeStep")]
        [HarmonyPatch(new Type[] { typeof(bool) })]
        class Patch
        {
            static bool Prefix(ref bool value)
            {
                value = false;
                return true;
            }
        }

        // Secondary patch: Many things expect Celeste.exe to be the entry point, so make it the entry point.

        [HarmonyPatch(typeof(Assembly))]
        [HarmonyPatch("GetEntryAssembly")]
        [HarmonyPatch(new Type[] { })]
        class Patch2
        {
            static bool Prefix(ref Assembly __result)
            {
                __result = typeof(Celeste.Celeste).Assembly;
                return false;
            }
        }
    }
}
