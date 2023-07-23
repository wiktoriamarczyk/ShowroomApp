using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SharpVersionData", menuName = "ScriptableObjects/SharpVersionData", order = 2)]
public class SharpVersionData : VersionData {
    public override string description => Common.descriptions[Common.eVersion.SHARP];

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

    public override IReadOnlyDictionary<string, string> colorsData { get; } = new Dictionary<string, string>() {
        { blackStone.localizationTableKey, blackStone.hex },
        { iceWhite.localizationTableKey, iceWhite.hex },
        { cloudBlue.localizationTableKey, cloudBlue.hex },
        { raceRed.localizationTableKey, raceRed.hex },
        { magentaFusion.localizationTableKey, magentaFusion.hex }
    };

    static Common.Item<Common.eRim> black = Common.FindRimByType(Common.eRim.BLACK);
    static Common.Item<Common.eRim> metal = Common.FindRimByType(Common.eRim.METAL);

    public override IReadOnlyDictionary<string, string> rimsData { get; } = new Dictionary<string, string>() {
        { black.localizationTableKey, black.hex },
        { metal.localizationTableKey, metal.hex }
    };
}
