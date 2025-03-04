using UnityEngine;

public class Player : MonoBehaviour
{
	public float moveSpeed;
	public float groundDrag;
	public Transform orientation;
	public float playerHeight;
	public LayerMask whatIsGround;
	bool grounded;
	public float jumpForce;
	public float jumpCooldown;
	public float airMultiplier;
	bool readyToJump;
	public KeyCode jumpKey = KeyCode.Space;


	float horizontalInput;
	float verticalInput;

	Vector3 moveDirection;

	Rigidbody rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		readyToJump = true;
	}

	private void Update()
	{
		grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

		if (grounded)
		{
			rb.linearDamping = groundDrag;
		}
		else
		{
			rb.linearDamping = 0;
		}

		MyInput();
		SpeedControl();
	}

	private void FixedUpdate()
	{
		MovePlayer();
	}


	private void MyInput()
	{
		horizontalInput = Input.GetAxisRaw("Horizontal");
		verticalInput = Input.GetAxisRaw("Vertical");

		if(Input.GetKey(jumpKey) && readyToJump && grounded)
		{
			 readyToJump = false;
			 Jump();
			 Invoke(nameof(ResetJump), jumpCooldown);
		}
	}

	private void MovePlayer()
	{
		moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;


		if(grounded)
		{
		   rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
		}
		else if(!grounded)
		{
			rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
		}
	}

	private void SpeedControl()
	{
		Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

		if (flatVel.magnitude > moveSpeed)
		{
			Vector3 limitedVel = flatVel.normalized * moveSpeed;
			rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
		}
	}

	private void Jump()
	{
		rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

		rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
	}

	private void ResetJump()
	{
		readyToJump = true;
	}
}