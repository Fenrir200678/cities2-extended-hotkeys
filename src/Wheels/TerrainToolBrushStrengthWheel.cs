using ExtendedHotkeys.Settings;
using Game.Audio;
using Game.Prefabs;
using Game.Tools;
using Unity.Entities;
using UnityEngine;

namespace ExtendedHotkeys.Wheels
{
    public sealed class TerrainToolBrushStrengthWheel : WheelBase
    {
        private readonly TerrainToolSystem m_TerrainToolSystem;

        public TerrainToolBrushStrengthWheel(ToolSystem toolSystem, TerrainToolSystem terrainToolSystem, EntityQuery soundQuery, LocalSettingsItem settings)
            : base(toolSystem, soundQuery, settings)
        {
            m_TerrainToolSystem = terrainToolSystem;
            UnityEngine.Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: {ToString()}");
        }

        public override void HandleAction()
        {
            if (!m_Settings.EnableBrushStrengthWheel || m_CameraMap == null)
                return;

            if (m_ToolSystem.activeTool is not TerrainToolSystem)
                return;

            if (IsHoldingKey(KeyCode.LeftAlt))
            {
                m_IsInProgress = true;

                if (m_Settings.EnableBrushStrengthReverse ? IsZoomingOut() : IsZoomingIn())
                {
                    switch (m_TerrainToolSystem.brushStrength)
                    {
                        case < 0.09f:
                            m_TerrainToolSystem.brushStrength += 0.01f;
                            break;
                        case < 1f:
                            m_TerrainToolSystem.brushStrength += 0.05f;
                            break;
                        case >= 1f:
                            m_TerrainToolSystem.brushStrength = 1.00f;
                            break;
                    }
                    
                    ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                    AudioManager.instance.PlayUISound(soundData.m_NetStartSound, 0.5f);
                }
                else if (m_Settings.EnableBrushStrengthReverse ? IsZoomingIn() : IsZoomingOut())
                {
                    switch (m_TerrainToolSystem.brushStrength)
                    {
                        case <= 0.01f:
                            m_TerrainToolSystem.brushStrength = 0.01f;
                            break;
                        case <= 0.10f:
                            m_TerrainToolSystem.brushStrength -= 0.01f;
                            break;
                        case <= 1f:
                            m_TerrainToolSystem.brushStrength -= 0.05f;
                            break;
                    }
                    
                    ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                    AudioManager.instance.PlayUISound(soundData.m_NetStartSound, 0.5f);
                }

                return;
            }

            m_IsInProgress = false;
        }
    }
}