using DG.Tweening;
using UnityEngine;

public class ScalablePanel : Panel {
    [SerializeField] float startAnimDuration = 0.5f;
    [SerializeField] float endAnimDuration = 0.35f;
    [Header("Is blocking other panels which have true on this var")]
    [SerializeField] bool exclusiveVisibilityMode = false;
    Tween currentTween;

    public override void PanelAwake() {
        base.exclusiveVisibility = exclusiveVisibilityMode;
    }

    public override void Hide() {
        base.Hide();
        currentTween?.Kill();
        currentTween = transform.DOScale(0, endAnimDuration);
        currentTween.onComplete = () => gameObject.SetActive(false);
    }

    public override void Show() {
        base.Show();
        currentTween?.Kill();
        gameObject.SetActive(true);
        currentTween = transform.DOScale(1, startAnimDuration);
    }

    void OnDestroy() {
        transform.DOKill();
    }
}
