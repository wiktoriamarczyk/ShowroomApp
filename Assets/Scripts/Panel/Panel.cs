using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Panel : MonoBehaviour {
    public UnityEvent onPanelOpened;
    public UnityEvent onPanelClosed;
    public virtual void Show() { onPanelOpened?.Invoke(); }
    public virtual void Hide() { onPanelClosed?.Invoke(); }
}
