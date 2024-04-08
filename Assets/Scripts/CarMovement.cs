using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float speed = 10f;
    public float endPosition;
    private int tileSize;
    private int tilesNumber;

    void Start()
    {
        tileSize = GameObject.Find("TilemapManager").GetComponent<TilemapManager>().tileSize;
        tilesNumber = GameObject.Find("TilemapManager").GetComponent<TilemapManager>().tilesNumberZ;
        endPosition = tilesNumber * tileSize + tileSize;
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (transform.position.x >= endPosition / 2)
        {
            Destroy(gameObject);
        }
    }
}
