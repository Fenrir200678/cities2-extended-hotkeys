using ExtendedHotkeys.Settings;
using ExtendedHotkeys.Wheels;
using Game;
using Game.Audio;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Tools;
using Unity.Entities;
using UnityEngine;

namespace ExtendedHotkeys.MouseWheels
{
    public class NetToolWheel : WheelBase
    {
        CameraController m_CameraController;

        public NetToolWheel(ToolSystem toolSystem, NetToolSystem netToolSystem, EntityQuery soundQuery, LocalSettingsItem settings)
            : base(toolSystem, netToolSystem, soundQuery, settings)
        {
            UnityEngine.Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: {ToString()}");
        }

        public override void HandleAction()
        {
            if (m_Settings.EnableNetToolWheel && m_ToolSystem.activeTool is NetToolSystem)
            {
                if (m_CameraController == null && CameraController.TryGet(out CameraController cameraController))
                {
                    m_CameraController = cameraController;
                }

                if (IsHoldingKey(m_Settings.NetToolKeyCode))
                {
                    m_CameraController.inputEnabled = false;
                    if (m_CameraMap != null)
                    {
                        cohtml.Net.View ui = GameManager.instance.userInterface.view.View;
                        if (IsZoomingIn())
                        {
                            if (m_NetToolSystem.mode == NetToolSystem.Mode.Grid)
                            {
                                m_NetToolSystem.mode = NetToolSystem.Mode.Straight;
                                ui.TriggerEvent("tool.selectToolMode", 0);
                                ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                                AudioManager.instance.PlayUISound(soundData.m_NetStartSound, 0.5f);
                            }
                            else
                            {
                                int mode = (int)m_NetToolSystem.mode;
                                int newMode = mode + 1;
                                m_NetToolSystem.mode = (NetToolSystem.Mode)newMode;
                                ui.TriggerEvent("tool.selectToolMode", newMode);
                                ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                                AudioManager.instance.PlayUISound(soundData.m_NetStartSound, 0.5f);
                            }
                        }
                        else if(IsZoomingOut())
                        {
                            if (m_NetToolSystem.mode == NetToolSystem.Mode.Straight)
                            {
                                m_NetToolSystem.mode = NetToolSystem.Mode.Grid;
                                ui.TriggerEvent("tool.selectToolMode", 4);
                                ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                                AudioManager.instance.PlayUISound(soundData.m_NetStartSound, 0.5f);
                            }
                            else
                            {
                                int mode = (int)m_NetToolSystem.mode;
                                int newMode = mode - 1;
                                m_NetToolSystem.mode = (NetToolSystem.Mode)newMode;

                                ui.TriggerEvent("tool.selectToolMode", newMode);
                                ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                                AudioManager.instance.PlayUISound(soundData.m_NetStartSound, 0.5f);
                            }
                        }
                    }
                }

                if (Input.GetKeyUp(m_Settings.NetToolKeyCode))
                {
                    m_CameraController.inputEnabled = true;
                }
            }
        }
    }
}
