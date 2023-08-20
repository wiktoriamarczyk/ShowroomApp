using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using static Common;
using System.Linq;
using UnityEngine.EventSystems;
using System.Collections;

public class PanelManager : MonoBehaviour {
    static public PanelManager instance { get; private set; }
    [SerializeField] GameObject panelContainer;
    [SerializeField] List<PopupController> popups;
    [SerializeField] PostProcessingEffectsModifier backgroundEffects;

    InteractionController interactionController;
    List<Panel> panels;
    List<Panel> prevOpenedPanels = new List<Panel>();
    Panel currentPanel;

    public event Action onPanelOpened;
    public event Action onPanelClosed;
    public event Action onPointerUp;

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

    //T GetPopupByType<T>(ePopupType popupType) where T : class {
    //    foreach (var popup in popups) {
    //        if (popup.GetPopupType() == popupType) {
    //            return popup as T;
    //        }
    //    }
    //    return null;
    //}


    T GetPopupPrefabByType<T>() where T : class {
        foreach (var popup in popups) {
            if (popup is T) {
                return popup as T;
            }
        }
        return null;
    }
    
    public async UniTask<PopupController.PopupShowResult<T>> ShowPopup2<T>(Action<T> InitFunc) where T : PopupController {
        PopupController.PopupShowResult<T> popupData;
        popupData.result = false;
        popupData.popupController = null;
        
        T selectedPopupPrefab = GetPopupPrefabByType<T>();
        if (selectedPopupPrefab == null) {
            return popupData;
        }

        GameObject newPopup = Instantiate(selectedPopupPrefab.gameObject, panelContainer.transform.parent);
        T newPopupControler = newPopup.GetComponent<T>();
        InitFunc?.Invoke(newPopupControler);
        
        TurnOnBackgroundEffects();
        interactionController.Block();
        newPopupControler.Show();
        bool result = await newPopupControler.WaitForCloseAsync();
        
        popupData.result = result;
        popupData.popupController = newPopupControler;

        HidePopup(newPopupControler);
        return popupData;
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
    
    void HidePopup(PopupController popup) {
        TurnOffBackgroundEffects();
        interactionController.Enable();
        popup.Hide();
        StartCoroutine(DeletePopup(popup));
    }

    IEnumerator DeletePopup(PopupController popup) {
        yield return new WaitForSeconds(0.5f);
        Destroy(popup.gameObject);
    }

    void Update() {
        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)) {
            onPointerUp?.Invoke();
        }
    }

    void Awake() {
        if (instance == null) {
            instance = this;
        }

        interactionController = panelContainer.GetComponent<InteractionController>();
        panels = panelContainer.GetComponentsInChildren<Panel>(true).ToList();

        foreach (var panel in panels) {
            panel.PanelAwake();
            if (panel.IsHiddenOnAwake()) {
                panel.Hide();
            } else {
                panel.Show();
            }
        }
    }

    void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }
}