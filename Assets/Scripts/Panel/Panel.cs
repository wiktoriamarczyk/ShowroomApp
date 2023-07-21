using UnityEngine;
using System;

public abstract class Panel : MonoBehaviour {
    public event Action onPanelOpened;
    public event Action onPanelClosed;
    protected bool blockingOthers = false;
    protected bool isHiddenOnAwake = true;

    public bool IsBlockingOthersWhichHaveTrue() {
        return blockingOthers;
    }
    public bool IsHiddenOnAwake() {
        return isHiddenOnAwake;
    }

    public abstract void PanelAwake();
    public virtual void Show() { onPanelOpened?.Invoke(); }
    public virtual void Hide() { onPanelClosed?.Invoke(); }
}
