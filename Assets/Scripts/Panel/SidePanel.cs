using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class SidePanel : Panel {
    [SerializeField] Vector2 endPosition = new Vector2(20, 0);
    [SerializeField] Vector2 startPosition = new Vector2(500, 0);
    [SerializeField] float animDuration = 0.5f;
    
    RectTransform rectTransform;

    void OnEnable() {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = startPosition;
    }

    public override void Hide() {
        base.Hide();
        rectTransform.DOAnchorPos(startPosition, animDuration).onComplete = () => gameObject.SetActive(false);
    }

    public override void Show() {
        base.Show();
        gameObject.SetActive(true);
        rectTransform.DOAnchorPos(endPosition, animDuration);
    }

    void OnDestroy() {
        rectTransform.DOKill();
    }
}

