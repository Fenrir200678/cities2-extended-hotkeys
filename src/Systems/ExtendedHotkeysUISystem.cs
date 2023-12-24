using Colossal.UI.Binding;
using ExtendedHotkeys.Settings;
using Game.UI;
using System;
using System.Collections.Generic;

namespace ExtendedHotkeys.Systems
{
    class ExtendedHotkeysUISystem : UISystemBase
    {
        private readonly string kGroup = "extendedHotkeys";
        private ExtendedHotkeysSystem m_ExtendedHotkeysSystem;
        private ExtendedHotKeysTranslationSystem m_CustomTranslationSystem;
        private LocalSettingsItem m_Settings;
        private Dictionary<string, string> m_SettingLocalization;

        private Dictionary<SettingKey, Action> toggleActions;
        private Dictionary<SettingKey, Action> expandActions;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_ExtendedHotkeysSystem = World.GetOrCreateSystemManaged<ExtendedHotkeysSystem>();
            m_CustomTranslationSystem = World.GetOrCreateSystemManaged<ExtendedHotKeysTranslationSystem>();
            m_Settings = m_ExtendedHotkeysSystem.m_LocalSettings.Settings;
            toggleActions = new Dictionary<SettingKey, Action>
            {
                // General hotkeys
                { SettingKey.DisableMod, () => m_Settings.DisableMod = !m_Settings.DisableMod },
                { SettingKey.EnableNetToolWheel, () => m_Settings.EnableNetToolWheel = !m_Settings.EnableNetToolWheel },
                { SettingKey.EnableNetToolReverse, () => m_Settings.EnableNetToolReverse = !m_Settings.EnableNetToolReverse },
                { SettingKey.EnableElevationWheel, () => m_Settings.EnableElevationWheel = !m_Settings.EnableElevationWheel },
                { SettingKey.EnableElevationReverse, () => m_Settings.EnableElevationReverse = !m_Settings.EnableElevationReverse },
                { SettingKey.EnableBrushStrengthWheel, () => m_Settings.EnableBrushStrengthWheel = !m_Settings.EnableBrushStrengthWheel },
                { SettingKey.EnableBrushSizeWheel, () => m_Settings.EnableBrushSizeWheel = !m_Settings.EnableBrushSizeWheel },
                { SettingKey.EnableBrushStrengthReverse, () => m_Settings.EnableBrushStrengthReverse = !m_Settings.EnableBrushStrengthReverse },
                { SettingKey.EnableBrushSizeReverse, () => m_Settings.EnableBrushSizeReverse = !m_Settings.EnableBrushSizeReverse },
                { SettingKey.EnableSnappingWheel, () => m_Settings.EnableSnappingWheel = !m_Settings.EnableSnappingWheel },
                { SettingKey.EnableElevationReset, () => m_Settings.EnableElevationReset = !m_Settings.EnableElevationReset },
                { SettingKey.EnableElevationStepScroll, () => m_Settings.EnableElevationStepScroll = !m_Settings.EnableElevationStepScroll },
                { SettingKey.EnableAnarchyMode, () => m_Settings.EnableAnarchyMode = !m_Settings.EnableAnarchyMode },
                { SettingKey.EnableNTMGroup, () => m_Settings.EnableNTMGroup = !m_Settings.EnableNTMGroup },
                { SettingKey.EnableNTMStraight, () => m_Settings.EnableNTMStraight = !m_Settings.EnableNTMStraight },
                { SettingKey.EnableNTMSimpleCurve, () => m_Settings.EnableNTMSimpleCurve = !m_Settings.EnableNTMSimpleCurve },
                { SettingKey.EnableNTMComplexCurve, () => m_Settings.EnableNTMComplexCurve = !m_Settings.EnableNTMComplexCurve },
                { SettingKey.EnableNTMContinuous, () => m_Settings.EnableNTMContinuous = !m_Settings.EnableNTMContinuous },
                { SettingKey.EnableNTMGrid, () => m_Settings.EnableNTMGrid = !m_Settings.EnableNTMGrid },

                // Open menus hotkeys
                { SettingKey.EnableOpenMenus, () => m_Settings.EnableOpenMenus = !m_Settings.EnableOpenMenus },
                { SettingKey.EnableNetToolHotkeys, () => m_Settings.EnableNetToolHotkeys = !m_Settings.EnableNetToolHotkeys },
                { SettingKey.EnableZonesHotkeys, () => m_Settings.EnableZonesHotkeys = !m_Settings.EnableZonesHotkeys },
                { SettingKey.EnableElectricityHotkeys, () => m_Settings.EnableElectricityHotkeys = !m_Settings.EnableElectricityHotkeys },
                { SettingKey.EnableWaterAndSewageHotkeys, () => m_Settings.EnableWaterAndSewageHotkeys = !m_Settings.EnableWaterAndSewageHotkeys },
                { SettingKey.EnableTransportationHotkeys, () => m_Settings.EnableTransportationHotkeys = !m_Settings.EnableTransportationHotkeys },
                { SettingKey.EnableHealthAndDeathcareHotkeys, () => m_Settings.EnableHealthAndDeathcareHotkeys = !m_Settings.EnableHealthAndDeathcareHotkeys },
                { SettingKey.EnableFireAndRescueHotkeys, () => m_Settings.EnableFireAndRescueHotkeys = !m_Settings.EnableFireAndRescueHotkeys },
                { SettingKey.EnableEducationAndResearchHotkeys, () => m_Settings.EnableEducationAndResearchHotkeys = !m_Settings.EnableEducationAndResearchHotkeys },
                { SettingKey.EnableGarbageManagementHotkeys, () => m_Settings.EnableGarbageManagementHotkeys = !m_Settings.EnableGarbageManagementHotkeys },
                { SettingKey.EnableParksAndRecreationHotkeys, () => m_Settings.EnableParksAndRecreationHotkeys = !m_Settings.EnableParksAndRecreationHotkeys },
                { SettingKey.EnableCommunicationsHotkeys, () => m_Settings.EnableCommunicationsHotkeys = !m_Settings.EnableCommunicationsHotkeys },
                { SettingKey.EnableLandscapingHotkeys, () => m_Settings.EnableLandscapingHotkeys = !m_Settings.EnableLandscapingHotkeys },
                { SettingKey.EnableRoadsHotkeys, () => m_Settings.EnableRoadsHotkeys = !m_Settings.EnableRoadsHotkeys },
                { SettingKey.EnableSignaturesHotkeys, () => m_Settings.EnableSignaturesHotkeys = !m_Settings.EnableSignaturesHotkeys },
                { SettingKey.EnableAreasHotkeys, () => m_Settings.EnableAreasHotkeys = !m_Settings.EnableAreasHotkeys },
                { SettingKey.EnablePoliceAndAdministrationHotkeys, () => m_Settings.EnablePoliceAndAdministrationHotkeys = !m_Settings.EnablePoliceAndAdministrationHotkeys },
            };

            // Expand actions
            expandActions = new Dictionary<SettingKey, Action>
            {
                { SettingKey.ExpandNTMGroup, () => m_Settings.ExpandNTMGroup = !m_Settings.ExpandNTMGroup },
                { SettingKey.ExpandReverseGroup, () => m_Settings.ExpandReverseGroup = !m_Settings.ExpandReverseGroup },
                { SettingKey.ExpandOpenMenus, () => m_Settings.ExpandOpenMenus = !m_Settings.ExpandOpenMenus }
            };

            m_SettingLocalization = new Dictionary<string, string>
            {
                // GENERAL
                { "disableMod", m_CustomTranslationSystem.GetTranslation("setting.disableMod", "Disable Mod") },
                { "disableMod.description", m_CustomTranslationSystem.GetTranslation("setting.disableMod.description", "Disable the mod globally.") },

                // WHEELS
                { "netToolModeWheel", m_CustomTranslationSystem.GetTranslation("setting.netToolModeWheel", "Tool Mode Wheel") },
                { "netToolModeWheel.description", m_CustomTranslationSystem.GetTranslation("setting.netToolModeWheel.description", "Scroll through NetTool modes.") },
                { "elevationWheel", m_CustomTranslationSystem.GetTranslation("setting.elevationWheel", "Elevation Wheel") },
                { "elevationWheel.description", m_CustomTranslationSystem.GetTranslation("setting.elevationWheel.description", "Increase/Decrease elevation.") },
                { "brushStrength", m_CustomTranslationSystem.GetTranslation("setting.brushStrength", "Brush Strength Wheel") },
                { "brushStrength.description", m_CustomTranslationSystem.GetTranslation("setting.brushStrength.description", "Increase/Decrease brush strength.") },
                { "brushSize", m_CustomTranslationSystem.GetTranslation("setting.brushSize", "Brush Size Wheel") },
                { "brushSize.description", m_CustomTranslationSystem.GetTranslation("setting.brushSize.description", "Increase/Decrease brush size.") },
                { "snappingWheel", m_CustomTranslationSystem.GetTranslation("setting.snappingWheel", "Snapping Wheel") },
                { "snappingWheel.description", m_CustomTranslationSystem.GetTranslation("setting.snappingWheel.description", "Increase/Decrease snapping.") },
                { "reverseScroll", m_CustomTranslationSystem.GetTranslation("setting.reverseScroll", "Reverse scroll direction.") },
                { "reverseScroll.description", m_CustomTranslationSystem.GetTranslation("setting.reverseScroll.description", "Reverse scroll direction.") },

                // HOTKEYS
                { "anarchyMode", m_CustomTranslationSystem.GetTranslation("setting.anarchyMode", "Anarchy Mode") },
                { "anarchyMode.description", m_CustomTranslationSystem.GetTranslation("setting.anarchyMode.description", "Toggle anarchy mode.") },

                { "elevationReset", m_CustomTranslationSystem.GetTranslation("setting.elevationReset", "Elevation Reset") },
                { "elevationReset.description", m_CustomTranslationSystem.GetTranslation("setting.elevationReset.description", "Resets elevation to ground floor.") },

                { "elevationStepScroll", m_CustomTranslationSystem.GetTranslation("setting.elevationStepScroll", "Elevation Step Scroll") },
                { "elevationStepScroll.description", m_CustomTranslationSystem.GetTranslation("", "Scroll elevation step level.") },

                { "netToolModes", m_CustomTranslationSystem.GetTranslation("Toolbar.TOOL_MODE_TITLE", "Tool Modes") },
                { "netToolModes.description", m_CustomTranslationSystem.GetTranslation("setting.netToolModes.description", "Hop through net tool modes.") },
                { "netToolModes.straight", m_CustomTranslationSystem.GetTranslation("Tools.TOOL_MODE[Straight]", "Straight") },
                { "netToolModes.curve", m_CustomTranslationSystem.GetTranslation("Tools.TOOL_MODE[SimpleCurve]", "Curve") },
                { "netToolModes.complexCurve", m_CustomTranslationSystem.GetTranslation("Tools.TOOL_MODE[ComplexCurve]", "Complex Curve") },
                { "netToolModes.continuous", m_CustomTranslationSystem.GetTranslation("Tools.TOOL_MODE[Continuous]", "Continuous") },
                { "netToolModes.grid", m_CustomTranslationSystem.GetTranslation("Tools.TOOL_MODE[Grid]", "Grid") },
                
                { "openMenus", m_CustomTranslationSystem.GetTranslation("settings.openMenus", "Open toolwindows") },
                { "openMenus.description", m_CustomTranslationSystem.GetTranslation("", "Open the game toolwindows") },
                { "menuZones", m_CustomTranslationSystem.GetTranslation("setting.zonesMenu", "Zones") },
                { "menuElectricity", m_CustomTranslationSystem.GetTranslation("setting.electricityMenu", "Electricity") },
                { "menuWaterAndSewage", m_CustomTranslationSystem.GetTranslation("setting.waterAndSewageMenu", "Water & Sewage") },
                { "menuTransportation", m_CustomTranslationSystem.GetTranslation("setting.transportationMenu", "Transportation") },
                { "menuHealthAndDeathcare", m_CustomTranslationSystem.GetTranslation("setting.healthAndDeathcareMenu", "Health & Deathcare") },
                { "menuFireAndRescue", m_CustomTranslationSystem.GetTranslation("setting.fireAndRescueMenu", "Fire & Rescue") },
                { "menuEducationAndResearch", m_CustomTranslationSystem.GetTranslation("setting.educationAndResearchMenu", "Education & Research") },
                { "menuGarbageManagement", m_CustomTranslationSystem.GetTranslation("setting.garbageManagementMenu", "Garbage Management") },
                { "menuParksAndRecreation", m_CustomTranslationSystem.GetTranslation("setting.parksAndRecreationMenu", "Parks & Recreation") },
                { "menuCommunications", m_CustomTranslationSystem.GetTranslation("setting.communicationsMenu", "Communications") },
                { "menuLandscaping", m_CustomTranslationSystem.GetTranslation("setting.landscapingMenu", "Landscaping") },
                { "menuRoads", m_CustomTranslationSystem.GetTranslation("setting.roadsMenu", "Roads") },
                { "menuSignatures", m_CustomTranslationSystem.GetTranslation("setting.signaturesMenu", "Signatures") },
                { "menuAreas", m_CustomTranslationSystem.GetTranslation("setting.areasMenu", "Areas") },
                { "menuPoliceAndAdministration", m_CustomTranslationSystem.GetTranslation("setting.policeAndAdministrationMenu", "Police & Administration") },
            };

            AddUpdateBinding(new GetterValueBinding<Dictionary<string, string>>(kGroup, "translations", () => m_SettingLocalization, new DictionaryWriter<string, string>().Nullable()));
            AddUpdateBinding(new GetterValueBinding<string>(kGroup, "version", () => MyPluginInfo.PLUGIN_VERSION));

            // GENERAL
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "disableMod", () => m_Settings.DisableMod));

            // Mouse Wheels
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNetToolWheel", () => m_Settings.EnableNetToolWheel));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNetToolReverse", () => m_Settings.EnableNetToolReverse));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableElevationWheel", () => m_Settings.EnableElevationWheel));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableElevationReverse", () => m_Settings.EnableElevationReverse));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableBrushStrengthWheel", () => m_Settings.EnableBrushStrengthWheel));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableBrushStrengthReverse", () => m_Settings.EnableBrushStrengthReverse));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableBrushSizeWheel", () => m_Settings.EnableBrushSizeWheel));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableBrushSizeReverse", () => m_Settings.EnableBrushSizeReverse));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "expandReverseGroup", () => m_Settings.ExpandReverseGroup));

            // General hotkeys
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableAnarchyMode", () => m_Settings.EnableAnarchyMode));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableElevationReset", () => m_Settings.EnableElevationReset));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "expandNTMGroup", () => m_Settings.ExpandNTMGroup));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNTMGroup", () => m_Settings.EnableNTMGroup));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNTMStraight", () => m_Settings.EnableNTMStraight));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNTMSimpleCurve", () => m_Settings.EnableNTMSimpleCurve));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNTMComplexCurve", () => m_Settings.EnableNTMComplexCurve));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNTMContinuous", () => m_Settings.EnableNTMContinuous));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNTMGrid", () => m_Settings.EnableNTMGrid));
            
            // Open menus hotkeys
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableOpenMenus", () => m_Settings.EnableOpenMenus));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "expandOpenMenus", () => m_Settings.ExpandOpenMenus));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableZonesHotkeys", () => m_Settings.EnableZonesHotkeys));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableElectricityHotkeys", () => m_Settings.EnableElectricityHotkeys));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableWaterAndSewageHotkeys", () => m_Settings.EnableWaterAndSewageHotkeys));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableTransportationHotkeys", () => m_Settings.EnableTransportationHotkeys));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableHealthAndDeathcareHotkeys", () => m_Settings.EnableHealthAndDeathcareHotkeys));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableFireAndRescueHotkeys", () => m_Settings.EnableFireAndRescueHotkeys));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableEducationAndResearchHotkeys", () => m_Settings.EnableEducationAndResearchHotkeys));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableGarbageManagementHotkeys", () => m_Settings.EnableGarbageManagementHotkeys));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableParksAndRecreationHotkeys", () => m_Settings.EnableParksAndRecreationHotkeys));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableCommunicationsHotkeys", () => m_Settings.EnableCommunicationsHotkeys));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableLandscapingHotkeys", () => m_Settings.EnableLandscapingHotkeys));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableRoadsHotkeys", () => m_Settings.EnableRoadsHotkeys));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableSignaturesHotkeys", () => m_Settings.EnableSignaturesHotkeys));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableAreasHotkeys", () => m_Settings.EnableAreasHotkeys));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enablePoliceAndAdministrationHotkeys", () => m_Settings.EnablePoliceAndAdministrationHotkeys));

            AddBinding(new TriggerBinding<int>(kGroup, "onToggle", OnToggle));
            AddBinding(new TriggerBinding<int>(kGroup, "onExpand", OnExpand));

            UnityEngine.Debug.Log("ExtendedHotkeysUISystem created.");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnityEngine.Debug.Log("ExtendedHotkeysUISystem destroyed.");
        }

        private async void OnToggle(int settingId)
        {
            if (toggleActions.TryGetValue((SettingKey)Enum.ToObject(typeof(SettingKey), settingId), out Action toggleAction))
            {
                toggleAction.Invoke();
                await m_ExtendedHotkeysSystem.m_LocalSettings.Save();
            }
            else
            {
                UnityEngine.Debug.Log($"Toggle Setting with Id {settingId} not found.");
            }
        }

        private async void OnExpand(int settingId)
        {
            if (expandActions.TryGetValue((SettingKey)Enum.ToObject(typeof(SettingKey), settingId), out Action expandAction))
            {
                expandAction.Invoke();
                await m_ExtendedHotkeysSystem.m_LocalSettings.Save();
            }
            else
            {
                UnityEngine.Debug.Log($"Expand setting with Id {settingId} not found.");
            }
        }
    }
}