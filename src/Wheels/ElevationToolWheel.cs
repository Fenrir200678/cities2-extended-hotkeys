using ExtendedHotkeys.Settings;
using ExtendedHotkeys.Wheels;
using Game;
using Game.Audio;
using Game.Prefabs;
using Game.Tools;
using Unity.Entities;
using UnityEngine;

namespace ExtendedHotkeys.MouseWheels
{
    internal class ElevationToolWheel: WheelBase
    {
        CameraController m_CameraController;

        public ElevationToolWheel(ToolSystem toolSystem, NetToolSystem netToolSystem, EntityQuery soundQuery, LocalSettingsItem settings)
            : base(toolSystem, netToolSystem, soundQuery, settings)
        {
            UnityEngine.Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: {ToString()}");
        }

        public override void HandleAction()
        {
            if (m_Settings.EnableElevationWheel && m_ToolSystem.activeTool is NetToolSystem)
            {
                if (m_CameraController == null && CameraController.TryGet(out CameraController cameraController))
                {
                    m_CameraController = cameraController;
                }

                if (IsHoldingKey(m_Settings.ElevationKeyCode))
                {
                    m_CameraController.inputEnabled = false;
                    if (m_CameraMap != null)
                    {
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
                    }
                }

                if (Input.GetKeyUp(m_Settings.ElevationKeyCode))
                {
                    m_CameraController.inputEnabled = true;
                }
            }
        }
    }
}
