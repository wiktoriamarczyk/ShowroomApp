using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Common;

[CreateAssetMenu(fileName = "PackageData", menuName = "ScriptableObjects/PackageData", order = 4)]
public class PackageData : LocalizableData {
    [SerializeField]
    Common.ePackage packageType = ePackage.WINTER;

    public Common.ePackage packageTypeProperty => packageType;

    [SerializeField]
    bool optional = true;

    public bool optionalProperty => optional;
}
