using System.Collections;
using UnityEngine;

public class Collision : MonoBehaviour
{

	private void OnTriggerEnter(Collider other)
	{
		if (!GameObject.Find("TilemapManager").GetComponent<TilemapManager>().destroying)
		{
			if (other.gameObject.CompareTag("Obstacle") || other.gameObject.CompareTag("ExitLevelDoor") || other.gameObject.CompareTag("NextLevelDoor"))
			{
				GameObject.Find("TilemapManager").GetComponent<TilemapManager>().destroying = true;
				GameObject parentObject = other.gameObject.transform.parent.gameObject;
				Debug.Log(this.gameObject.name + " collided with " + other.gameObject.name);
				Destroy(parentObject);
				StartCoroutine(Destroying());
			}
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	IEnumerator Destroying()
	{
		yield return new WaitForSeconds(2f);
		GameObject.Find("TilemapManager").GetComponent<TilemapManager>().destroying = false;
	}
}
