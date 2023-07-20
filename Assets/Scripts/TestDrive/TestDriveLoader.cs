using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static TestDriveData;

public class TestDriveLoader : MonoBehaviour {
    public async UniTask<List<string>> GetData(string url) {
        using (UnityWebRequest uwr = UnityWebRequest.Get(url)) {
            await uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success) {
                Debug.Log(uwr.error);
                return null;
            }

            string json = uwr.downloadHandler.text;
            RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(json);
            List<string> data = new List<string>();

            foreach (Entry entry in rootObject.dreamlo.leaderboard.entry) {
                data.Add(entry.name + " " + entry.text);
            }

            return data;
        }
    }

    public async UniTask<bool> UpdateData(string url) {
        using (UnityWebRequest uwr = UnityWebRequest.Get(url)) {
            await uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success) {
                Debug.Log(uwr.error);
                return false;
            }
            return true;
        }
    }
}
