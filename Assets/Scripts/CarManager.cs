using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    public bool running = true;

    public void Awake()
    {
        StartCoroutine(CreateCarsCoroutine());
    }

    private IEnumerator CreateCarsCoroutine()
    {
        while (running)
        {
            GameObject.Find("TilemapManager").GetComponent<TilemapManager>().CreateCar(this.gameObject);

            yield return new WaitForSeconds(3);
        }
    }
}
