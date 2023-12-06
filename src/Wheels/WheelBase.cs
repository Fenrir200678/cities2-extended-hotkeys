using Game.Input;
using Game.Tools;
using UnityEngine;
using ExtendedHotkeys.Settings;
using Unity.Entities;
using Game.SceneFlow;
using cohtml.Net;

namespace ExtendedHotkeys.Wheels
{
    public abstract class WheelBase
    {
        protected readonly NetToolSystem m_NetToolSystem;
        protected readonly ToolSystem m_ToolSystem;
        protected readonly EntityQuery m_SoundQuery;
        protected readonly LocalSettingsItem m_Settings;
        protected readonly View m_View;

        protected readonly ProxyAction m_MouseZoomAction;
        protected readonly ProxyActionMap m_CameraMap;

        protected bool m_IsInProgress;
        public bool IsActive => m_IsInProgress;

        protected WheelBase(ToolSystem toolSystem, NetToolSystem netToolSystem, EntityQuery soundQuery, LocalSettingsItem settings)
        {
            m_ToolSystem = toolSystem;
            m_NetToolSystem = netToolSystem;
            m_SoundQuery = soundQuery;
            m_Settings = settings;

            m_CameraMap = InputManager.instance.FindActionMap("Camera");
            m_MouseZoomAction = m_CameraMap.FindAction("Zoom Mouse");
            m_View = GameManager.instance.userInterface.view.View;
            m_IsInProgress = false;
        }

        public abstract void HandleAction();

        protected bool IsHoldingKey(KeyCode keyCode)
        {
            return Input.GetKey(keyCode);
        }

        protected bool IsZoomingIn(WheelSensitivityFactor sensitivityFactor = WheelSensitivityFactor.Medium)
        {
            float mouseZoomValue = m_MouseZoomAction.ReadValue<float>();
            return mouseZoomValue < -GetZoomFactor(sensitivityFactor);
        }

        protected bool IsZoomingOut(WheelSensitivityFactor sensitivityFactor = WheelSensitivityFactor.Medium)
        {
            float mouseZoomValue = m_MouseZoomAction.ReadValue<float>();
            return mouseZoomValue > GetZoomFactor(sensitivityFactor);
        }

        private float GetZoomFactor(WheelSensitivityFactor factor)
        {
            return factor switch
            {
                WheelSensitivityFactor.Low => 0.02f,
                WheelSensitivityFactor.High => 0.008f,
                _ => 0.013f,
            };
        }

        protected enum WheelSensitivityFactor
        {
            Low = 1,
            Medium = 2,
            High = 3
        }
    }
}
