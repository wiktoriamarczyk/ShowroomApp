using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class PanelManager : MonoBehaviour {
    [SerializeField] List<GameObject> panels;
    GameObject currentPanel;

    void Awake() {
        foreach (var panel in panels) {
            panel.SetActive(false);
        }
    }

    public void ShowPanel(GameObject panel) {
        HideCurrentPanel();
        panel?.SetActive(true);
        currentPanel = panel;
    }

    public void HideCurrentPanel() {
        if (currentPanel == null) {
            return;
        }
        currentPanel.SetActive(false);
        currentPanel = null;
    }
}