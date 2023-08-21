using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleBehaviour : MonoBehaviour {
    [SerializeField] Color imageColorOn;
    [SerializeField] Color frameColorOn;
    [SerializeField] Image image;
    [SerializeField] Image frameImage;
    [SerializeField] UnityEvent onToggleOn;
    [SerializeField] UnityEvent onToggleOff;
    [SerializeField] float onThickness = 10f;
    Color imageDefaultColor;
    Color frameDefaultColor;
    float frameDefaultThickness;
    bool isOn = false;
    Icon icon;

    public void OnToggleValueChanged(bool value) {
        if (value) {
            onToggleOn?.Invoke();
            ToggleOn();
        }
        else {
            onToggleOff?.Invoke();
            ToggleOff();
        }
    }

    public void ToggleOn() {
        if (isOn) {
            return;
        }
        icon?.ChangeToAlternativeColor();
        SetColors(imageColorOn, frameColorOn);
        ChangeOutlineThickness(onThickness);
        isOn = true;
    }

    public void ToggleOff() {
        if (!isOn) {
            return;
        }
        SetColors(imageDefaultColor, frameDefaultColor);
        icon?.ChangeToDefaultColor();
        ChangeOutlineThickness(frameDefaultThickness);
        isOn = false;
    }

    void ChangeOutlineThickness(float thickness) {
        if (frameImage != null) {
            frameImage.material.SetFloat("_Thickness",thickness);
        }
    }

    public void SetDefaultColor(Color color) {
        imageDefaultColor = color;
        frameDefaultColor = color;
        if (!isOn) {
            SetColors(imageDefaultColor, frameDefaultColor);
        }
    }

    void SetColors(Color imgColor, Color frameColor) {
        if (image != null) {
            image.color = imgColor;
        }

        if (frameImage != null) {
            frameImage.color = frameColor;
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

            Material mat = Instantiate(frameImage.material);
            frameImage.material = mat;

            frameDefaultThickness = frameImage.material.GetFloat("_Thickness");
        }
        Toggle toggle = GetComponent<Toggle>();
        if (toggle) {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        icon = GetComponentInChildren<Icon>();
    }

}
