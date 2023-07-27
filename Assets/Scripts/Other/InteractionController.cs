using UnityEngine;

public class InteractionController : MonoBehaviour {

    CanvasGroup canvasGroup;

    void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Block() {
        canvasGroup.interactable = false;
    }

    public void Enable() {
        canvasGroup.interactable = true;
    }
}
