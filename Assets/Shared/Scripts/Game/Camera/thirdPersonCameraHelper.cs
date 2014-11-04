using UnityEngine;

public static class thirdPersonCameraHelper
{
	public struct ClipPlanePoints
	{
		public Vector3 upperLeft;
		public Vector3 upperRight;
		public Vector3 lowerLeft;
		public Vector3 lowerRight;
	}

	// Clamp angle to between -360, 360.
	public static float clampingAngle(float angle, float min, float max)
	{
		do
		{
			if (angle < -360)
				angle += 360;
			if (angle > 360)
				angle -= 360;
		} while (angle < -360 || angle > 360);



		return Mathf.Clamp(angle, min, max);
	}

	public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos)
	{
		ClipPlanePoints clipPlanePoints = new ClipPlanePoints();

		// Ensure there is a main camera for us to check.
		if (!Camera.main)
			return clipPlanePoints;

		// Properties to determine the main camera's near clip plane.
		Transform mainCameraTransform = Camera.main.transform;
		float halfFieldOfView = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;
		float aspect = Camera.main.aspect;
		float distance = Camera.main.nearClipPlane;
		float height = distance * Mathf.Tan(halfFieldOfView);
		float width = height * aspect;

		clipPlanePoints.lowerRight = pos + mainCameraTransform.right * width;
		clipPlanePoints.lowerRight -= mainCameraTransform.up * height;
		clipPlanePoints.lowerRight += mainCameraTransform.forward * distance;

		clipPlanePoints.lowerLeft = pos - mainCameraTransform.right * width;
		clipPlanePoints.lowerLeft -= mainCameraTransform.up * height;
		clipPlanePoints.lowerLeft += mainCameraTransform.forward * distance;

		clipPlanePoints.upperRight = pos + mainCameraTransform.right * width;
		clipPlanePoints.upperRight += mainCameraTransform.up * height;
		clipPlanePoints.upperRight += mainCameraTransform.forward * distance;

		clipPlanePoints.upperLeft = pos - mainCameraTransform.right * width;
		clipPlanePoints.upperLeft += mainCameraTransform.up * height;
		clipPlanePoints.upperLeft += mainCameraTransform.forward * distance;

		return clipPlanePoints;
	}
}