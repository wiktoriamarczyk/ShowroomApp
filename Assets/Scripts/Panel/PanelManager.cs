using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using static Common;

public class PanelManager : MonoBehaviour {
    static public PanelManager Instance;

    [SerializeField] InteractionController interactionController;
    [SerializeField] List<Panel> panels;
    [SerializeField] List<PopupController> popups;
    [SerializeField] PostProcessingEffectsModifier backgroundEffects;

    Panel currentPanel;

    public event Action onPanelOpened;
    public event Action onPanelClosed;

    List<Panel> prevOpenedPanels = new List<Panel>();

    public void HideAllPanels() {
        prevOpenedPanels.Clear();
        foreach (Panel panel in panels) {
            if (IsPanelShown(panel.gameObject)) {
                prevOpenedPanels.Add(panel);
                HidePanel(panel);
            }
        }
    }

    public void ShowAllPrevOpenedPanels() {
        foreach (Panel panel in prevOpenedPanels) {
            ShowPanel(panel);
        }
    }

    public void ShowPanel(Panel panel) {
        bool exclusiveVisibility = panel.ExclusiveVisibility();
        if (exclusiveVisibility) {
            if (currentPanel == panel && IsPanelShown(panel.gameObject)) {
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

    public void HideCurrentPanel() {
        if (currentPanel == null) {
            return;
        }
        HidePanel(currentPanel);
        currentPanel = null;
    }

    T GetPopupByType<T>(ePopupType popupType) where T : class {
        foreach (var popup in popups) {
            if (popup.GetPopupType() == popupType) {
                return popup as T;
            }
        }
        return null;
    }

    public async UniTask<bool> ShowPopup(ePopupType popupType, string text) {
        PopupController selectedPopupController = GetPopupByType <PopupController>(popupType);
        if (selectedPopupController == null) {
            return false;
        }
        TurnOnBackgroundEffects();
        interactionController.Block();
        selectedPopupController.Show();
        selectedPopupController.SetTextToDisplay(text);
        bool mainPopupClicked = await selectedPopupController.WaitForCloseAsync();
        HidePopup(selectedPopupController);
        return mainPopupClicked;
    }

    public void SetPopupDefaultInput(string text) {
        GetPopupByType<PopupController>(Common.ePopupType.INPUT_FIELD)?.SetInputFieldPlaceholder(text);
    }

    public string GetUserPopupInput() {
        return GetPopupByType<PopupController>(Common.ePopupType.INPUT_FIELD)?.GetUserInput();
    }

    public void HidePopup(PopupController popup) {
        TurnOffBackgroundEffects();
        interactionController.Enable();
        popup.Hide();
    }

    public bool IsPanelShown(GameObject obj) {
        Panel panel = obj.GetComponent<Panel>();
        if (panel != null) {
            return panel.IsPanelShown();
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

        foreach (var popup in popups) {
            popup.PopupAwake();
        }

        foreach (var panel in panels) {
            panel.PanelAwake();
            if (panel.IsHiddenOnAwake()) {
                panel.Hide();
            } else {
                panel.Show();
            }
        }
    }
}