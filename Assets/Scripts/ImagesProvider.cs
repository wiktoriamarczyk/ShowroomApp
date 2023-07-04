using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.IO;
using System.Threading;

public class ImagesProvider : MonoBehaviour {
    const string siteURL = "http://itsilesia.com/3d/data/PraktykiGaleria/";
    List<Texture2D> textures = new List<Texture2D>();

    public List<Texture2D> texturesGetter {
        get => textures;
    }

    public async UniTask LoadImages(CancellationToken cancellationToken) {
        string url = Path.Combine(siteURL, "manifest.txt");

        using (UnityWebRequest uwr = UnityWebRequest.Get(url)) {
            cancellationToken.ThrowIfCancellationRequested();
            await uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success) {
                Debug.Log(uwr.error);
                return;
            }

            string manifestText = uwr.downloadHandler.text;
            string[] imageNames = manifestText.Split('\n');

            foreach (string imageName in imageNames) {
                cancellationToken.ThrowIfCancellationRequested();

                string imgURL = Path.Combine(siteURL, imageName.Trim());
                Texture2D texture = await GetImage(imgURL, cancellationToken);
                textures.Add(texture);
                Debug.Log("Loaded image name: " + imageName);
            }
        }
    }

    async UniTask<Texture2D> GetImage(string imagePath, CancellationToken cancellationToken) {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imagePath)) {
            cancellationToken.ThrowIfCancellationRequested();
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
