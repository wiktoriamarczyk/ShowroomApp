using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupBehaviour : MonoBehaviour {
    [SerializeField] Color toggleOffColor;
    [SerializeField] Color toggleOnColor;
    Toggle defaultToggle;
    Toggle selectedToggle;
    Toggle[] toggles;

    public event Action onToggleChanged;
    public event Action onToggleGroupInitialized;

    public Toggle GetSelectedToggle() {
        return selectedToggle;
    }

    public Toggle[] GetToggles() {
        return toggles;
    }

    public void OnToggleStatusChanged() {
        if (!selectedToggle.gameObject.activeSelf) {
            selectedToggle.isOn = false;
            SelectFirstActiveToggle();
        }
    }

    void SelectFirstActiveToggle() {
        foreach (Toggle toggle in toggles) {
            if (toggle.gameObject.activeSelf) {
                selectedToggle = toggle;
                toggle.isOn = true;
                break;
            }
        }
    }

    public void InitializeToggles() {
        toggles = GetComponentsInChildren<Toggle>();
        foreach (Toggle toggle in toggles) {
            toggle.isOn = false;
            toggle.onValueChanged.AddListener((bool value) => OnToggleValueChanged(toggle, value));
        }
        defaultToggle = toggles[0];
        OnToggleValueChanged(defaultToggle, true);
        onToggleGroupInitialized?.Invoke();
    }

    void Start() {
        toggles = GetComponentsInChildren<Toggle>();
        if (toggles == null || toggles.Length < 1) {
            return;
        }
        else {
            InitializeToggles();
        }
    }

    void OnToggleValueChanged(Toggle toggle, bool value) {
        ColorBlock colorBlock;
        colorBlock = toggle.colors;
        Icon icon = toggle.GetComponentInChildren<Icon>();
        if (value) {
            selectedToggle = toggle;
            onToggleChanged?.Invoke();
            colorBlock.normalColor = toggleOnColor;
            icon?.ChangeToAlternativeColor();
        }
        else {
            colorBlock.normalColor = toggleOffColor;
            icon?.ChangeToDefaultColor();
        }
        toggle.colors = colorBlock;
        toggle.interactable = !value;
    }

    void OnEnable() {
        if (selectedToggle != null) {
            selectedToggle.isOn = true;
        }
    }

    void OnDisable() {
        foreach (Toggle toggle in toggles) {
            toggle.isOn = false;
        }
    }
}