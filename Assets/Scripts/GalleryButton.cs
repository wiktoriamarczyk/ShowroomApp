using UnityEngine;
using UnityEngine.EventSystems;

public class GalleryButton : MonoBehaviour, IDeselectHandler, ISelectHandler {
    [SerializeField] ComboBoxDisplayer myComboBox;
    [SerializeField] Panel galleryPanel;
    [SerializeField] PostProcessingEffectsModifier galleryEffectsModifier;

    public void OnDeselect(BaseEventData eventData) {
        PanelManager.Instance.HideCurrentPanel();
        galleryEffectsModifier.TurnOffGalleryEffects();
        myComboBox.SelectDefaultButton();
    }

    public void OnSelect(BaseEventData eventData) {
        PanelManager.Instance.ShowPanel(galleryPanel);
        galleryEffectsModifier.TurnOnGalleryEffects();
    }
}
