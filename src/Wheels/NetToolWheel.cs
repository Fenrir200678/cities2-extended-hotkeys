using ExtendedHotkeys.Settings;
using ExtendedHotkeys.Wheels;
using Game.Audio;
using Game.Prefabs;
using Game.Tools;
using Unity.Entities;
using UnityEngine;

namespace ExtendedHotkeys.MouseWheels
{
    public class NetToolWheel : WheelBase
    {
        public NetToolWheel(ToolSystem toolSystem, NetToolSystem netToolSystem, EntityQuery soundQuery, LocalSettingsItem settings)
            : base(toolSystem, netToolSystem, soundQuery, settings)
        {
            UnityEngine.Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: {ToString()}");
        }

        public override void HandleAction()
        {
            if (m_Settings.EnableNetToolWheel && m_CameraMap != null)
            {
                if (m_ToolSystem.activeTool is not NetToolSystem)
                    return;

                if (IsHoldingKey(KeyCode.LeftControl))
                {
                    m_IsInProgress = true;

                    if (IsZoomingIn())
                    {
                        if (m_NetToolSystem.mode == NetToolSystem.Mode.Replace)
                        {
                            m_NetToolSystem.mode = NetToolSystem.Mode.Straight;
                            m_View.TriggerEvent("tool.selectToolMode", 0);
                            ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                            AudioManager.instance.PlayUISound(soundData.m_NetStartSound, 0.5f);
                        }
                        else
                        {
                            int mode = (int)m_NetToolSystem.mode;
                            int newMode = mode + 1;
                            m_NetToolSystem.mode = (NetToolSystem.Mode)newMode;
                            m_View.TriggerEvent("tool.selectToolMode", newMode);
                            ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                            AudioManager.instance.PlayUISound(soundData.m_NetStartSound, 0.5f);
                        }
                    }
                    else if (IsZoomingOut())
                    {
                        if (m_NetToolSystem.mode == NetToolSystem.Mode.Straight)
                        {
                            m_NetToolSystem.mode = NetToolSystem.Mode.Replace;
                            m_View.TriggerEvent("tool.selectToolMode", 5);
                            ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                            AudioManager.instance.PlayUISound(soundData.m_NetStartSound, 0.5f);
                        }
                        else
                        {
                            int mode = (int)m_NetToolSystem.mode;
                            int newMode = mode - 1;
                            m_NetToolSystem.mode = (NetToolSystem.Mode)newMode;

                            m_View.TriggerEvent("tool.selectToolMode", newMode);
                            ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                            AudioManager.instance.PlayUISound(soundData.m_NetStartSound, 0.5f);
                        }
                    }

                    return;
                }

                m_IsInProgress = false;
            }
        }
    }
}
