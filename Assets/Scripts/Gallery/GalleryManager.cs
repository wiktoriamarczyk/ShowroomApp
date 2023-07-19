using System;
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

    public event Action onGalleryLoaded;

    const string siteURL = "http://itsilesia.com/3d/data/PraktykiGaleria/";

    public void DisplayNextCenterImage() {
        imgSetter.DisplayNextCenterImage();
    }

    public void DisplayPreviousCenterImage() {
        imgSetter.DisplayPreviousCenterImage();
    }

    public void DisableCenterImage() {
        imgSetter.DisableCenterImage();
    }

    public Texture2D GetFullSizeFromThumbnail(string imageName) {
        return imgProvider.LoadTextureFromDisk(imageName);
    }

    async void Awake() {
        return;
        cancelTokenSrc = new CancellationTokenSource();
        cancelToken = cancelTokenSrc.Token;

        imgProvider = new ImagesProvider();
        imgSetter = new ImagesSetter();
        centerImage.gameObject.SetActive(false);
        imgSetter.centerImageProperty = centerImage;
        imgSetter.LoadTexture = GetFullSizeFromThumbnail;

        galleryPanel.onPanelOpened += TurnOnGalleryEffects;
        galleryPanel.onPanelClosed += TurnOffGalleryEffects;

        try {
            await imgProvider.LoadTexturesFromManifest(cancelToken, siteURL, new Vector2Int(385, 250));
        }
        catch (System.OperationCanceledException) {
            Debug.Log("Image loading canceled");
            return;
        }

        imgSetter.DisplayThumbnails(imgProvider.LoadThumbnails(), imagesContainer.transform);
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
        galleryPanel.onPanelOpened -= TurnOnGalleryEffects;
        galleryPanel.onPanelClosed -= TurnOffGalleryEffects;
    }
}
