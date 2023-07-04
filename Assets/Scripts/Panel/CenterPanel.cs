using DG.Tweening;
using UnityEngine;

public class CenterPanel : Panel {
    [SerializeField] float startDuration = 0.5f;
    [SerializeField] float endDuration = 0.35f;

    public override void Hide() {
        transform.DOScale(0, endDuration).onComplete = () => gameObject.SetActive(false);
    }

    public override void Show() {
        gameObject.SetActive(true);
        transform.DOScale(1, startDuration);
    }

    void OnDestroy() {
        transform.DOKill();
    }
}
