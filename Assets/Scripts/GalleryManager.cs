using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour {
    [SerializeField] Panel galleryPanel;
    [SerializeField] GameObject imagesContainer;
    [SerializeField] Image centerImage;
    [SerializeField] PostProcessingEffectsModifier galleryEffectsModifier;

    ImagesProvider imgProvider;
    ImagesSetter imgSetter;
    
    CancellationTokenSource cancelTokenSrc;
    CancellationToken cancelToken;

    public UnityEvent onGalleryLoaded;

    public void DisplayNextCenterImage() {
        imgSetter.DisplayNextCenterImage();
    }

    public void DisplayPreviousCenterImage() {
        imgSetter.DisplayPreviousCenterImage();
    }

    public void DisableCenterImage() {
        imgSetter.DisableCenterImage();
    }

    async void Awake() {
        cancelTokenSrc = new CancellationTokenSource();
        cancelToken = cancelTokenSrc.Token;
        
        imgProvider = new ImagesProvider();
        imgSetter = new ImagesSetter();
        centerImage.gameObject.SetActive(false);
        imgSetter.centerImageProperty = centerImage;

        galleryPanel.onPanelOpened.AddListener(TurnOnGalleryEffects);
        galleryPanel.onPanelClosed.AddListener(TurnOffGalleryEffects);

        try {
            await imgProvider.LoadImages(cancelToken);
        }
        catch (System.OperationCanceledException) {
            Debug.Log("Image loading canceled");
            return;
        }
        
        imgSetter.DisplayThumbnails(imgProvider.texturesGetter, imagesContainer.transform);
        onGalleryLoaded?.Invoke();
    }

    void TurnOnGalleryEffects() {
        galleryEffectsModifier.TurnOnGalleryEffects();
    }

    void TurnOffGalleryEffects() {
        galleryEffectsModifier.TurnOffGalleryEffects();
    }

    void OnDestroy() {
        cancelTokenSrc.Cancel();
        cancelTokenSrc.Dispose();
        galleryPanel.onPanelOpened.RemoveListener(TurnOnGalleryEffects);
        galleryPanel.onPanelClosed.RemoveListener(TurnOffGalleryEffects);
    }
}
