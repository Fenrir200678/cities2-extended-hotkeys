using ExtendedHotkeys.Settings;
using Game.Audio;
using Game.Prefabs;
using Game.Tools;
using Unity.Entities;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ExtendedHotkeys.Wheels
{
    public sealed class TerrainToolBrushSizeWheel : WheelBase
    {
        private readonly TerrainToolSystem m_TerrainToolSystem;

        public TerrainToolBrushSizeWheel(ToolSystem toolSystem, TerrainToolSystem terrainToolSystem, EntityQuery soundQuery, LocalSettingsItem settings)
            : base(toolSystem, soundQuery, settings)
        {
            m_TerrainToolSystem = terrainToolSystem;
            Debug.Log($"[{MyPluginInfo.PLUGIN_NAME}]: {ToString()}");
        }

        public override void HandleAction()
        {
            if (!m_Settings.EnableBrushSizeWheel || m_CameraMap == null)
                return;
            
            if (m_ToolSystem.activeTool is not TerrainToolSystem)
                return;

            if (IsHoldingKey(KeyCode.LeftControl))
            {
                m_IsInProgress = true;

                if (IsZoomingIn())
                {
                    switch (m_TerrainToolSystem.brushSize)
                    {
                        case < 100:
                            m_TerrainToolSystem.brushSize += 10f;
                            break;
                        case < 500:
                            m_TerrainToolSystem.brushSize += 50f;
                            break;
                        case < 1000:
                            m_TerrainToolSystem.brushSize += 100f;
                            break;
                        case >= 1000:
                            m_TerrainToolSystem.brushSize = 1000f;
                            break;
                    }

                    ToolUXSoundSettingsData soundData = m_SoundQuery.GetSingleton<ToolUXSoundSettingsData>();
                    AudioManager.instance.PlayUISound(soundData.m_NetStartSound, 0.5f);
                }
                else if (IsZoomingOut())
                {
                    switch (m_TerrainToolSystem.brushSize)
                    {
                        case <= 10:
                            m_TerrainToolSystem.brushSize = 10f;
                            break;
                        case < 100:
                            m_TerrainToolSystem.brushSize -= 10f;
                            break;
                        case < 500:
                            m_TerrainToolSystem.brushSize -= 50f;
                            break;
                        case <= 1000:
                            m_TerrainToolSystem.brushSize -= 100f;
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