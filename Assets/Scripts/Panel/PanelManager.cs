using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class PanelManager : MonoBehaviour {
    static public PanelManager Instance;
    [SerializeField] List<Panel> panels;
    public Panel currentPanel { get; private set; }

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

    public void ShowPanel(Panel panel) {
        if (currentPanel == panel)
            return;
        HideCurrentPanel();
        panel.Show();
        currentPanel = panel;
    }

    public void HideCurrentPanel() {
        if (currentPanel == null) {
            return;
        }
        currentPanel.Hide();
        currentPanel = null;
    }
}