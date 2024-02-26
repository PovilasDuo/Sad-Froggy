using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject gameObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnObject(gameObject);

	}

    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void SpawnObject(GameObject gameObject)
    {
        Instantiate(gameObject);
    }
}
