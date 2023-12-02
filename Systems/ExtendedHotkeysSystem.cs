using cohtml.Net;
using Game;
using Game.Audio;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Tools;
using System;
using Unity.Entities;
using UnityEngine.InputSystem;

namespace ExtendedHotkeys.Systems
{
    class ExtendedHotkeysSystem : GameSystemBase
    {
        private ToolSystem m_ToolSystem;
        protected override void OnCreate()
        {
            base.OnCreate();

            m_ToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();

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

            UnityEngine.Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}] System created!");
        }

        protected override void OnUpdate()
        {
        }

        private void OnSetNetToolSystemMode(InputAction.CallbackContext _, NetToolSystem.Mode mode)
        {
            if (m_ToolSystem.activeTool is not NetToolSystem)
                return;

            NetToolSystem netToolSystem = World.GetExistingSystemManaged<NetToolSystem>();
            netToolSystem.mode = mode;

            View ui = GameManager.instance.userInterface.view.View;
            ui.TriggerEvent("tool.selectToolMode", (int)mode);

            EntityQuery soundQuery = GetEntityQuery(ComponentType.ReadOnly<ToolUXSoundSettingsData>());
            ToolUXSoundSettingsData soundData = soundQuery.GetSingleton<ToolUXSoundSettingsData>();
            AudioManager.instance.PlayUISound(soundData.m_NetStartSound, 1.0f);

            UnityEngine.Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: Set NetToolSystem.Mode to " + mode.ToString());
        }

        private void OnResetElevation(InputAction.CallbackContext _)
        {
            if (m_ToolSystem.activeTool is not NetToolSystem)
                return;

            if (GameManager.instance.gameMode == GameMode.MainMenu)
            {
                UnityEngine.Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: Cannot trigger in main menu.");
                return;
            }

            World.GetOrCreateSystemManaged<NetToolSystem>().elevation = 0f;
            EntityQuery uxSoundQuery = GetEntityQuery(ComponentType.ReadOnly<ToolUXSoundSettingsData>());
            ToolUXSoundSettingsData soundSettingsData = uxSoundQuery.GetSingleton<ToolUXSoundSettingsData>();
            AudioManager.instance.PlayUISound(soundSettingsData.m_NetElevationDownSound);

            UnityEngine.Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: Reset elevation to 0.");
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
    }
}