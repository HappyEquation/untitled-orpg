using UnityEngine;
using System.Collections;

public class thirdPersonAnimator : MonoBehaviour
{
	public static thirdPersonAnimator instance;			// Self reference

	public enum Direction
	{
		Stationary, Forward, Backward, Left, Right,
		LeftForward, RightForward, LeftBackward, RightBackward
	}

	public Direction moveDirection { get; set; }

	private void Awake()
	{
		instance = this;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void DetermineCurrentMoveDirection()
	{
		bool forward = false;
		bool backward = false;
		bool left = false;
		bool right = false;

		if (thirdPersonMotor.instance.moveVector.z > 0)
			forward = true;

		if (thirdPersonMotor.instance.moveVector.z < 0)
			backward = true;

		if (thirdPersonMotor.instance.moveVector.x > 0)
			right = true;

		if (thirdPersonMotor.instance.moveVector.x < 0)
			left = true;

		if (forward)
		{
			if (left)
				moveDirection = Direction.LeftForward;
			else if (right)
				moveDirection = Direction.RightForward;
			else
				moveDirection = Direction.Forward;
		}
		else if (backward)
		{
			if (left)
				moveDirection = Direction.LeftBackward;
			else if (right)
				moveDirection = Direction.RightBackward;
			else
				moveDirection = Direction.Backward;
		}
		else if (left)
			moveDirection = Direction.Left;
		else if (right)
			moveDirection = Direction.Right;
		else
			moveDirection = Direction.Stationary;
	}
}