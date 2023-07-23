using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour {
    [SerializeField] List<GameObject> elementsToChange;
    Color lastColor;
    float duration = 1f;
    float progress;

    public void ChangeElementsColor(string col) {
        StartCoroutine(ChangeElementsColor(Common.ColorFromHex(col)));
    }

    IEnumerator ChangeElementsColor(Color color) {
        Renderer renderer = elementsToChange[1].GetComponent<Renderer>();
        lastColor = renderer.material.color;
        while (progress < 1){
            progress += Time.deltaTime * 1 / duration;
            foreach (var obj in elementsToChange) {
               obj.GetComponent<Renderer>().material.color = Color.Lerp(lastColor, color, progress);
            }
            yield return null;
        }
    }
 }