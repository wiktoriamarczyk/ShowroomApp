using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Localization.Components;
using System.Collections.Generic;
using static Common;
using Unity.VisualScripting;
using System.Linq;

public class ConfiguratorPanelBehaviour : MonoBehaviour {
    [SerializeField] RectTransform transformToRefresh;
    [SerializeField] ReactVersionData reactVersionData;
    [SerializeField] SharpVersionData sharpVersionData;
    [SerializeField] UnityVersionData unityVersionData;

    [SerializeField] GameObject textTogglePrefab;
    [SerializeField] GameObject versionTogglePrefab;
    [SerializeField] GameObject colorTogglePrefab;

    [SerializeField] ToggleGroup versionToggleGroup;
    [SerializeField] ToggleGroup driveToggleGroup;
    [SerializeField] ToggleGroup colorsToggleGroup;
    [SerializeField] ToggleGroup rimsToggleGroup;
    [SerializeField] GameObject packages;
    [SerializeField] TextMeshProUGUI versionDescription;

    ToggleGroupBehaviour versionToggleGroupBehaviour;
    ToggleGroupBehaviour driveToggleGroupBehaviour;
    ToggleGroupBehaviour colorsToggleGroupBehaviour;
    ToggleGroupBehaviour rimsToggleGroupBehaviour;

    VersionData currentVersion;
    Dictionary<GameObject, VersionData> versions = new Dictionary<GameObject, VersionData>();
    bool isCoroutineActive = false;

    void Start() {
        versionToggleGroupBehaviour = versionToggleGroup.GetComponent<ToggleGroupBehaviour>();
        driveToggleGroupBehaviour = driveToggleGroup.GetComponent<ToggleGroupBehaviour>();
        colorsToggleGroupBehaviour = colorsToggleGroup.GetComponent<ToggleGroupBehaviour>();
        rimsToggleGroupBehaviour = rimsToggleGroup.GetComponent<ToggleGroupBehaviour>();

        CreateUIElements();
        RefreshData();
    }

    GameObject CreateUIElement(string name, MonoBehaviour parent, GameObject prefab) {
        GameObject ui = Instantiate(prefab);
        ui.name = name;
        ui.transform.SetParent(parent.transform);
        ui.transform.localScale = Vector3.one;
        var toggleGroup = parent as ToggleGroup;
        if (toggleGroup != null) {
            ui.GetComponent<Toggle>().group = toggleGroup;
        }
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

    void CreateUIElements() {
        foreach (var version in Common.versions) {
            GameObject versionObject = CreateUIElement(version.Value, versionToggleGroup, versionTogglePrefab);
            versions.Add(versionObject, GetSOForVersion(version.Key));
        }

        foreach (var drive in Common.drives) {
            CreateUIElement(drive.Value, driveToggleGroup, textTogglePrefab);
        }

        foreach (var color in Common.colors) {
            var colorToggle = CreateUIElement(color.localizationTableKey, colorsToggleGroup, colorTogglePrefab);
            colorToggle.GetComponentInChildren<Image>().color = Common.ColorFromHex(color.hex);
        }

        foreach (var rim in Common.rims) {
            CreateUIElement(rim.localizationTableKey, rimsToggleGroup, colorTogglePrefab);
        }
        versionToggleGroupBehaviour.InitializeToggles();
        driveToggleGroupBehaviour.InitializeToggles();
        colorsToggleGroupBehaviour.InitializeToggles();
        rimsToggleGroupBehaviour.InitializeToggles();

        versionToggleGroupBehaviour.onToggleChanged += RefreshData;
        LocaleSelector.onLanguageChanged += RefreshData;
    }

    public void ActivateToggles<T>(ToggleGroupBehaviour toggleGroup, T dataCollection) where T : class {
        Toggle[] toggles = toggleGroup.GetToggles();

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
    void ActivateElements() {
        ActivateToggles(driveToggleGroupBehaviour, currentVersion.drives);
        ActivateToggles(colorsToggleGroupBehaviour, currentVersion.colorsData);
        ActivateToggles(rimsToggleGroupBehaviour, currentVersion.rimsData);
    }

    void RefreshData() {
        if (isCoroutineActive) {
            return;
        }
        SetCurrentVersion(versionToggleGroupBehaviour.GetSelectedToggle().gameObject);
        StartCoroutine(ChangeDescription());
        ActivateElements();
        StartCoroutine(RefreshLayout());
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
        var operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(Common.localizationTableName, localizationTableKey, new object[] { description });

        yield return operation;

        if (operation.Status == AsyncOperationStatus.Succeeded) {
            versionDescription.text = operation.Result;
        }
        else {
            Debug.Log("Failed to get localized description!");
        }

        isCoroutineActive = false;
    }

    IEnumerator RefreshLayout() {
        isCoroutineActive = true;
        yield return new WaitForSeconds(0.01f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transformToRefresh);
        isCoroutineActive = false;
    }

    void OnDestroy() {
        versionToggleGroupBehaviour.onToggleChanged -= RefreshData;
        LocaleSelector.onLanguageChanged -= RefreshData;
    }
}
