using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    [SerializeField] GameObject splashScreen;
    [SerializeField] CameraController cameraController;
    [SerializeField] GalleryManager gallery;

    void Awake() {
        cameraController.onScreenSaver += PanelManager.Instance.HideAllPanels;
        cameraController.onCameraMovement += PanelManager.Instance.ShowAllPrevOpenedPanels;

        return;
        gallery.onGalleryLoaded += DisableSplashScreen;

        splashScreen.SetActive(true);
        //cameraController.enabled = false;
        cameraController.DisableRotation();
    }

    void DisableSplashScreen() {
        splashScreen.SetActive(false);
        //cameraController.enabled = true;
        cameraController.EnableRotation();
    }

    void OnDestroy() {
        gallery.onGalleryLoaded -= DisableSplashScreen;
        cameraController.onScreenSaver -= PanelManager.Instance.HideAllPanels;
        cameraController.onCameraMovement -= PanelManager.Instance.ShowAllPrevOpenedPanels;
    }
}
