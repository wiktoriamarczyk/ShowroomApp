using UnityEngine;
using System;

public abstract class Panel : MonoBehaviour {
    public event Action onPanelOpened;
    public event Action onPanelClosed;
    public virtual void Show() { onPanelOpened?.Invoke(); }
    public virtual void Hide() { onPanelClosed?.Invoke(); }
}
