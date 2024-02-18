using UnityEngine;

public class Movement : MonoBehaviour
{
	public Animator animator;

	private Actions controls;
	private Vector2 movementInput;
	//private bool canMove = true;
	private float nextMoveTime = 0f;

	public float moveSpeed = 5f;

	private void Awake()
	{
		controls = new Actions();

		controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
		controls.Player.Move.canceled += ctx => movementInput = Vector2.zero;

		controls.Player.Enable();
	}

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	private void FixedUpdate()
	{
		//Move();
		//Rotate();
		if (controls.Player.Move.IsPressed() && Time.time >= nextMoveTime)
		{
			Move();
			Rotate();
			nextMoveTime = Time.time + 1f / moveSpeed;
		}
	}

	private void Move()
	{
		animator.SetBool("JumpBool", true);
		Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);
		//transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
		transform.Translate(moveDirection * moveSpeed, Space.World);
		animator.SetBool("JumpBool", false);
	}

	/// <summary>
	/// Rotates the player to the inputed direction.
	/// Rotation is performed instantaneous
	/// </summary>
	private void Rotate()
	{
		if (movementInput != Vector2.zero)
		{
			// Calculate the angle based on the movement input
			float targetAngle = Mathf.Atan2(movementInput.x, movementInput.y) * Mathf.Rad2Deg;

			// Create a rotation only around the Y-axis
			Quaternion toRotation = Quaternion.Euler(-90f, targetAngle, 0f);

			// Snap the rotation directly
			transform.rotation = toRotation;
		}
	}
}

