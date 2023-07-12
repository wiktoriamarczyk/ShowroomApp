using UnityEngine;

public class SceneManager : MonoBehaviour {
    [SerializeField] GameObject splashScreen;
    [SerializeField] CameraController cameraController;
    [SerializeField] GalleryManager gallery;

    void Awake() {
        PanelManager.Instance.onPanelOpened.AddListener(() => cameraController.DisableRotation());
        PanelManager.Instance.onPanelClosed.AddListener(() => cameraController.EnableRotation());
        gallery.onGalleryLoaded.AddListener(DisableSplashScreen);
        return;
        splashScreen.SetActive(true);
        cameraController.enabled = false;
    }
    
    public void DisableSplashScreen() {
        splashScreen.SetActive(false);
        cameraController.enabled = true;
    }


}
