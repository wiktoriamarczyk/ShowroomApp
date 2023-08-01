using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOverlayController : MonoBehaviour
{
    [SerializeField] GameObject overlay;
    [SerializeField] GameObject optionalIndicator;
    public void SetOverlayVisibility(bool visible) {
        overlay.SetActive(visible);
    }
    public void SetOptionalIndicatorVisibility(bool visible) {
        optionalIndicator.SetActive(visible);
    }
}
