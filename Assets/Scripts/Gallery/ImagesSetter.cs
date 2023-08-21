using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagesSetter : MonoBehaviour {
    Image centerImageContainer;
    Image centerImage;
    List<Image> thumbnails = new List<Image>();
    int imgIndex = 0;

    public Func<string, Texture2D> LoadTexture;
    public Image centerImageContainerProperty {
        get => centerImageContainer;
        set => centerImageContainer = value;
    }
    public Image centerImageProperty {
        get => centerImage;
        set => centerImage = value;
    }

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
        DisposeCenterImage();
        DisplayCenterImage(thumbnails[imgIndex]);
    }

    public void DisplayPreviousCenterImage() {
        imgIndex--;
        if (imgIndex < 0) {
            imgIndex = thumbnails.Count - 1;
        }
        DisposeCenterImage();
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
        if (centerImage.sprite != null) {
            Destroy(centerImage.sprite.texture);
            Destroy(centerImage.sprite);
        }
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
            Destroy(image.sprite.texture);
            Destroy(image.sprite);
            Destroy(image.gameObject);
        }
        thumbnails.Clear();
    }
}
