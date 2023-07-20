using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsTarget : MonoBehaviour {
    [SerializeField] GameObject rotationTarget;
    [SerializeField] GameObject positionTarget;
    Vector3 lastPosition = new Vector3();
    float speed = 2f;

    public void ChangePosition() {
        Vector3 targetPosition = positionTarget.transform.position;
        if (transform.position == positionTarget.transform.position) {
            targetPosition = lastPosition;
        }
        else {
            lastPosition = transform.position;
            targetPosition = positionTarget.transform.position;
        }
        StartCoroutine(SetNewPosition(targetPosition));
    }

    IEnumerator SetNewPosition(Vector3 targetPosition) {
        while (transform.position != targetPosition) {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            transform.LookAt(rotationTarget.transform.position);
            yield return null;
        }
    }

}
