using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour 
{
	public float speed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;
	public float rotateSpeed = 80f;

	public Camera camera;
	private CharacterController controller;
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 forward = Vector3.zero;
	private Vector3 right = Vector3.zero;

    // FixedUpdate is called once per frame before physics are applied.
    void FixedUpdate()
    {
		//forward = transform.forward;
		//right = new Vector3(forward.z, 0, -forward.x);

		float moveHorizontal = Input.GetAxisRaw("Horizontal");
		float moveVertical = Input.GetAxisRaw("Vertical");

		//Vector3 targetDirection = moveHorizontal * right + moveVertical * forward;

		controller = gameObject.GetComponent<CharacterController>();

		//moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, 200 * Mathf.Deg2Rad * Time.deltaTime, 1000);
		
		//transform.Rotate(0, Input.GetAxis("Rotate") * rotateSpeed * Time.deltaTime, 0);
		
		if(controller.isGrounded)
		{
			moveDirection = new Vector3(moveHorizontal, 0, moveVertical);
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if(Input.GetButton("Jump"))
			{
				moveDirection.y = jumpSpeed;
			}
		}

		
		
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
		transform.Rotate(0, camera.transform.rotation.x, 0);
		//if (targetDirection != Vector3.zero)
		//{
		//	//transform.rotation = Quaternion.LookRotation(moveDirection);
		//}

		//Vector3 movement = moveDirection * Time.deltaTime * 2;
		//controller.Move(movement);
		
    }
}
