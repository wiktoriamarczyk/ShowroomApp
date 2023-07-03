using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.UI;

public class ImagesProvider : MonoBehaviour {
    const string siteURL = "http://itsilesia.com/3d/data/PraktykiGaleria/";
    List<Texture2D> textures = new List<Texture2D>();

    public List<Texture2D> texturesGetter {
        get => textures;
    }

    public async UniTask LoadImages() {
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
