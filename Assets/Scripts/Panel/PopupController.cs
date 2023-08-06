using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class PopupController : MonoBehaviour {
    [SerializeField] Button mainButton;
    [SerializeField] Button altButton;
    [SerializeField] TextMeshProUGUI textToDisplay;
    [SerializeField] Common.ePopupType type;

    Panel panel;
    UniTaskCompletionSource<bool> popupTask;
    LocalizeStringEvent localization;

    public void Show() {
        panel.Show();
    }

    public void Hide() {
        panel.Hide();
    }

    public Common.ePopupType GetPopupType() {
        return type;
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

   public virtual void PopupAwake() {
        panel = GetComponent<Panel>();
        mainButton.onClick.AddListener(() => OnButtonClick(true));
        altButton.onClick.AddListener(() => OnButtonClick(false));
        localization = textToDisplay.GetComponent<LocalizeStringEvent>();
    }

    void OnButtonClick(bool value) {
        popupTask.TrySetResult(value);
    }
}
