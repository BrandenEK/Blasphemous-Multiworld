using BepInEx;
using HarmonyLib;
using BlasphemousRandomizer;

namespace BlasphemousMultiworld
{
    [BepInPlugin("com.damocles.blasphemous.multiworld", "Blasphemous Multiworld", PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.damocles.blasphemous.randomizer", "1.2.1")]
    [BepInProcess("Blasphemous.exe")]
    public class Main : BaseUnityPlugin
    {
        public static Multiworld Multiworld;
        public static Randomizer Randomizer;
        private static Main instance;

        private void Awake()
        {
            Randomizer = BlasphemousRandomizer.Main.Randomizer;
            Multiworld = new Multiworld();
            instance = this;
            Patch();
        }

        private void Update()
        {
            Multiworld.update();
        }

        private void Patch()
        {
            Harmony harmony = new Harmony("com.damocles.blasphemous.multiworld");
            harmony.PatchAll();
        }

        private void Log(string message)
        {
            Logger.LogMessage(message);
        }
        public static void UnityLog(string message)
        {
            instance.Log(message);
        }
    }
}
