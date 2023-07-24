using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Common;

public class ConfigurationsSaver : MonoBehaviour {
    [SerializeField] Button saveButton;
    ConfiguratorPanelBehaviour configuratorPanelBehaviour;

    const string configFileName = "configData.json";

    void Awake() {
        configuratorPanelBehaviour = GetComponent<ConfiguratorPanelBehaviour>();
        saveButton.onClick.AddListener(GetAndSaveConfigurations);
    }

    void GetAndSaveConfigurations() {
        List<Toggle> configurations = configuratorPanelBehaviour.GetSelectedConfigurations();

        string version = configurations[(int)eConfigurationType.VERSION].name;
        string drive = configurations[(int)eConfigurationType.DRIVE].name;
        string color = configurations[(int)eConfigurationType.COLOR].name;
        string rims = configurations[(int)eConfigurationType.RIMS].name;
        List<string> packages = new List<string>();

        int packageFirstIndex = (int)eConfigurationType.PACKAGE;

        for (int i = packageFirstIndex; i < configurations.Count; ++i) {
            packages.Add(configurations[i].name);
        }

        string currentDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        Common.SetConfigurationCount(Common.GetConfigurationCount() + 1);
        int configNumber = Common.GetConfigurationCount();

        ConfigData configData = new ConfigData {
            configName = Common.configName,
            configNumber = configNumber,
            configDate = currentDate,
            version = version,
            drive = drive,
            color = color,
            rims = rims,
            packages = packages
        };

        SaveDataOnDisk(configData);
    }

    void SaveDataOnDisk(ConfigData newConfig) {
        string filePath = Path.Combine(Application.persistentDataPath, configFileName);
        List<ConfigData> configList = new List<ConfigData>();
        if (File.Exists(filePath)) {
            string fileContent = File.ReadAllText(filePath);
            if (!string.IsNullOrWhiteSpace(fileContent)) {
                string existingData = File.ReadAllText(filePath);
                if (existingData.StartsWith("[") && existingData.EndsWith("]")) {
                    configList = JsonConvert.DeserializeObject<List<ConfigData>>(existingData);
                }
                else {
                    ConfigData existingConfig = JsonConvert.DeserializeObject<ConfigData>(existingData);
                    configList.Add(existingConfig);
                }
            }
        }
        configList.Add(newConfig);
        string jsonData = JsonConvert.SerializeObject(configList);
        File.WriteAllText(filePath, jsonData);
        Debug.Log(filePath);
    }
}
