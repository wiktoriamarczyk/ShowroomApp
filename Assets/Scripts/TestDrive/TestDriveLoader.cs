using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
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
            List<string> testDrivesList = new List<string>();

            foreach (Entry entry in rootObject.dreamlo.leaderboard.entry) {
                string dateString = entry.name;
                string format = "yyyy-MM-dd-HH-mm";
                string newFormat = "dd/MM/yyyy HH:mm";
                DateTime date = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
                string formattedDate = date.ToString(newFormat, CultureInfo.InvariantCulture);
                testDrivesList.Add(formattedDate + " " + entry.text);
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
