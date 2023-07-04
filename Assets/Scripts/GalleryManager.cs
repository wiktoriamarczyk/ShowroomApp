using System.Threading;
using UnityEngine;

public class GalleryManager : MonoBehaviour {
    [SerializeField] SceneManager canvasManager;
    [SerializeField] GameObject gallery;

    ImagesProvider imgProvider;
    ImagesSetter imgSetter;

    CancellationTokenSource cancelTokenSrc;
    CancellationToken cancelToken;

    async void Awake() {
        cancelTokenSrc = new CancellationTokenSource();
        cancelToken = cancelTokenSrc.Token;

        imgProvider = new ImagesProvider();
        imgSetter = new ImagesSetter();
        try {
            await imgProvider.LoadImages(cancelToken);
        }
        catch (System.OperationCanceledException) {
            Debug.Log("Image loading canceled");
            return;
        }
        imgSetter.DisplayThumbnails(imgProvider.texturesGetter, gallery.transform);
        canvasManager.DisableSplashScreen();
    }

    void OnDestroy() {
        cancelTokenSrc.Cancel();
    }
}
