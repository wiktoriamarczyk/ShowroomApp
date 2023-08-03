using DG.Tweening;
using UnityEngine;

public class Loader : MonoBehaviour {
    RectTransform rectComponent;
    float duration = Common.dreamloDebugDelay;
    Tween tween;

    public void StartLoader() {
        rectComponent = GetComponent<RectTransform>();
        tween = rectComponent.DORotate(new Vector3(0, 0, -360), duration, RotateMode.FastBeyond360);
        tween.SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Restart);
    }
    
    public void StopLoader() {
        tween?.Kill(true);
    }
}
