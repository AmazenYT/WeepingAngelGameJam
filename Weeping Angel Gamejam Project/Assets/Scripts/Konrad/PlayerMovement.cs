using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private float moveSpeed;
	public float walkSpeed;
	public float sprintSpeed;
	public float crouchSpeed;
	public float crouchYScale;
	private float startYScale;
	public float groundDrag;
	public Transform orientation;
	public float playerHeight;
	public LayerMask whatIsGround;
	public bool grounded;
	public float jumpForce;
	public float jumpCooldown;
	public float airMultiplier;
	bool readyToJump;
	public float maxSlopeAngle;
	private RaycastHit slopeHit;
	private bool exitingSlope;
	public KeyCode jumpKey = KeyCode.Space;
	public KeyCode sprintKey = KeyCode.LeftShift;
	public KeyCode crouchKey = KeyCode.LeftControl;

	//For Footsteps//
	public AudioSource footstepsSound;
	public KeyCode w = KeyCode.W;
    public KeyCode a = KeyCode.A;
    public KeyCode s = KeyCode.S;
    public KeyCode d = KeyCode.D;


    float horizontalInput;
	float verticalInput;

	Vector3 moveDirection;

	Rigidbody rb;

	public MovementState state;

	public enum MovementState
	{
		walking,
		sprinting,
		crouching,
		air
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		readyToJump = true;
		startYScale = transform.localScale.y;
	}

	private void Update()
	{
		grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

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
		StateHandler();
	}

	private void FixedUpdate()
	{
		MovePlayer();

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
		{
			footstepsSound.enabled = true;
		}
		else
		{
			footstepsSound.enabled = false;
		}

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

		if(Input.GetKeyDown(crouchKey))
		{
			transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
			rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
		}

		if(Input.GetKeyUp(crouchKey))
		{
			transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
		}
	}


	private void StateHandler()
	{
		if(grounded && Input.GetKey(sprintKey))
		{
			state = MovementState.sprinting;
			moveSpeed = sprintSpeed;
		}
		else if(grounded)
		{
			state = MovementState.walking;
			moveSpeed = walkSpeed;
		}
		else
		{
			state = MovementState.air;
		}

		if(Input.GetKey(crouchKey))
		{
			state = MovementState.crouching;
			moveSpeed = crouchSpeed;
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

		if(OnSlope() && !exitingSlope)
		{
			rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

			if(rb.linearVelocity.y > 0)
			{
				rb.AddForce(Vector3.down * 80f, ForceMode.Force);
			}
		}

		rb.useGravity = !OnSlope();

	}

	private void SpeedControl()
	{
		if(OnSlope() && !exitingSlope)
		{
			if(rb.linearVelocity.magnitude > moveSpeed)
			{
				rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
			}
			else
			{
				Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
				if (flatVel.magnitude > moveSpeed)
		        {
			           Vector3 limitedVel = flatVel.normalized * moveSpeed;
			           rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
		        }
			}
		}
	}

	private void Jump()
	{
		rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

		rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

		exitingSlope = true;
	}

	private void ResetJump()
	{
		readyToJump = true;

		exitingSlope = false;
	}

	public bool OnSlope()
	{
		if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
		{
			float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
			return angle < maxSlopeAngle && angle != 0;
		}

		return false;
	}

	public Vector3 GetSlopeMoveDirection(Vector3 direction)
	{
		return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
	}
}