using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float speed = 10f;
    public float endPosition;
    private int tileSize;
    private int tilesNumber;
    public bool backwards = false;


    void Start()
    {
        tileSize = GameObject.Find("TilemapManager").GetComponent<TilemapManager>().tileSize;
        tilesNumber = GameObject.Find("TilemapManager").GetComponent<TilemapManager>().tilesNumberX;
		endPosition = tilesNumber * tileSize + tileSize;
		if (backwards)
        {
			endPosition *= -1;
		}
	}

    void Update()
    {
        if (backwards)
        {
			if (transform.position.x <= endPosition / 2)
			{
				Destroy(gameObject);
			}
		}
		else if (transform.position.x >= endPosition / 2)
		{
			Destroy(gameObject);
		}
		transform.Translate(Vector3.right * speed * Time.deltaTime);
	}
}
