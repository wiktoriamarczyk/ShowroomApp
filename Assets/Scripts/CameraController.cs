using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    [SerializeField] GameObject target;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 startRotation;

    const float minHeight = 0.4f;
    const float maxHeight = 3.80f;
    const float radius = 3f;
    const float movementSpeed = 0.5f;
    const float screenSaverSpeed = 150f;
    const float timeToScreenSaver = 3f;

    float lastTime;
    float timer = timeToScreenSaver;
    bool directMovementControl = false;
    bool dampingEnabled = true;
    Vector3 lastPosition;
    Vector3 swipeStartPos;
    Vector3 sphereCoord;
    Vector3 speed;
    public eMovementType movementType { get; set; } = eMovementType.PLAYER_INPUT;

    public enum eMovementType {
        NONE,
        PLAYER_INPUT,
        SCREEN_SAVER
    }

    void Start() {
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
        sphereCoord = CoordinatesConverter.GetSphericalCoordinates(transform.position - target.transform.position);
    }

    void Update() {
        if (movementType == eMovementType.NONE) {
            return;
        }
        else if (movementType == eMovementType.PLAYER_INPUT) {
            HandlePlayerInput();
        } 
        else if (movementType == eMovementType.SCREEN_SAVER) {
            SimulateMovement();
        }
        
        CameraMovement();

        HandleScreenSaver();

        // update the last mouse position
        lastPosition = Input.mousePosition;
    }

    void HandleScreenSaver() {
        if (!directMovementControl) {
            timer -= Time.deltaTime;
        }
        if (timer <= 0) {
            movementType = eMovementType.SCREEN_SAVER;
        }
        if (Input.mousePosition != lastPosition) {
            timer = timeToScreenSaver;
            dampingEnabled = true;
            movementType = eMovementType.PLAYER_INPUT;
        }
    }

    void HandlePlayerInput() {
        /* Whenever the left mouse button is pressed, the mouse cursor's position
         and current time is remembered */
        if (Input.GetMouseButtonDown(0)) {
            lastTime = Time.time;
            swipeStartPos = lastPosition = Input.mousePosition;
            directMovementControl = true;
        }
        // while the left mouse button is pressed, we manually calculate the camera's speed
        if (directMovementControl) {
            speed = (lastPosition - Input.mousePosition) * movementSpeed;
        }
        // when user releases left mouse button, gesture is ended
        if (Input.GetMouseButtonUp(0)) {
            directMovementControl = false;
            // calculate time from gesture start to now
            float time = Time.time - lastTime;
            // and remember current time in lastTime variable (used for damping effect)
            lastTime = Time.time;
            /* our speed is a vector from start to end cursor's position
             divided by the gesture duration (vector length is proportional to speed) */
            speed = (swipeStartPos - Input.mousePosition) / time;
        }
    }

    void SimulateMovement() {
        directMovementControl = false;
        dampingEnabled = false;
        speed = new Vector3(screenSaverSpeed, 0, 0);
    }

    void CameraMovement() {
        if (speed.magnitude > 0) {
            // get the deltas that describe how much the mouse cursor got moved between frames
            float dx = speed.x;
            float dy = speed.y;
            float dampingFactor = 1f;
            
            if (dampingEnabled) {
                dampingFactor = DampingMultiplier(Time.time - lastTime);
            }
            if (!directMovementControl) {
                // use DampingMultiplier to smoothly deaccelerate camera movement
                dx *= Time.deltaTime * dampingFactor;
                dy *= Time.deltaTime * dampingFactor;
            }

            // update camera's posiiton
            if (dx != 0f || dy != 0f) {
                // rotate the camera
                sphereCoord.y -= dx * Time.deltaTime;
                // and prevent it from turning upside down (1.5f = approx. Pi / 2)
                sphereCoord.z = Mathf.Clamp(sphereCoord.z - dy * Time.deltaTime, -1.5f, 1.5f);

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
        }
    }

    /* This function calculates damping multiplier as logarithmic function (1/x)
     which is modified in such a way that its value in 0 is ~1 and value above 1 is 0 */
    static float DampingMultiplier(float t)
    {
        float Val = 1 / (t * 10 + 1) - 1 / 11.0f;
        Val = Mathf.Clamp(Val, 0, 1);
        return Val;
    }
}
