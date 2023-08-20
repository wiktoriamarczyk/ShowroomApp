using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public abstract class Panel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public event Action onPanelOpened;
    public event Action onPanelClosed;
    public UnityEvent onPanelCloseRequest;
    protected bool isPanelShown = false;
    protected bool exclusiveVisibility = false;
    protected bool isHiddenOnAwake = true;
    protected bool isPointerInside = false;
    protected float lastShowTime = 0f;

    const float timeThreshold = 0.5f;

    protected void Start() {
        PanelManager.instance.onPointerUp += OnPointerUp;
    }

    protected void OnDestroy() {
        PanelManager.instance.onPointerUp -= OnPointerUp;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        SceneManager.instance.DisableCameraRotation();
        isPointerInside = true;
    }
    public void OnPointerExit(PointerEventData eventData) {
        SceneManager.instance.EnableCameraRotation();
        isPointerInside = false;
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
        lastShowTime = Time.time;
    }
    public virtual void Hide() {
        onPanelClosed?.Invoke();
        isPanelShown = false;
    }

    public void OnPointerUp() {
        if (!isPointerInside && isPanelShown && (Time.time - lastShowTime) >= timeThreshold) {
            onPanelCloseRequest?.Invoke();
        }
    }

}
