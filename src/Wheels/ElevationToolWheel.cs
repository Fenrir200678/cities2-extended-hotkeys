using ExtendedHotkeys.Settings;
using Game.Audio;
using Game.Input;
using Game.Prefabs;
using Game.Tools;
using Unity.Entities;
using UnityEngine;

namespace ExtendedHotkeys.Wheels
{
    internal class ElevationToolWheel: WheelBase
    {
        public ProxyAction m_RightMouseClick;
        private readonly NetToolSystem m_NetToolSystem;
        public ElevationToolWheel(ToolSystem toolSystem, NetToolSystem netToolSystem, EntityQuery soundQuery, LocalSettingsItem settings)
            : base(toolSystem, soundQuery, settings)
        {
            m_NetToolSystem = netToolSystem;
            m_RightMouseClick = InputManager.instance.FindAction("Tool", "Mouse Cancel");
            UnityEngine.Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: {ToString()}");
        }

        public override void HandleAction()
        {
            if (!m_Settings.EnableElevationWheel || m_CameraMap == null)
                return;

            // Disable wheel action if NetTool is not active
            if (m_ToolSystem.activeTool is not NetToolSystem)
                return; 

            if (IsHoldingKey(KeyCode.LeftAlt))
            {
                m_RightMouseClick.shouldBeEnabled = false;
                m_IsInProgress = true;

                if (m_Settings.EnableElevationReverse ? IsZoomingIn() : IsZoomingOut())
                {
                    m_NetToolSystem.ElevationUp();
                    ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                    AudioManager.instance.PlayUISound(soundData.m_NetElevationUpSound);
                }
                else if (m_Settings.EnableElevationReverse ? IsZoomingOut() : IsZoomingIn())
                {
                    m_NetToolSystem.ElevationDown();
                    ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                    AudioManager.instance.PlayUISound(soundData.m_NetElevationDownSound);
                }

                return;
            }

            m_RightMouseClick.shouldBeEnabled = true;
            m_IsInProgress = false;
        }
    }
}
