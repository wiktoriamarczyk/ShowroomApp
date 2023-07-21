using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingEffectsModifier : MonoBehaviour {
    [SerializeField] PostProcessVolume blurryDarkBackground;

    public void TurnOnBlurryDarkBackground() {
        blurryDarkBackground.enabled = true;
    }

    public void TurnOffBlurryDarkBackground() {
        blurryDarkBackground.enabled = false;
    }
}
