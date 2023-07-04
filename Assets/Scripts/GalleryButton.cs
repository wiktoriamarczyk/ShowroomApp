using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GalleryButton : MonoBehaviour, IDeselectHandler, ISelectHandler {
    [SerializeField] SelectionMenuView selectionMenu;
    [SerializeField] Panel galleryPanel;
    [SerializeField] PostProcessingEffectsModifier galleryEffectsModifier;

    public void OnDeselect(BaseEventData eventData) {
        PanelManager.Instance.HideCurrentPanel();
        galleryEffectsModifier.TurnOffGalleryEffects();
        selectionMenu.SelectDefaultButton();
        GetComponent<Button>().Select();
    }



    public void OnSelect(BaseEventData eventData) {
        PanelManager.Instance.ShowPanel(galleryPanel);
        galleryEffectsModifier.TurnOnGalleryEffects();
    }
}
