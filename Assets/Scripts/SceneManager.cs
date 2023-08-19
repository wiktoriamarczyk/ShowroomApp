using UnityEngine;

public class SceneManager : MonoBehaviour {
    public static SceneManager instance { get; private set; }
    [SerializeField] GameObject splashScreen;
    [SerializeField] OutsideCameraMovement cameraController;
    [SerializeField] GalleryManager gallery;

    public void DisableCameraRotation() {
        cameraController.DisableRotation();
    }

    public void EnableCameraRotation() {
        cameraController.EnableRotation();
    }

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        cameraController.onScreenSaver += PanelManager.instance.HideAllPanels;
        cameraController.onCameraMovement += PanelManager.instance.ShowAllPrevOpenedPanels;
        return;
        gallery.onGalleryLoaded += DisableSplashScreen;

        splashScreen.SetActive(true);
        cameraController.DisableRotation();
    }

    void DisableSplashScreen() {
        splashScreen.SetActive(false);
        cameraController.EnableRotation();
    }

    void OnDestroy() {
        gallery.onGalleryLoaded -= DisableSplashScreen;
        cameraController.onScreenSaver -= PanelManager.instance.HideAllPanels;
        cameraController.onCameraMovement -= PanelManager.instance.ShowAllPrevOpenedPanels;
        if (instance == this) {
            instance = null;
        }
    }
}
