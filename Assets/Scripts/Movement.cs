using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Movement : MonoBehaviour
{
	public Animator animator;

	private Actions controls;
	private Vector2 movementInput;
	//private bool canMove = true;
	private float nextMoveTime = 0f;

	public float moveSpeed = 1f;

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

	private void Update()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !animator.IsInTransition(0))
		{
			animator.speed = moveSpeed;
		}
		else
		{
			animator.speed = 1f;
		}
		Move();
	}

	private void Move()
	{
		if (controls.Player.Move.IsPressed() && Time.time >= nextMoveTime)
		{
			if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !animator.IsInTransition(0))
			{
				//animator.SetBool("JumpBool", true);
				StartCoroutine(JumpAndWait());
			}
			Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);
			transform.Translate(moveDirection * moveSpeed, Space.World);
			Rotate();
			nextMoveTime = Time.time + 1.25f / moveSpeed;
		}
	}

	private IEnumerator JumpAndWait()
	{
		animator.SetBool("JumpBool", true);

		// Wait until the jump animation starts playing
		while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
		{
			yield return null;
		}

		// Wait for the jump animation to complete
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / moveSpeed / 2);

		// Check if the player is still moving
		if (!controls.Player.Move.IsPressed())
		{
			// If not, transition back to the Idle state
			animator.SetBool("JumpBool", false);
		}
		else
		{
			// If the player is still moving, wait for the movement to stop
			while (controls.Player.Move.IsPressed())
			{
				yield return null;
			}

			// Transition back to the Idle state
			animator.SetBool("JumpBool", false);
		}
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
			Quaternion toRotation = Quaternion.Euler(0f, targetAngle, 0f);

			// Snap the rotation directly
			transform.rotation = toRotation;
		}
	}

	bool IsAnimationPlaying(string animationName)
	{
		// Get the current state of the Animator
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

		// Check if the specified animation is playing by comparing the name
		return stateInfo.IsName(animationName);
	}
}

