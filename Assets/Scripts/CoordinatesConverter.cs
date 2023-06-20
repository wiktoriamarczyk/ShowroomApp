using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CoordinatesConverter : MonoBehaviour
{
	/* A point in Cartesian space can be represented in spherical space,
	it is done by describing it using a radius of the sphere and two angles. */
	public static Vector3 GetSphericalCoordinates(Vector3 cartesian) {
		float x = cartesian.x;
		float y = cartesian.y;
		float z = cartesian.z;

		float radius = Mathf.Sqrt(
			Mathf.Pow(x, 2) +
			Mathf.Pow(y, 2) +
			Mathf.Pow(z, 2)
		);

		float phi = Mathf.Atan2(z / x, x);
		float theta = Mathf.Acos(y / radius);

		return new Vector3(radius, phi, theta);
	}

	public static Vector3 GetCartesianCoordinates(Vector3 spherical) {
		float radius = spherical.x;
		float phi = spherical.y;
		float theta = spherical.z;

		Vector3 ret = new Vector3();

		ret.x = radius * Mathf.Cos(theta) * Mathf.Cos(phi);
		ret.y = radius * Mathf.Sin(theta);
		ret.z = radius * Mathf.Cos(theta) * Mathf.Sin(phi);

		return ret;
	}
}
