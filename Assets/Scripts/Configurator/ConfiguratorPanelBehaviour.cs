using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using System.Collections.Generic;
using System.Linq;
using static Common;
using UnityEngine.Events;

public class ConfiguratorPanelBehaviour : MonoBehaviour {
    [SerializeField] ColorChanger carColorChanger;
    [SerializeField] ColorChanger rimsColorChanger;
    [SerializeField] ColorChanger seamsColorChanger;

    [SerializeField] RectTransform transformToRefresh;
    [SerializeField] TextMeshProUGUI versionDescription;

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
    Dictionary<GameObject, VersionData> versions = new Dictionary<GameObject, VersionData>();
    List<Toggle> packages = new List<Toggle>();
    VersionData currentVersion;
    List<GameObject> configObjects = new List<GameObject>();
    bool isCoroutineActive = false;
    bool isPanelInitialized = false;

    public void SelectConfigurations(ConfigData config) {
        SelectToggles(versionToggleGroupBehaviour, config.version);
        SetCurrentVersion(versionToggleGroupBehaviour.GetSelectedToggle().gameObject);

        RefreshData();

        SelectToggles(driveToggleGroupBehaviour, config.drive);
        SelectToggles(colorsToggleGroupBehaviour, config.color);
        SelectToggles(rimsToggleGroupBehaviour, config.rims);

        foreach (var package in packages) {
            package.isOn = config.packages.Contains(package.name);
        }
    }

    public List<string> GetSelectedConfigurations() {
        var selectedPackages = packages.Where(toggle => toggle.isOn).ToList();
        int typesCount = (int)Common.eConfigurationType.PACKAGE;
        typesCount += selectedPackages.Count();

        string[] selectedToggles = new string[typesCount];
        selectedToggles[(int)eConfigurationType.VERSION] = versionToggleGroupBehaviour.GetSelectedToggle().name;
        selectedToggles[(int)eConfigurationType.DRIVE] = driveToggleGroupBehaviour.GetSelectedToggle().name;
        selectedToggles[(int)eConfigurationType.COLOR] = colorsToggleGroupBehaviour.GetSelectedToggle().name;
        selectedToggles[(int)eConfigurationType.RIMS] = rimsToggleGroupBehaviour.GetSelectedToggle().name;

        for (int i = 0; i < selectedPackages.Count(); i++) {
            selectedToggles[(int)eConfigurationType.PACKAGE + i] = selectedPackages[i].name;
        }

        return selectedToggles.ToList();
    }

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
        isPanelInitialized = true;
        RefreshData();
    }

    void SelectToggles(ToggleGroupBehaviour toggleGroup, string nameToSelect) {
        var toggles = toggleGroup.GetToggles();

        foreach (var toggle in toggles) {
            if (toggle.name == nameToSelect) {
                toggleGroup.SelectToggle(toggle);
                break;
            }
        }
    }

    void ActivateToggles<T>(List<Toggle> toggles, List<T> dataCollection) where T : LocalizableData {
        foreach (Toggle toggle in toggles) {
            string toggleName = toggle.gameObject.name;

            bool isActive = false; // dataCollection.Find(data => data.localizationTableKeyProperty == toggleName) != null;
            ToggleWithContext toggleWithContext = toggle as ToggleWithContext;
            if (toggleWithContext != null) {
                isActive = dataCollection.Find(data => data == toggleWithContext.context) != null;
            }

            toggle.gameObject.SetActive(isActive);
        }
    }

    void ActivateObjects() {
        ActivateToggles(driveToggleGroupBehaviour.GetToggles(), currentVersion.drivesProperty);
        ActivateToggles(colorsToggleGroupBehaviour.GetToggles(), currentVersion.colorsDataProperty);
        ActivateToggles(rimsToggleGroupBehaviour.GetToggles(), currentVersion.rimsDataProperty);
        ActivateToggles(packages, currentVersion.packagesProperty);
    }

    async void RefreshData() {
        if (!gameObject.activeSelf) {
            return;
        }
        if (isCoroutineActive) {
            StopAllCoroutines();
        }
        SetCurrentVersion(versionToggleGroupBehaviour.GetSelectedToggle().gameObject);
        versionDescription.text = await Common.GetLocalizationEntry(currentVersion.localizationTableKeyProperty);
        ActivateObjects();
        RefreshSelectedToggles();
        StartCoroutine(RefreshLayout());
    }

    void RefreshSelectedToggles() {
        driveToggleGroupBehaviour.OnToggleStatusChanged();
        colorsToggleGroupBehaviour.OnToggleStatusChanged();
        rimsToggleGroupBehaviour.OnToggleStatusChanged();
    }

    GameObject CreateUIObject(string name, GameObject parent, GameObject prefab, Object context) {
        GameObject ui = Instantiate(prefab);
        ui.name = name;
        ui.transform.SetParent(parent.transform);
        ui.transform.localScale = Vector3.one;
        ToggleWithContext toggleWithContext = ui.GetComponent<ToggleWithContext>();
        if (toggleWithContext != null) {
            toggleWithContext.context = context;
        }
        var localization = ui.GetComponentInChildren<LocalizeStringEvent>();
        var textMeshPro = ui.GetComponentInChildren<TextMeshProUGUI>();
        if (localization != null) {
            localization.OnUpdateString.AddListener((string value) => { textMeshPro.text = value; });
            localization.StringReference.TableReference = Common.localizationTableName;
            localization.StringReference.TableEntryReference = name;
        }
        else if (textMeshPro != null && string.IsNullOrEmpty(textMeshPro.text)) {
            textMeshPro.text = name;
        }
        configObjects.Add(ui);
        return ui;
    }

    void InitializeColorToggles(IReadOnlyList<PaintColorData> list, ToggleGroup toggleGroup, GameObject togglePrefab, UnityAction<bool,PaintColorData> callback) {
        foreach (var element in list) {
            var colorObject = CreateUIObject(element.localizationTableKeyProperty, toggleGroup.gameObject, togglePrefab, element);
            Toggle toggleComponent = colorObject.GetComponent<Toggle>();
            Image image = colorObject.GetComponentInChildren<Image>();
            toggleComponent.group = toggleGroup;
            image.color = element.colorProperty;
            toggleComponent.onValueChanged.AddListener((value) => callback(value, element) );
            colorObject.GetComponentInChildren<OptionalIndicatorController>().SetOptionalIndicatorVisibility(element.optionalProperty);
        }
    }

    void InitializeUIObjects() {
        foreach (var version in Common.versionsData) {
            GameObject versionObject = CreateUIObject(version.localizationTableKeyProperty, versionToggleGroup.gameObject, versionTogglePrefab, version);
            versionObject.GetComponent<Toggle>().group = versionToggleGroup;
            versions.Add(versionObject, GetSOForVersion(version.versionType));
        }

        foreach (var drive in Common.drivesData) {
            GameObject driveObject = CreateUIObject(drive.localizationTableKeyProperty, driveToggleGroup.gameObject, textTogglePrefab, drive);
            driveObject.GetComponent<Toggle>().group = driveToggleGroup;
        }

        InitializeColorToggles(Common.bodyPaintColorsData, colorsToggleGroup, colorTogglePrefab, OnCarColorChanged );
        InitializeColorToggles(Common.rimsPaintColorsData, rimsToggleGroup, colorTogglePrefab, OnRimsColorChanged);


        /* let toggle group know that toggles were made dynamically */
        versionToggleGroupBehaviour.InitializeToggles();
        driveToggleGroupBehaviour.InitializeToggles();
        colorsToggleGroupBehaviour.InitializeToggles();
        rimsToggleGroupBehaviour.InitializeToggles();

        versionToggleGroupBehaviour.onToggleChanged += RefreshData;
        LocaleSelector.onLanguageChanged += RefreshData;

        foreach (var package in Common.packagesData) {
            var packageObject = CreateUIObject(package.localizationTableKeyProperty, packageList, textTogglePrefab, package);
            packages.Add(packageObject.GetComponent<Toggle>());
        }
    }

    void OnCarColorChanged(bool value, PaintColorData color) {
        if (value) {
            carColorChanger.ChangeElementsColor(color);
            BodyPaintColorData bodyPaint = color as BodyPaintColorData;
            if (bodyPaint != null && bodyPaint.seamsColorProperty != null) {
                seamsColorChanger.ChangeElementsColor(bodyPaint.seamsColorProperty);
            }

            UpdateColorMatch(color.colorProperty);
            if (rimsToggleGroupBehaviour.GetSelectedToggle().name == FindRimsColorByType(eColor.COLORMATCH)?.localizationTableKeyProperty) {
                OnRimsColorChanged(true, color);
            }
        }
    }

    void UpdateColorMatch(Color color) {
        PaintColorData colorMatchRims = FindRimsColorByType(eColor.COLORMATCH);
        colorMatchRims?.SetColor(color);

        GameObject foundObject = configObjects.Find(obj => obj.name == FindRimsColorByType(eColor.COLORMATCH)?.localizationTableKeyProperty);
        if (foundObject != null) {
            foundObject.GetComponentInChildren<Image>().color = color;
        }
    }

    void OnRimsColorChanged(bool value, PaintColorData color) {
        if (value) {
            rimsColorChanger.ChangeElementsColor(color);
        }
    }

    void SetCurrentVersion(GameObject versionToggle) {
        currentVersion = versions[versionToggle];
    }

    VersionData GetSOForVersion(Common.eVersion version) {
        foreach (var versionData in Common.versionsData) {
            if (version == versionData.versionType) {
                return versionData;
            }
        }
        return null;
    }

    IEnumerator RefreshLayout() {
        isCoroutineActive = true;
        yield return new WaitForSeconds(0.01f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transformToRefresh);
        isCoroutineActive = false;
    }

    void OnDisable() {
        //carColorChanger.ChangeElementsColorToDefault();
        //rimsColorChanger.ChangeElementsColorToDefault();

        foreach (var package in packages) {
            package.isOn = false;
        }
    }

    void OnDestroy() {
        versionToggleGroupBehaviour.onToggleChanged -= RefreshData;
        LocaleSelector.onLanguageChanged -= RefreshData;

        foreach(var obj in configObjects) {
            Destroy(obj);
        }
    }
}
