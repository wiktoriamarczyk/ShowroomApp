using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOverlayController : MonoBehaviour
{
    [SerializeField] GameObject overlay;
    public void SetOverlayVisibility(bool visible) {
        overlay.SetActive(visible);
    }
}
