using DG.Tweening;
using UnityEngine;

public class CenterPanel : Panel {
    [SerializeField] float startAnimDuration = 0.5f;
    [SerializeField] float endAnimDuration = 0.35f;

    public override void Hide() {
        base.Hide();
        transform.DOScale(0, endAnimDuration).onComplete = () => gameObject.SetActive(false);
    }

    public override void Show() {
        base.Show();
        gameObject.SetActive(true);
        transform.DOScale(1, startAnimDuration);
    }

    void OnDestroy() {
        transform.DOKill();
    }
}
