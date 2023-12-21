using cohtml.Net;
using ExtendedHotkeys.Settings;
using ExtendedHotkeys.Wheels;
using Game;
using Game.Audio;
using Game.Input;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Tools;
using Game.UI.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;


namespace ExtendedHotkeys.Systems
{
    class ExtendedHotkeysSystem : GameSystemBase
    {
        private GameScreenUISystem m_GameScreenUISystem;
        private ToolSystem m_ToolSystem;
        private NetToolSystem m_NetToolSystem;
        private TerrainToolSystem m_TerrainToolSystem;
        private ToolUXSoundSettingsData m_SoundData;

        private readonly List<WheelBase> m_Wheels = [];
        private CameraController m_CameraController;

        public LocalSettings m_LocalSettings;
        private LocalSettingsItem m_Settings;

        private PrefabSystem m_PrefabSystem;

        private bool m_LocalSettingsLoaded;
        public bool hotkeyPressed = false;

        private WheelBase ActiveWheel => m_Wheels.Find((WheelBase wheel) => wheel.IsActive);
        private bool IsAnyWheelActive => m_Wheels.Any((WheelBase wheel) => wheel.IsActive);

        // Available KeyCodes a user can choose for hotkeys
        public List<KeyCode> availableKeyCodes =
        [
            KeyCode.LeftShift,
            KeyCode.LeftControl,
            KeyCode.LeftAlt,
            KeyCode.RightShift,
            KeyCode.RightControl,
            KeyCode.RightAlt,
            KeyCode.Space
        ];


        [UnityEngine.Scripting.Preserve]
        public ExtendedHotkeysSystem()
        {
        }

        [UnityEngine.Scripting.Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();
            LoadSettings();

            m_ToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_NetToolSystem = World.GetExistingSystemManaged<NetToolSystem>();
            m_TerrainToolSystem = World.GetExistingSystemManaged<TerrainToolSystem>();
            World.GetOrCreateSystemManaged<ExtendedHotKeysTranslationSystem>();
            m_GameScreenUISystem = World.GetExistingSystemManaged<GameScreenUISystem>();
            EntityQuery soundQuery = GetEntityQuery(ComponentType.ReadOnly<ToolUXSoundSettingsData>());
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            // Add hotkeys
            CreateNetToolBindings();
            CreateOpenToolWindowBindings();

            // Add mouse wheels
            m_Wheels.Add(new NetToolWheel(m_ToolSystem, m_NetToolSystem, soundQuery, m_Settings));
            m_Wheels.Add(new ElevationToolWheel(m_ToolSystem, m_NetToolSystem, soundQuery, m_Settings));
            m_Wheels.Add(new TerrainToolBrushSizeWheel(m_ToolSystem, m_TerrainToolSystem, soundQuery, m_Settings));
            m_Wheels.Add(new TerrainToolBrushStrengthWheel(m_ToolSystem, m_TerrainToolSystem, soundQuery, m_Settings));

            Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}] System created!");
        }

        [UnityEngine.Scripting.Preserve]
        protected override void OnUpdate()
        {
            if (!m_LocalSettingsLoaded)
                return;

            if (m_CameraController == null && CameraController.TryGet(out CameraController cameraController))
            {
                m_CameraController = cameraController;
            }

            if (m_ToolSystem != null && m_CameraController != null && InputManager.instance.mouseOnScreen)
            {
                foreach (WheelBase wheel in m_Wheels)
                {
                    wheel.HandleAction();
                }
            }

            m_CameraController.inputEnabled = !IsAnyWheelActive;
        }

        private void CreateCameraMoveReplacement()
        {
        }

        private void CreateNetToolBindings()
        {
            string binding = "<Keyboard>/home";
            bool developerMode = GameManager.instance.configuration.developerMode;

            if (developerMode)
            {
                Debug.Log("Developer mode is enabled. Set binding to END instead.");
                binding = "<Keyboard>/end";
            }

            AddBinding(name: "ResetElevationToZero", binding: binding, callback: OnResetElevation);
            AddCombinedBinding(name: "SetStraight", modifier: "<keyboard>/ctrl", binding: "<Keyboard>/q", callback: (_) => OnSetNetToolSystemMode(_, NetToolSystem.Mode.Straight));
            AddCombinedBinding(name: "SetCurveTool", modifier: "<keyboard>/ctrl", binding: "<Keyboard>/w", callback: (_) => OnSetNetToolSystemMode(_, NetToolSystem.Mode.SimpleCurve));
            AddCombinedBinding(name: "SetAdvCurveTool", modifier: "<keyboard>/ctrl", binding: "<Keyboard>/e", callback: (_) => OnSetNetToolSystemMode(_, NetToolSystem.Mode.ComplexCurve));
            AddCombinedBinding(name: "SetContinuousMode", modifier: "<keyboard>/ctrl", binding: "<Keyboard>/r", callback: (_) => OnSetNetToolSystemMode(_, NetToolSystem.Mode.Continuous));
            AddCombinedBinding(name: "SetGridMode", modifier: "<keyboard>/ctrl", binding: "<Keyboard>/t", callback: (_) => OnSetNetToolSystemMode(_, NetToolSystem.Mode.Grid));

            // Add bindings for elevation tool ALT + Right Mouse
            AddCombinedBinding(name: "UpdateElevationStep", modifier: "<keyboard>/leftAlt", binding: "<mouse>/rightButton", callback: OnElevationScroll);

            // Add binding for anarchy mode ALT + A
            AddCombinedBinding(name: "EnableAnarchyMode", modifier: "<keyboard>/leftAlt", binding: "<keyboard>/a", callback: EnableAnarchyModeAction);
        }

        private void CreateOpenToolWindowBindings()
        {
            // TODO: Add support for custom key bindings from user selection
            // For now, we'll just use e,r & t to open the most used tool windows that are available at the start of the game
            AddBinding("OpenRoads", binding: "<Keyboard>/r", callback: (_) => OnOpenToolWindow(_, "Roads"));
            AddBinding("OpenZones", binding: "<Keyboard>/e", callback: (_) => OnOpenToolWindow(_, "Zones"));
            AddBinding("OpenLandscaping", binding: "<Keyboard>/t", callback: (_) => OnOpenToolWindow(_, "Landscaping"));
            
            /*
            AddCombinedBinding("OpenElectricity", modifier: "<keyboard>/ctrl", binding: "<Keyboard>/4", callback: (_) => OnOpenToolWindow(_, "Electricity"));
            AddCombinedBinding("OpenWaterAndSewage", modifier: "<keyboard>/ctrl", binding: "<Keyboard>/5", callback: (_) => OnOpenToolWindow(_, "Water & Sewage"));
            AddCombinedBinding("OpenHealthAndDeathcare", modifier:"<keyboard>/ctrl", binding: "###", callback: (_) => OnOpenToolWindow(_, "Health & Deathcare"));
            AddCombinedBinding("OpenGarbageManagement", modifier:"<keyboard>/ctrl", binding: "###", callback: (_) => OnOpenToolWindow(_, "Garbage Management"));
            AddCombinedBinding("OpenEducationAndResearch", modifier:"<keyboard>/ctrl", binding: "###", callback: (_) => OnOpenToolWindow(_, "Education & Research"));
            AddCombinedBinding("OpenFireAndRescue", modifier:"<keyboard>/ctrl", binding: "###", callback: (_) => OnOpenToolWindow(_, "Fire & Rescue"));
            AddCombinedBinding("OpenTransportation", modifier:"<keyboard>/ctrl", binding: "###", callback: (_) => OnOpenToolWindow(_, "Transportation"));
            AddCombinedBinding("OpenParksAndRecreation", modifier:"<keyboard>/ctrl", binding: "###", callback: (_) => OnOpenToolWindow(_, "Parks & Recreation"));
            AddCombinedBinding("OpenCommunications", modifier:"<keyboard>/ctrl", binding: "###", callback: (_) => OnOpenToolWindow(_, "Communications"));

            AddCombinedBinding("OpenSignatures", modifier:"<keyboard>/ctrl", binding: "###", callback: (_) => OnOpenToolWindow(_, "Signatures"));
            AddCombinedBinding("OpenPoliceAndAdministration", modifier:"<keyboard>/ctrl", binding: "###", callback: (_) => OnOpenToolWindow(_, "Police & Administration"));
            */
        }

        private Entity GetMenuEntity(string prefabName)
        {
            EntityQuery assetMenuData = GetEntityQuery(ComponentType.ReadOnly<UIAssetMenuData>());
            NativeArray<Entity> menuEntities = assetMenuData.ToEntityArray(Allocator.Temp);
            Entity menuEntity = Entity.Null;

            foreach (Entity entity in menuEntities)
            {
                UIAssetMenuPrefab assetMenuPrefab = m_PrefabSystem.GetPrefab<UIAssetMenuPrefab>(entity);
                if (assetMenuPrefab.name == prefabName)
                {
                    menuEntity = entity;
                }
            }

            return menuEntity;
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
            catch (Exception e)
            {
                Debug.Log($"Error loading settings: {e.Message}");
            }
        }

        private void OnOpenToolWindow(InputAction.CallbackContext _, string prefabName)
        {
            if (m_GameScreenUISystem.isMenuActive)
                return;
            
            View ui = GameManager.instance.userInterface.view.View;
            Entity menuEntity = GetMenuEntity(prefabName);

            if (menuEntity == Entity.Null)
            {
                Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: Could not find entity for prefab {prefabName}");
                return;
            }

            // We need to convert the entity to a dynamic object to be able to pass it to the UI event handler
            var menuObject = new
            {
                __Type = menuEntity.GetType().ToString(),
                index = menuEntity.Index,
                version = menuEntity.Version
            };

            ui.TriggerEvent("toolbar.selectAssetMenu", menuObject);
        }

        private void OnSetNetToolSystemMode(InputAction.CallbackContext _, NetToolSystem.Mode mode)
        {
            if (!m_Settings.EnableNTMGroup || m_ToolSystem.activeTool is not NetToolSystem)
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
            View ui = GameManager.instance.userInterface.view.View;
            ui.TriggerEvent("tool.selectToolMode", (int)mode);

            AudioManager.instance.PlayUISound(m_SoundData.m_NetStartSound);
        }

        private void OnResetElevation(InputAction.CallbackContext _)
        {
            if (!m_Settings.EnableElevationReset || m_ToolSystem.activeTool is not NetToolSystem)
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
            if (!m_Settings.EnableElevationStepScroll || m_ToolSystem.activeTool is not NetToolSystem || InputManager.instance.mouseOverUI)
                return;

            float newElevation = m_NetToolSystem.elevationStep / 2.0f;
            newElevation = newElevation < 1.25f ? 10f : newElevation;

            View ui = GameManager.instance.userInterface.view.View;
            ui.TriggerEvent("tool.setElevationStep", newElevation);

            EntityQuery uxSoundQuery = GetEntityQuery(ComponentType.ReadOnly<ToolUXSoundSettingsData>());
            ToolUXSoundSettingsData soundSettingsData = uxSoundQuery.GetSingleton<ToolUXSoundSettingsData>();
            AudioManager.instance.PlayUISound(soundSettingsData.m_NetStartSound, 0.5f);
        }

        private static void AddBinding(string name, string binding, Action<InputAction.CallbackContext> callback)
        {
            InputAction customInputAction = new(name: name, binding: binding);
            customInputAction.performed += callback;
            customInputAction.Enable();

            Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: Added binding " + name + " with key " + binding);
        }

        private static void AddCombinedBinding(string name, string binding, string modifier, Action<InputAction.CallbackContext> callback)
        {
            InputAction customInputAction = new(name);
            customInputAction.AddCompositeBinding("ButtonWithOneModifier")
                .With("Modifier", modifier)
                .With("Button", binding);
            customInputAction.performed += callback;
            customInputAction.Enable();
            Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: Added binding " + name + " with key combo " + modifier + " + " + binding);
        }
    }
}