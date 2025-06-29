using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour {
    [SerializeField] Panel galleryPanel;
    [SerializeField] GameObject imagesContainer;
    [SerializeField] Image centerImageContainer;
    [SerializeField] Image centerImageHolder;

    ImagesProvider imgProvider;
    ImagesSetter imgSetter;

    CancellationTokenSource cancelTokenSrc;
    CancellationToken cancelToken;

    public event Action<bool> onGalleryLoaded;

    const string siteURL = "http://itsilesia.com/3d/data/PraktykiGaleria/";
    const int centerImgWidth = 385;
    const int centerImgHeight = 250;

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

    async void Start() {
        cancelTokenSrc = new CancellationTokenSource();
        cancelToken = cancelTokenSrc.Token;

        imgProvider = new ImagesProvider();
        imgSetter = GetComponent<ImagesSetter>();
        centerImageContainer.gameObject.SetActive(false);
        imgSetter.centerImageContainerProperty = centerImageContainer;
        imgSetter.centerImageProperty = centerImageHolder;
        imgSetter.LoadTexture = GetFullSizeFromThumbnail;

        galleryPanel.onPanelOpened += PanelManager.instance.TurnOnBackgroundEffects;
        galleryPanel.onPanelClosed += PanelManager.instance.TurnOffBackgroundEffects;
        try {
            await imgProvider.LoadTexturesFromManifest(cancelToken, siteURL, new Vector2Int(centerImgWidth, centerImgHeight));
        }
        catch (Exception e) {
            Debug.LogError($"Error loading gallery: {e.Message}");
            onGalleryLoaded?.Invoke(false);
            return;
        }

        imgSetter.DisplayThumbnails(imgProvider.LoadThumbnails(), imagesContainer.transform);
        onGalleryLoaded?.Invoke(true);
    }

    void OnDestroy() {
        cancelTokenSrc.Cancel();
        cancelTokenSrc.Dispose();
    }
}
