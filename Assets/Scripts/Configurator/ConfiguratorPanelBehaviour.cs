using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ConfiguratorPanelBehaviour : MonoBehaviour {
    [SerializeField] RectTransform transformToRefresh;
    [SerializeField] ToggleGroup versionToggleGroup;
    [SerializeField] TextMeshProUGUI versionDescription;
    ToggleGroupBehaviour versionToggleGroupBehaviour;
    Common.eVersion currentVersion;
    bool isCoroutineActive = false;

    // problems with OnEnable when programmer left this panel as active
    void OnEnable() {
        versionToggleGroupBehaviour = versionToggleGroup.GetComponent<ToggleGroupBehaviour>();
        versionToggleGroupBehaviour.onToggleChanged += RefreshDescriptions;
        LocaleSelector.onLanguageChanged += RefreshDescriptions;
        RefreshDescriptions();
    }

    void RefreshDescriptions() {
        if (isCoroutineActive) {
            return;
        }

        int toggleIndex = versionToggleGroupBehaviour.GetSelectedToggleIndex();
        if (toggleIndex < 0) {
            return;
        }

        currentVersion = (Common.eVersion)toggleIndex;
        StartCoroutine(ChangeDescription(currentVersion));
    }

    IEnumerator ChangeDescription(Common.eVersion version) {
        isCoroutineActive = true;
        string localizationTableKey = Common.descriptions[version];
        string description = String.Empty;
        var operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(Common.localizationTableName, localizationTableKey, new object[] { description });

        yield return operation;

        if (operation.Status == AsyncOperationStatus.Succeeded) {
            versionDescription.text = operation.Result;
        }
        else {
            Debug.Log("Failed to get localized description!");
        }

        // low effort fix for proper layout :/
        yield return new WaitForSeconds(0.01f);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transformToRefresh);
        isCoroutineActive = false;
    }

    void OnDestroy() {
        versionToggleGroupBehaviour.onToggleChanged -= RefreshDescriptions;
        LocaleSelector.onLanguageChanged -= RefreshDescriptions;
    }

}

