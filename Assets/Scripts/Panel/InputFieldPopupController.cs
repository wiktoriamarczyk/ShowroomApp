using TMPro;

public class InputFieldPopupController : PopupController {
    TMP_InputField inputField;
    string placeholderText;

    public void SetInputFieldPlaceholder(string text) {
        TextMeshProUGUI placeholder = inputField?.placeholder.GetComponent<TextMeshProUGUI>();
        if (placeholder == null) {
            return;
        }
        placeholder.text = text;
        placeholderText = text;
    }

    public string GetUserInput() {
        string result = string.Empty;
        if (inputField != null) {
            result = inputField.text;
        }
        return result;
    }

    public void OnEnable() {
        SetInputFieldPlaceholder(placeholderText);
    }

    public override void PopupAwake() {
        base.PopupAwake();
        inputField = GetComponentInChildren<TMP_InputField>();
        inputField?.onValueChanged.AddListener(OnInputFieldValueChanged);
    }

    void OnInputFieldValueChanged(string text) {
        if (text.Length > Common.maxInputLength) {
            return;
        }
    }
}
