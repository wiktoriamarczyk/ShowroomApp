using UnityEngine;
using System;
using UnityEngine.EventSystems;

public abstract class Panel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public event Action onPanelOpened;
    public event Action onPanelClosed;
    protected bool isPanelShown = false;
    protected bool exclusiveVisibility = false;
    protected bool isHiddenOnAwake = true;

    public void OnPointerEnter(PointerEventData eventData) {
        SceneManager.instance.DisableCameraRotation();
    }
    public void OnPointerExit(PointerEventData eventData) {
        SceneManager.instance.EnableCameraRotation();
    }
    public bool ExclusiveVisibility() {
        return exclusiveVisibility;
    }
    public bool IsHiddenOnAwake() {
        return isHiddenOnAwake;
    }
    public bool IsPanelShown() {
        return isPanelShown;
    }
    public abstract void PanelAwake();
    public virtual void Show() {
        onPanelOpened?.Invoke();
        isPanelShown = true;
    }
    public virtual void Hide() {
        onPanelClosed?.Invoke();
        isPanelShown = false;
    }
}
