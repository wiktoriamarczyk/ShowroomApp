using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SidePanel : Panel {
    [SerializeField] Vector2 endPosition = new Vector2(20, 0);
    [SerializeField] Vector2 startPosition = new Vector2(500, 0);
    [SerializeField] float duration = 0.5f;
    RectTransform rectTransform;

    void OnEnable() {
        rectTransform = GetComponent<RectTransform>();
        Debug.Log("lokalna: " + rectTransform.localPosition);
        Debug.Log("globalna: " + rectTransform.position);
        rectTransform.anchoredPosition = startPosition;
    }

    public override void Hide() {
        rectTransform.DOAnchorPos(startPosition, duration).onComplete = () => gameObject.SetActive(false);
    }

    public override void Show() {
        gameObject.SetActive(true);
        rectTransform.DOAnchorPos(endPosition, duration);
    }

    void OnDestroy() {
        rectTransform.DOKill();
    }
}

