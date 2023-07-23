using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ReactVersionData", menuName = "ScriptableObjects/ReactVersionData", order = 1)]
public class ReactVersionData : VersionData {
    public override string version => Common.descriptions[Common.eVersion.REACT];
    public override string description => Common.descriptions[Common.eVersion.REACT];

    public override IReadOnlyList<string> drives { get; } = new[] {
        Common.drives[Common.eDrive.T3_MANUAL],
        Common.drives[Common.eDrive.T3_AUTOMATIC],
    };

    public override IReadOnlyList<string> packages { get; } = new[] {
        Common.packages[Common.ePackage.WINTER],
        Common.packages[Common.ePackage.PARKING],
    };

    static Common.Item<Common.eColor> blackStone = Common.FindColorByType(Common.eColor.BLACK_STONE);
    static Common.Item<Common.eColor> iceWhite = Common.FindColorByType(Common.eColor.ICE_WHITE);
    static Common.Item<Common.eColor> cloudBlue = Common.FindColorByType(Common.eColor.CLOUD_BLUE);

    public override IReadOnlyDictionary<string, string> colorsData { get; } = new Dictionary<string, string>() {
        { blackStone.localizationTableKey, blackStone.hex },
        { iceWhite.localizationTableKey, iceWhite.hex },
        { cloudBlue.localizationTableKey, cloudBlue.hex }
    };

    static Common.Item<Common.eRim> black = Common.FindRimByType(Common.eRim.BLACK);
    static Common.Item<Common.eRim> metal = Common.FindRimByType(Common.eRim.METAL);

    public override IReadOnlyDictionary<string, string> rimsData { get; } = new Dictionary<string, string>() {
        { black.localizationTableKey, black.hex },
        { metal.localizationTableKey, metal.hex }
    };
}
