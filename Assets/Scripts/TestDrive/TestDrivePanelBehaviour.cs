using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Localization.LocalizationTableCollection;

public class TestDrivePanelBehaviour : MonoBehaviour {
    [SerializeField] TMP_Dropdown driveDate;
    [SerializeField] TMP_Dropdown driveTime;
    [SerializeField] TMP_InputField? driverName;
    [SerializeField] Button submit;
    [SerializeField] Button refresh;
    [SerializeField] GameObject testDriveItemPrefab;
    [SerializeField] GameObject testDriveList;

    TestDriveLoader modifier;
    List<GameObject> testDriveObjects = new List<GameObject>();

    const string modifyDataPath = "http://dreamlo.com/lb/-Ur0ruQAokKXyyv8uxxT0wOw8r3LFlWUyISb24jdTvEw/";
    const string dataPath = "http://dreamlo.com/lb/6486be2b8f40bb7d84121bba/json";
    const int earliesHour = 8;
    const int lastHour = 22;
    const int minuteStep = 30;

    async void Awake() {
        modifier = new TestDriveLoader();
        refresh.onClick.AddListener(async () => await RefreshData());
        submit.onClick.AddListener(OnSubmit);
        driveDate.onValueChanged.AddListener(delegate { OnDateChanged(); });
        InsertData();
        await RefreshData();
    }

    void OnDateChanged() {
        driveTime.ClearOptions();
        List<string> hoursList = new List<string>();
        DateTime earliestTime = DateTime.Now;
        if (driveDate.value == 0) {
            earliestTime = SetProperTime();
        }
        else {
            earliestTime = DateTime.MinValue.AddHours(8);
        }
        for (DateTime time = earliestTime; time.Hour < lastHour; time = time.AddMinutes(minuteStep)) {
            hoursList.Add(time.Hour + ":" + time.Minute);
        }
        driveTime.AddOptions(hoursList);
    }

    DateTime SetProperTime() {
        DateTime time = DateTime.Now;
        if (time.Hour >= lastHour) {
            time = time.AddDays(1);
            time = new DateTime(time.Year, time.Month, time.Day, earliesHour, 0, 0);
            return time;
        }
        if (time.Minute > minuteStep) {
            time = time.AddHours(1);
            time = new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
        }
        else {
            time = time.AddMinutes(minuteStep - time.Minute);
        }
        return time;
    }

    void InsertData() {
        driveDate.ClearOptions();
        driveTime.ClearOptions();

        List<string> dateList = new List<string>();
        List<string> hoursList = new List<string>();
        DateTime earliestDate = DateTime.Now;
        if (earliestDate.Hour >= lastHour) {
            earliestDate = earliestDate.AddDays(1);
        }
        DateTime lastDate = earliestDate.AddDays(6);
        CultureInfo cultureInfo = new CultureInfo("en-US");

        for (DateTime date = earliestDate; date <= lastDate; date = date.AddDays(1)) {
            dateList.Add(date.ToString("dd/MM/yyyy", cultureInfo));
        }

        DateTime earliestTime = SetProperTime();

        for (DateTime time = earliestTime; time.Hour < lastHour; time = time.AddMinutes(minuteStep)) {
            hoursList.Add(time.Hour + ":" + time.Minute);
        }

        driveDate.AddOptions(dateList);
        driveTime.AddOptions(hoursList);
    }

    void EraseDropdownOptionAtIndex(TMP_Dropdown dropdown, int index) {
        dropdown.options.RemoveAt(index);
        dropdown.RefreshShownValue();
    }


    async void OnSubmit() {
        string date = driveDate.captionText.text;
        string time = driveTime.captionText.text;
        string name = driverName.text;

        string urlAddition = "/1";
        if (name != null) {
            urlAddition += "/0/" + name;
        }
        string finalUrl = Path.Combine(modifyDataPath, "add/" + date.Replace("/", "-") + "-" + time.Replace(":", "-") + urlAddition);

        await modifier.UpdateData(finalUrl);
        AddTestDrive(date + " " + time + " " + name);
        EraseDropdownOptionAtIndex(driveTime, driveTime.value);
        if (driveTime.options.Count <= 1) {
            EraseDropdownOptionAtIndex(driveDate, driveDate.value);
            OnDateChanged();
        }
    }

    async void OnDelete(GameObject item) {
        TextMeshProUGUI textMeshPro = item.GetComponentInChildren<TextMeshProUGUI>();
        string date = textMeshPro.text;
        string url = Path.Combine(modifyDataPath, "delete/" + date.Replace("/", "-"));
        
        await modifier.UpdateData(url);
        testDriveObjects.Remove(item);
        Destroy(item);
        await RefreshData();
    }
    
    void AddTestDrive(string testDriveInfo) {
        GameObject testDriveItem = Instantiate(testDriveItemPrefab);
        testDriveItem.name = "TestDriveItem";
        TextMeshProUGUI textMeshPro = testDriveItem.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.text = testDriveInfo;
        Button deleteButton = testDriveItem.GetComponentInChildren<Button>();
        deleteButton.onClick.AddListener(() => { OnDelete(testDriveItem); });
        testDriveItem.transform.SetParent(testDriveList.transform);
        testDriveItem.transform.localScale = Vector3.one;
        testDriveObjects.Add(testDriveItem);
    }

    async UniTask RefreshData() {
        if (testDriveObjects.Count > 0) {
            foreach (var item in testDriveObjects) {
                Destroy(item);
            }
            testDriveObjects.Clear();
        }
        List<string> data = await modifier.GetData(dataPath);
        foreach (var testDriveInfo in data) {
            AddTestDrive(testDriveInfo);
        }
    }
}
