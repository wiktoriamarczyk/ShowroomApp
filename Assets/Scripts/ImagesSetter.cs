using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ImagesSetter : MonoBehaviour {
    List<Image> images = new List<Image>();
    Vector2 thumbnailSize = new Vector2(385, 250);

    public void DisplayThumbnails(List<Texture2D> textures, Transform parent) {
        foreach (var texture in textures) {
            GameObject imageObject = new GameObject("Image");
            Image imageComponent = imageObject.AddComponent<Image>();
            imageComponent.sprite = GetSprite(texture);
            Vector2 originalSize = imageComponent.sprite.rect.size;
            imageComponent.rectTransform.sizeDelta = GetScaledSize(originalSize, thumbnailSize);
            images.Add(imageComponent);
            imageObject.transform.SetParent(parent);
        }
        SetProperThumbnailsScale();
    }

    void SetProperThumbnailsScale() {
        foreach (var thumbnail in images) {
            thumbnail.rectTransform.localScale = Vector3.one;
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

    Vector2 GetScaledSize(Vector2 originalSize, Vector2 desiredSize) {
        float originalAspectRatio = originalSize.x / originalSize.y;
        float desiredAspectRatio = desiredSize.x / desiredSize.y;
        if (originalAspectRatio > desiredAspectRatio) {
            return new Vector2(desiredSize.x, desiredSize.x / originalAspectRatio);
        }
        else {
            return new Vector2(desiredSize.y * originalAspectRatio, desiredSize.y);
        }
    }
}
