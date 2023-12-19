using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtendedHotkeys.Settings
{
    public class LocalSettings
    {
        public LocalSettingsItem Settings { get; private set; }

        public void Init() => Load();
        public void Reload() => Load();

        /// <summary>
        /// Save settings to a local JSON file
        /// </summary>
        public async Task Save()
        {
            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            const string filename = "UserSettings.json";

            if (assemblyDirectory != null)
            {
                string fullFilePath = Path.Combine(assemblyDirectory, filename);

                try
                {
                    string updatedSettingsJson = JsonConvert.SerializeObject(Settings);
                    await using StreamWriter writer = new(fullFilePath, false, Encoding.UTF8);
                    await writer.WriteAsync(updatedSettingsJson);
                }
                catch (Exception e)
                {
                    Debug.Log($"Error saving settings: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Load settings from a local JSON file
        /// </summary>
        private void Load()
        {
            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            const string filename = "UserSettings.json";

            if (assemblyDirectory == null)
                return;

            string fullFilePath = Path.Combine(assemblyDirectory, filename);

            if (!File.Exists(fullFilePath))
            {
                Debug.Log("No user settings found. Use default settings.");
                fullFilePath = Path.Combine(assemblyDirectory, "DefaultSettings.json");
                if (!File.Exists(fullFilePath))
                {
                    Debug.Log($"Error loading settings: {fullFilePath} does not exist.");
                    return;
                }
            }
            else
            {
                Debug.Log("User settings successfully loaded.");
            }

            try
            {
                // Access settings
                string settingsJson = File.ReadAllText(fullFilePath);
                LocalSettingsItem localSettingsItem = JsonConvert.DeserializeObject<LocalSettingsItem>(settingsJson);

                if (localSettingsItem != null)
                {
                    Settings = localSettingsItem;
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Error loading settings: {e.Message}");
            }
        }
    }
}
