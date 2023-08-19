using UnityEngine;

[CreateAssetMenu(fileName = "BodyPaintColorData", menuName = "ScriptableObjects/BodyPaintColorData", order = 4)]
public class BodyPaintColorData : PaintColorData {
    [SerializeField] PaintColorData seamsColor;
    public PaintColorData seamsColorProperty => seamsColor;
}
