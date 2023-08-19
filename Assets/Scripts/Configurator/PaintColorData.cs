using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Common;


[CreateAssetMenu(fileName = "PaintColorData", menuName = "ScriptableObjects/PaintColorData", order = 3)]
public class PaintColorData : LocalizableData {
    [SerializeField]
    Common.eColor colorType = eColor.BLACK_STONE;

    public Common.eColor colorTypeProperty => colorType;

    [SerializeField]
    Color color;

    public Color colorProperty => color;

    public void SetColor(Color color) {
        this.color = color;
    }

    [SerializeField]
    float metallic = 0.0f;

    public float metallicProperty => metallic;

    [SerializeField]
    float smoothness = 1.0f;

    public float smoothnessProperty => smoothness;

    [SerializeField]
    bool optional = false;

    public bool optionalProperty => optional;
}
