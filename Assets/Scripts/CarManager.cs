using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TilemapManager;

public class CarManager : MonoBehaviour
{
    public bool running = true;
	public LinearObstacles[] occupiedRows;
    private TilemapManager tilemapManager;

	public void Start()
    {
        tilemapManager = GameObject.Find("TilemapManager").GetComponent<TilemapManager>();
		StartCoroutine(CreateCarsCoroutine());
    }

    private IEnumerator CreateCarsCoroutine()
    {
        while (running)
        {
			yield return new WaitForSeconds(Random.Range(0f, 2f));
			GameObject car = tilemapManager.CreateCar(this.gameObject, occupiedRows);
            float wait = (tilemapManager.tilesNumberX * tilemapManager.tileSize) / car.GetComponent<CarMovement>().speed;
            yield return new WaitForSeconds(wait);
        }
    }
}
