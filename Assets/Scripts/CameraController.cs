using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 startRotation;

    const float minHeight = 0.4f;
    const float maxHeight = 3.80f;
    const float radius = 3f;
    const float movementSpeed = 0.5f;

    float lastTime;
    bool movementEnabled = false;
    Vector3 lastPosition;
    Vector3 swipeStartPos;
    Vector3 sphereCoord;
    Vector3 speed;

    void Start() {
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
        sphereCoord = CoordinatesConverter.GetSphericalCoordinates(transform.position);
    }

    void Update() {
        CameraMovement();
    }

    void CameraMovement() {
        /* whenever the left mouse button is pressed, the mouse cursor's position
         and current time is remembered */
        if (Input.GetMouseButtonDown(0)) {
            lastTime = Time.time;
            swipeStartPos = lastPosition = Input.mousePosition;
            movementEnabled = true;
        }

        // when user releases left mouse button, gesture is ended
        if (Input.GetMouseButtonUp(0)) {
            movementEnabled = false;
            // calculate time from gesture start to now
            float time = Time.time - lastTime;
            // and remember current time in lastTime variable (used for damping effect)
            lastTime = Time.time;
            /* our speed is a vector from start to end cursor's position
             divided by the gesture duration (vector length is proportional to speed) */
            speed = (swipeStartPos - Input.mousePosition) / time;
        }

        if (movementEnabled || speed.magnitude > 0) {
            // get the deltas that describe how much the mouse cursor got moved between frames
            float dx = 0;
            float dy = 0;
            if (movementEnabled) {
                dx = (lastPosition.x - Input.mousePosition.x) * movementSpeed;
                dy = (lastPosition.y - Input.mousePosition.y) * movementSpeed;
            }
            else {
                // use DampingMultiplier to smoothly deaccelerate camera movement
                dx = speed.x * Time.deltaTime * DampingMultiplier(Time.time - lastTime);
                dy = speed.y * Time.deltaTime * DampingMultiplier(Time.time - lastTime);
            }

            // update camera's posiiton
            if (dx != 0f || dy != 0f) {
                // rotate the camera
                sphereCoord.y += dx * Time.deltaTime;
                // and prevent it from turning upside down (1.5f = approx. Pi / 2)
                sphereCoord.z = Mathf.Clamp(sphereCoord.z + dy * Time.deltaTime, -1.5f, 1.5f);

                // calculate the cartesian coordinates for Unity
                transform.position = CoordinatesConverter.GetCartesianCoordinates(sphereCoord) + target.transform.position;

                // prevent the camera from going below the ground and above the ceiling
                if (transform.position.y <= minHeight)
                    transform.position = new Vector3(transform.position.x, minHeight, transform.position.z);
                else if (transform.position.y >= maxHeight)
                    transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);

                // make the camera look at the target
                transform.LookAt(target.transform.position);
            }
            // update the last mouse position
            lastPosition = Input.mousePosition;
        }
    }

    /* this function calculates damping multiplier as logarithmic function (1/x)
     which is modified in such a way that its value in 0 is ~1 and value above 1 is 0 */
    static float DampingMultiplier(float t)
    {
        float Val = 1 / (t * 10 + 1) - 1 / 11.0f;
        Val = Mathf.Clamp(Val, 0, 1);
        return Val;
    }
}
