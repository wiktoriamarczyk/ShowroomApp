using UnityEngine;

public class SceneManager : MonoBehaviour {
    [SerializeField] GameObject splashScreen;
    [SerializeField] CameraController cameraController;

    void Awake() {
        splashScreen.SetActive(true);
        cameraController.enabled = false;
    }
    
    public void DisableSplashScreen() {
        splashScreen.SetActive(false);
        cameraController.enabled = true;
    }
}
