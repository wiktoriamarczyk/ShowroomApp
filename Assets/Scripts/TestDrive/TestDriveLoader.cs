using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestDriveLoader : MonoBehaviour {
    public async UniTask<List<string>> GetData(string url) { 
        using (UnityWebRequest uwr = UnityWebRequest.Get(url)) {
            await uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success) {
                Debug.Log(uwr.error);
                return null;
            }

            string json = uwr.downloadHandler.text;
            string[] drives = json.Split("},{");
            List<string> testDrivesList = new List<string>();

            for (int i = 0; i < drives.Length; ++i) {
                TestDriveData.RootObject testDriveObject = JsonConvert.DeserializeObject<TestDriveData.RootObject>(drives[i]);
                TestDriveData.Entry testDriveData = testDriveObject.dreamlo.leaderboard.entry;
                testDrivesList.Add(testDriveData.name.Replace("-", "/") + " " + testDriveData.text);
            }
            
            return testDrivesList;
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
