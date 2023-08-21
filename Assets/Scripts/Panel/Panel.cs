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

    bool IsScreenPositionInside(Vector2 screenSpacePos) {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenSpacePos, null, out localPoint);
        return rectTransform.rect.Contains(localPoint);

    }

    protected void Start() {
        PanelManager.instance.onPointerDown += OnCustomPointerDown;
        PanelManager.instance.onPointerUp += OnCustomPointerUp;
    }

    protected void OnDestroy() {
        PanelManager.instance.onPointerDown -= OnCustomPointerDown;
        PanelManager.instance.onPointerUp -= OnCustomPointerUp;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (Input.touchCount == 0)
            OnCustomPointerEnter();
    }
    public void OnPointerExit(PointerEventData eventData) {
        if (Input.touchCount == 0)
            OnCustomPointerExit();
    }
    public void OnCustomPointerEnter() {
        SceneManager.instance.DisableCameraRotation();
        isPointerInside = true;
    }
    public void OnCustomPointerExit() {
        if (isPointerInside)
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
    public void OnCustomPointerDown(Vector2 position) {
        bool IsInside = IsScreenPositionInside(position);
        if (Input.touchCount > 0 && IsInside != isPointerInside) {
            if (IsInside)
                OnCustomPointerEnter();
            else
                OnCustomPointerExit();
        }
    }

    public void OnCustomPointerUp(Vector2 position) {
        bool IsInside = IsScreenPositionInside(position);
        if (Input.touchCount > 0 && IsInside != isPointerInside) {
            if (!IsInside)
                OnCustomPointerExit();
        }

        if (!isPointerInside && isPanelShown && (Time.time - lastShowTime) >= timeThreshold) {
            onPanelCloseRequest?.Invoke();
        }
    }
}
