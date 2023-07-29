using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Localization.Editor;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class PopupController : MonoBehaviour {
    [SerializeField] Button mainButton;
    [SerializeField] Button altButton;
    [SerializeField] TextMeshProUGUI textToDisplay;

    Panel panel;
    UniTaskCompletionSource<bool> popupTask;
    LocalizeStringEvent localization;
    TMP_InputField inputField;

    public void OnEnable() {
        if (inputField != null) {
            inputField.text = inputField.placeholder.GetComponent<TextMeshProUGUI>().text;
        }
    }

    public void Show() {
        panel.Show();
    }

    public void Hide() {
        panel.Hide();
    }

    public UniTask<bool> WaitForCloseAsync() {
        popupTask = new UniTaskCompletionSource<bool>();
        return popupTask.Task;
    }

    public void SetTextToDisplay(string tableEntryReference) {
        if (localization != null) {
            localization.OnUpdateString.AddListener((string value) => { textToDisplay.text = value; });
            localization.StringReference.TableReference = Common.localizationTableName;
            localization.StringReference.TableEntryReference = tableEntryReference;
        }
    }

    public void SetInputFieldPlaceholder(string text) {
        TextMeshProUGUI placeholder = inputField.placeholder.GetComponent<TextMeshProUGUI>();
        if (placeholder == null) {
            return;
        }
        placeholder.text = text;
    }

    public string GetUserInput() {
        string result = string.Empty;
        if (inputField != null) {
            result = inputField.text;
        }
        return result;
    }

   public void InitializePopup() {
        panel = GetComponent<Panel>();
        mainButton.onClick.AddListener(() => OnButtonClick(true));
        altButton.onClick.AddListener(() => OnButtonClick(false));
        localization = textToDisplay.GetComponent<LocalizeStringEvent>();
        inputField = GetComponentInChildren<TMP_InputField>();
        inputField?.onValueChanged.AddListener(OnInputFieldUpdate);
    }

    void OnInputFieldUpdate(string text) {
        if (text == String.Empty) {
            inputField.text = inputField.placeholder.GetComponent<TextMeshProUGUI>().text;
        }
    }

    void OnButtonClick(bool value) {
        popupTask.TrySetResult(value);
    }
}
