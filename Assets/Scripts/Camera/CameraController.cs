using UnityEngine;
using System;

public class CameraController : MonoBehaviour {
    [SerializeField] GameObject target;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 startRotation;

    const float minHeight = 0.4f;
    const float maxHeight = 3.80f;
    const float radius = 3f;
    const float movementSpeed = 0.5f;
    const float screenSaverSpeed = 0.75f;
    const float timeToScreenSaver = 5f;
    const float distanceToSpeedMultiplier = 0.01f;

    float lastTime;
    float timer = timeToScreenSaver;
    bool directMovementControl = false;
    bool dampingEnabled = true;
    Vector3 lastPosition;
    Vector3 swipeStartPos;
    Vector3 sphereCoord;
    Vector3 speed;

    public event Action onScreenSaver;
    public event Action onCameraMovement;

    public eMovementType movementType { get; set; } = eMovementType.PLAYER_INPUT;

    [SerializeField] UnityEngine.UI.Text text;

    public enum eMovementType {
        NONE,
        PLAYER_INPUT,
        SCREEN_SAVER
    }

    void Awake() {
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
        sphereCoord = CoordinatesConverter.GetSphericalCoordinates(transform.position - target.transform.position);
        Input.simulateMouseWithTouches = false;
    }

    void Update() {
        text.text = $"Movement type: {movementType}\n" +
                    $"Speed: {speed}\n" +
                    $"Timer: {timer}\n" +
                    $"Last position: {lastPosition}\n" +
                    $"Direct movement control: {directMovementControl}\n" +
                    $"Damping enabled: {dampingEnabled}\n" +
                    $"Sphere coord: {sphereCoord}\n" +
                    $"Camera position: {transform.position}\n" +
                    $"Camera rotation: {transform.eulerAngles}\n" +
                    $"Input.mousePosition: {Input.mousePosition}\n" +
                    $"ScreenToWorldPosition: {ScreenToWorldPosition(Input.mousePosition)}\n";



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
        lastPosition = ScreenToWorldPosition(Input.mousePosition);
    }

    Vector3 ScreenToWorldPosition(Vector3 screenPosition) {
        return new Vector3( (screenPosition.x / Camera.main.pixelWidth) * 1920 ,
                            (screenPosition.y / Camera.main.pixelHeight) * 1080,
                            0);
    }

    public void EnableRotation() {
        if (movementType != eMovementType.PLAYER_INPUT) {
             onCameraMovement?.Invoke();
        }
        movementType = eMovementType.PLAYER_INPUT;
        timer = timeToScreenSaver;
        dampingEnabled = true;
    }

    public void DisableRotation() {
        movementType = eMovementType.NONE;
        dampingEnabled = false;
    }

    void HandleScreenSaver() {
        if (!directMovementControl) {
            timer -= Time.deltaTime;
        }
        if (timer <= 0) {
            if (movementType != eMovementType.SCREEN_SAVER) {
                onScreenSaver?.Invoke();
            }
            movementType = eMovementType.SCREEN_SAVER;
        }
        if (ScreenToWorldPosition(Input.mousePosition) != lastPosition) {
            EnableRotation();
        }
    }

    void HandlePlayerInput() {
        /* Whenever the left mouse button is pressed, the mouse cursor's position
         and current time is remembered */
        bool IsTouchActive = Input.touchCount > 0;

        if ( (!IsTouchActive && Input.GetMouseButtonDown(0)) ||
             (IsTouchActive && Input.GetTouch(0).phase == TouchPhase.Began)) {
            lastTime = Time.time;
            swipeStartPos = lastPosition = ScreenToWorldPosition(Input.mousePosition);
            directMovementControl = true;
        }
        // while the left mouse button is pressed, we manually calculate the camera's speed
        if (directMovementControl) {
            speed = (lastPosition - ScreenToWorldPosition(Input.mousePosition)) / Time.deltaTime * movementSpeed * distanceToSpeedMultiplier;
        }
        // when user releases left mouse button, gesture is ended
        if ( (!IsTouchActive && Input.GetMouseButtonUp(0)) ||
             (IsTouchActive && Input.GetTouch(0).phase == TouchPhase.Ended) ) {
            directMovementControl = false;
            // calculate time from gesture start to now
            float time = Time.time - lastTime;
            // and remember current time in lastTime variable (used for damping effect)
            lastTime = Time.time;
            /* our speed is a vector from start to end cursor's position
             divided by the gesture duration (vector length is proportional to speed) */
            speed = (swipeStartPos - ScreenToWorldPosition(Input.mousePosition)) / time * movementSpeed * distanceToSpeedMultiplier;
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
                dx *= dampingFactor;
                dy *= dampingFactor;
            }

            // update camera's posiiton
            if (dx != 0f || dy != 0f) {
                // rotate the camera
                sphereCoord.y -= dx * Time.deltaTime;
                // and prevent it from turning upside down (1.5f = approx. Pi / 2)
                sphereCoord.z = Mathf.Clamp(sphereCoord.z - dy * Time.deltaTime, 0.01f, 1.5f);

                // calculate the cartesian coordinates for Unity
                transform.position = CoordinatesConverter.GetCartesianCoordinates(sphereCoord) + target.transform.position;

                // make the camera look at the target
                transform.LookAt(target.transform.position);
            }
        }
    }

    /* This function calculates damping multiplier as logarithmic function (1/x)
     which is modified in such a way that its value in 0 is ~1 and value above 1 is 0 */
    static float DampingMultiplier(float t) {
        float Val = 1 / (t * 10 + 1) - 1 / 11.0f;
        Val = Mathf.Clamp(Val, 0, 1);
        return Val;
    }
}
