using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class CameraController : MonoBehaviour
{
	float minHeight = 0.4f;
	float maxHeight = 3.80f;
	float radius = 3f;

	// Is true when the user wants to rotate the camera
	bool movementEnabled = false;
	float movementSpeed = 0.5f;
	
	// cursor's position in the last frame
	Vector3 lastPosition = new Vector3();
	// object around which the camera rotates
	[SerializeField] GameObject target;
	Vector3 targetPosition;
	
	[SerializeField] Vector3 startPosition;
	[SerializeField] Vector3 startRotation;

	// The spherical coordinates
	Vector3 sphereCord = new Vector3();

	void Start() {
		transform.position = startPosition;
		transform.eulerAngles = startRotation;
		sphereCord = CoordinatesConverter.GetSphericalCoordinates(transform.position);
	}

	Vector3? swipeStartPos;
	Vector3 speed;
	float lastTime;

	// this function calculates damping multiplier as logarithmic function (1/x)
	// which is modified in such a way that its value in 0 is ~1 and value above 1 is 0
	static float DampingMultiplier(float t) {
		float Val = 1 / (t * 10 + 1) - 1 / 11.0f;
		Val = Mathf.Clamp( Val , 0 , 1 );
		return Val;
	}

	// Update is called once per frame
	void Update() {
		// Whenever the left mouse button is pressed, the
		// mouse cursor's position is stored for the arc-
		// ball camera as a reference.
		if (Input.GetMouseButtonDown(0)) {
			// if we started gesture, remember time and cursor's position
			if (swipeStartPos == null) {
				lastTime = Time.time;
				swipeStartPos = Input.mousePosition;
			}

			lastPosition = Input.mousePosition;
			movementEnabled = true;
		}

		// when the user releases the left mouse button
		if (Input.GetMouseButtonUp(0)) {	
			movementEnabled = false;
			// if we are ending the gesture
			if (swipeStartPos != null) {
				// calculate time from gesture start to now
				float time = Time.time - lastTime;
				// and remember current time in lastTime variable (used for damping effect)
				lastTime = Time.time;
				// our speed is a vector from start to end cursor's position
				// divided by the gesture duration (vector length is proportional to speed)
				speed = (swipeStartPos.Value - Input.mousePosition) / time;
				swipeStartPos = null;
			}
		}

		if (movementEnabled || speed.magnitude>0) {
			// get the deltas that describe how much the mouse cursor got moved between frames
			float dx = 0;
			float dy = 0;
			if(movementEnabled) {
				dx = (lastPosition.x - Input.mousePosition.x) * movementSpeed;
				dy = (lastPosition.y - Input.mousePosition.y) * movementSpeed;
			}
			else {
				// used DampingMultiplier to smoothly deaccelerate camera movement
				dx = speed.x * Time.deltaTime * DampingMultiplier(Time.time - lastTime);
				dy = speed.y * Time.deltaTime * DampingMultiplier(Time.time - lastTime);
			}


			// Only update the camera's position if the mouse got moved in either direction
			if (dx != 0f || dy != 0f) {
				// Rotate the camera left and right
				sphereCord.y += dx * Time.deltaTime;

				// Rotate the camera up and down
				// Prevent the camera from turning upside down (1.5f = approx. Pi / 2)
				sphereCord.z = Mathf.Clamp(sphereCord.z + dy * Time.deltaTime, -1.5f, 1.5f);

				// Calculate the cartesian coordinates for unity
				transform.position = CoordinatesConverter.GetCartesianCoordinates(sphereCord) + targetPosition;

				if (transform.position.y <= minHeight)
					transform.position = new Vector3(transform.position.x, minHeight, transform.position.z);
				else if (transform.position.y >= maxHeight)
					transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);

				//Make the camera look at the target
				transform.LookAt(targetPosition);
			}
			// Update the last mouse position
			lastPosition = Input.mousePosition;
		}

	}
}
