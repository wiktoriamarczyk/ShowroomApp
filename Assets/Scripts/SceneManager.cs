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
        gallery.onGalleryLoaded += DisableSplashScreen;

        return;
        splashScreen.SetActive(true);
        cameraController.enabled = false;
    }

    void DisableSplashScreen() {
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
        gallery.onGalleryLoaded -= DisableSplashScreen;
    }
}
