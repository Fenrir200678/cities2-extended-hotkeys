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
            toggleActions = new()
            {
                { SettingKey.DisableMod, () => m_Settings.DisableMod = !m_Settings.DisableMod },
                { SettingKey.EnableNetToolWheel, () => m_Settings.EnableNetToolWheel = !m_Settings.EnableNetToolWheel },
                { SettingKey.EnableElevationWheel, () => m_Settings.EnableElevationWheel = !m_Settings.EnableElevationWheel },
                { SettingKey.EnableSnappingWheel, () => m_Settings.EnableSnappingWheel = !m_Settings.EnableSnappingWheel },
                { SettingKey.EnableElevationReset, () => m_Settings.EnableElevationReset = !m_Settings.EnableElevationReset },
                { SettingKey.EnableElevationStepScroll, () => m_Settings.EnableElevationStepScroll = !m_Settings.EnableElevationStepScroll },
                { SettingKey.EnableAnarchyMode, () => m_Settings.EnableAnarchyMode = !m_Settings.EnableAnarchyMode },
                { SettingKey.EnableNTMGroup, () => m_Settings.EnableNTMGroup = !m_Settings.EnableNTMGroup },
                { SettingKey.EnableNTMStraight, () => m_Settings.EnableNTMStraight = !m_Settings.EnableNTMStraight },
                { SettingKey.EnableNTMSimpleCurve, () => m_Settings.EnableNTMSimpleCurve = !m_Settings.EnableNTMSimpleCurve },
                { SettingKey.EnableNTMComplexCurve, () => m_Settings.EnableNTMComplexCurve = !m_Settings.EnableNTMComplexCurve },
                { SettingKey.EnableNTMContinuous, () => m_Settings.EnableNTMContinuous = !m_Settings.EnableNTMContinuous },
                { SettingKey.EnableNTMGrid, () => m_Settings.EnableNTMGrid = !m_Settings.EnableNTMGrid}
            };

            expandActions = new()
            {
                { SettingKey.ExpandNTMGroup, () => m_Settings.ExpandNTMGroup = !m_Settings.ExpandNTMGroup },
            };

            m_SettingLocalization = new()
            {
                // GENERAL
                { "disableMod", m_CustomTranslationSystem.GetTranslation("setting.disableMod", "Disable Mod") },
                { "disableMod.description", m_CustomTranslationSystem.GetTranslation("setting.disableMod.description", "Disable the mod globally.") },

                // WHEELS
                { "netToolModeWheel", m_CustomTranslationSystem.GetTranslation("setting.netToolModeWheel", "Tool Mode Wheel") },
                { "netToolModeWheel.description", m_CustomTranslationSystem.GetTranslation("setting.netToolModeWheel.description", "Scroll through NetTool modes.") },
                { "elevationWheel", m_CustomTranslationSystem.GetTranslation("setting.elevationWheel", "Elevation Wheel") },
                { "elevationWheel.description", m_CustomTranslationSystem.GetTranslation("setting.elevationWheel.description", "Increase/Decrease elevation.") },

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
            };

            AddUpdateBinding(new GetterValueBinding<Dictionary<string, string>>(kGroup, "translations", () => m_SettingLocalization, new DictionaryWriter<string, string>(null, null).Nullable(), null));
            AddUpdateBinding(new GetterValueBinding<string>(kGroup, "version", () => MyPluginInfo.PLUGIN_VERSION, null, null));

            /// GENERAL
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "disableMod", () => m_Settings.DisableMod, null, null));

            /// Mouse Wheels
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNetToolWheel", () => m_Settings.EnableNetToolWheel, null, null));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableElevationWheel", () => m_Settings.EnableElevationWheel, null, null));

            /// Hotkeys
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableAnarchyMode", () => m_Settings.EnableAnarchyMode, null, null));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableElevationReset", () => m_Settings.EnableElevationReset, null, null));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "expandNTMGroup", () => m_Settings.ExpandNTMGroup, null, null));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNTMGroup", () => m_Settings.EnableNTMGroup, null, null));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNTMStraight", () => m_Settings.EnableNTMStraight, null, null));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNTMSimpleCurve", () => m_Settings.EnableNTMSimpleCurve, null, null));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNTMComplexCurve", () => m_Settings.EnableNTMComplexCurve, null, null));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNTMContinuous", () => m_Settings.EnableNTMContinuous, null, null));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "enableNTMGrid", () => m_Settings.EnableNTMGrid, null, null));

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
            if (expandActions.TryGetValue((SettingKey) Enum.ToObject(typeof(SettingKey), settingId), out Action expandAction))
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
