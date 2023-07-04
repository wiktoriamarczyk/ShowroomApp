using UnityEngine;

public class CanvasManager : MonoBehaviour {
    [SerializeField] GameObject splashScreen;
    [SerializeField] GameObject mainScene;

    void Awake() {
        splashScreen.SetActive(true);
    }

    public void DisableSplashScreen() {
        splashScreen.SetActive(false);
    }
}
