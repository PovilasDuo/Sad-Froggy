using UnityEngine;

public class CameraVisibility : MonoBehaviour
{
	public float speed = 10f;

	private Camera mainCamera;
	private GameObject froggy;

	// Start is called before the first frame update
	void Start()
	{
		mainCamera = Camera.main;
		froggy = GameObject.Find("Froggy");
	}

	// Update is called once per frame
	void Update()
	{
		Debug.Log("camera updating");
		if (froggy != null)
		{
			mainCamera.transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
			if (!CheckVisibility())
			{
				Destroy(froggy);
				GameObject.Find("UIManager").GetComponent<UIManager>().ShowGameOver();
			}
		}
	}

	/// <summary>
	/// Checks if the object is visible within the cameras projected planes
	/// </summary>
	private bool CheckVisibility()
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
		return GeometryUtility.TestPlanesAABB(planes, froggy.GetComponent<BoxCollider>().bounds);
	}
}