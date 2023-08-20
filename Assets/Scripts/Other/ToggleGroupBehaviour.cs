using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupBehaviour : MonoBehaviour {
    Toggle defaultToggle;
    Toggle selectedToggle;
    Toggle[] toggles;
    bool isInitialized = false;

    public event Action onToggleChanged;
    public event Action onToggleGroupInitialized;

    public Toggle GetSelectedToggle() {
        return selectedToggle;
    }

    public List<Toggle> GetToggles() {
        return toggles.ToList();
    }

    public void SelectDefaultToggle() {
        if (defaultToggle != null) {
            SelectToggle(defaultToggle);
        }
    }

    public void SelectToggle(Toggle toggle) {
        if (selectedToggle != null) {
            selectedToggle.isOn = false;
        }
        selectedToggle = toggle;
        selectedToggle.isOn = true;
    }

    public void OnToggleStatusChanged() {
        if (!selectedToggle.gameObject.activeSelf) {
            selectedToggle.isOn = false;
            SelectFirstActiveToggle();
        }
    }

    public void InitializeToggles() {
        if (isInitialized) {
            return;
        }
        isInitialized = true;
        toggles = GetComponentsInChildren<Toggle>();
        if (toggles == null || toggles.Length < 1) {
            return;
        }
        foreach (Toggle toggle in toggles) {
            toggle.isOn = false;
            toggle.onValueChanged.AddListener((bool value) => OnToggleValueChanged(toggle, value));
        }
        defaultToggle = toggles[0];
        OnToggleValueChanged(defaultToggle, true);
        onToggleGroupInitialized?.Invoke();
    }

    void OnToggleValueChanged(Toggle toggle, bool value) {
        if (value) {
            selectedToggle = toggle;
            onToggleChanged?.Invoke();
        }
    }

    void SelectFirstActiveToggle() {
        foreach (Toggle toggle in toggles) {
            if (toggle.gameObject.activeSelf) {
                SelectToggle(toggle);
                break;
            }
        }
    }

    void Start() {
        /* Call initialize only when at least one toggle exists.
         * This occurs when toggles are defined statically. */
        Toggle[] currentToggles = GetComponentsInChildren<Toggle>();
        if (currentToggles != null && currentToggles.Length >= 1) {
            InitializeToggles();
        }
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