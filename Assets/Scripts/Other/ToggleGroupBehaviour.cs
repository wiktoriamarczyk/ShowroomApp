using System;
using System.Linq;
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

    public void InitializeToggles() {
        toggles = GetComponentsInChildren<Toggle>();
        foreach (Toggle toggle in toggles) {
            toggle.onValueChanged.AddListener((bool value) => OnToggleValueChanged(toggle, value));
        }
        defaultToggle = toggles[0];
        SelectDefaultToggle();
        onToggleGroupInitialized?.Invoke();
    }

    void OnEnable() {
        toggles = GetComponentsInChildren<Toggle>();
        if (toggles == null || toggles.Length < 1) {
            return;
        }
        else {
            InitializeToggles();
        }
    }

    void SelectDefaultToggle() {
        defaultToggle.isOn = true;
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
}