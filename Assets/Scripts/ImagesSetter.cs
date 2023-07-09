using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ImagesSetter : MonoBehaviour {
    public GameObject fullSizeImage { get; set; }
    List<Image> images = new List<Image>();

    public void DisplayThumbnails(List<Texture2D> textures, Transform parent) {
        foreach (var texture in textures) {
            GameObject imageObject = new GameObject("Image");
            imageObject.AddComponent<RectTransform>();
            imageObject.AddComponent<RectMask2D>();
            GameObject imageHolder = new GameObject("ImageHolder");
            imageHolder.transform.SetParent(imageObject.transform);
            Image imageComponent = imageHolder.AddComponent<Image>();
            AspectRatioFitter aspect = imageHolder.AddComponent<AspectRatioFitter>();
            aspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            aspect.aspectRatio = (float)texture.width / (float)texture.height;
            imageComponent.sprite = GetSprite(texture);

            Button button = imageObject.AddComponent<Button>();
            button.onClick.AddListener(() => DisplayFullSizeImage(imageComponent.sprite));

            images.Add(imageComponent);
            imageObject.transform.SetParent(parent);
            imageObject.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }
    
    void DisplayFullSizeImage(Sprite sprite) {
        fullSizeImage.SetActive(true);
        fullSizeImage.GetComponentsInChildren<Image>()[1].sprite = sprite;
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
        foreach (var image in images) {
            Destroy(image.sprite);
        }
    }
}
