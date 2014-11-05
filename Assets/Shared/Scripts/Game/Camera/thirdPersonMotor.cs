using UnityEngine;
using System.Collections;

public class thirdPersonMotor : MonoBehaviour
{
	public static thirdPersonMotor instance;			// Reference to itself.

	// Player movespeeds.
	public float forwardSpeed = 10.0f;
	public float backwardSpeed = 6f;
	public float strafingSpeed = 8f;
	public float slidingSpeed = 10.0f;

	public float jumpSpeed = 5.5f;						// Player jump speed.
	public Vector3 moveVector { get; set; }

	public float gravity = 9.81f;
	public float terminalVelocity = 20f;
	public float verticalVelocity { get; set; }

	public float slideThreshold = 0.7f;					// Threshold that determines what kind of slopes will cause the player to slide.
	public float maxControllableSlideMagnitude = 0.4f;	// Max slope at which the character can still be controlled.

	private Vector3 slideDirection;

	private void Awake()
	{
		instance = this;
	}

	public void UpdateMotor()
	{
		ProcessMotion();
		SnapAlignCharacterWithCamera();
	}

	public void ProcessMotion()
	{
		// Transform moveVector to world space.
		moveVector = transform.TransformDirection(moveVector);

		// Normalize moveVector if magnitude > 1.
		if (moveVector.magnitude > 1)
			moveVector = moveVector.normalized;

		// Apply sliding if requirements are met.
		ApplySlide();

		// Multiply moveVector by moveSpeed and deltaTime.
		moveVector *= moveSpeed();

		// Reapply vertical velocity to moveVector.y.
		moveVector = new Vector3(moveVector.x, verticalVelocity, moveVector.z);

		// Apply Gravity
		ApplyGravity();

		// Move character in world space.
		thirdPersonController.charController.Move(moveVector * Time.deltaTime);
		
	}

	// If camera is moving, snap character to it.
	public void SnapAlignCharacterWithCamera()
	{
		//Debug.Log("moveVector.x: "+moveVector.x+" freeLook: " + thirdPersonCamera.instance.freeLook);
		if (moveVector != Vector3.zero && !thirdPersonCamera.instance.freeLook)
		{
			transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
												   Camera.main.transform.eulerAngles.y,
												   transform.eulerAngles.z
												   );
		}
		else if (thirdPersonCamera.instance.freeLook)
		{
			//transform.Rotate(0, Camera.main.transform.rotation.x, 0);
		}
	}

	private void ApplyGravity()
	{
		// Apply gravity as long as the character is not in terminal velocity.
		if (moveVector.y > -terminalVelocity)
		{
			moveVector = new Vector3(moveVector.x, moveVector.y - gravity * Time.deltaTime, moveVector.z);
		}

		// If grounded, don't apply gravity.
		if (thirdPersonController.charController.isGrounded && moveVector.y < 0)
		{
			moveVector = new Vector3(moveVector.x, 0, moveVector.z);
		}
	}

	private void ApplySlide()
	{
		// Verify character is on the ground first.
		if (!thirdPersonController.charController.isGrounded)
		{
			return;
		}

		slideDirection = Vector3.zero;

		RaycastHit hitInfo;				// We use a raycast to determine the slope the character is on and store its data.

		if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo))
		{
			if (hitInfo.normal.y < slideThreshold)
			{
				slideDirection = new Vector3(hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z);
			}
		}

		// Check magnitude of the slope and handle normal control of character.
		if (slideDirection.magnitude < maxControllableSlideMagnitude)
		{
			moveVector += slideDirection;
		}
		else
		{
			moveVector = slideDirection;
		}
	}

	public void Jump()
	{
		if (thirdPersonController.charController.isGrounded)
		{
			verticalVelocity = jumpSpeed;
		}
	}

	private float moveSpeed()
	{
		float moveSpeed = 0f;

		switch (thirdPersonAnimator.instance.moveDirection)
		{
			case thirdPersonAnimator.Direction.Stationary:
				moveSpeed = 0f;
				break;
			case thirdPersonAnimator.Direction.Forward:
				moveSpeed = forwardSpeed;
				break;
			case thirdPersonAnimator.Direction.Backward:
				moveSpeed = backwardSpeed;
				break;
			case thirdPersonAnimator.Direction.LeftForward:
				moveSpeed = forwardSpeed;
				break;
			case thirdPersonAnimator.Direction.RightForward:
				moveSpeed = forwardSpeed;
				break;
			case thirdPersonAnimator.Direction.LeftBackward:
				moveSpeed = backwardSpeed;
				break;
			case thirdPersonAnimator.Direction.RightBackward:
				moveSpeed = backwardSpeed;
				break;
			case thirdPersonAnimator.Direction.Left:
				moveSpeed = strafingSpeed;
				break;
			case thirdPersonAnimator.Direction.Right:
				moveSpeed = strafingSpeed;
				break;
		}

		if (slideDirection.magnitude > 0)
			moveSpeed = slidingSpeed;

		return moveSpeed;
	}
}