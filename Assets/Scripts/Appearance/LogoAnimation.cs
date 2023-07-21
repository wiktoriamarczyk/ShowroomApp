using UnityEngine;
using DG.Tweening;

public class LogoAnimation : MonoBehaviour {
    RectTransform rectTransform;
    Quaternion startRotation;
    Quaternion endRotation;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.DOScale(1.2f, 1.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    void OnDisable() {
        rectTransform.DOKill();
    }
}