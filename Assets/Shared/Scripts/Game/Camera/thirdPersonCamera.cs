using UnityEngine;
using System.Collections;

public class thirdPersonCamera : MonoBehaviour
{
	public static thirdPersonCamera instance;				// Reference to the Main Camera.
	public Transform targetLookTransform;			// Transform of the camera will be looking at.
	public float distance = 5f;						// Start distance of camera.
	public float distanceMin = 3f;					// Minimum distance of camera zoom.
	public float distanceMax = 10f;					// Maximum distance of camera zoom.
	public float distanceSmooth = 0.05f;			// Camera zooming smooth factor.
	public float distanceCameraResumeSmooth = 1f;	// Distance at which point smoothing is resumed after occlusion handling is no longer occuring.
	private float distanceCameraSmooth = 0f;		// Camera smoothing distance (after occlusion is no longer happening).
	private float preOccludedDistance = 0f;

	public float mouseXSensitivity = 5f;			// Mouse X sensitivity.
	public float mouseYSensitivity = 5f;			// Mouse Y sensitivity.
	public float mouseWheelSensitivity = 5f;		// Mouse wheel/scroll sensitivity.
	public float xSmooth = 0.05f;					// Smoothness factor for x position calculations.
	public float ySmooth = 0.1f;					// Smoothness factor for y position calculations.
	public float yMinLimit = -40f;
	public float yMaxLimit = 80f;
	private float mouseX = 0f;
	private float mouseY = 0f;
	
	private float velocityX = 0f;
	private float velocityY = 0f;
	private float velocityZ = 0f;
	private float velocityDistance = 0f;

	private float lastRightMousePos = 0f;
	private float lastLeftMousePos = 0f;

	public float occlusionDistanceStep = 0.5f;
	public int maxOcclusionChecks = 10;				// Max number of times to check for occlusion.

	private float startDistance = 0f;
	private float desiredDistance = 0f;

	private Vector3 position = new Vector3(768f, 3.5f, 903f);
	private Vector3 desiredPosition = new Vector3(768f, 3.5f, 903f);

	public bool freeLook = false;

	private void Awake()
	{
		instance = this;
	}

	void Start()
	{
		distance = Mathf.Clamp(distance, distanceMin, distanceMax);	// Ensure our distance is between min and max (valid)
		startDistance = distance;
		Reset();
	}

	private void LateUpdate()
	{
		if (targetLookTransform == null)
		{
			return;
		}

		if (!ChatWindow.instance.selectTextfield && !ChatWindow.instance.isScrolling)
		{
			HandlePlayerInput();
		}

		int count = 0;
		do
		{
			CalculateDesiredPosition();
			count++;
		} while (CheckIfOccluded(count));

		UpdatePosition();
	}

	private void HandlePlayerInput()
	{
		float deadZone = 0.01f;

		
		if (Input.GetMouseButton(0)){
			float currLeftMousePos = Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y");
			if(currLeftMousePos != lastLeftMousePos) freeLook = true;
			lastLeftMousePos = currLeftMousePos;
		}

		// If right mouse button is down, get mouse axis input.
		if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
		{
			mouseX += Input.GetAxis("Mouse X") * mouseXSensitivity;
			mouseY -= Input.GetAxis("Mouse Y") * mouseYSensitivity;
			float currRightMousePos = mouseX + mouseY;
			//Debug.Log("currMousePos:" + currMousePos + " lastMousePos" + lastMousePos);
			// Reset freeLook if the right mouse is down and has moved.
			if (Input.GetMouseButton(1) && currRightMousePos != lastRightMousePos){
				freeLook = false;
			}
			lastRightMousePos = currRightMousePos;
		}
		else if(Input.GetButton("Rotate"))
		{
			mouseX += Input.GetAxis("Rotate") * mouseXSensitivity;
			if(mouseX != 0) freeLook = false;
		}

		// Clamp (limit) mouse Y rotation. Uses thirdPersonCameraHelper.cs.
		mouseY = thirdPersonCameraHelper.clampingAngle(mouseY, yMinLimit, yMaxLimit);

		// Clamp (limit) mouse scroll wheel.
		if (Input.GetAxis("Mouse ScrollWheel") > deadZone || Input.GetAxis("Mouse ScrollWheel") < -deadZone)
		{
			desiredDistance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * mouseWheelSensitivity,
										   distanceMin,
										   distanceMax
										   );
			preOccludedDistance = desiredDistance;
			distanceCameraSmooth = distanceSmooth;
		}
	}

	// Smoothing.
	private void CalculateDesiredPosition()
	{
		// Evaluate distance.
		ResetDesiredDistance();
		distance = Mathf.SmoothDamp(distance, desiredDistance, ref velocityDistance, distanceCameraSmooth);

		// Calculate desired position.
		desiredPosition = CalculatePosition(mouseY, mouseX, distance);
	}

	private bool CheckIfOccluded(int count)
	{
		bool isOccluded = false;
		float nearestDistance = CheckCameraPoints(targetLookTransform.position, desiredPosition);

		if (nearestDistance != -1)
		{
			if (count < maxOcclusionChecks)
			{
				isOccluded = true;
				distance -= occlusionDistanceStep;

				// 0.25 is a good default value.
				if (distance < 0.25f)
				{
					distance = 0.25f;
				}
			}
			else
			{
				distance = nearestDistance - Camera.main.nearClipPlane;
			}
			desiredDistance = distance;
			distanceCameraSmooth = distanceCameraResumeSmooth;
		}

		return isOccluded;
	}

	private Vector3 CalculatePosition(float rotX, float rotY, float rotDist)
	{
		Vector3 direction = new Vector3(0, 0, -distance); 			// -distance because we want it to point behind our character.
		Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);
		return targetLookTransform.position + (rotation * direction);
	}

	private float CheckCameraPoints(Vector3 from, Vector3 to)
	{
		float nearestDistance = -1f;

		RaycastHit hitInfo;

		thirdPersonCameraHelper.ClipPlanePoints clipPlanePoints =
			thirdPersonCameraHelper.ClipPlaneAtNear(to);

		// Draw the raycasts going through the near clip plane vertexes.
		Debug.DrawLine(from, to + transform.forward * -camera.nearClipPlane, Color.red);
		Debug.DrawLine(from, clipPlanePoints.upperLeft, Color.red);
		Debug.DrawLine(from, clipPlanePoints.upperRight, Color.red);
		Debug.DrawLine(from, clipPlanePoints.lowerLeft, Color.red);
		Debug.DrawLine(from, clipPlanePoints.lowerRight, Color.red);
		Debug.DrawLine(clipPlanePoints.upperLeft, clipPlanePoints.upperRight, Color.red);
		Debug.DrawLine(clipPlanePoints.upperRight, clipPlanePoints.lowerRight, Color.red);
		Debug.DrawLine(clipPlanePoints.lowerRight, clipPlanePoints.lowerLeft, Color.red);
		Debug.DrawLine(clipPlanePoints.lowerLeft, clipPlanePoints.upperLeft, Color.red);

		if (Physics.Linecast(from, clipPlanePoints.upperLeft, out hitInfo) && hitInfo.collider.tag != "Player")
			nearestDistance = hitInfo.distance;
		if (Physics.Linecast(from, clipPlanePoints.lowerLeft, out hitInfo) && hitInfo.collider.tag != "Player")
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		if (Physics.Linecast(from, clipPlanePoints.upperRight, out hitInfo) && hitInfo.collider.tag != "Player")
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		if (Physics.Linecast(from, to + transform.forward * -camera.nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player")
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;

		return nearestDistance;
	}

	private void ResetDesiredDistance()
	{
		if (desiredDistance < preOccludedDistance)
		{
			Vector3 pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);
			float nearestDistance = CheckCameraPoints(targetLookTransform.position, pos);

			if (nearestDistance == -1 || nearestDistance > preOccludedDistance)
			{
				desiredDistance = preOccludedDistance;
			}
		}
	}

	private void UpdatePosition()
	{
		float posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velocityX, xSmooth * Time.deltaTime);
		float posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velocityY, ySmooth * Time.deltaTime);
		float posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velocityZ, xSmooth * Time.deltaTime);

		position = new Vector3(posX, posY, posZ);

		transform.position = position;

		transform.LookAt(targetLookTransform);
	}

	public void Reset()
	{
		mouseX = 0f;
		mouseY = 10f;
		distance = startDistance;
		desiredDistance = distance;
		preOccludedDistance = distance;
	}

	public static void UseExistingOrCreateMainCamera()
	{
		GameObject tempCamera;
		GameObject targetLookAt;
		thirdPersonCamera myCamera;

		if (Camera.main != null)
		{
			tempCamera = Camera.main.gameObject;
		}
		else
		{
			tempCamera = new GameObject("Main Camera");
			tempCamera.AddComponent("Camera");
			tempCamera.tag = "MainCamera";				// This is what actually makes our created camera the main camera.
		}

		tempCamera.AddComponent("thirdPersonCamera");
		myCamera = tempCamera.GetComponent<thirdPersonCamera>();

		targetLookAt = GameObject.FindGameObjectWithTag("CameraFocus");

		if (targetLookAt == null)
		{
			targetLookAt = new GameObject("Player");
			targetLookAt.transform.position = new Vector3(768f, 0, 900f);
		}

		myCamera.targetLookTransform = targetLookAt.transform;
	}
}