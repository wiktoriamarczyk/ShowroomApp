using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Cysharp.Threading.Tasks;

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

    public async UniTask<bool> ShowPopup(string text) {
        popupController.InitializePopup();
        TurnOnBackgroundEffects();
        interactionController.Block();
        popupController.Show();
        popupController.SetTextToDisplay(text);
        bool mainPopupClicked = await popupController.WaitForCloseAsync();
        HidePopup();
        return mainPopupClicked;
    }

    public void HidePopup() {
        TurnOffBackgroundEffects();
        interactionController.Enable();
        popupController.Hide();
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
        inputFieldPopupController = inputFieldPopup.GetComponent<PopupController>();

        foreach (var panel in panels) {
            panel.PanelAwake();
            if (panel.IsHiddenOnAwake()) {
                panel.Hide();
            }
        }
    }
}