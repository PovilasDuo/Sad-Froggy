using System.Collections;
using UnityEngine;
public class Movement : MonoBehaviour
{
    public PlayerSettings playerSettings;

    public Animator animator;
	public AudioSource jumpSound;
	public AudioSource deathSound;

	public float moveSpeed;
	public float moveDistance = 5f;

	private FrogActions controls;
	private Vector2 movementInput;
	private float nextMoveTime = 0f;

	public bool hasKey;

	//Sets variables necessary for the movement to perform correctly
	private void Awake()
	{
		controls = new FrogActions();

		controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>(); //Reads values on key press
		controls.Player.Move.canceled += ctx => movementInput = Vector2.zero; //Sets the value to zero if it is released

		controls.Player.Enable(); //Enables the controls
	}

	private void Start()
	{
		moveSpeed = playerSettings.moveSpeed;
		AudioSource[] audioSources = GetComponents<AudioSource>();
		jumpSound = audioSources[0];
		deathSound = audioSources[1];

		animator = GetComponent<Animator>();
	}


	private void Update()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") || animator.IsInTransition(0))
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
				Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);
				RaycastHit hit;
				Physics.SphereCast(transform.position, 1.0f, moveDirection, out hit, moveDistance);

                if (hit.collider != null && hit.collider.tag == "Obstacle")
				{

				}
                else if (hit.collider != null && hit.collider.tag == "NextLevelDoor" && !hasKey || hit.collider != null && hit.collider.tag == "ExitLevelDoor" && !hasKey)
				{

                }
                else
				{
					nextMoveTime = Time.time + (1.25f / 2 / moveSpeed);
					transform.Translate(moveDirection * moveDistance, Space.World);
				}
				Rotate();
			}
		}
	}

	/// <summary>
	/// Jumps and waits for a period
	/// </summary>
	private IEnumerator JumpAndWait()
	{
		animator.SetBool("JumpBool", true);
		//yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 2 / moveSpeed);
		yield return null;
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
			float targetAngle = Mathf.Atan2(movementInput.x, movementInput.y) * Mathf.Rad2Deg;
			Quaternion toRotation = Quaternion.Euler(0f, targetAngle, 0f);
			transform.rotation = toRotation;
		}
	}
}

