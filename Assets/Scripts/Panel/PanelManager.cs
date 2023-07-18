using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using System;

public class PanelManager : MonoBehaviour {
    static public PanelManager Instance;

    [SerializeField] List<Panel> panels;

    Panel currentPanel;
    public event Action onPanelOpened;
    public event Action onPanelClosed;

    public void ShowPanel(Panel panel) {
        if (currentPanel == panel) {
            return;
        }
        HideCurrentPanel();
        panel.Show();
        currentPanel = panel;
        onPanelOpened?.Invoke();
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
            panel.Hide();
        }
    }
}