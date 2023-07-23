using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "UnityVersionData", menuName = "ScriptableObjects/UnityVersionData", order = 3)]
public class UnityVersionData : VersionData {
    public override string description => Common.descriptions[Common.eVersion.UNITY];

    public override IReadOnlyList<string> drives { get; } = new[] {
        Common.drives[Common.eDrive.T3_AUTOMATIC],
        Common.drives[Common.eDrive.B4_MILD_HYBRID],
        Common.drives[Common.eDrive.B4_AWD_MILD_HYBRID],
        Common.drives[Common.eDrive.B5_AWD_MILD_HYBRID],
    };

    public override IReadOnlyList<string> packages { get; } = new[] {
        Common.packages[Common.ePackage.WINTER],
        Common.packages[Common.ePackage.PARKING],
        Common.packages[Common.ePackage.TECHNOLOGY],
        Common.packages[Common.ePackage.LIGHTNING],
    };

    static Common.Item<Common.eColor> blackStone = Common.FindColorByType(Common.eColor.BLACK_STONE);
    static Common.Item<Common.eColor> iceWhite = Common.FindColorByType(Common.eColor.ICE_WHITE);
    static Common.Item<Common.eColor> cloudBlue = Common.FindColorByType(Common.eColor.CLOUD_BLUE);
    static Common.Item<Common.eColor> raceRed = Common.FindColorByType(Common.eColor.RACE_RED);
    static Common.Item<Common.eColor> magentaFusion = Common.FindColorByType(Common.eColor.MAGENTA_FUSION);
    static Common.Item<Common.eColor> limeGreen = Common.FindColorByType(Common.eColor.ITS_LIME_GREEN);

    public override IReadOnlyDictionary<string, string> colorsData { get; } = new Dictionary<string, string>() {
        { blackStone.localizationTableKey, blackStone.hex },
        { iceWhite.localizationTableKey, iceWhite.hex },
        { cloudBlue.localizationTableKey, cloudBlue.hex },
        { raceRed.localizationTableKey, raceRed.hex },
        { magentaFusion.localizationTableKey, magentaFusion.hex },
        { limeGreen.localizationTableKey, limeGreen.hex }
    };

    static Common.Item<Common.eRim> black = Common.FindRimByType(Common.eRim.BLACK);
    static Common.Item<Common.eRim> metal = Common.FindRimByType(Common.eRim.METAL);
    static Common.Item<Common.eRim> colorMatch = Common.FindRimByType(Common.eRim.COLORMATCH);

    public override IReadOnlyDictionary<string, string> rimsData { get; } = new Dictionary<string, string>() {
        { black.localizationTableKey, black.hex },
        { metal.localizationTableKey, metal.hex },
        { colorMatch.localizationTableKey, colorMatch.hex }
    };
}
