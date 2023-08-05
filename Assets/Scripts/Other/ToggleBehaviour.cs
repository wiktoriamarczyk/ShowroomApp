using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleBehaviour : MonoBehaviour {
    [SerializeField] Color imageColorOn;
    [SerializeField] Color frameColorOn;
    [SerializeField] Image image;
    [SerializeField] Image frameImage;
    [SerializeField] UnityEvent onToggleOn;
    Color imageDefaultColor;
    Color frameDefaultColor;
    bool isOn = false;

    public void OnToggleValueChanged(bool value) {
        if (value) {
            onToggleOn?.Invoke();
            ToggleOn();
        }
        else {
            ToggleOff();
        }
    }

    void SetColors(Color imgColor, Color frameColor) {
        if (image != null)
            image.color = imgColor;
        if (frameImage != null)
            frameImage.color = frameColor;
    }

    public void ToggleOn() {
        if (isOn) {
            return;
        }
        SetColors(imageColorOn, frameColorOn);
        isOn = true;
    }

    public void ToggleOff() {
        if (!isOn) {
            return;
        }
        SetColors(imageDefaultColor, frameDefaultColor);
        isOn = false;
    }

    public void SetDefaultColor(Color color) {
        imageDefaultColor = color;
        frameDefaultColor = color;
        if (!isOn) {
            SetColors(imageDefaultColor, frameDefaultColor);
        }
    }

    void Awake() {
        if (image == null) {
            image = GetComponent<Image>();
        }
        if (image != null) {
            imageDefaultColor = image.color;
        }
        if (frameImage != null) {
            frameDefaultColor = frameImage.color;
        }
        Toggle toggle = GetComponent<Toggle>();
        if (toggle)
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        else {
            int i = 0;
        }
    }

}
