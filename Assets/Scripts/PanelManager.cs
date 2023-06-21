using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour {
    [SerializeField] List<GameObject> panels;
    [SerializeField] GameObject startingPanel;
    GameObject currentPanel;
    Icon? currentIcon;

    void Awake() {
        foreach (var panel in panels) {
            panel.SetActive(false);
        }
       // ShowPanel(startingPanel);
    }

    public void ShowPanel(GameObject panel) {
        HideCurrentPanel();
        panel?.SetActive(true);
        currentPanel = panel;
    }

    public void HideCurrentPanel() {
        if (currentPanel == null)
            return;
        currentIcon?.ChangeToDefaultColor();
        currentIcon = null;
        currentPanel.SetActive(false);
        currentPanel = null;
    }
}