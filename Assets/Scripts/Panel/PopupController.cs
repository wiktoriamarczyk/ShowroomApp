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

    public string GetUserInput() {
        string result = string.Empty;
        var inputField = GetComponent<TMP_InputField>();
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
        Hide();
    }

    void OnButtonClick(bool value) {
        popupTask.TrySetResult(value);
    }
}
