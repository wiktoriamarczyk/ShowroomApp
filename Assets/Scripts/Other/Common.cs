using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class Common {
    /* ----------COMMON CONST---------- */
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

    public const float dreamloDebugDelay = 2f;
    public const int maxInputLength = 20;

    /* ----------COMMON TYPES---------- */
    public enum ePopupType {
        DEFAULT,
        INPUT_FIELD
    }
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
        BLACK_STONE,
        ICE_WHITE,
        CLOUD_BLUE,
        RACE_RED,
        MAGENTA_FUSION,
        ITS_LIME_GREEN
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
    public class Item<T> {
        public T type;
        public string localizationTableKey;
        public string hex;
        public bool optional;

        public void UpdateHex(string newHex) {
            this.hex = newHex;
        }
    }
    public struct VersionInfo {
        eVersion type;
        string name;
        string desciprion;
    }

    public readonly static IReadOnlyDictionary<eVersion, string> versions = new Dictionary<eVersion, string> {
        { eVersion.REACT, "React" },
        { eVersion.SHARP, "Sharp" },
        { eVersion.UNITY, "Unity" },
    };
    public readonly static IReadOnlyDictionary<eVersion, string> descriptions = new Dictionary<eVersion, string> {
        { eVersion.REACT, "React Version Text" },
        { eVersion.SHARP, "Sharp Version Text" },
        { eVersion.UNITY, "Unity Version Text" },
    };
    public readonly static IReadOnlyDictionary<eDrive, string> drives = new Dictionary<eDrive, string> {
        { eDrive.T3_MANUAL,             "T3 manual" },
        { eDrive.T3_AUTOMATIC,          "T3 automatic" },
        { eDrive.B4_MILD_HYBRID,        "B4 mild hybrid" },
        { eDrive.B4_AWD_MILD_HYBRID,    "B4 AWD mild hybrid" },
        { eDrive.B5_AWD_MILD_HYBRID,    "B5 AWD mild hybrid" }
    };
    public readonly static IReadOnlyDictionary<ePackage, string> packages = new Dictionary<ePackage, string> {
        { ePackage.WINTER,      "Winter Package" },
        { ePackage.PARKING,     "Parking Package" },
        { ePackage.TECHNOLOGY,  "Technology Package" },
        { ePackage.LIGHTNING,   "Lightning Package" }
    };
    public readonly static IReadOnlyList<Item<eColor>> colors = new List<Item<eColor>> {
        { new Item<eColor>{ type = eColor.BLACK_STONE,     localizationTableKey = "Black Stone Color",     hex = "0x111111",   optional = false } },
        { new Item<eColor>{ type = eColor.ICE_WHITE,       localizationTableKey = "Ice White Color",       hex = "0xEEEEEE",   optional = false } },
        { new Item<eColor>{ type = eColor.CLOUD_BLUE,      localizationTableKey = "Cloud Blue Color",      hex = "0x55BFD4",   optional = true } },
        { new Item<eColor>{ type = eColor.RACE_RED,        localizationTableKey = "Race Red Color",        hex = "0xBD162C",   optional = true } },
        { new Item<eColor>{ type = eColor.MAGENTA_FUSION,  localizationTableKey = "Magenta Fusion Color",  hex = "0xFF00FF",   optional = true } },
        { new Item<eColor>{ type = eColor.ITS_LIME_GREEN,  localizationTableKey = "Its Lime Green Color",  hex = "0x32CD32",   optional = true } },
    };
    public readonly static IReadOnlyList<Item<eRims>> rims = new List<Item<eRims>> {
        { new Item<eRims>{ type = eRims.BLACK,               localizationTableKey = "Black Color",           hex = "0x111111", optional = false } },
        { new Item<eRims>{ type = eRims.METAL,               localizationTableKey = "Metal Color",           hex = "0xEEEEEE", optional = false } },
        { new Item<eRims>{ type = eRims.COLORMATCH,          localizationTableKey = "ColorMatch Color",      hex = "0x000000", optional = false } },
    };

    /* ----------COMMON FUNCTIONS---------- */
    public static Item<eColor> FindColorByType(eColor type) {
        return Common.colors.FirstOrDefault(color => color.type == type);
    }
    public static Item<eColor> FindColorByName(string name) {
        return Common.colors.FirstOrDefault(color => color.localizationTableKey == name);
    }
    public static Item<eRims> FindRimByType(eRims type) {
        return Common.rims.FirstOrDefault(rims => rims.type == type);
    }
    public static Color ColorFromHex(string col) {
        int intColor = Convert.ToInt32(col, 16); ;

        float R = ((0xFF0000 & intColor) >> 16) / 255.0f;
        float G = ((0xFF00 & intColor) >> 8) / 255.0f;
        float B = ((0xFF & intColor) >> 0) / 255.0f;
        return new Color(R, G, B);
    }
    public static string GetDefaultConfigName() {
        return defaultConfigName + " " + GetConfigurationCount();
    }

    public static int GetConfigurationCount() {
        return PlayerPrefs.GetInt(playerPrefsConfigCountName);
    }
    public static void SetConfigurationCount(int value) {
        PlayerPrefs.SetInt(playerPrefsConfigCountName, value);
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

}
