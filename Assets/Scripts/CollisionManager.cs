using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    private UIManager uIManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnCollisionEnter(Collision collision)
	{
		GameObject collided = collision.gameObject;
		if (collided.tag == "Tadpole")
		{
			uIManager.IncreasePoints(1);
			Destroy(collided);
		}
	}
}
