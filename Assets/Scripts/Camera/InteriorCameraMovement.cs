using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorCameraMovement : MonoBehaviour {
    [SerializeField] GameObject viewPoint;
    [SerializeField] float speed = 5.0f;
    [SerializeField] float sensitivity = 5.0f;
    
    void Update() {
        // Rotate the camera based on the mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Vector3 rot = transform.eulerAngles + new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0);
        if (rot.x > 180) {
            rot.x -= 360;
        }
        if (rot.y > 180) {
            rot.y -= 360;
        }

        rot.x = Mathf.Clamp(rot.x, -40, 40);
        rot.y = Mathf.Clamp(rot.y, -40, 40);
        transform.eulerAngles = rot;

        Debug.Log(rot);
    }
}
