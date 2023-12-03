using ExtendedHotkeys.MouseWheels;
using ExtendedHotkeys.Settings;
using ExtendedHotkeys.Wheels;
using Game;
using Game.Audio;
using Game.Input;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Tools;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

namespace ExtendedHotkeys.Systems
{
    class ExtendedHotkeysSystem : GameSystemBase
    {
        private ToolSystem m_ToolSystem;
        private NetToolSystem m_NetToolSystem;
        private ExtendedHotKeysTranslationSystem m_CustomTranslationSystem;

        private ProxyActionMap m_CameraMap;
        private ProxyAction m_MouseZoomAction;
        private ToolUXSoundSettingsData m_SoundData;

        private readonly List<WheelBase> m_Wheels = [];

        public LocalSettings m_LocalSettings;
        public LocalSettingsItem m_Settings;

        public bool m_LocalSettingsLoaded = false;
        public bool hotkeyPressed = false;

        // Available KeyCodes a user can choose for hotkeys
        public List<KeyCode> availableKeyCodes = [
            KeyCode.LeftShift,
            KeyCode.LeftControl,
            KeyCode.LeftAlt,
            KeyCode.RightShift,
            KeyCode.RightControl,
            KeyCode.RightAlt,
            KeyCode.Space
        ];

        private readonly float[] m_ElevationSteps = [1.25f, 2.5f, 5f, 10f];

        protected override void OnCreate()
        {
            base.OnCreate();
            LoadSettings();

            m_ToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_NetToolSystem = World.GetExistingSystemManaged<NetToolSystem>();
            m_CustomTranslationSystem = World.GetOrCreateSystemManaged<ExtendedHotKeysTranslationSystem>();
            EntityQuery soundQuery = GetEntityQuery(ComponentType.ReadOnly<ToolUXSoundSettingsData>());

            // Add hotkeys
            CreateNetToolBindings();

            // Add mouse wheels
            m_Wheels.Add(new NetToolWheel(m_ToolSystem, m_NetToolSystem, soundQuery, m_Settings));
            m_Wheels.Add(new ElevationToolWheel(m_ToolSystem, m_NetToolSystem, soundQuery, m_Settings));

            UnityEngine.Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}] System created!");
        }

        [Preserve]
        protected override void OnUpdate()
        {
            if (m_LocalSettingsLoaded && m_ToolSystem != null && InputManager.instance.mouseOnScreen)
            {
                foreach (WheelBase wheel in m_Wheels)
                {
                    wheel.HandleAction();
                }
            }
        }

        private void CreateNetToolBindings()
        {
            string binding = "<Keyboard>/home";
            bool developerMode = GameManager.instance.configuration.developerMode;

            if (developerMode)
            {
                UnityEngine.Debug.Log("Developer mode is enabled. Set binding to END instead.");
                binding = "<Keyboard>/end";
            }

            AddBinding(name: "ResetElevationToZero", binding: binding, callback: OnResetElevation);
            AddCombindBinding(name: "SetStraight", modifier: "<keyboard>/ctrl", binding: "<Keyboard>/q", callback: (_) => OnSetNetToolSystemMode(_, NetToolSystem.Mode.Straight));
            AddCombindBinding(name: "SetCurveTool", modifier: "<keyboard>/ctrl", binding: "<Keyboard>/w", callback: (_) => OnSetNetToolSystemMode(_, NetToolSystem.Mode.SimpleCurve));
            AddCombindBinding(name: "SetAdvCurveTool", modifier: "<keyboard>/ctrl", binding: "<Keyboard>/e", callback: (_) => OnSetNetToolSystemMode(_, NetToolSystem.Mode.ComplexCurve));
            AddCombindBinding(name: "SetContinuousMode", modifier: "<keyboard>/ctrl", binding: "<Keyboard>/r", callback: (_) => OnSetNetToolSystemMode(_, NetToolSystem.Mode.Continuous));
            AddCombindBinding(name: "SetGridMode", modifier: "<keyboard>/ctrl", binding: "<Keyboard>/t", callback: (_) => OnSetNetToolSystemMode(_, NetToolSystem.Mode.Grid));
            
            // Add bindings for elevation tool ALT + Right Mouse
            AddCombindBinding(name: "UpdateElevationStep", modifier: "<keyboard>/leftAlt", binding: "<mouse>/rightButton", callback: (_) => OnElevationScroll(_));

            // Add binding for anarchy mode ALT + A
            AddCombindBinding(name: "EnableAnarchyMode", modifier: "<keyboard>/leftAlt", binding: "<keyboard>/a", callback: (_) => EnableAnarchyModeAction(_));
        }

        private void LoadSettings()
        {
            try
            {
                m_LocalSettings = new();
                m_LocalSettings.Init();
                m_LocalSettingsLoaded = true;
                m_Settings = m_LocalSettings.Settings;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log($"Error loading settings: {e.Message}");
            }
        }

        private void OnSetNetToolSystemMode(InputAction.CallbackContext _, NetToolSystem.Mode mode)
        {
            if (!m_Settings.EnableNTMGroup && m_ToolSystem.activeTool is not NetToolSystem)
                return;

            if (mode == NetToolSystem.Mode.Straight && !m_Settings.EnableNTMStraight)
                return;

            if (mode == NetToolSystem.Mode.SimpleCurve && !m_Settings.EnableNTMSimpleCurve)
                return;

            if (mode == NetToolSystem.Mode.ComplexCurve && !m_Settings.EnableNTMComplexCurve)
                return;

            if (mode == NetToolSystem.Mode.Continuous && !m_Settings.EnableNTMContinuous)
                return;

            if (mode == NetToolSystem.Mode.Grid && !m_Settings.EnableNTMGrid)
                return;

            m_NetToolSystem.mode = mode;
            cohtml.Net.View ui = GameManager.instance.userInterface.view.View;
            ui.TriggerEvent("tool.selectToolMode", (int)mode);
            
            AudioManager.instance.PlayUISound(m_SoundData.m_NetStartSound);
        }

        private void OnResetElevation(InputAction.CallbackContext _)
        {
            if (!m_Settings.EnableElevationReset && m_ToolSystem.activeTool is not NetToolSystem)
                return;

            World.GetOrCreateSystemManaged<NetToolSystem>().elevation = 0f;

            EntityQuery uxSoundQuery = GetEntityQuery(ComponentType.ReadOnly<ToolUXSoundSettingsData>());
            ToolUXSoundSettingsData soundSettingsData = uxSoundQuery.GetSingleton<ToolUXSoundSettingsData>();
            AudioManager.instance.PlayUISound(soundSettingsData.m_NetStartSound, 0.5f);
        }

        private void EnableAnarchyModeAction(InputAction.CallbackContext _)
        {
            if (!m_Settings.EnableAnarchyMode)
                return;

            m_ToolSystem.ignoreErrors = !m_ToolSystem.ignoreErrors;

            EntityQuery uxSoundQuery = GetEntityQuery(ComponentType.ReadOnly<ToolUXSoundSettingsData>());
            ToolUXSoundSettingsData soundSettingsData = uxSoundQuery.GetSingleton<ToolUXSoundSettingsData>();
            AudioManager.instance.PlayUISound(soundSettingsData.m_AreaMarqueeEndSound, 0.5f);
        }

        private void OnElevationScroll(InputAction.CallbackContext _)
        {
            if (m_ToolSystem.activeTool is not NetToolSystem || InputManager.instance.mouseOverUI)
                return;

            float newElevation = m_NetToolSystem.elevationStep / 2.0f;
            newElevation = (newElevation < 1.25f == true) ? 10f : newElevation;

            cohtml.Net.View ui = GameManager.instance.userInterface.view.View;
            ui.TriggerEvent("tool.setElevationStep", newElevation);

            EntityQuery uxSoundQuery = GetEntityQuery(ComponentType.ReadOnly<ToolUXSoundSettingsData>());
            ToolUXSoundSettingsData soundSettingsData = uxSoundQuery.GetSingleton<ToolUXSoundSettingsData>();
            AudioManager.instance.PlayUISound(soundSettingsData.m_NetStartSound, 0.5f);
        }

        private void AddBinding(string name, string binding, Action<InputAction.CallbackContext> callback)
        {
            InputAction customInputAction = new(name: name, binding: binding);
            customInputAction.performed += callback;
            customInputAction.Enable();

            UnityEngine.Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: Added binding " + name + " with key " + binding);
        }

        private void AddCombindBinding(string name, string binding, string modifier, Action<InputAction.CallbackContext> callback)
        {
            InputAction customInputAction = new(name);
            customInputAction.AddCompositeBinding("ButtonWithOneModifier")
                    .With("Modifier", modifier)
                    .With("Button", binding);
            customInputAction.performed += callback;
            customInputAction.Enable();
            UnityEngine.Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: Added binding " + name + " with key combo " + modifier + " + " + binding);
        }

        [Preserve]
        public ExtendedHotkeysSystem()
        {
        }
    }
}