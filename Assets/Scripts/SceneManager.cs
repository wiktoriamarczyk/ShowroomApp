using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    [SerializeField] GameObject splashScreen;
    [SerializeField] CameraController cameraController;
    [SerializeField] GalleryManager gallery;
    [SerializeField] List<Panel> UIToHide;

    void Awake() {
        PanelManager.Instance.onPanelOpened += cameraController.DisableRotation;
        PanelManager.Instance.onPanelClosed += cameraController.EnableRotation;
        cameraController.onScreenSaver += HideUI;
        cameraController.onCameraMovement += ShowHiddenUI;

        return;
        splashScreen.SetActive(true);
        cameraController.enabled = false;
    }

    public void DisableSplashScreen() {
        splashScreen.SetActive(false);
        cameraController.enabled = true;
    }

    void HideUI() {
        foreach (var panel in UIToHide) {
            panel.Hide();
        }
    }

    void ShowHiddenUI() {
        foreach (var panel in UIToHide) {
            panel.Show();
        }
    }

    void OnDestroy() {
        PanelManager.Instance.onPanelOpened -= cameraController.DisableRotation;
        PanelManager.Instance.onPanelClosed -= cameraController.EnableRotation;
        cameraController.onScreenSaver -= HideUI;
        cameraController.onCameraMovement -= ShowHiddenUI;
    }
}
