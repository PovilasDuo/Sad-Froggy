using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject tileMapGO;
    public GameObject UIManagerGO;

    private TilemapManager tilemapManagerInstance;
    private UIManager uIManagerInstance;

    public float tadPoleCount;

    void Start()
    {
		tileMapGO = GameObject.Find("TilemapManager");
        UIManagerGO = GameObject.Find("UIManager");
		tilemapManagerInstance = tileMapGO.GetComponent<TilemapManager>();
		uIManagerInstance = UIManagerGO.GetComponent<UIManager>();

        tilemapManagerInstance.GenerateMap(false, false);
		tilemapManagerInstance.GenerateMap(true, false);
		tilemapManagerInstance.GenerateMap(true, false);
		tadPoleCount = tilemapManagerInstance.tadpoleCount;
	}
}
