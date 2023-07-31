using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using static Common;

public class PanelManager : MonoBehaviour {
    static public PanelManager Instance;

    [SerializeField] InteractionController interactionController;
    [SerializeField] List<Panel> panels;
    [SerializeField] GameObject popup;
    [SerializeField] GameObject inputFieldPopup;
    [SerializeField] PostProcessingEffectsModifier backgroundEffects;

    Panel currentPanel;
    PopupController popupController;
    PopupController inputFieldPopupController;

    public event Action onPanelOpened;
    public event Action onPanelClosed;

    public void ShowPanel(Panel panel) {
        bool exclusiveVisibility = panel.ExclusiveVisibility();
        if (exclusiveVisibility) {
            if (currentPanel == panel) {
                return;
            }
            HideCurrentPanel();
            currentPanel = panel;
        }
        panel.Show();
        onPanelOpened?.Invoke();
    }

    public void HidePanel(Panel panel) {
        panel.Hide();
        onPanelClosed?.Invoke();
    }

    public async UniTask<bool> ShowPopup(ePopupType popupType, string text) {
        PopupController popup;
        if (popupType == ePopupType.INPUT_FIELD) {
            popup = inputFieldPopupController;
        } else {
            popup = popupController;
        }
        TurnOnBackgroundEffects();
        interactionController.Block();
        popup.Show();
        popup.SetTextToDisplay(text);
        bool mainPopupClicked = await popup.WaitForCloseAsync();
        HidePopup(popup);
        return mainPopupClicked;
    }

    public void SetPopupDefaultInput(string text) {
        inputFieldPopupController.SetInputFieldPlaceholder(text);
    }

    public string GetUserPopupInput() {
        return inputFieldPopupController.GetUserInput();
    }

    public void HidePopup(PopupController popup) {
        TurnOffBackgroundEffects();
        interactionController.Enable();
        popup.Hide();
    }

    public void HideCurrentPanel() {
        if (currentPanel == null) {
            return;
        }
        HidePanel(currentPanel);
        currentPanel = null;
    }

    public bool IsPanelShown(GameObject obj) {
        if (currentPanel == null) {
            return false;
        }
        else if (obj == currentPanel.gameObject) {
            return true;
        }
        return false;
    }

    public void TurnOnBackgroundEffects() {
        backgroundEffects.TurnOnBlurryDarkBackground();
    }

    public void TurnOffBackgroundEffects() {
        backgroundEffects.TurnOffBlurryDarkBackground();
    }

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }

        popupController = popup.GetComponent<PopupController>();
        popupController.InitializePopup();
        inputFieldPopupController = inputFieldPopup.GetComponent<PopupController>();
        inputFieldPopupController.InitializePopup();

        foreach (var panel in panels) {
            panel.PanelAwake();
            if (panel.IsHiddenOnAwake()) {
                panel.Hide();
            }
        }
    }
}