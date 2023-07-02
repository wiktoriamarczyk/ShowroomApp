using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.IO;
using System;
using System.Reflection;
using UnityEngine.UI;

public class ImageProvider : MonoBehaviour {
    const string siteURL = "http://itsilesia.com/3d/data/PraktykiGaleria/";
    List<Texture2D> textures = new List<Texture2D>();
    List<string> pathsToImages;
    List<Image> thumbnails = new List<Image>();
    Vector2 thumbnaileSize = new Vector2(385, 250);

    async void Awake() {
        await LoadImages();
        FillGalleryWithThumbnails();
        SetProperThumbnailScale();
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


    void FillGalleryWithThumbnails() {
        foreach (var texture in textures) {
            GameObject imageObject = new GameObject("Image");
            Image imageComponent = imageObject.AddComponent<Image>();
            imageComponent.sprite = GetSprite(texture);
            Vector2 originalSize = imageComponent.sprite.rect.size;
            imageComponent.rectTransform.sizeDelta = GetScaledSize(originalSize, thumbnaileSize);
            imageComponent.rectTransform.localScale = Vector3.one;
            imageObject.transform.SetParent(transform);
            thumbnails.Add(imageComponent);
        }
    }

    void SetProperThumbnailScale() {
        foreach (var thumbnail in thumbnails) {
            thumbnail.rectTransform.localScale = Vector3.one;
        }
    }

    async UniTask LoadImages() {
        string url = Path.Combine(siteURL, "manifest.txt");

        using (UnityWebRequest uwr = UnityWebRequest.Get(url)) {
            await uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success) {
                Debug.Log(uwr.error);
                return;
            }

            string manifestText = uwr.downloadHandler.text;
            string[] imageNames = manifestText.Split('\n');

            foreach (string imageName in imageNames) {
                string imgURL = Path.Combine(siteURL, imageName.Trim());

                Texture2D texture = await GetImage(imgURL);
                textures.Add(texture);
                Debug.Log("Loaded image name: " + imageName);
            }
        }
    }

    async UniTask<Texture2D> GetImage(string imagePath) {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imagePath)) {
            await uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success) {
                Debug.Log(uwr.error);
                return null;
            }
            else {
                var texture = DownloadHandlerTexture.GetContent(uwr);
                return texture;
            }
        }
    }
}
