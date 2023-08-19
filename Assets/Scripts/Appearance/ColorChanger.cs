using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour {

    struct MaterialProperties {
        public Material material;
        public int metallicValueId;
        public int smoothnessValueId;
    }

    [SerializeField] List<GameObject> objects;
    Color lastColor;
    float duration = 1f;
    float progress;
    bool isCoroutineActive = false;
    List<MaterialProperties> materialProperties = new List<MaterialProperties>();

    void Awake() {
        foreach (var obj in objects) {
            Material material = obj.GetComponent<Renderer>().material;
            materialProperties.Add(new MaterialProperties() {
                material = material,
                metallicValueId = Shader.PropertyToID("_Metallic"),
                smoothnessValueId = Shader.PropertyToID("_Glossiness")
            });
        }
    }

    public void ChangeElementsColor(PaintColorData colorData) {
        if (isCoroutineActive) {
            StopAllCoroutines();
        }
        StartCoroutine(ChangeElementsColorCoroutine(colorData));
    }

    IEnumerator ChangeElementsColorCoroutine(PaintColorData colorData) {
        isCoroutineActive = true;
        progress = 0f;
        float lastMetallicValue = 0f;
        float lastSmoothnessValue = 0f;
        while (progress < 1){
            progress += Time.deltaTime * 1 / duration;
            for (int i = 0; i < materialProperties.Count; ++i) {
                lastColor = materialProperties[i].material.color;
                lastMetallicValue = materialProperties[i].material.GetFloat(materialProperties[i].metallicValueId);
                lastSmoothnessValue = materialProperties[i].material.GetFloat(materialProperties[i].smoothnessValueId);
                materialProperties[i].material.color = Color.Lerp(lastColor, colorData.colorProperty, progress);
                materialProperties[i].material.SetFloat(materialProperties[i].metallicValueId, Mathf.Lerp(lastMetallicValue, colorData.metallicProperty, progress));
                materialProperties[i].material.SetFloat(materialProperties[i].smoothnessValueId, Mathf.Lerp(lastSmoothnessValue, colorData.smoothnessProperty, progress));
            }
            yield return null;
        }
        isCoroutineActive = false;
    }

    void OnDestroy() {
        StopAllCoroutines();
    }
}