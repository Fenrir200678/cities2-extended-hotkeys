using ExtendedHotkeys.Settings;
using ExtendedHotkeys.Wheels;
using Game.Audio;
using Game.Prefabs;
using Game.Tools;
using Unity.Entities;
using UnityEngine;

namespace ExtendedHotkeys.MouseWheels
{
    internal class ElevationToolWheel: WheelBase
    {
        public ElevationToolWheel(ToolSystem toolSystem, NetToolSystem netToolSystem, EntityQuery soundQuery, LocalSettingsItem settings)
            : base(toolSystem, netToolSystem, soundQuery, settings)
        {
            UnityEngine.Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: {ToString()}");
        }

        public override void HandleAction()
        {
            if (m_Settings.EnableElevationWheel && m_CameraMap != null)
            {
                // Disable wheel action if NetTool is not active
                if (m_ToolSystem.activeTool is not NetToolSystem)
                    return; 

                if (IsHoldingKey(KeyCode.LeftAlt))
                {
                    m_IsInProgress = true;

                    if (IsZoomingOut())
                    {
                        m_NetToolSystem.ElevationUp();
                        ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                        AudioManager.instance.PlayUISound(soundData.m_NetElevationUpSound);
                    }
                    else if (IsZoomingIn())
                    {
                        m_NetToolSystem.ElevationDown();
                        ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                        AudioManager.instance.PlayUISound(soundData.m_NetElevationDownSound);
                    }

                    return;
                }

                m_IsInProgress = false;
            }
        }
    }
}
