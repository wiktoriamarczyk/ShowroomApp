using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class MovingPanel : Panel {
    [SerializeField] GameObject hidePositionTarget;
    [SerializeField] bool hiddenOnAwake = true;
    [Header("Is blocking other panels which have true on this var")]
    [SerializeField] bool exclusiveVisibilityMode = false;
    [SerializeField] float animDuration = 0.5f;
    Tween currentTween;
    RectTransform rectTransform;
    Vector2 showPosition;
    Vector2 hidePosition;

    public override void PanelAwake() {
        base.exclusiveVisibility = exclusiveVisibilityMode;
        base.isHiddenOnAwake = hiddenOnAwake;
        rectTransform = GetComponent<RectTransform>();
        showPosition = rectTransform.anchoredPosition;
        hidePosition = hidePositionTarget.GetComponent<RectTransform>().anchoredPosition;
    }

    public override void Hide() {
        base.Hide();
        currentTween?.Kill();
        currentTween = rectTransform.DOAnchorPos(hidePosition, animDuration);
        currentTween.onComplete = () => gameObject.SetActive(false);
        if (gameObject.name == "Test Drive Panel")
            Debug.Log(gameObject.name + "|||CLOSED|||");
    }

    public override void Show() {
        base.Show();
        currentTween?.Kill();
        gameObject.SetActive(true);
        currentTween = rectTransform.DOAnchorPos(showPosition, animDuration);
        if (gameObject.name == "Test Drive Panel")
        Debug.Log(gameObject.name + "---OPENED---");
    }

    void OnEnable() {
        if (hiddenOnAwake) {
            rectTransform.anchoredPosition = hidePosition;
        }
    }

    void OnDestroy() {
        rectTransform.DOKill();
    }
}

