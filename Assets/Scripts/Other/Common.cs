using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Common {
    public const string localizationTableName = "UI Text";
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
    public enum eRim {
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
    public struct Item<T> {
        public T type;
        public string localizationTableKey;
        public string hex;
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
        { new Item<eColor>{ type = eColor.BLACK_STONE,     localizationTableKey = "Black Stone Color",     hex = "0x000000" } },
        { new Item<eColor>{ type = eColor.ICE_WHITE,       localizationTableKey = "Ice White Color",       hex = "0xFFFFFF" } },
        { new Item<eColor>{ type = eColor.CLOUD_BLUE,      localizationTableKey = "Cloud Blue Color",      hex = "0xA2B6B9" } },
        { new Item<eColor>{ type = eColor.RACE_RED,        localizationTableKey = "Race Red Color",        hex = "0xBD162C" } },
        { new Item<eColor>{ type = eColor.MAGENTA_FUSION,  localizationTableKey = "Magenta Fusion Color",  hex = "0xFF00FF" } },
        { new Item<eColor>{ type = eColor.ITS_LIME_GREEN,  localizationTableKey = "Its Lime Green Color",  hex = "0x32CD32" } },
    };
    public readonly static IReadOnlyList<Item<eRim>> rims = new List<Item<eRim>> {
        { new Item<eRim>{ type = eRim.BLACK,               localizationTableKey = "Black Color",           hex = "0x000000" } },
        { new Item<eRim>{ type = eRim.METAL,               localizationTableKey = "Metal Color",           hex = "0xFFFFFF" } },
        { new Item<eRim>{ type = eRim.COLORMATCH,          localizationTableKey = "ColorMatch Color",      hex = "0xA2B6B9" } },
    };
    public static Item<eColor> FindColorByType(eColor type) {
        return Common.colors.FirstOrDefault(color => color.type == type);
    }
    public static Item<eRim> FindRimByType(eRim type) {
        return Common.rims.FirstOrDefault(rim => rim.type == type);
    }

    public static Color ColorFromHex(string col) {
        int intColor = Convert.ToInt32(col, 16); ;

        float R = ((0xFF0000 & intColor) >> 16) / 255.0f;
        float G = ((0xFF00 & intColor) >> 8) / 255.0f;
        float B = ((0xFF & intColor) >> 0) / 255.0f;
        return new Color(R, G, B);
    }
}
