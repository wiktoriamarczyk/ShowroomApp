using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
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
    [SerializeField] GameObject loaderObject;

    TestDriveLoader modifier;
    List<GameObject> testDriveObjects = new List<GameObject>();
    CancellationTokenSource cancelTokenSrc;
    CancellationToken cancelToken;
    eDisplayedDrives currDisplayedDrives = eDisplayedDrives.ALL;
    Dictionary<eDisplayedDrives, string> displayedDrivesLocalization = new Dictionary<eDisplayedDrives, string>() {
        { eDisplayedDrives.ALL, Common.localizationAll },
        { eDisplayedDrives.TODAY, Common.localizationToday },
    };

    const string modifyDataPath = "http://dreamlo.com/lb/-Ur0ruQAokKXyyv8uxxT0wOw8r3LFlWUyISb24jdTvEw/";
    const string dataPath = "http://dreamlo.com/lb/6486be2b8f40bb7d84121bba/json";


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

    void Awake() {
        modifier = new TestDriveLoader();
        LocaleSelector.onLanguageChanged += UpdateLocalization;
        refresh.onClick.AddListener(async () => await RefreshAndInsertRegisteredData());
        submit.onClick.AddListener(async () => await OnSubmit());
        restart.onClick.AddListener(RestartFirstDayOfEvent);
        driveDate.onValueChanged.AddListener(delegate { RefreshTimeDropdown(); });
        driverName.onValueChanged.AddListener(delegate { OnNameChanged(); });
        registeredDates.onValueChanged.AddListener(delegate { DisplayRegisteredTestDrives(); });
    }

    async void OnEnable() {
        cancelTokenSrc = new CancellationTokenSource();
        cancelToken = cancelTokenSrc.Token;
        await RefreshAndInsertRegisteredData();
        InsertDateAndTimeData();
    }

    void RestartFirstDayOfEvent() {
        Common.RestartFirstDayOfEvent();
        InsertDateAndTimeData();
    }


    void OnNameChanged() {
        driverName.text = driverName.text.Trim();
        if (driverName.text.Length > Common.maxInputLength) {
            return;
        }
    }

    void RefreshTimeDropdown() {
        driveTime.ClearOptions();
        List<string> hoursList = new List<string>();
        DateTime earliestTime = DateTime.Now;
        if (earliestTime.Hour >= Common.latestDrivingHour || earliestTime.Hour <= Common.earliestDrivingHour) {
            EraseDropdownOptionAtIndex(driveDate, (int)eDate.TODAY);
        }
        if (driveDate.value == (int)eDate.TODAY) {
            earliestTime = GetEarliestTestDriveTimeFromDate(DateTime.Now);
        }
        else {
            earliestTime = DateTime.MinValue.AddHours(Common.earliestDrivingHour);
        }
        for (DateTime time = earliestTime; time.Hour < Common.latestDrivingHour; time = time.AddMinutes(Common.minuteStep)) {
            hoursList.Add(time.ToString("HH:mm"));
        }
        driveTime.AddOptions(RemoveOccupiedHours(hoursList));
    }

    List<string> RemoveOccupiedHours(List<string> hours) {
        int selectedDateIndex = driveDate.value;
        if (selectedDateIndex < 0) {
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

    async void InsertRegisteredTestDrivesDatesToDropdown() {
        registeredDates.ClearOptions();
        List<string> dropdownInfo = new List<string>();
        string dropdownAllOption = await Common.GetLocalizationEntry(Common.localizationAll);
        string dropdownTodayOption = await Common.GetLocalizationEntry(Common.localizationToday);
        dropdownInfo.Add(dropdownAllOption);
        dropdownInfo.Add(dropdownTodayOption);

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

    async void UpdateLocalization() {
        await UpdateLocalizationAsync();
    }

    async UniTask UpdateLocalizationAsync() {
        registeredDates.options[(int)eDisplayedDrives.ALL].text =
            await Common.GetLocalizationEntry(displayedDrivesLocalization[eDisplayedDrives.ALL]);
        registeredDates.options[(int)eDisplayedDrives.TODAY].text =
            await Common.GetLocalizationEntry(displayedDrivesLocalization[eDisplayedDrives.TODAY]);
        string selectedText = registeredDates.options[registeredDates.value].text;
        string localizationVersion = await Common.GetLocalizationEntry(displayedDrivesLocalization[currDisplayedDrives]);
        registeredDates.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = localizationVersion;
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
                currDisplayedDrives = eDisplayedDrives.ALL;
            }
            else if (selectedDateIndex == (int)eDisplayedDrives.TODAY) {
                DateTime driveDateTime;
                DateTime.TryParseExact(dates[i], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out driveDateTime);
                if (driveDateTime == DateTime.Now.Date) {
                    testDriveObjects[i].SetActive(true);
                }
                currDisplayedDrives = eDisplayedDrives.TODAY;
            }
            else if (selectedDate == dates[i]) {
                testDriveObjects[i].SetActive(true);
            }
        }
    }

    void InsertDateAndTimeData() {
        driveDate.ClearOptions();

        List<string> dateList = new List<string>();
        DateTime earliestDate = Common.GetStartEventDate();
        if (earliestDate.Date == DateTime.Now.Date && earliestDate.Hour >= Common.latestDrivingHour) {
            earliestDate = earliestDate.AddDays(1);
        }

        DateTime lastDate = earliestDate.AddDays(Common.eventDays);
        for (DateTime date = DateTime.Now; date <= lastDate; date = date.AddDays(1)) {
            dateList.Add(date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        driveDate.AddOptions(dateList);
        RefreshTimeDropdown();
    }

    async UniTask OnSubmit() {
        string date = driveDate.captionText.text;
        string time = driveTime.captionText.text;
        string name = driverName.text;
        if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(time)) {
            await PanelManager.instance.ShowPopup2<PopupController>(p => p.Init(Common.localizationIncorectDataWarning));
            return;
        }

        string urlAddition = "/1";
        if (name != null && name != String.Empty) {
            urlAddition += "/0/" + name;
        }

        string formattedDate = ConvertInfoFromDisplayedToSaved(date + " " + time);
        string finalUrl = Path.Combine(modifyDataPath, "add/" + formattedDate + urlAddition);

        bool webOperationResult = await SendDataToWeb(finalUrl);
        if (!webOperationResult) {
            await PanelManager.instance.ShowPopup2<PopupController>(p => p.Init(Common.localizationOperationFailedWarning));
            return;
        }

        AddTestDrive(date + " " + time + " " + name);

        EraseDropdownOptionAtIndex(driveTime, driveTime.value);
        if (driveTime.options.Count < 1) {
            EraseDropdownOptionAtIndex(driveDate, driveDate.value);
            RefreshTimeDropdown();
        }
        InsertRegisteredTestDrivesDatesToDropdown();
        DisplayRegisteredTestDrives();
    }

    async UniTask ShowDeletePopup(GameObject item) {
        var popup = await PanelManager.instance.ShowPopup2<PopupController>(p => p.Init(Common.localizationDeleteWarning));
        if (popup.result) {
            await OnDelete(item);
        }
    }

    async UniTask OnDelete(GameObject item) {
        TextMeshProUGUI textMeshPro = item.GetComponentInChildren<TextMeshProUGUI>();
        string itemDateAndTime = textMeshPro.text;
        string formattedDate = ConvertInfoFromDisplayedToSaved(itemDateAndTime);
        string url = Path.Combine(modifyDataPath, "delete/" + formattedDate);
        bool webOperationResult = await SendDataToWeb(url);
        if (!webOperationResult) {
            await PanelManager.instance.ShowPopup2<PopupController>(p => p.Init(Common.localizationOperationFailedWarning));
            return;
        }
        testDriveObjects.Remove(item);
        Destroy(item);
        await RefreshAndInsertRegisteredData();
        RefreshTimeDropdown();
    }

    void AddTestDrive(string testDriveInfo) {
        GameObject testDriveItem = Instantiate(testDriveItemPrefab);
        testDriveItem.name = "TestDriveItem";
        TextMeshProUGUI textMeshPro = testDriveItem.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.text = testDriveInfo;
        Button deleteButton = testDriveItem.GetComponentInChildren<Button>();
        deleteButton.onClick.AddListener(async () => { await ShowDeletePopup(testDriveItem); });
        testDriveItem.transform.SetParent(testDriveList.transform);
        testDriveItem.transform.localScale = Vector3.one;
        testDriveObjects.Add(testDriveItem);
    }

    async UniTask RefreshAndInsertRegisteredData() {
        DestroyObjects();

        List<string> data = await GetWebData(dataPath);
        data.Sort();
        for (int i = 0; i < data.Count; ++i) {
            data[i] = ConvertInfoFromSavedToDisplayed(data[i]);
            AddTestDrive(data[i]);
        }
        InsertRegisteredTestDrivesDatesToDropdown();
        DisplayRegisteredTestDrives();
    }

    void DestroyObjects() {
        foreach (var obj in testDriveObjects) {
            Destroy(obj);
        }
        testDriveObjects.Clear();
    }

    void ActivateLoader() {
        loaderObject.SetActive(true);
        loaderObject.GetComponent<Loader>().StartLoader();
    }

    void DeactivateLoader() {
        loaderObject.GetComponent<Loader>().StopLoader();
        loaderObject.SetActive(false);
    }

    async UniTask<List<string>> GetWebData(string url) {
        ActivateLoader();
        List<string> data = new List<string>();
        try {
            data = await modifier.GetData(dataPath, cancelToken);
        }
         catch (System.OperationCanceledException) {
            Debug.Log("Test drive operation canceled");
        }
        DeactivateLoader();
        return data;
    }

    async UniTask<bool> SendDataToWeb(string url) {
        ActivateLoader();
        bool result = false;
        try {
            result = await modifier.UpdateData(url, cancelToken);
        }
        catch (System.OperationCanceledException) {
            Debug.Log("Test drive operation canceled");
        }
        DeactivateLoader();
        return result;
    }

    void OnDisable() {
        cancelTokenSrc.Cancel();
        cancelTokenSrc.Dispose();
        DestroyObjects();
    }

    static void EraseDropdownOptionAtIndex(TMP_Dropdown dropdown, int index) {
        dropdown.options.RemoveAt(index);
        dropdown.RefreshShownValue();
    }

    static string ReturnOnlyDate(string date) {
        string[] parts = date.Split(' ');
        string dateString = parts[(int)eTestDriveInfoDisplayed.DATE];
        return dateString;
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
        if (time.Hour >= Common.latestDrivingHour || time.Hour <= Common.earliestDrivingHour) {
            time = time.AddDays(1);
            time = new DateTime(time.Year, time.Month, time.Day, Common.earliestDrivingHour, 00, 00);
            return time;
        }
        if (time.Minute > Common.minuteStep) {
            time = time.AddHours(1);
            time = new DateTime(time.Year, time.Month, time.Day, time.Hour, 00, 00);
        }
        else {
            time = time.AddMinutes(Common.minuteStep - time.Minute);
        }
        return time;
    }
}
