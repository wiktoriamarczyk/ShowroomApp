using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionChanger : MonoBehaviour {
    [SerializeField] GameObject target;
    Vector3 newPosition = new Vector3(-0.3f, 1, 0);
    Vector3 lastPosition = new Vector3();
    float speed = 2f;
    CameraController cameraController;

    private void Awake() {
        cameraController = GetComponent<CameraController>();
    }

    public void ChangePosition() {
        Vector3 targetPosition = newPosition;
        if (transform.position == newPosition) {
            targetPosition = lastPosition;
        }
        else {
            lastPosition = transform.position;
            targetPosition = newPosition;
        }

        StartCoroutine(SetNewPosition(targetPosition));
    }

    IEnumerator SetNewPosition(Vector3 targetPosition) {
        while (transform.position != targetPosition) {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            transform.LookAt(target.transform.position);
            yield return null;
        }
    }

}
