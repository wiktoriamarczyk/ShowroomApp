using Cysharp.Threading.Tasks;
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
        cameraController.onCameraMovement += ShowHiddenUI;
        cameraController.onScreenSaver += HideUI;
        return;
        gallery.onGalleryLoaded += DisableSplashScreen;

        splashScreen.SetActive(true);
        cameraController.enabled = false;
    }

    void DisableSplashScreen() {
        splashScreen.SetActive(false);
        cameraController.enabled = true;
    }

    void HideUI() {
        foreach (var ui in UIToHide) {
            PanelManager.Instance.HidePanel(ui);
        }
    }

    void ShowHiddenUI() {
        foreach (var ui in UIToHide) {
            PanelManager.Instance.ShowPanel(ui);
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
