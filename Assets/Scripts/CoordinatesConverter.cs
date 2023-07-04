using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;


public class CoordinatesConverter : MonoBehaviour {
    /* A point in Cartesian space can be represented in spherical space,
    it is done by describing it using a radius of the sphere and two angles. */
    public static Vector3 GetSphericalCoordinates(Vector3 cartesian) {
        float x = cartesian.x;
        float y = cartesian.y;
        float z = cartesian.z;

        float radius = cartesian.magnitude;

        float phi = Mathf.Atan2(x, z);
        float theta = Mathf.Acos(y / radius);

        return new Vector3(radius, phi, theta);
    }

    public static Vector3 GetCartesianCoordinates(Vector3 spherical) {
        float radius = spherical.x;
        float phi = spherical.y;
        float theta = spherical.z;

        Vector3 result;
        var sinPhiRadius = Mathf.Sin(theta) * radius;

        result.x = sinPhiRadius * Mathf.Sin(phi);
        result.y = Mathf.Cos(theta) * radius;
        result.z = sinPhiRadius * Mathf.Cos(phi);

        return result;
    }
}
