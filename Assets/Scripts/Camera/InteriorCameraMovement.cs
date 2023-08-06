using UnityEngine;

public class InteriorCameraMovement : MonoBehaviour {
    [SerializeField] GameObject viewPoint;
    [SerializeField] float speed = 5.0f;
    [SerializeField] float sensitivity = 5.0f;

    const int maxCameraAngle = 30;
    const int minCameraAngle = -30;

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

        rot.x = Mathf.Clamp(rot.x, minCameraAngle, maxCameraAngle);
        rot.y = Mathf.Clamp(rot.y, minCameraAngle, maxCameraAngle);
        transform.eulerAngles = rot;
    }
}
