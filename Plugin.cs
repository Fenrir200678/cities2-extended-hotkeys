using BepInEx;
using HarmonyLib;
using System.Reflection;
using System.Linq;
using HookUILib.Core;

#if BEPINEX_V6
    using BepInEx.Unity.Mono;
#endif

namespace ExtendedHotkeys
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        
        private void Awake()
        {
            var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID + "_Cities2Harmony");
            var patchedMethods = harmony.GetPatchedMethods().ToArray();

            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded! Patched methods " + patchedMethods.Length);

            foreach (MethodBase patchedMethod in patchedMethods)
            {
                Logger.LogInfo($"Patched method: {patchedMethod.Module.Name}:{patchedMethod.Name}");
            }
        }
    }

    public sealed class ExtendedHotkeysUI : UIExtension
    {
        public ExtendedHotkeysUI()
        {
            extensionContent = LoadEmbeddedResource("ExtendedHotkeys.dist.extended-hotkeys-ui.transpiled.js");
        }

        public new readonly ExtensionType extensionType = ExtensionType.Panel;
        public new readonly string extensionID = "89pleasure.extendedHotkeys";
        public new readonly string extensionContent;
    }
}