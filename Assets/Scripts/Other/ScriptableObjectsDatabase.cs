using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectsDatabase : MonoBehaviour {
    [SerializeField] List<ScriptableObject> objects;

    public List<ScriptableObject> objectsProperty => objects;
}
