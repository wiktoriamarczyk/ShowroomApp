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

    public Toggle GetSelectedToggle() {
        return selectedToggle;
    }

    public int GetSelectedToggleIndex() {
        for (int i = 0; i < toggles.Count(); ++i) {
            if (selectedToggle == toggles[i]) {
                return i;
            }
        }
        return -1;
    }

    void SelectDefaultToggle() {
        defaultToggle.isOn = true;
    }

    void OnEnable() {
        toggles = GetComponentsInChildren<Toggle>();
        foreach (Toggle toggle in toggles) {
            toggle.onValueChanged.AddListener((bool value) => OnToggleValueChanged(toggle, value));
        }
        defaultToggle = toggles[0];
        SelectDefaultToggle();
    }

    void OnToggleValueChanged(Toggle toggle, bool value) {
        selectedToggle = toggle;
        ColorBlock colorBlock;
        colorBlock = toggle.colors;
        Icon icon = toggle.GetComponentInChildren<Icon>();
        if (value) {
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