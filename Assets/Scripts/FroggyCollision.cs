using UnityEngine;

public class FroggyCollision : Collision
{
	private UIManager uIManagerInstance;
	private float collisionTime = 0;

	void Start()
	{
		uIManagerInstance = GameObject.Find("UIManager").GetComponent<UIManager>();
	}
/*
	private void OnCollisionEnter(UnityEngine.Collision collision)
	{
		GameObject collided = collision.gameObject;

		if (collisionTime != Time.time)
		{
			if (collided.tag == "DeadlyObstacle")
			{
				uIManagerInstance.ShowGameOver();
			}
			collisionTime = Time.time;
		}
	}*/

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
				this.gameObject.GetComponent<Movement>().hasKey = true;
				uIManagerInstance.KeyTextAppear(true);
				Destroy(collided);
			}
			if (gameObject.GetComponent<Movement>().hasKey)
			{
				if (collided.tag == "ExitLevelDoor")
				{
					uIManagerInstance.BackToMainMenu();
					Destroy(collided);
					this.gameObject.GetComponent<Movement>().hasKey = false;
					uIManagerInstance.KeyTextAppear(false);
				}
				else if (collided.tag == "NextLevelDoor")
				{
					GameObject.Find("TilemapManager").GetComponent<TilemapManager>().GenerateMap(true, true);
					Destroy(collided);
					this.gameObject.GetComponent<Movement>().hasKey = false;
					uIManagerInstance.KeyTextAppear(false);
				}
			}
			if (collided.tag == "Tadpole")
			{
				uIManagerInstance.IncreasePoints(1);
				Destroy(collided);
			}
			collisionTime = Time.time;
		}
	}
}
