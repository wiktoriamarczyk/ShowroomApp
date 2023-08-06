using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour {
    [SerializeField] List<GameObject> objects;
    Color lastColor;
    float duration = 1f;
    float progress;
    bool isCoroutineActive = false;
    List<Color> defaultColors = new List<Color>();
    List<Material> objectMaterials = new List<Material>();

    void Awake() {
        foreach (var obj in objects) {
            Material material = obj.GetComponent<Renderer>().material;
            objectMaterials.Add(material);
            defaultColors.Add(material.color);
        }
    }

    public void ChangeElementsColorToDefault() {
        if (isCoroutineActive) {
            StopAllCoroutines();
        }
        StartCoroutine(ChangeColorToDefault());
    }

    public void ChangeElementsColor(string col) {
        if (isCoroutineActive) {
            StopAllCoroutines();
        }
        StartCoroutine(ChangeElementsColor(Common.ColorFromHex(col)));
    }

    IEnumerator ChangeColorToDefault() {
        isCoroutineActive = true;
        progress = 0f;
            while (progress < 1) {
                progress += Time.deltaTime * 1 / duration;
                for (int i = 0; i < objects.Count; ++i) {
                lastColor = objectMaterials[i].color;
                objectMaterials[i].color = Color.Lerp(lastColor, defaultColors[i], progress);
            }
            yield return null;
        }
        isCoroutineActive = false;
    }

    IEnumerator ChangeElementsColor(Color color) {
        isCoroutineActive = true;
        progress = 0f;
        while (progress < 1){
            progress += Time.deltaTime * 1 / duration;
               for (int i = 0; i < objectMaterials.Count; ++i) {
                lastColor = objectMaterials[i].color;
                objectMaterials[i].color = Color.Lerp(lastColor, color, progress);
            }
            yield return null;
        }
        isCoroutineActive = false;
    }

    void OnDestroy() {
        StopAllCoroutines();
    }
}