using BepInEx;
using HarmonyLib;

namespace StationeersBepInEx.FixModLoading
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class FixModLoadingPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            var harmony = new Harmony("org.bepinex.plugins.zach2039.stationeersbepinex.fixmodloading");
            harmony.PatchAll();
            Logger.LogInfo($"Patched WorldManager.LoadModDataFiles with Harmony.");
        }
    }
}
