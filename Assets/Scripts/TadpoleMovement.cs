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
        if(Time.time > nextMoveInterval) 
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
        moveDirection = new Vector3(Random.value * Random.Range(-1, 1), 0, Random.value * Random.Range(-1, 1));
	}

    private void setNextMoveTime()
    {
        nextMoveInterval = Time.time + Random.Range(minInterval, maxInterval);
    }

	public void RotateTowardsMovementDirection()
	{
		// Calculate the target rotation angle based on the movement direction
		float targetAngle = Mathf.Atan2(Mathf.Abs(moveDirection.x), Mathf.Abs(moveDirection.z)) * Mathf.Rad2Deg;
        Debug.Log(targetAngle);
		// Create a quaternion for the rotation
		Quaternion toRotation = Quaternion.Euler(0f, targetAngle, 0f);

		// Smoothly rotate towards the target rotation
		this.transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, speed * Time.deltaTime);
	}
}
