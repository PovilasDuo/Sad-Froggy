using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Movement : MonoBehaviour
{
	public Animator animator;
	public AudioSource jumpSound;
	public AudioSource deathSound;

	public float moveSpeed = 1f;


	private FrogActions controls;
	private Vector2 movementInput;
	private float nextMoveTime = 0f;
	private void Awake()
	{
		controls = new FrogActions();

		controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
		controls.Player.Move.canceled += ctx => movementInput = Vector2.zero;

		controls.Player.Enable();
	}

	private void Start()
	{
		AudioSource[] audioSources = GetComponents<AudioSource>();
		jumpSound = audioSources[0];
		deathSound = audioSources[1];
		animator = GetComponent<Animator>();
	}


	private void Update()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !animator.IsInTransition(0))
		{
			animator.speed = moveSpeed; // Jump animation has to be according to the movement speed
		}
		else
		{
			animator.speed = 1f; // Jump animation has to be according to the movement speed
		}
		Move();
	}

	/// <summary>
	/// Moves the player in the desired direction
	/// </summary>
	private void Move()
	{
		if (controls.Player.Move.IsPressed() && Time.time >= nextMoveTime)
		{
			if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !animator.IsInTransition(0))
			{
				jumpSound.Play();
				StartCoroutine(JumpAndWait());
				if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !animator.IsInTransition(0))
				{
					Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);
					transform.Translate(moveDirection * moveSpeed, Space.World);
					Rotate();
					nextMoveTime = (Time.time + 1.25f) / 2 / moveSpeed; //1.25f is base animation 
				}
				else
				{
					Debug.Log("Timming of movement and animation is off"); //For dev purposes
				}
			}
		}
	}

	/// <summary>
	/// Jumps and waits for a period
	/// </summary>
	private IEnumerator JumpAndWait()
	{
		animator.SetBool("JumpBool", true);
		// Wait until the jump animation starts playing
		while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
		{
			yield return null;
		}

		// Wait for the jump animation to complete (only half of the animation is played)
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 2 / moveSpeed);
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
			Quaternion toRotation = Quaternion.Euler(0f, targetAngle, 0f);

			// Snap the rotation directly
			transform.rotation = toRotation;
		}
	}
}

