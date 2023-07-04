using UnityEngine;
using UnityEngine.UI;

public class SelectionMenuView : MonoBehaviour {
    Button defaultButton;
    Button[] buttons;
    Button selectedButton;
    [SerializeField] Color deselectedBttnColor = Color.white;
    [SerializeField] Color selectedBttnColor;

    public void SelectDefaultButton() {
        defaultButton.Select();
        OnSelect(defaultButton);
    }

    void Awake() {
        buttons = GetComponentsInChildren<Button>();
        defaultButton = buttons[0];
        foreach (Button button in buttons) {
            button.onClick.AddListener(() => OnSelect(button));
        }
        SelectDefaultButton();
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
