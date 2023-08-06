using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct CatmulRomSegment {
    public Vector3[] points;
    public float speedMultiplier;
}

public class MoveTowardsTarget : MonoBehaviour {
    [SerializeField] GameObject rotationTarget;
    [SerializeField] GameObject positionTarget;
    [SerializeField] GameObject leftWindowCP;
    [SerializeField] GameObject rightWindowCP;
    [SerializeField] float cameraSpeed = 3f;

    Vector3 lastPosition = new Vector3();
    List<CatmulRomSegment> currentCameraPath;
    public event Action<eAnimType> onAnimStart;
    public event Action<eAnimType> onAnimEnd;
    bool isCoroutineActive = false;
    public enum eAnimType {
        ENTER_CAR,
        EXIT_CAR
    }
    
    public bool hasCameraReachedDestination {
        get { return isCoroutineActive; }
    }

    const float offsetFromWindow = 1f;
    const float offsetFromTarget = 0.66f;

    public void ChangePosition(bool forward) {
        if (isCoroutineActive) {
            return;
        }
        eAnimType type = forward ? eAnimType.ENTER_CAR : eAnimType.EXIT_CAR;

        if (forward) {
            lastPosition = transform.position;
        }

        currentCameraPath = GenerateCatmulRomSegments(lastPosition, cameraSpeed, forward);
        StartCoroutine(SetNewPosition(type));
    }

    List<CatmulRomSegment> GenerateCatmulRomSegments(Vector3 camPos, float speed, bool forward) {
        Vector3 leftWindowPos = leftWindowCP.transform.position;
        Vector3 rightWindowPos = rightWindowCP.transform.position;
        Vector3 seatPos = positionTarget.transform.position;

        Vector3 leftWindowDir = leftWindowPos - camPos;
        Vector3 rightWindowDir = rightWindowPos - camPos;

        Vector3 currentWindowPos;
        Vector3 dir;
        
        if (leftWindowDir.magnitude < rightWindowDir.magnitude) {
            currentWindowPos = leftWindowPos;
            dir = (leftWindowPos - rightWindowPos).normalized;
        }
        else {
            currentWindowPos = rightWindowPos;
            dir = (rightWindowPos - leftWindowPos).normalized;
        }

        Vector3 x = currentWindowPos + dir * offsetFromWindow;
        Vector3 y = x - camPos;
        Vector3 w = camPos + y * offsetFromTarget;

        Vector3 start = camPos - y.normalized;
        Vector3 end = seatPos + (seatPos- currentWindowPos).normalized;

        //Debug.DrawLine(leftWindowPos, leftWindowPos + Vector3.up, Color.cyan, 60);
        //Debug.DrawLine(rightWindowPos, rightWindowPos + Vector3.up, Color.cyan, 60);

        //Debug.DrawLine(start, start + Vector3.up, Color.green, 60);
        //Debug.DrawLine(camPos, camPos + Vector3.up, Color.green, 60);
        //Debug.DrawLine(end, end + Vector3.up, Color.red, 60);
        //Debug.DrawLine(seatPos, seatPos + Vector3.up, Color.red, 60);


        //Debug.DrawLine(x, x + Vector3.up, Color.blue, 60);
        //Debug.DrawLine(w, w + Vector3.up, Color.magenta, 60);

        List<Vector3> points = new List<Vector3> {
            start,
            camPos,
            w,
            x,
            seatPos,
            end
        };

        if (!forward) {
            points.Reverse();
        }

        List<CatmulRomSegment> segments = new List<CatmulRomSegment>();

        for (int i = 3; i < points.Count; ++i) {
            float approxLength = (points[i - 2] - points[i - 1]).magnitude;
 
            CatmulRomSegment segment;
            segment.points = new Vector3[4] { points[i - 3], points[i - 2], points[i - 1], points[i] };
            segment.speedMultiplier = speed / approxLength;

            segments.Add(segment);
        }

        return segments;
    }

    static Vector3 CatmullRom(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t){
        float a = ((-t + 2.0f) * t - 1.0f) * t * 0.5f;
        float b = (((3.0f * t - 5.0f) * t) * t + 2.0f) * 0.5f;
        float c = ((-3.0f * t + 4.0f) * t + 1.0f) * t * 0.5f;
        float d = ((t - 1.0f) * t * t) * 0.5f;

        return p1* a + p2* b + p3* c + p4* d;
    }
    static Vector3 CatmullRom(CatmulRomSegment segment, float t) {
        return CatmullRom(segment.points[0], segment.points[1], segment.points[2], segment.points[3], t);
    }

    IEnumerator SetNewPosition(eAnimType type) {
        isCoroutineActive = true;
        onAnimStart?.Invoke(type);
        while (currentCameraPath.Count > 0) {
            CatmulRomSegment currentSegment = currentCameraPath[0];
            currentCameraPath.RemoveAt(0);

            float progress = 0; // from 0 to 1
            while(progress < 1.0f) {
                progress = Mathf.Clamp01(progress + Time.deltaTime * currentSegment.speedMultiplier);
                transform.position = CatmullRom(currentSegment, progress);
                transform.LookAt(rotationTarget.transform.position);
                yield return null;
            }
        }
        isCoroutineActive = false;
        onAnimEnd?.Invoke(type);
    }

}
