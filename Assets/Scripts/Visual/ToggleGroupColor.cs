using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupColor : MonoBehaviour {
    [SerializeField] Color toggleOnColor = Color.white;
    [SerializeField] Color toggleOffColor;
    Toggle defaultToggle;

    public void SelectDefaultToggle() {
        defaultToggle.isOn = true;
    }

    void Awake() {
        Toggle[] toggles = GetComponentsInChildren<Toggle>();
        foreach (Toggle toggle in toggles) {
            toggle.onValueChanged.AddListener((bool value) => OnToggleValueChanged(toggle, value));
        }
        defaultToggle = toggles[0];
        SelectDefaultToggle();
    }

    void OnToggleValueChanged(Toggle toggle, bool value) {
        ColorBlock colorBlock;
        colorBlock = toggle.colors;
        Icon icon = toggle.GetComponentInChildren<Icon>();
        if (value) {
            colorBlock.normalColor = toggleOffColor;
            icon.ChangeToAlternativeColor();
        }
        else {
            colorBlock.normalColor = toggleOnColor;
            icon.ChangeToDefaultColor();
        }
        toggle.colors = colorBlock;
        toggle.interactable = !value;
    }

}
