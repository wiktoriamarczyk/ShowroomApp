using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.IO;
using System.Threading;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

public class ImagesProvider : MonoBehaviour {
    string[] imagesNames;
    List<Texture2D> thumbnails = new List<Texture2D>();

    public List<Texture2D> LoadThumbnails() {
        return thumbnails;
    }

    public static Texture2D Resize(Texture2D source, int newWidth, int newHeight) {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D nTex = new Texture2D(newWidth, newHeight);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        nTex.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return nTex;
    }

    void WriteImageOnDisk(Texture2D texture) {
        byte[] textureBytes = texture.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, texture.name + ".png");
        File.WriteAllBytes(path, textureBytes);
        Debug.Log("Saved image: " + texture.name + " to: " + path);
    }

    public List<Texture2D> LoadTexturesFromDisk() {
        List<Texture2D> textures = new List<Texture2D>();
        foreach (var name in imagesNames) {
            textures.Add(LoadTextureFromDisk(name));
        }
        return textures;
    }

    public Texture2D LoadTextureFromDisk(string imageName) {
        byte[] textureBytes = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, imageName + ".png"));
        Texture2D loadedTexture = new Texture2D(2, 2);
        loadedTexture.LoadImage(textureBytes);
        return loadedTexture;
    }

    public async UniTask LoadTexturesFromManifest(CancellationToken cancellationToken, string siteURL, Vector2Int thumbnailSize) {
        string url = Path.Combine(siteURL, "manifest.txt");

        using (UnityWebRequest uwr = UnityWebRequest.Get(url)) {
            cancellationToken.ThrowIfCancellationRequested();
            await uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success) {
                Debug.Log(uwr.error);
                return;
            }

            string manifestText = uwr.downloadHandler.text;
            imagesNames = manifestText.Split('\n');

            float thumbnailAspectRatio = (float)thumbnailSize.x / thumbnailSize.y;

            foreach (string imageName in imagesNames) {
                cancellationToken.ThrowIfCancellationRequested();
                string imgURL = Path.Combine(siteURL, imageName.Trim());
                Texture2D texture = await GetTexture(cancellationToken, imgURL);
                texture.name = imageName;
                WriteImageOnDisk(texture);
                Vector2Int newSize;
                float aspectRatio = (float)texture.width / texture.height;
                if (aspectRatio < thumbnailAspectRatio) {
                    // thumbnail width
                    newSize = new Vector2Int(thumbnailSize.x, 0);
                    newSize.y = (int)(newSize.x / aspectRatio);
                }
                else {
                    // thumbnail height
                    newSize = new Vector2Int(0, thumbnailSize.y);
                    newSize.x = (int)(newSize.y * aspectRatio);
                }
                Texture2D thumbnail = Resize(texture, newSize.x, newSize.y);
                thumbnail.name = imageName;
                thumbnails.Add(thumbnail);
            }
        }
    }

    async UniTask<Texture2D> GetTexture(CancellationToken cancellationToken, string imagePath) {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imagePath)) {
            cancellationToken.ThrowIfCancellationRequested();
            await uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success) {
                Debug.Log(uwr.error);
                return null;
            }
            var texture = DownloadHandlerTexture.GetContent(uwr);
            return texture;
        }
    }
}
