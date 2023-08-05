using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionalIndicatorController : MonoBehaviour {
    [SerializeField] GameObject optionalIndicator;
    public void SetOptionalIndicatorVisibility(bool visible) {
        optionalIndicator.SetActive(visible);
    }
}
