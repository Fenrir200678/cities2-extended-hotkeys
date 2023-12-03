using Game;
using Game.Common;
using HarmonyLib;
using ExtendedHotkeys.Systems;

namespace ExtendedHotkeys.Patches
{
    [HarmonyPatch(typeof(SystemOrder))]
    public static class SystemOrder_Patches
    {
        [HarmonyPatch(nameof(SystemOrder.Initialize))]
        [HarmonyPostfix]
        public static void Postfix(UpdateSystem updateSystem)
        {
            updateSystem?.UpdateAt<ExtendedHotKeysTranslationSystem>(SystemUpdatePhase.UIUpdate);
            updateSystem?.UpdateAt<ExtendedHotkeysUISystem>(SystemUpdatePhase.UIUpdate);
            updateSystem?.UpdateAt<ExtendedHotkeysSystem>(SystemUpdatePhase.ToolUpdate);
        }
    }
}
