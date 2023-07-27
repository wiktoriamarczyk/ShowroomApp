using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        List<string> configurations = configuratorPanelBehaviour.GetSelectedConfigurations();

        string version = configurations[(int)eConfigurationType.VERSION];
        string drive = configurations[(int)eConfigurationType.DRIVE];
        string color = configurations[(int)eConfigurationType.COLOR];
        string rims = configurations[(int)eConfigurationType.RIMS];
        List<string> packages = new List<string>();

        int packageFirstIndex = (int)eConfigurationType.PACKAGE;

        for (int i = packageFirstIndex; i < configurations.Count; ++i) {
            packages.Add(configurations[i]);
        }

        string currentDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        Common.SetConfigurationCount(Common.GetConfigurationCount() + 1);
        int configNumber = Common.GetConfigurationCount();

        ConfigData configData = new ConfigData {
            configName = Common.defaultConfigName,
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
            object configs = JsonConvert.DeserializeObject(File.ReadAllText(filePath));

            if (configs is JArray entryArray) {
                // if it is an array, deserialize it as a list of objects
                configList = entryArray.ToObject<List<ConfigData>>();
            }
            else if (configs is JObject entryObject) {
                // if it's a single object, deserialize it as a single object and add it to the list
                configList.Add(entryObject.ToObject<ConfigData>());
            }
        }

        configList.Add(newConfig);
        string jsonData = JsonConvert.SerializeObject(configList);
        File.WriteAllText(filePath, jsonData);
        Debug.Log(filePath);
    }
}
