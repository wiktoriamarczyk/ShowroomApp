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
        int intColor = Convert.ToInt32(col, 16); ;

        float R = ((0xFF0000 & intColor) >> 16) / 255.0f;
        float G = ((0xFF00 & intColor) >> 8) / 255.0f;
        float B = ((0xFF & intColor) >> 0) / 255.0f;
        StartCoroutine(ChangeElementsColor(new Color(R, G, B)));
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