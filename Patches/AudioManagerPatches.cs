using ExtendedHotkeys.Systems;
using Game;
using Game.Audio;
using HarmonyLib;

namespace ExtendedHotkeys.Patches
{
    [HarmonyPatch(typeof(AudioManager), "OnGameLoadingComplete")]
    class AudioManager_OnGameLoadingCompletePatch
    {
        static void Postfix(AudioManager __instance, Colossal.Serialization.Entities.Purpose purpose, GameMode mode)
        {
            if (!mode.IsGameOrEditor())
                return;

            __instance.World.GetOrCreateSystem<ExtendedHotkeysSystem>();
        }
    }
}
