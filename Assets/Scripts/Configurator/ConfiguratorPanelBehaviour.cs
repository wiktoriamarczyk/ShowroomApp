using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Linq;

public class ConfiguratorPanelBehaviour : MonoBehaviour {
    [SerializeField] RectTransform transformToRefresh;
    [SerializeField] ReactVersionData reactVersionData;
    [SerializeField] SharpVersionData sharpVersionData;
    [SerializeField] UnityVersionData unityVersionData;

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
    bool isCoroutineActive = false;
    int initializedTogglesCounter;

    const int togglesToInitialize = 3;

    // problems with OnEnable when programmer left this panel as active
    void OnEnable() {
        versionToggleGroupBehaviour = versionToggleGroup.GetComponent<ToggleGroupBehaviour>();
        driveToggleGroupBehaviour = driveToggleGroup.GetComponent<ToggleGroupBehaviour>();
        colorsToggleGroupBehaviour = colorsToggleGroup.GetComponent<ToggleGroupBehaviour>();
        rimsToggleGroupBehaviour = rimsToggleGroup.GetComponent<ToggleGroupBehaviour>();

        driveToggleGroupBehaviour.onToggleGroupInitialized += InitializeToggle;
        colorsToggleGroupBehaviour.onToggleGroupInitialized += InitializeToggle;
        rimsToggleGroupBehaviour.onToggleGroupInitialized += InitializeToggle;

        versionToggleGroupBehaviour.onToggleChanged += RefreshData;
        LocaleSelector.onLanguageChanged += RefreshData;

        RefreshData();
    }

    void InitializeToggle() {
        initializedTogglesCounter++;
        if (initializedTogglesCounter == togglesToInitialize) {
            InitializeAllToggles();
            ActivateTogglesAccordingToVersion();
        }
    }

    // Set toggles correct name
    void InitializeAllToggles() {
        Toggle[] driveToggles = driveToggleGroupBehaviour.GetToggles();
        for (int i = 0; i < driveToggles.Length; ++i) {
            driveToggles[i].gameObject.name = Common.drives[(Common.eDrive)i];
            driveToggles[i].gameObject.SetActive(false);
        }

        Toggle[] colorToggles = colorsToggleGroupBehaviour.GetToggles();
        for (int i = 0; i < colorToggles.Length; ++i) {
            colorToggles[i].gameObject.name = Common.colors[i].localizationTableKey;
            colorToggles[i].gameObject.SetActive(false);
        }

        Toggle[] rimToggles = rimsToggleGroupBehaviour.GetToggles();
        for (int i = 0; i < rimToggles.Length; ++i) {
            rimToggles[i].gameObject.name = Common.rims[i].localizationTableKey;
            rimToggles[i].gameObject.SetActive(false);
        }
    }

    void ActivateTogglesAccordingToVersion() {
        if (initializedTogglesCounter != togglesToInitialize) {
            return;
        }
        Toggle[] driveToggles = driveToggleGroupBehaviour.GetToggles();
        foreach (Toggle toggle in driveToggles) {
            if (currentVersion.drives.Contains(toggle.gameObject.name)) {
                toggle.gameObject.SetActive(true);
            } else {
                toggle.gameObject.SetActive(false);
            }
        }

        Toggle[] colorToggles = colorsToggleGroupBehaviour.GetToggles();
        foreach (Toggle toggle in colorToggles) {
            if (currentVersion.colorsData.ContainsKey(toggle.gameObject.name)) {
                toggle.gameObject.SetActive(true);
            }
            else {
                toggle.gameObject.SetActive(false);
            }
        }

        Toggle[] rimToggles = rimsToggleGroupBehaviour.GetToggles();
        foreach (Toggle toggle in rimToggles) {
            if (currentVersion.rimsData.ContainsKey(toggle.gameObject.name)) {
                toggle.gameObject.SetActive(true);
            }
            else {
                toggle.gameObject.SetActive(false);
            }
        }
        StartCoroutine(RefreshLayout());
    }

    void RefreshData() {
        if (isCoroutineActive) {
            return;
        }

        int toggleIndex = versionToggleGroupBehaviour.GetSelectedToggleIndex();
        if (toggleIndex < 0) {
            return;
        }

        SetCurrentVersion((Common.eVersion)toggleIndex);
        StartCoroutine(ChangeDescription());
        ActivateTogglesAccordingToVersion();
        StartCoroutine(RefreshLayout());
    }

    void SetCurrentVersion(Common.eVersion version) {
        if (version == Common.eVersion.REACT) {
            currentVersion = reactVersionData;
        }
        else if (version == Common.eVersion.SHARP) {
            currentVersion = sharpVersionData;
        }
        else if (version == Common.eVersion.UNITY) {
            currentVersion = unityVersionData;
        }
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
        // low effort fix for proper layout :/
        yield return new WaitForSeconds(0.01f);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transformToRefresh);
        isCoroutineActive = false;
    }

    void OnDestroy() {
        versionToggleGroupBehaviour.onToggleChanged -= RefreshData;
        LocaleSelector.onLanguageChanged -= RefreshData;
    }
}
