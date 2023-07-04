using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour {
    [SerializeField] CanvasManager canvasManager;
    [SerializeField] GameObject gallery;
    ImagesProvider imgProvider;
    ImagesSetter imgSetter;

    async void Awake() {
        imgProvider = new ImagesProvider();
        imgSetter = new ImagesSetter();
        await imgProvider.LoadImages();
        imgSetter.DisplayThumbnails(imgProvider.texturesGetter, gallery.transform);
        canvasManager.DisableSplashScreen();
    }
}
