using TMPro;
using Unity.VisualScripting;

public class InputFieldPopupController : PopupController {
    TMP_InputField inputField;
    string placeholderText;
    

    public void Init(string text, string placeholderText) {
        base.Init(text);
        SetInputFieldPlaceholder(placeholderText);
    }

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

    public override void Awake() {
        base.Awake();
        inputField = GetComponentInChildren<TMP_InputField>();
        inputField?.onValueChanged.AddListener(OnInputFieldValueChanged);
    }

    void OnInputFieldValueChanged(string text) {
        if (text.Length > Common.maxInputLength) {
            return;
        }
    }
}
