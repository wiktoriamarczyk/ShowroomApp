using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static Common;

public class ConfigurationLoader : MonoBehaviour {
   [SerializeField] GameObject configList;
   [SerializeField] GameObject configPrefab;
   [SerializeField] Panel configPanel;
   [SerializeField] ColorChanger carColorChanger;
   [SerializeField] ColorChanger rimsColorChanger;

    List<ConfigData> configurations = new List<ConfigData>();
    List<GameObject> configurationObjects = new List<GameObject>();
    ConfiguratorPanelBehaviour configBehaviour;

    const string configFileName = "configData.json";

    private void Awake() {
        configBehaviour = configPanel.GetComponent<ConfiguratorPanelBehaviour>();
    }

    void OnEnable() {
        LoadConfigurations();
        CreateConfigurationObjects();
    }

    void LoadConfigurations() {
        string filePath = Path.Combine(Application.persistentDataPath, configFileName);

        if (!File.Exists(filePath)) {
            return;
        }

        configurations = JsonConvert.DeserializeObject<List<ConfigData>>(File.ReadAllText(filePath));
    }

    void CreateConfigurationObject(ConfigData config) {
        GameObject configObject = Instantiate(configPrefab);
        configObject.name = config.configName;
        configObject.transform.SetParent(configList.transform);
        configObject.transform.localScale = Vector3.one;

        var textMeshPro = configObject.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.text = configObject.name + " " + config.configDate;

        Button button = configObject.GetComponent<Button>();
        button.onClick.AddListener(() => DisplayClickedConfiguration(config));

        configurationObjects.Add(configObject);
    }

    void CreateConfigurationObjects() {
        foreach (ConfigData config in configurations) {
            CreateConfigurationObject(config);
        }
    }
    void DestroyConfigObjects() {
        foreach (GameObject configObject in configurationObjects) {
            Destroy(configObject);
        }
    }

    void DisplayClickedConfiguration(ConfigData config) {
        PanelManager.instance.ShowPanel(configPanel);
        configBehaviour.SelectConfigurations(config);
    }

    void OnDisable() {
        DestroyConfigObjects();
    }

}
