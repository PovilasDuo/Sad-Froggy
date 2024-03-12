using UnityEngine;

public class TadpoleMovement : MonoBehaviour
{
	public float speed = 2f;
	public float minInterval = 1f;
	public float maxInterval = 3f;

	private float nextMoveInterval = 0;
	private Vector3 moveDirection;

	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (Time.time > nextMoveInterval)
		{
			MovemeRandomely();
			setNextMoveTime();
		}
	}

	public void MovemeRandomely()
	{
		GenerateDirection();
		this.transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
		RotateTowardsMovementDirection();
	}

	public void GenerateDirection()
	{
		moveDirection = new Vector3(Mathf.RoundToInt(Random.Range(-1f, 1f)), 0, Mathf.RoundToInt(Random.Range(-1f, 1f)));

		RaycastHit hit;
		Physics.SphereCast(transform.position, 1.0f, moveDirection, out hit, speed * Time.deltaTime);

		if (hit.collider != null && hit.collider.tag == "Obstacle")
		{
			moveDirection = new Vector3(Mathf.RoundToInt(Random.Range(-1f, 1f)), 0, Mathf.RoundToInt(Random.Range(-1f, 1f)));
			GenerateDirection();
		}
	}

	private void setNextMoveTime()
	{
		nextMoveInterval = Time.time + Random.Range(minInterval, maxInterval);
	}

	public void RotateTowardsMovementDirection()
	{
		float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
		Quaternion toRotation = Quaternion.Euler(0f, targetAngle, 0f);
		transform.rotation = toRotation;
	}
}
