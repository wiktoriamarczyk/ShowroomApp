using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DriveData", menuName = "ScriptableObjects/DriveData", order = 2)]
public class DriveData : LocalizableData {
    [SerializeField]
    Common.eDrive driveType = Common.eDrive.T3_MANUAL;

    public Common.eDrive driveTypeProperty => driveType;

    [SerializeField]
    bool optional = false;

    public bool optionalProperty => optional;
}
