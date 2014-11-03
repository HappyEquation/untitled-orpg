using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour 
{
	public float speed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;
	private Vector3 moveDirection = Vector3.zero;

    // FixedUpdate is called once per frame before physics are applied.
    void FixedUpdate()
    {
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");
		
		CharacterController controller = GetComponent<CharacterController>();
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
    }
}
