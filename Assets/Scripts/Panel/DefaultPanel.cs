using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class DefaultPanel : Panel {
    [SerializeField] float animDuration = 0.5f;

    RectTransform rectTransform;

    void OnEnable() {
        rectTransform = GetComponent<RectTransform>();
    }

    public override void Hide() {
        base.Hide();
        rectTransform.DOScale(0, animDuration).onComplete = () => gameObject.SetActive(false);
    }

    public override void Show() {
        base.Show();
        gameObject.SetActive(true);
        rectTransform.DOScale(1, animDuration);
    }

    void OnDestroy() {
        rectTransform.DOKill();
    }
}