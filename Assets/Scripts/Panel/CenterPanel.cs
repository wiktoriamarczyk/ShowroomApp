using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPanel : Panel {
    [SerializeField] float duration = 0.5f;

    public override void Hide() {
        transform.DOScale(0, duration).onComplete = () => gameObject.SetActive(false);
    }

    public override void Show() {
        gameObject.SetActive(true);
        transform.DOScale(1, duration);
    }

    void OnDestroy() {
        transform.DOKill();
    }
}
