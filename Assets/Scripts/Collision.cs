using UnityEngine;

public class Collision : MonoBehaviour
{
	GameObject ps;
	AudioSource[] audioSources;

	private void Awake()
	{
		audioSources = GameObject.Find("AudioManager").GetComponents<AudioSource>();
	}

	/// <summary>
	/// Hanldes trigger collisions
	/// </summary>
	/// <param name="other">The GameObject that this GameObject collided with</param>
	private void OnTriggerEnter(Collider other)
	{
		GameObject collided = other.gameObject;
		if (collided.CompareTag("DeadlyObstacle"))
		{
			GameObject psPrefab = Resources.Load<GameObject>("ParticleSystem/Squash");
			if (psPrefab != null)
			{
				GameObject ps = Instantiate(psPrefab, transform.position, Quaternion.identity);
				ps.GetComponent<ParticleSystem>().Play();
			}
			if (audioSources != null)
			{
				if (collided.name == "car_rot(Clone)") //Car prefab
				{
					audioSources[1].Play();
				}
				else
				{
					audioSources[2].Play();
				}
			}
			else Debug.Log("Null");
			if (gameObject.name == "Froggy")
			{
				GameObject.Find("UIManager").GetComponent<UIManager>().ShowGameOver();
			}
			Destroy(gameObject);
		}
	}
}
