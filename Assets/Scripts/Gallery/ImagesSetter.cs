using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagesSetter {
    public Image centerImageProperty {
        get {
            return centerImageContainer;
        }
        set {
            centerImageContainer = value;
            centerImage = centerImageContainer.GetComponentsInChildren<Image>()[2];
        }
    }
    Image centerImageContainer { get; set; }
    Image centerImage;
    List<Image> thumbnails = new List<Image>();
    int imgIndex = 0;

    public Func<string, Texture2D> LoadTexture;

    public void DisplayThumbnails(List<Texture2D> textures, Transform parent) {
        foreach (var texture in textures) {
            GameObject imageObject = new GameObject("Image");
            imageObject.AddComponent<RectTransform>();
            imageObject.AddComponent<RectMask2D>();
            GameObject imageHolder = new GameObject("ImageHolder");
            imageHolder.transform.SetParent(imageObject.transform);
            Image imageComponent = imageHolder.AddComponent<Image>();
            imageComponent.name = texture.name;
            AspectRatioFitter aspect = imageHolder.AddComponent<AspectRatioFitter>();
            aspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            aspect.aspectRatio = (float)texture.width / (float)texture.height;
            imageComponent.sprite = GetSprite(texture);

            Button button = imageObject.AddComponent<Button>();
            button.onClick.AddListener(() => DisplayCenterImage(imageComponent));

            thumbnails.Add(imageComponent);
            imageObject.transform.SetParent(parent);
            imageObject.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    public void DisableCenterImage() {
        DisposeCenterImage();
        centerImageContainer.gameObject.SetActive(false);
    }

    public void DisplayNextCenterImage() {
        imgIndex++;
        if (imgIndex >= thumbnails.Count) {
            imgIndex = 0;
        }
        DisplayCenterImage(thumbnails[imgIndex]);
    }

    public void DisplayPreviousCenterImage() {
        imgIndex--;
        if (imgIndex < 0) {
            imgIndex = thumbnails.Count - 1;
        }
        DisplayCenterImage(thumbnails[imgIndex]);
    }

    void DisplayCenterImage(Image thumbnail) {
        centerImageContainer.gameObject.SetActive(true);
        imgIndex = thumbnails.IndexOf(thumbnail);
        Texture2D texture = LoadTexture(thumbnail.name);
        AspectRatioFitter aspect = centerImage.GetComponent<AspectRatioFitter>();
        aspect.aspectRatio = (float)texture.width / (float)texture.height;
        centerImage.sprite = GetSprite(texture);
    }

    void DisposeCenterImage() {
        GameObject.Destroy(centerImageContainer.sprite);
    }

    Sprite GetSprite(Texture2D tex) {
        if (tex == null) {
            return null;
        }
        Sprite s = Sprite.Create(
            tex,
            new Rect(0.0f, 0.0f, tex.width, tex.height),
            new Vector2(0.5f, 0.5f),
            100.0f,
            0,
            SpriteMeshType.FullRect);
        s.name = tex.name;
        return s;
    }

    void OnDestroy() {
        foreach (var image in thumbnails) {
            GameObject.Destroy(image.sprite);
        }
    }
}
