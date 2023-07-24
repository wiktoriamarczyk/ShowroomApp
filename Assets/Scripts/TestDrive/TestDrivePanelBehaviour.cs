using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TestDrivePanelBehaviour : MonoBehaviour {
    [SerializeField] TMP_Dropdown driveDate;
    [SerializeField] TMP_Dropdown driveTime;
    [SerializeField] TMP_Dropdown registeredDates;
    [SerializeField] TMP_InputField? driverName;
    [SerializeField] Button submit;
    [SerializeField] Button refresh;
    [SerializeField] Button restart;
    [SerializeField] GameObject testDriveItemPrefab;
    [SerializeField] GameObject testDriveList;

    TestDriveLoader modifier;
    List<GameObject> testDriveObjects = new List<GameObject>();

    const string modifyDataPath = "http://dreamlo.com/lb/-Ur0ruQAokKXyyv8uxxT0wOw8r3LFlWUyISb24jdTvEw/";
    const string dataPath = "http://dreamlo.com/lb/6486be2b8f40bb7d84121bba/json";
    const int earliestDrivingHour = 8;
    const int latestDrivingHour = 22;
    const int minuteStep = 30;
    const int eventDays = 7;
    const int maxNameLength = 20;

    const string startEventDay = "startEventDay";
    const string startEventMonth = "startEventMonth";
    const string startEventYear = "startEventYear";

    enum eDate {
        TODAY = 0,
    }

    enum eDisplayedDrives {
        ALL = 0,
        TODAY = 1,
    }

    enum eTestDriveInfoDisplayed : int {
        DATE = 0,
        TIME = 1,
        NAME = 2,
    }
    enum eTestDriveInfoSaved: int {
        DATE_AND_TIME = 0,
        NAME = 1,
    }

    async void OnEnable() {
        modifier = new TestDriveLoader();
        refresh.onClick.AddListener(async () => await RefreshAndInsertData());
        submit.onClick.AddListener(OnSubmit);
        restart.onClick.AddListener(RestartFirstDayOfEvent);
        driveDate.onValueChanged.AddListener(delegate { ChangeDisplayedTime(); });
        driverName.onValueChanged.AddListener(delegate { OnNameChanged(); });
        registeredDates.onValueChanged.AddListener(delegate { DisplayRegisteredTestDrives(); });
        await RefreshAndInsertData();
        InsertDateAndTimeData();
    }

    void RestartFirstDayOfEvent() {
        PlayerPrefs.SetInt(startEventDay, DateTime.Now.Day);
        PlayerPrefs.SetInt(startEventMonth, DateTime.Now.Month);
        PlayerPrefs.SetInt(startEventYear, DateTime.Now.Year);
        InsertDateAndTimeData();
    }

    DateTime GetStartEventDate() {
        return new DateTime(PlayerPrefs.GetInt(startEventYear),
                            PlayerPrefs.GetInt(startEventMonth),
                            PlayerPrefs.GetInt(startEventDay));
    }

    void OnNameChanged() {
        driverName.text = driverName.text.Trim();
        if (driverName.text.Length > maxNameLength) {
            return;
        }
    }

    void ChangeDisplayedTime() {
        driveTime.ClearOptions();
        List<string> hoursList = new List<string>();
        DateTime earliestTime = DateTime.Now;
        if (driveDate.value == (int)eDate.TODAY) {
            earliestTime = GetEarliestTestDriveTimeFromDate(DateTime.Now);
        }
        else {
            earliestTime = DateTime.MinValue.AddHours(earliestDrivingHour);
        }
        for (DateTime time = earliestTime; time.Hour < latestDrivingHour; time = time.AddMinutes(minuteStep)) {
            hoursList.Add(time.ToString("HH:mm"));
        }
        driveTime.AddOptions(RemoveOccupiedHours(hoursList));
    }

    List<string> RemoveOccupiedHours(List<string> hours) {
        int selectedDateIndex = driveDate.value;
        if (selectedDateIndex < 1) {
            return hours;
        }
        string selectedDate = driveDate.options[selectedDateIndex].text;
        List<string> testDrives = GetRegisteredTestDrivesInfo();

        foreach (string testDrive in testDrives) {
            string testDriveDate = testDrive.Split(' ')[(int)eTestDriveInfoDisplayed.DATE];
            string testDriveTime = testDrive.Split(' ')[(int)eTestDriveInfoDisplayed.TIME];
            if (testDriveDate == selectedDate) {
                hours.Remove(testDriveTime);
            }
        }
        return hours;
    }

    List<string> GetRegisteredTestDrivesInfo() {
        List<string> testDrives = new List<string>();
        foreach (var testDrive in testDriveObjects) {
            TextMeshProUGUI textMeshPro = testDrive.GetComponentInChildren<TextMeshProUGUI>();
            string testDriveDate = textMeshPro.text;
            testDrives.Add(testDriveDate);
        }
        return testDrives;
    }

    List<string> GetRegisteredTestDrivesDates() {
        List<string> testDrives = GetRegisteredTestDrivesInfo();
        List<string> testDrivesDates = new List<string>();
        for (int i = 0; i < testDrives.Count; ++i) {
            string testDriveDate = testDrives[i].Split(' ')[(int)eTestDriveInfoDisplayed.DATE];
            testDrivesDates.Add(testDriveDate);
        }
        return testDrivesDates;
    }

    void InsertRegisteredTestDrivesDatesToDropdown() {
        registeredDates.ClearOptions();
        List<string> dropdownInfo = new List<string>();
        dropdownInfo.Add("All");
        dropdownInfo.Add("Today");
        List<string> testDrives = GetRegisteredTestDrivesDates();
        List<string> uniqueTestDrives = testDrives.Union(testDrives).ToList();

        foreach (var drive in uniqueTestDrives) {
            string driveDate = drive;
            DateTime driveDateTime;
            DateTime.TryParseExact(driveDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out driveDateTime);
            if (driveDateTime == DateTime.Now.Date) {
                uniqueTestDrives.Remove(driveDate);
                break;
            }
        }

        dropdownInfo.AddRange(uniqueTestDrives);
        registeredDates.AddOptions(dropdownInfo);
    }

    void DisplayRegisteredTestDrives() {
        List<string> dates = GetRegisteredTestDrivesDates();
        int selectedDateIndex = registeredDates.value;
        string selectedDate = registeredDates.options[registeredDates.value].text;
        foreach (var testDrive in testDriveObjects) {
            testDrive.SetActive(false);
        }

        for (int i = 0; i < dates.Count; ++i) {
            if (selectedDateIndex == (int)eDisplayedDrives.ALL) {
                testDriveObjects[i].SetActive(true);
            }
            else if (selectedDateIndex == (int)eDisplayedDrives.TODAY) {
                DateTime driveDateTime;
                DateTime.TryParseExact(dates[i], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out driveDateTime);
                if (driveDateTime == DateTime.Now.Date) {
                    testDriveObjects[i].SetActive(true);
                }
            }
            else if (selectedDate == dates[i]) {
                testDriveObjects[i].SetActive(true);
            }
        }
    }

    void InsertDateAndTimeData() {
        driveDate.ClearOptions();

        List<string> dateList = new List<string>();
        DateTime earliestDate = GetStartEventDate();
        if (earliestDate.Date == DateTime.Now.Date && earliestDate.Hour >= latestDrivingHour) {
            earliestDate = earliestDate.AddDays(1);
        }

        DateTime lastDate = earliestDate.AddDays(eventDays);
        for (DateTime date = DateTime.Now; date <= lastDate; date = date.AddDays(1)) {
            dateList.Add(date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        driveDate.AddOptions(dateList);
        ChangeDisplayedTime();
    }


    async void OnSubmit() {
        string date = driveDate.captionText.text;
        string time = driveTime.captionText.text;
        string name = driverName.text;

        if (date == null || date == String.Empty ||
            time == null || time == string.Empty) {
            Debug.Log("Enter proper data!");
            return;
        }

        string urlAddition = "/1";
        if (name != null && name != String.Empty) {
            urlAddition += "/0/" + name;
        }

        string formattedDate = ConvertInfoFromDisplayedToSaved(date + " " + time);
        string finalUrl = Path.Combine(modifyDataPath, "add/" + formattedDate + urlAddition);

        await modifier.UpdateData(finalUrl);
        AddTestDrive(date + " " + time + " " + name);
        EraseDropdownOptionAtIndex(driveTime, driveTime.value);
        if (driveTime.options.Count < 1) {
            EraseDropdownOptionAtIndex(driveDate, driveDate.value);
            ChangeDisplayedTime();
        }
        InsertRegisteredTestDrivesDatesToDropdown();
        DisplayRegisteredTestDrives();
    }

    async void OnDelete(GameObject item) {
        TextMeshProUGUI textMeshPro = item.GetComponentInChildren<TextMeshProUGUI>();
        string listItem = textMeshPro.text;
        string formattedDate = ConvertInfoFromDisplayedToSaved(listItem);
        string url = Path.Combine(modifyDataPath, "delete/" + formattedDate);

        await modifier.UpdateData(url);
        testDriveObjects.Remove(item);
        Destroy(item);
        await RefreshAndInsertData();
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

    async UniTask RefreshAndInsertData() {
        if (testDriveObjects.Count > 0) {
            foreach (var item in testDriveObjects) {
                Destroy(item);
            }
            testDriveObjects.Clear();
        }
        List<string> data = await modifier.GetData(dataPath);
        data.Sort();
        for (int i = 0; i < data.Count; ++i) {
            data[i] = ConvertInfoFromSavedToDisplayed(data[i]);
            AddTestDrive(data[i]);
        }
        InsertRegisteredTestDrivesDatesToDropdown();
        DisplayRegisteredTestDrives();
    }

    void OnDestroy() {
        foreach (var obj in testDriveObjects) {
            Destroy(obj);
        }
    }

    static void EraseDropdownOptionAtIndex(TMP_Dropdown dropdown, int index) {
        dropdown.options.RemoveAt(index);
        dropdown.RefreshShownValue();
    }

    static string ConvertInfoFromDisplayedToSaved(string date) {
        string[] parts = date.Split(' ');
        string dateString = parts[(int)eTestDriveInfoDisplayed.DATE] + " " + parts[(int)eTestDriveInfoDisplayed.TIME];
        string format = "dd/MM/yyyy HH:mm";
        string newFormat = "yyyy-MM-dd-HH-mm";
        DateTime dateTime = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
        return dateTime.ToString(newFormat, CultureInfo.InvariantCulture);
    }

    static string ConvertInfoFromSavedToDisplayed(string date) {
        string[] parts = date.Split(' ');
        string dateString = parts[(int)eTestDriveInfoSaved.DATE_AND_TIME];
        string format = "yyyy-MM-dd-HH-mm";
        string newFormat = "dd/MM/yyyy HH:mm";
        DateTime dateTime = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
        string result = dateTime.ToString(newFormat, CultureInfo.InvariantCulture);
        if (parts.Length > 1 && parts[1] != String.Empty) {
            result += " " + parts[(int)eTestDriveInfoSaved.NAME];
        }
        return result;
    }

    static DateTime GetEarliestTestDriveTimeFromDate(DateTime time) {
        if (time.Hour >= latestDrivingHour || time.Hour <= earliestDrivingHour) {
            time = time.AddDays(1);
            time = new DateTime(time.Year, time.Month, time.Day, earliestDrivingHour, 00, 00);
            return time;
        }
        if (time.Minute > minuteStep) {
            time = time.AddHours(1);
            time = new DateTime(time.Year, time.Month, time.Day, time.Hour, 00, 00);
        }
        else {
            time = time.AddMinutes(minuteStep - time.Minute);
        }
        return time;
    }
}
