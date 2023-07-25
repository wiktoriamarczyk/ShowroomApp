using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static TestDriveData;

public class TestDriveLoader {
    public async UniTask<List<string>> GetData(string url) {
        using (UnityWebRequest uwr = UnityWebRequest.Get(url)) {
            await uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success) {
                Debug.Log(uwr.error);
                return null;
            }

            string json = uwr.downloadHandler.text;
            RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(json);
            List<Entry> entries = new List<Entry>();
            List<string> data = new List<string>();

            if (rootObject.dreamlo.leaderboard.entry != null) {
                var entry = rootObject.dreamlo.leaderboard.entry;

                // check entry type
                if (entry is JArray entryArray) {
                    // if it is an array, deserialize it as a list of Entry objects
                    entries = entryArray.ToObject<List<Entry>>();
                }
                else if (entry is JObject entryObject) {
                    // if it's a single object, deserialize it as a single Entry object and add it to the list
                    entries.Add(entryObject.ToObject<Entry>());
                }
            }

            foreach (Entry entry in entries) {
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
