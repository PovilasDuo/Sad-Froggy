using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    private UIManager uIManagerInstance;
	private float collisionTime = 0;

    void Start()
    {
		uIManagerInstance = GameObject.Find("UIManager").GetComponent<UIManager>();

	}

	/// <summary>
	/// Handles Game Object collision
	/// </summary>
	/// <param name="collision">Collision that occured</param>
	private void OnCollisionEnter(Collision collision)
	{
		GameObject collided = collision.gameObject;
		if (collisionTime != Time.time)
		{
			if (collided.tag == "Tadpole")
			{
				uIManagerInstance.IncreasePoints(1);
				Destroy(collided);
			}
			collisionTime = Time.time;
		}
	}

	/// <summary>
	/// Hanldes trigger collisions
	/// </summary>
	/// <param name="other">The GameObject that this GameObject collided with</param>
	private void OnTriggerEnter(Collider other)
	{
		GameObject collided = other.gameObject;
		if (collisionTime != Time.time)
		{
			if (collided.tag == "Key")
			{
				uIManagerInstance.KeyTextAppear();
				Destroy(collided);
			}
			collisionTime = Time.time;
		}
	}
}
