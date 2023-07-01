using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour
{
    [SerializeField] Color iconDefaultColor = new Color(101, 101, 101);
    [SerializeField] Color iconAlternativeColor = Color.white;

    void Awake() {
        iconDefaultColor = ConvertColor(iconDefaultColor);
    }

    Color ConvertColor(Color color) {
        float red = color.r / 255f;
        float green = color.g / 255f;
        float blue = color.b / 255f;

        return new Color(red, green, blue);
    }

    public void ChangeToDefaultColor() {
        var image = GetComponent<Image>();
        image.color = iconDefaultColor;
    }

    public void ChangeToAlternativeColor() {
        var image = GetComponent<Image>();
        image.color = iconAlternativeColor;
    }
}
