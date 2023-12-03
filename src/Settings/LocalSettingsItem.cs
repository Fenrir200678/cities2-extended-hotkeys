using UnityEngine;

namespace ExtendedHotkeys.Settings
{
    public class LocalSettingsItem
    {

        public string Version { get; } = MyPluginInfo.PLUGIN_VERSION;

        // GENERAL
        public bool DisableMod { get; set; } = false;
        public bool EnableNetToolWheel { get; set; } = true;
        public bool EnableElevationWheel { get; set; } = true;
        public bool EnableNetToolHotkeys { get; set; } = true;
        public bool EnableSnappingWheel { get; set; } = true;
        public bool EnableAnarchyMode { get; set; } = true;
        public bool EnableElevationReset { get; set; } = true;
        public bool ExpandNTMGroup { get; set; } = false;
        public bool EnableNTMGroup { get; set; } = true;
        public bool EnableNTMStraight { get; set; } = true;
        public bool EnableNTMSimpleCurve { get; set; } = true;
        public bool EnableNTMComplexCurve { get; set; } = true;
        public bool EnableNTMContinuous { get; set; } = true;
        public bool EnableNTMGrid { get; set; } = true;
        public KeyCode NetToolKeyCode { get; set; } = KeyCode.LeftShift;
        public KeyCode SnappingKeyCode { get; set; } = KeyCode.LeftControl;
        public KeyCode ElevationKeyCode { get; set; } = KeyCode.LeftAlt;
    }
}
