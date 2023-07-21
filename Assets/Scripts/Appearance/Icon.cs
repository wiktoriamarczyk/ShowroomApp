using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour {
    [SerializeField] Color iconDefaultColor;
    [SerializeField] Color iconAlternativeColor = Color.white;

    public void ChangeToDefaultColor() {
        var image = GetComponent<Image>();
        image.color = iconDefaultColor;
    }

    public void ChangeToAlternativeColor() {
        var image = GetComponent<Image>();
        image.color = iconAlternativeColor;
    }
}
