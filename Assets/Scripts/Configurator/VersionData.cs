using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[CreateAssetMenu(fileName = "VersionData", menuName = "ScriptableObjects/VersionData", order = 1)]
public class VersionData : LocalizableData {

    [SerializeField]
    public Common.eVersion versionType = Common.eVersion.UNITY;

    [SerializeField]
    string descriptionLocalizationKey;

    public string descriptionLocalizationKeyProperty => descriptionLocalizationKey;

    [SerializeField]
    List<DriveData> drives = new List<DriveData>();

    public List<DriveData> drivesProperty => drives;

    [SerializeField]
    List<PackageData> packages = new List<PackageData>();

    public List<PackageData> packagesProperty => packages;

    [SerializeField]
    List<PaintColorData> colorsData = new List<PaintColorData>();

    public List<PaintColorData> colorsDataProperty => colorsData;

    [SerializeField]
    List<PaintColorData> rimsData = new List<PaintColorData>();

    public List<PaintColorData> rimsDataProperty => rimsData;
}
