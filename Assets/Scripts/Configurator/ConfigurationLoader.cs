using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using System;
using static Common;
using System.Linq;
using System.Drawing;

public class ConfigurationLoader : MonoBehaviour {
   [SerializeField] GameObject configList;
   [SerializeField] GameObject configPrefab;
   [SerializeField] ColorChanger carColorChanger;
   [SerializeField] ColorChanger rimsColorChanger;

    List<ConfigData> configurations = new List<ConfigData>();
    List<GameObject> configurationObjects = new List<GameObject>();
    bool isConfigurationSelected = false;

    const string configFileName = "configData.json";

    void OnEnable() {
        isConfigurationSelected = false;
        LoadConfigurations();
        CreateConfigurationObjects();
    }

    void LoadConfigurations() {
        string filePath = Path.Combine(Application.persistentDataPath, configFileName);

        if (!File.Exists(filePath)) {
            return;
        }

        configurations = new List<ConfigData>();
        object configs = JsonConvert.DeserializeObject(File.ReadAllText(filePath));

        if (configs is JArray entryArray) {
            // if it is an array, deserialize it as a list of objects
            configurations = entryArray.ToObject<List<ConfigData>>();
        }
        else if (configs is JObject entryObject) {
            // if it's a single object, deserialize it as a single object and add it to the list
            configurations.Add(entryObject.ToObject<ConfigData>());
        }
    }

    void CreateConfigurationObject(ConfigData config) {
        GameObject configObject = Instantiate(configPrefab);
        configObject.name = config.configName + " " + config.configNumber;
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
        isConfigurationSelected = true;

        Item<eColor> color = Common.FindColorByName(config.color);
        carColorChanger.ChangeElementsColor(color.hex);
        Item<eRims> rims = Common.FindRimsByName(config.rims);
        rimsColorChanger.ChangeElementsColor(rims.hex);
    }

    void OnDisable() {
        if (isConfigurationSelected) {
            carColorChanger.ChangeElementsColorToDefault();
            rimsColorChanger.ChangeElementsColorToDefault();
        }
        DestroyConfigObjects();
    }

}
