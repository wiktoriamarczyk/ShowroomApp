using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Localization.Components;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using System.Drawing;
using UnityEngine.Events;

public class ConfiguratorPanelBehaviour : MonoBehaviour {
    [SerializeField] ColorChanger carColorChanger;
    [SerializeField] ColorChanger rimsColorChanger;

    [SerializeField] RectTransform transformToRefresh;
    [SerializeField] TextMeshProUGUI versionDescription;
    [SerializeField] ReactVersionData reactVersionData;
    [SerializeField] SharpVersionData sharpVersionData;
    [SerializeField] UnityVersionData unityVersionData;

    [SerializeField] GameObject textTogglePrefab;
    [SerializeField] GameObject versionTogglePrefab;
    [SerializeField] GameObject colorTogglePrefab;

    /* one element from these groups must be selected */
    [SerializeField] ToggleGroup versionToggleGroup;
    [SerializeField] ToggleGroup driveToggleGroup;
    [SerializeField] ToggleGroup colorsToggleGroup;
    [SerializeField] ToggleGroup rimsToggleGroup;
    /* none or all packages may be selected so they are not grouped into a toggle group */
    [SerializeField] GameObject  packageList;

    ToggleGroupBehaviour versionToggleGroupBehaviour;
    ToggleGroupBehaviour driveToggleGroupBehaviour;
    ToggleGroupBehaviour colorsToggleGroupBehaviour;
    ToggleGroupBehaviour rimsToggleGroupBehaviour;
    List<Toggle> packages = new List<Toggle>();

    VersionData currentVersion;
    Dictionary<GameObject, VersionData> versions = new Dictionary<GameObject, VersionData>();
    bool isCoroutineActive = false;
    bool isPanelInitialized = false;

    void Awake() {
        versionToggleGroupBehaviour = versionToggleGroup.GetComponent<ToggleGroupBehaviour>();
        driveToggleGroupBehaviour = driveToggleGroup.GetComponent<ToggleGroupBehaviour>();
        colorsToggleGroupBehaviour = colorsToggleGroup.GetComponent<ToggleGroupBehaviour>();
        rimsToggleGroupBehaviour = rimsToggleGroup.GetComponent<ToggleGroupBehaviour>();
    }

    void OnEnable() {
        bool isPanelShown = PanelManager.Instance.IsPanelShown(gameObject);
        if (!isPanelShown) {
            return;
        }
        if (!isPanelInitialized) {
            InitializeUIObjects();
        }
        RefreshData();
        isPanelInitialized = true;
    }

    void ActivateToggles<T>(Toggle[] toggles, T dataCollection) where T : class {
        foreach (Toggle toggle in toggles) {
            string toggleName = toggle.gameObject.name;

            bool isActive = dataCollection switch {
                IReadOnlyList<string> list => list.Contains(toggleName),
                IReadOnlyDictionary<string, string> dictionary => dictionary.ContainsKey(toggleName),
                _ => false
            };

            toggle.gameObject.SetActive(isActive);
        }
    }

    void ActivateObjects() {
        ActivateToggles(driveToggleGroupBehaviour.GetToggles(), currentVersion.drives);
        ActivateToggles(colorsToggleGroupBehaviour.GetToggles(), currentVersion.colorsData);
        ActivateToggles(rimsToggleGroupBehaviour.GetToggles(), currentVersion.rimsData);
        ActivateToggles(packages.ToArray(), currentVersion.packages);
    }

    void RefreshData() {
        if (isCoroutineActive) {
            return;
        }
        SetCurrentVersion(versionToggleGroupBehaviour.GetSelectedToggle().gameObject);
        StartCoroutine(ChangeDescription());
        ActivateObjects();
        RefreshSelectedToggles();
    }

    void RefreshSelectedToggles() {
        driveToggleGroupBehaviour.OnToggleStatusChanged();
        colorsToggleGroupBehaviour.OnToggleStatusChanged();
        rimsToggleGroupBehaviour.OnToggleStatusChanged();
    }

    GameObject CreateUIObject(string name, GameObject parent, GameObject prefab) {
        GameObject ui = Instantiate(prefab);
        ui.name = name;
        ui.transform.SetParent(parent.transform);
        ui.transform.localScale = Vector3.one;
        var localization = ui.GetComponentInChildren<LocalizeStringEvent>();
        var textMeshPro = ui.GetComponentInChildren<TextMeshProUGUI>();
        if (localization != null) {
            localization.OnUpdateString.AddListener((string value) => { textMeshPro.text = value; });
            localization.StringReference.TableReference = Common.localizationTableName;
            localization.StringReference.TableEntryReference = name;
        } else if (textMeshPro != null) {
            textMeshPro.text = name;
        }
        return ui;
    }


    void InitializeUIObjects() {
        foreach (var version in Common.versions) {
            GameObject versionObject = CreateUIObject(version.Value, versionToggleGroup.gameObject, versionTogglePrefab);
            versionObject.GetComponent<Toggle>().group = versionToggleGroup;
            versions.Add(versionObject, GetSOForVersion(version.Key));
        }

        foreach (var drive in Common.drives) {
            GameObject driveObject = CreateUIObject(drive.Value, driveToggleGroup.gameObject, textTogglePrefab);
            driveObject.GetComponent<Toggle>().group = driveToggleGroup;
        }

        foreach (var color in Common.colors) {
            var colorObject = CreateUIObject(color.localizationTableKey, colorsToggleGroup.gameObject, colorTogglePrefab);
            colorObject.GetComponent<Toggle>().group = colorsToggleGroup;
            colorObject.GetComponentInChildren<Image>().color = Common.ColorFromHex(color.hex);
            colorObject.GetComponent<Toggle>().onValueChanged.AddListener((value) => OnColorChanged(value, color.hex, carColorChanger));
        }

        foreach (var rim in Common.rims) {
            var rimObject = CreateUIObject(rim.localizationTableKey, rimsToggleGroup.gameObject, colorTogglePrefab);
            rimObject.GetComponent<Toggle>().group = rimsToggleGroup;
            rimObject.GetComponentInChildren<Image>().color = Common.ColorFromHex(rim.hex);
            rimObject.GetComponent<Toggle>().onValueChanged.AddListener((value) => OnColorChanged(value, rim.hex, rimsColorChanger));
        }

        /* let toggle group know that toggles were made dynamically */
        versionToggleGroupBehaviour.InitializeToggles();
        driveToggleGroupBehaviour.InitializeToggles();
        colorsToggleGroupBehaviour.InitializeToggles();
        rimsToggleGroupBehaviour.InitializeToggles();

        versionToggleGroupBehaviour.onToggleChanged += RefreshData;
        LocaleSelector.onLanguageChanged += RefreshData;

        foreach (var package in Common.packages) {
            var packageObject = CreateUIObject(package.Value, packageList, textTogglePrefab);
            packages.Add(packageObject.GetComponent<Toggle>());
        }
    }

    void OnColorChanged(bool value, string colorHex, ColorChanger colorChanger) {
        if (value) {
            colorChanger.ChangeElementsColor(colorHex);
        }
    }

    void SetCurrentVersion(GameObject versionToggle) {
        currentVersion = versions[versionToggle];
    }

    VersionData GetSOForVersion(Common.eVersion version) {
        if (version == Common.eVersion.REACT) {
            return reactVersionData;
        }
        else if (version == Common.eVersion.SHARP) {
            return sharpVersionData;
        }
        else if (version == Common.eVersion.UNITY) {
            return unityVersionData;
        }
        return null;
    }

    IEnumerator ChangeDescription() {
        isCoroutineActive = true;
        string localizationTableKey = currentVersion.description;
        string description = String.Empty;
        var operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(Common.localizationTableName,
                                                            localizationTableKey, new object[] { description });

        yield return operation;

        if (operation.Status == AsyncOperationStatus.Succeeded) {
            versionDescription.text = operation.Result;
        }
        else {
            Debug.Log("Failed to get localized description!");
        }

        isCoroutineActive = false;
        StartCoroutine(RefreshLayout());
    }

    IEnumerator RefreshLayout() {
        isCoroutineActive = true;
        yield return new WaitForSeconds(0.01f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transformToRefresh);
        isCoroutineActive = false;
    }

    void OnDisable() {
        carColorChanger.ChangeElementsColorToDefault();
        rimsColorChanger.ChangeElementsColorToDefault();
    }

    void OnDestroy() {
        versionToggleGroupBehaviour.onToggleChanged -= RefreshData;
        LocaleSelector.onLanguageChanged -= RefreshData;
    }
}
