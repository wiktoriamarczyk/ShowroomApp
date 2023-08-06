using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
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
        saveButton.onClick.AddListener(async () =>  await GetAndSaveConfiguration() );
    }

    async UniTask GetAndSaveConfiguration() {
        PanelManager.Instance.SetPopupDefaultInput(Common.GetDefaultConfigName());

        bool result = await PanelManager.Instance.ShowPopup(Common.ePopupType.INPUT_FIELD, Common.localizationConfigNameInfo);
        if (!result) {
            return;
        }

        string configName = PanelManager.Instance.GetPopupInput();
        if (configName == String.Empty) {
            configName = GetDefaultConfigName();
        }

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

        ConfigData configData = new ConfigData {
            configName = configName,
            configDate = currentDate,
            version = version,
            drive = drive,
            color = color,
            rims = rims,
            packages = packages
        };

        await SaveDataOnDisk(configData);
    }

    async UniTask SaveDataOnDisk(ConfigData newConfig) {
        string filePath = Path.Combine(Application.persistentDataPath, configFileName);
        List<ConfigData> configList = LoadConfigList(filePath);
        bool dataOverwrite = false;

        if (configList != null) {
            dataOverwrite = TryOverwriteConfig(newConfig, configList);
            if (dataOverwrite) {
                bool result = await PanelManager.Instance.ShowPopup(Common.ePopupType.DEFAULT, Common.localizationOverrideDataWarning);
                if (!result) {
                    return;
                }
            }
        }
        else {
            configList = new List<ConfigData>();
        }

        if (!dataOverwrite) {
            configList.Add(newConfig);
        }
        string jsonData = JsonConvert.SerializeObject(configList);
        File.WriteAllText(filePath, jsonData);

        if (newConfig.configName == Common.GetDefaultConfigName()) {
            Common.SetConfigurationCount(Common.GetConfigurationCount() + 1);
        }

        Debug.Log("Config saved to: " + filePath);
    }

    List<ConfigData> LoadConfigList(string filePath) {
        if (File.Exists(filePath)) {
            string fileContent = File.ReadAllText(filePath);
            if (!string.IsNullOrWhiteSpace(fileContent)) {
                return JsonConvert.DeserializeObject<List<ConfigData>>(fileContent);
            }
        }
        return null;
    }

    bool TryOverwriteConfig(ConfigData newConfig, List<ConfigData> configList) {
        for (int i = 0; i < configList.Count; ++i) {
            if (configList[i].configName == newConfig.configName) {
                configList[i] = newConfig;
                return true;
            }
        }
        return false;
    }
}
