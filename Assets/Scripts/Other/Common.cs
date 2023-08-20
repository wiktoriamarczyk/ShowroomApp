using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class Common {
    public const string localizationTableName = "UI Text";
    public const string localizationDeleteWarning = "Data Deletion Warning";
    public const string localizationIncorectDataWarning = "Incorrect Data Warning";
    public const string localizationOverrideDataWarning = "Override Data Warning";
    public const string localizationOperationFailedWarning = "Operation Failed Warning";
    public const string localizationConfigNameInfo= "Enter Config Name";
    public const string localizationAll = "All";
    public const string localizationToday = "Today";

    public const string defaultConfigName = "Config";
    public const string playerPrefsConfigCountName = "ConfigurationCount";
    public const string startEventDay = "startEventDay";
    public const string startEventMonth = "startEventMonth";
    public const string startEventYear = "startEventYear";
    public const int    eventDays = 7;
    public const int    earliestDrivingHour = 8;
    public const int    latestDrivingHour = 22;
    public const int    minuteStep = 30;

    public const float dreamloDebugDelay = 2f;
    public const int   maxInputLength = 20;

    public enum eConfigurationType {
        VERSION,
        DRIVE,
        COLOR,
        RIMS,
        PACKAGE
    }
    public enum eVersion {
        REACT,
        SHARP,
        UNITY
    }
    public enum eDrive {
        T3_MANUAL,
        T3_AUTOMATIC,
        B4_MILD_HYBRID,
        B4_AWD_MILD_HYBRID,
        B5_AWD_MILD_HYBRID
    };
    public enum eColor {
        UNDEFINED,
        BLACK_STONE,
        ICE_WHITE,
        CLOUD_BLUE,
        RACE_RED,
        MAGENTA_FUSION,
        ITS_LIME_GREEN,
        BLACK,
        METAL,
        COLORMATCH
    };
    public enum eRims {
        BLACK,
        METAL,
        COLORMATCH
    }
    public enum ePackage {
        WINTER,
        PARKING,
        TECHNOLOGY,
        LIGHTNING
    }
    public enum eColorUsage {
        UNDEFINED,
        BODY,
        RIMS
    }

    public struct VersionInfo {
        eVersion type;
        string name;
        string desciprion;
    }

    static public List<VersionData> versionsData;
    static public List<BodyPaintColorData> bodyPaintColorsData;
    static public List<PaintColorData> rimsPaintColorsData;
    static public List<DriveData> drivesData;
    static public List<PackageData> packagesData;

    static Common() {
        versionsData = Common.GetAllScriptableObejctsInstances<VersionData>();
        bodyPaintColorsData = Common.GetAllScriptableObejctsInstances<BodyPaintColorData>();
        bodyPaintColorsData.Sort((a, b) => a.colorTypeProperty.CompareTo(b.colorTypeProperty));
        bodyPaintColorsData = bodyPaintColorsData.Where(color => color.colorUsageProperty == eColorUsage.BODY).ToList();
        rimsPaintColorsData = Common.GetAllScriptableObejctsInstances<PaintColorData>();
        rimsPaintColorsData.Sort((a, b) => a.colorTypeProperty.CompareTo(b.colorTypeProperty));
        rimsPaintColorsData = rimsPaintColorsData.Where(color => color.colorUsageProperty == eColorUsage.RIMS).ToList();
        drivesData = Common.GetAllScriptableObejctsInstances<DriveData>();
        drivesData.Sort((a, b) => a.driveTypeProperty.CompareTo(b.driveTypeProperty));
        packagesData = Common.GetAllScriptableObejctsInstances<PackageData>();
        packagesData.Sort((a, b) => a.packageTypeProperty.CompareTo(b.packageTypeProperty));

        TimeSpan duration = DateTime.Now - GetStartEventDate();
        if (duration.Days >= eventDays || duration.Days < 0) {
            RestartFirstDayOfEvent();
        }
    }

    public static BodyPaintColorData FindBodyColorByType(eColor type) {
        return bodyPaintColorsData.FirstOrDefault(color => color.colorTypeProperty == type);
    }

    public static PaintColorData FindRimsColorByType(eColor type) {
        return rimsPaintColorsData.FirstOrDefault(color => color.colorTypeProperty == type);
    }

    public static string GetDefaultConfigName() {
        return defaultConfigName + " " + GetConfigurationCount();
    }

    /* ---PLAYER PREFS--- */
    public static int GetConfigurationCount() {
        return PlayerPrefs.GetInt(playerPrefsConfigCountName);
    }
    public static void SetConfigurationCount(int value) {
        PlayerPrefs.SetInt(playerPrefsConfigCountName, value);
    }
    public static void RestartFirstDayOfEvent() {
        PlayerPrefs.SetInt(startEventDay, DateTime.Now.Day);
        PlayerPrefs.SetInt(startEventMonth, DateTime.Now.Month);
        PlayerPrefs.SetInt(startEventYear, DateTime.Now.Year);
    }
    public static DateTime GetStartEventDate() {
        return new DateTime(PlayerPrefs.GetInt(startEventYear),
                            PlayerPrefs.GetInt(startEventMonth),
                            PlayerPrefs.GetInt(startEventDay));
    }

    public static async UniTask<string> GetLocalizationEntry(string localizationKey) {
        string localizationTableKey = localizationKey;
        string text = String.Empty;
        var operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(Common.localizationTableName, localizationTableKey, new object[] { text });

        await operation;

        if (operation.Status == AsyncOperationStatus.Succeeded) {
            return operation.Result;
        }
        else {
            Debug.Log("Failed to get localization entry!");
            return String.Empty;
        }
    }
    public static List<T> GetAllScriptableObejctsInstances<T>() where T : ScriptableObject {
        string[] guids = AssetDatabase.FindAssets($"t: {typeof(T).Name}"); //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++) //probably could get optimized
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
        }

        return a.ToList();
    }

}
