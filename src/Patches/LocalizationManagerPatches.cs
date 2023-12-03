using Colossal.Localization;
using ExtendedHotkeys.Systems;
using HarmonyLib;
using Unity.Entities;

namespace ExtendedHotkeys.Patches
{
    [HarmonyPatch(typeof(LocalizationManager), "SetActiveLocale")]
    class LocalizationManager_Patches
    {
        static void Postfix(string localeId)
        {
            // Check if the system is already created by another mod
            if (World.DefaultGameObjectInjectionWorld == null)
            {
                return;
            }

            ExtendedHotKeysTranslationSystem customTranslationSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<ExtendedHotKeysTranslationSystem>();
            if (customTranslationSystem.CurrentLanguageCode != localeId)
            {
                customTranslationSystem.ReloadTranslations(localeId);
            }   
        }
    }
}
