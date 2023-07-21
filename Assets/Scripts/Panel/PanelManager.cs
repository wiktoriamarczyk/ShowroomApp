using System.Collections.Generic;
using UnityEngine;
using System;

public class PanelManager : MonoBehaviour {
    static public PanelManager Instance;

    [SerializeField] List<Panel> panels;

    Panel currentPanel;
    public event Action onPanelOpened;
    public event Action onPanelClosed;

    public void ShowPanel(Panel panel) {
        bool blockingPanel = panel.IsBlockingOthersWhichHaveTrue();
        if (blockingPanel) {
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

    public void HideCurrentPanel() {
        if (currentPanel == null) {
            return;
        }
        currentPanel.Hide();
        currentPanel = null;
        onPanelClosed?.Invoke();
    }

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }

        foreach (var panel in panels) {
            panel.PanelAwake();
            if (panel.IsHiddenOnAwake()) {
                panel.Hide();
            }
        }
    }
}