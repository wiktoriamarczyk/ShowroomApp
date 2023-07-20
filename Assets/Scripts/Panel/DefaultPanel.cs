using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class DefaultPanel : Panel {
    [SerializeField] float animDuration = 0.5f;
    Tween currentTween;
    RectTransform rectTransform;

    void OnEnable() {
        rectTransform = GetComponent<RectTransform>();
    }

    public override void Hide() {
        base.Hide();
        currentTween?.Kill();
        currentTween = rectTransform.DOScale(0, animDuration);
        currentTween.onComplete = () => gameObject.SetActive(false);
    }

    public override void Show() {
        base.Show();
        currentTween?.Kill();
        gameObject.SetActive(true);
        currentTween = rectTransform.DOScale(1, animDuration);
    }

    void OnDestroy() {
        rectTransform.DOKill();
    }
}