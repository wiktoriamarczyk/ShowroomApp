using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VersionData : ScriptableObject {
    public abstract string description { get; }
    public abstract IReadOnlyList<string> drives { get; }
    public abstract IReadOnlyList<string> packages { get; }
    public abstract IReadOnlyDictionary<string, string> colorsData { get; }
    public abstract IReadOnlyDictionary<string, string> rimsData { get; }
}
