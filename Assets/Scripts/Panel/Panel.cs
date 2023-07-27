using UnityEngine;
using System;

public abstract class Panel : MonoBehaviour {
    public event Action onPanelOpened;
    public event Action onPanelClosed;
    protected bool exclusiveVisibility = false;
    protected bool isHiddenOnAwake = true;
    public bool ExclusiveVisibility() {
        return exclusiveVisibility;
    }
    public bool IsHiddenOnAwake() {
        return isHiddenOnAwake;
    }
    public abstract void PanelAwake();
    public virtual void Show() { onPanelOpened?.Invoke(); }
    public virtual void Hide() { onPanelClosed?.Invoke(); }
}
