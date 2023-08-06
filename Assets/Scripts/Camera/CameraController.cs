using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    InteriorCameraMovement interiorCamera;
    OutsideCameraMovement outsideCamera;
    MoveTowardsTarget moveTowardsTarget;
    [SerializeField] Toggle interiorCameraActivator;

    void Awake() {
        interiorCamera = GetComponent<InteriorCameraMovement>();
        outsideCamera = GetComponent<OutsideCameraMovement>();
        moveTowardsTarget = GetComponent<MoveTowardsTarget>();
        interiorCamera.enabled = false;
        interiorCameraActivator.onValueChanged.AddListener(moveTowardsTarget.ChangePosition);
        moveTowardsTarget.onAnimStart += OnAnimStart;
        moveTowardsTarget.onAnimEnd += OnAnimEnd;
    }

    void OnAnimEnd(MoveTowardsTarget.eAnimType type) {
        if (type == MoveTowardsTarget.eAnimType.ENTER_CAR) {
            interiorCamera.enabled = true;
        }
        else {
            outsideCamera.EnableRotation();
        }
        interiorCameraActivator.interactable = true;
    }

    void OnAnimStart(MoveTowardsTarget.eAnimType type) {
        interiorCameraActivator.interactable = false;
        outsideCamera.DisableRotation();
        interiorCamera.enabled = false;
    }
}
