using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizableData : ScriptableObject {

    [SerializeField]
    string localizationTableKey;

    public string localizationTableKeyProperty => localizationTableKey;

}
