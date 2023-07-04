using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingEffectsModifier : MonoBehaviour {
    [SerializeField] PostProcessVolume galleryEffects;

    public void TurnOnGalleryEffects() {
        galleryEffects.enabled = true;
    }

    public void TurnOffGalleryEffects() {
        galleryEffects.enabled = false;
    }
}
