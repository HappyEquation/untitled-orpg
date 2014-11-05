using UnityEngine;
using System.Collections;

public class thirdPersonController : MonoBehaviour
{
	public static CharacterController charController;	// Reference to player's character controller
	public static thirdPersonController instance;		// Reference to itself

	void Awake()
	{
		charController = GetComponent<CharacterController>();
		instance = this;
		thirdPersonCamera.UseExistingOrCreateMainCamera();
	}

	void Update()
	{
		if (Camera.main == null)
		{
			return;
		}
		GetLocomotionInput();
		HandleActionInput();

		thirdPersonMotor.instance.UpdateMotor();
	}

	public void GetLocomotionInput()
	{
		float deadZone = 0.1f;									// Leeway distance that is ignored.

		thirdPersonMotor.instance.verticalVelocity = thirdPersonMotor.instance.moveVector.y;
		thirdPersonMotor.instance.moveVector = Vector3.zero;

		if (!ChatWindow.instance.selectTextfield) {
			// Match moveVector.z to vertical input
			if (Input.GetAxis("Vertical") > deadZone || Input.GetAxis("Vertical") < -deadZone)
			{
				thirdPersonMotor.instance.moveVector += new Vector3(0, 0, Input.GetAxis("Vertical"));
			}

			// Match moveVector.x to horizontal input
			if (Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < -deadZone)
			{
				thirdPersonMotor.instance.moveVector += new Vector3(Input.GetAxis("Horizontal"), 0, 0);
			}
		}
		// Get direction.
		thirdPersonAnimator.instance.DetermineCurrentMoveDirection();
	}

	private void HandleActionInput()
	{
		if (Input.GetButton("Jump"))
		{
			Jump();
		}
	}

	private void Jump()
	{
		thirdPersonMotor.instance.Jump();
	}
}