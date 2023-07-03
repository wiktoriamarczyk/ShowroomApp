using UnityEngine;
using UnityEngine.UI;

public class ComboBoxDisplayer : MonoBehaviour {
    Button[] buttons;
    Button selectedButton;
    [SerializeField] Color deselectedBttnColor = Color.white;
    [SerializeField] Color selectedBttnColor;

    void Awake() {
        buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons) {
            button.onClick.AddListener(() => OnSelect(button));
        }
        buttons[0].Select();
        OnSelect(buttons[0]);
    }

    void OnSelect(Button button) {
        Icon[] icons;
        if (selectedButton != null) {
            SetButtonNormalColor(selectedButton, deselectedBttnColor);
            icons = selectedButton.GetComponentsInChildren<Icon>();
            icons[0].ChangeToDefaultColor();
        }
        SetButtonNormalColor(button, selectedBttnColor);
        icons = button.GetComponentsInChildren<Icon>();
        icons[0].ChangeToAlternativeColor();
        selectedButton = button;
    }

    void SetButtonNormalColor(Button button, Color color) {
        ColorBlock colorBlock;
        colorBlock = button.colors;
        colorBlock.normalColor = color;
        button.colors = colorBlock;
    }

    void OnDestroy() {
        foreach (Button button in buttons) {
             button.onClick.RemoveAllListeners();
        }
    }
}
