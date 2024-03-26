using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TilemapManager : MonoBehaviour
{
	[Header("Tilemap")]
	public Tilemap tilemap;
	public int tilesNumberX = 11;
	public int tilesNumberZ = 11;
	private int tileSize = 10;

	[Header("Obstacles")]
	public GameObject roadTile;
	public GameObject riverTile;
	public GameObject grassTile;
	public GameObject tadpole;
	public GameObject rock;
	public GameObject key;
	public GameObject border;

	[Header("Obstacle counts")]
	public int roadCount = 2;
	public int riverCount = 2;
	public int rockCount = 5;
	public int tadpoleCount = 7;

	[Header("Doors")]
	public GameObject exitDoor;
	public GameObject nextLevelDoor;

	private bool[] occupiedRows;
	private bool[][] occupiedTiles;

	private int borderNumber = 0;
	private int borderCount = 0;
	private int exitGap, nextLevelGap;

	public GameObject levelGameobject;
	public float levelNr = 1;

	public bool destroying = false;
	private void Awake()
	{
		levelGameobject = new GameObject();
		levelGameobject.name = "Level " + levelNr.ToString();
	}
	void Start()
	{

		tilemap = GetComponent<Tilemap>();
	}

	/// <summary>
	/// Generates a map of default size
	/// </summary>
	public void GenerateMap()
	{
		ClearOccupation();
		SetTilemapSize(tilesNumberX, tilesNumberZ);
		SetTilemapPosition();

		CreateLinearObstacles(roadTile, roadCount);
		CreateLinearObstacles(riverTile, riverCount);
		CreateObstacles(rock, rockCount);

		CreateCollectibles(tadpole, tadpoleCount);
		SpawnKeyRandom();

		CreateBase();
		CreateBorders(true, true, true, true);
		PrepareNextLevel();
	}

	/// <summary>
	/// Generates a map based on the position
	/// </summary>
	/// <param name="position">The position that it takes into account</param>
	public void GenerateMap(Vector3 position)
	{
		ClearOccupation();
		SetTilemapSize(tilesNumberX, tilesNumberZ);
		SetTilemapPosition();

		CreateLinearObstacles(roadTile, roadCount);
		CreateLinearObstacles(riverTile, riverCount);
		CreateObstacles(rock, rockCount);

		CreateCollectibles(tadpole, tadpoleCount);
		SpawnKeyRandom();

		CreateBase();
		CheckBorders(position);

		PrepareNextLevel();
	}

	public void CheckBorders(Vector3 position)
	{
		if ((position.x >= 0 && position.z >= 0) && (position.x > position.z))
		{
			//Debug.Log("right");
			levelGameobject.transform.position = new Vector3(levelGameobject.transform.position.x + (tilesNumberX + 1) * tileSize, 0, levelGameobject.transform.position.z);
			StartCoroutine(SmoothCameraMovement(new Vector3(Camera.main.transform.position.x + (tilesNumberX + 1) * tileSize * 1f, Camera.main.transform.position.y, Camera.main.transform.position.z), 3f));
			CreateBorders(true, true, true, false);
		}
		else if (position.x < 0 && position.z >= 0)
		{
			//Debug.Log("left");
			levelGameobject.transform.position = new Vector3(levelGameobject.transform.position.x - (tilesNumberX + 1) * tileSize, 0, levelGameobject.transform.position.z);
			StartCoroutine(SmoothCameraMovement(new Vector3(Camera.main.transform.position.x - (tilesNumberX + 1) * tileSize * 1f, Camera.main.transform.position.y, Camera.main.transform.position.z), 3f));
			CreateBorders(true, true, false, true);
		}
		else if (position.z >= 0 && position.z > position.x)
		{
			//Debug.Log("top");
			levelGameobject.transform.position = new Vector3(levelGameobject.transform.position.x, 0, levelGameobject.transform.position.z + (tilesNumberZ + 1) * tileSize);
			StartCoroutine(SmoothCameraMovement(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + (tilesNumberX + 1) * tileSize * 1f), 3f));
			CreateBorders(true, false, true, true);
		}
		else
		{
			//Debug.Log("bottom");
			levelGameobject.transform.position = new Vector3(levelGameobject.transform.position.x, 0, levelGameobject.transform.position.z - (tilesNumberZ + 1) * tileSize);
			StartCoroutine(SmoothCameraMovement(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - (tilesNumberX + 1) * tileSize * 1f), 3f));
			CreateBorders(false, true, true, true);
		}
	}

	/// <summary>
	/// Enumerates camera positino to the targeted position
	/// </summary>
	/// <param name="targetPosition">The position the camera should be</param>
	/// <param name="duration">Duration of the movement</param>
	/// <returns>Waits for the next fram</returns>
	IEnumerator SmoothCameraMovement(Vector3 targetPosition, float duration)
	{
		Vector3 startPosition = Camera.main.transform.position;
		float startTime = Time.time;

		while (Time.time - startTime < duration)
		{
			float t = (Time.time - startTime) / duration;
			Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
			yield return null; // Waits for the next frame
		}
		Camera.main.transform.position = targetPosition;
	}

	/// <summary>
	/// Clears variables for the next level
	/// </summary>
	private void ClearOccupation()
	{
		occupiedRows = new bool[tilesNumberZ];
		occupiedTiles = new bool[tilesNumberX][];
		for (int i = 0; i < tilesNumberX; i++)
		{
			occupiedTiles[i] = new bool[tilesNumberZ];
		}
		borderCount = 0;
		borderNumber = 0;
	}

	/// <summary>
	/// Prepares levelGameojbect for the next level
	/// </summary>
	private void PrepareNextLevel()
	{
		Vector3 position = levelGameobject.transform.position;
		levelNr++;
		levelGameobject = new GameObject();
		levelGameobject.name = "Level " + levelNr.ToString();
		levelGameobject.transform.position = position;

		if (levelNr > 4)
		{
			Destroy(GameObject.Find("Level " + (levelNr - 5).ToString()));
		}
	}

	/// <summary>
	/// sets prefered tilemap size
	/// </summary>
	/// <param name="sizeX">Tiles Number X</param>
	/// <param name="sizeZ">Tiles Number Z</param>
	private void SetTilemapSize(int sizeX, int sizeZ)
	{
		tilemap.size = new Vector3Int(sizeX, 1, sizeZ);
		tilemap.tileAnchor = new Vector3(0f, 0f, 0f);
	}

	/// <summary>
	/// set tilemap in the middle
	/// </summary>
	private void SetTilemapPosition()
	{
		tilemap.transform.position = new Vector3(-tileSize * tilesNumberX / 2 + tileSize / 2, 0, 0);
	}

	/// <summary>
	/// creates linear obstacles like roads
	/// </summary>
	/// <param name="obstacle">square obstacle object</param>
	/// <param name="obstacleCount">count of obstacles (e.g. roads)</param>
	public void CreateLinearObstacles(GameObject obstacle, int obstacleCount)
	{
		Vector3 tilemapOrigin = tilemap.transform.position;

		for (int c = 0; c < obstacleCount; c++)
		{
			int obstaclePosition = GetRandomEmptyRow();
			occupiedRows[obstaclePosition] = true;

			for (int x = 0; x < tilesNumberX; x++)
			{
				occupiedTiles[x][obstaclePosition] = true;
				Vector3 worldPos = tilemapOrigin + new Vector3(x * tileSize, 0, obstaclePosition * tileSize);

				GameObject go = Instantiate(obstacle, worldPos, Quaternion.identity);
				go.transform.SetParent(levelGameobject.transform, false);
			}
		}
	}

	/// <summary>
	/// creates simple obstacles
	/// </summary>
	/// <param name="obstacle">obstacle object</param>
	/// <param name="obstacleCount">count of obstacles (e.g. rocks)</param>
	public void CreateObstacles(GameObject obstacle, int obstacleCount)
	{
		Vector3 tilemapOrigin = tilemap.transform.position;

		for (int c = 0; c < obstacleCount; c++)
		{
			Vector3 obstaclePosition = GetRandomCell();
			occupiedTiles[(int)obstaclePosition.x][(int)obstaclePosition.z] = true;

			Vector3 worldPos = tilemapOrigin + new Vector3(obstaclePosition.x * tileSize, obstacle.GetComponent<Renderer>().bounds.size.y / 2, obstaclePosition.z * tileSize);

			GameObject go = Instantiate(obstacle, worldPos, Quaternion.identity);
			go.transform.SetParent(levelGameobject.transform, false);
		}
	}

	/// <summary>
	/// Creates collectible gameobjects
	/// </summary>
	/// <param name="obstacle">Collectible gameobject</param>
	/// <param name="obstacleCount">How many of them are there</param>
	public void CreateCollectibles(GameObject obstacle, int obstacleCount)
	{
		Vector3 tilemapOrigin = tilemap.transform.position;

		for (int c = 0; c < obstacleCount; c++)
		{
			Vector3 obstaclePosition = GetRandomCell();
			occupiedTiles[(int)obstaclePosition.x][(int)obstaclePosition.z] = true;

			Vector3 worldPos = tilemapOrigin + new Vector3(obstaclePosition.x * tileSize, 2, obstaclePosition.z * tileSize);

			GameObject go = Instantiate(obstacle, worldPos, Quaternion.identity);
			go.transform.SetParent(levelGameobject.transform, false);
		}
	}

	/// <summary>
	/// creates base under obstacles(e.g. grass)
	/// </summary>
	public void CreateBase()
	{
		Vector3 tilemapOrigin = tilemap.transform.position;

		for (int z = 0; z < tilesNumberZ; z++)
		{
			if (occupiedRows[z] == false)
			{
				for (int x = 0; x < tilesNumberX; x++)
				{
					Vector3 worldPos = tilemapOrigin + new Vector3(x * tileSize, 0, z * tileSize);

					GameObject go = Instantiate(grassTile, worldPos, Quaternion.identity);
					go.transform.SetParent(levelGameobject.transform, false);
				}
			}
		}
	}

	/// <summary>
	/// Creates borders for the level
	/// </summary>
	/// <param name="top">If TOP the border should be created</param>
	/// <param name="bottom">If BOTTOM the border should be created</param>
	/// <param name="right">If RIGHT the border should be created</param>
	/// <param name="left">If LEFT the border should be created</param>
	public void CreateBorders(bool top, bool bottom, bool right, bool left)
	{
		Vector3 tilemapOrigin = tilemap.transform.position;
		Vector3Int mapSize = tilemap.size;

		borderCount = (tilesNumberX + tilesNumberZ) * 2 + 2;
		GetRandomGaps(top, bottom, right, left);

		if (top)
		{
			for (int x = -1; x <= mapSize.x; x++)
			{
				CreateBorderTile(x * tileSize, 0, mapSize.z * tileSize, tilemapOrigin, Vector3.zero); // Bottom border
			}
		}
		if (right)
		{
			for (int z = mapSize.z; z >= 0; z--)
			{
				CreateBorderTile(mapSize.x * tileSize, 0, z * tileSize, tilemapOrigin, new Vector3(0f, 90f, 0f)); // Right border
			}
		}
		if (bottom)
		{
			for (int x = mapSize.x; x >= -1; x--)
			{
				CreateBorderTile(x * tileSize, 0, -tileSize, tilemapOrigin, Vector3.zero); // Top border
			}
		}
		if (left)
		{
			for (int z = 0; z <= mapSize.z; z++)
			{
				CreateBorderTile(-tileSize, 0, z * tileSize, tilemapOrigin, new Vector3(0f, -90f, 0f)); // Left border
			}
		}
	}

	/// <summary>
	/// Creates a border tile for the borders. Including the doors
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">>Y coordinate</param>
	/// <param name="z">Z coordinate</param>
	/// <param name="origin">Origin position</param>
	/// <param name="rotation">Rotation in which to rotate the border or door</param>
	private void CreateBorderTile(float x, float y, float z, Vector3 origin, Vector3 rotation)
	{
		borderNumber++;
		Vector3 worldPos = new Vector3(x, y, z) + origin;
		GameObject go = Instantiate(grassTile, worldPos, Quaternion.identity);
		go.transform.SetParent(levelGameobject.transform, false);

		Vector3 localPos = go.transform.localPosition;
		Vector3 worldPosForRaycast = go.transform.parent.TransformPoint(localPos);

		RaycastHit hit;
		if (Physics.SphereCast(worldPosForRaycast, 3.0f, Vector3.up, out hit, 3f) ||
			Physics.SphereCast(worldPosForRaycast, 3.0f, Vector3.down, out hit, 3f))
		{
			Debug.Log("Parent object: " + hit.collider.name );
			Destroy(hit.collider.transform.parent.gameObject);

		}

		if (borderNumber == exitGap)
		{
			worldPos.y += 3; // TODO remove when we have a model for door
			go = Instantiate(exitDoor, worldPos, Quaternion.Euler(rotation));
		}
		else if (borderNumber == nextLevelGap)
		{
			worldPos.y += 3; // TODO remove when we have a model for door
			go = Instantiate(nextLevelDoor, worldPos, Quaternion.Euler(rotation));
		}
		else
		{
			go = Instantiate(border, worldPos, Quaternion.Euler(rotation));
		}
		go.transform.SetParent(levelGameobject.transform, false);
	}

	/// <summary>
	/// Searches and creates exits
	/// </summary>
	/// <param name="top">If TOP the border should be created</param>
	/// <param name="bottom">If BOTTOM the border should be created</param>
	/// <param name="right">If RIGHT the border should be created</param>
	/// <param name="left">If LEFT the border should be created</param>
	private void GetRandomGaps(bool top, bool bottom, bool right, bool left)
	{
		int randomN = (int)Mathf.Round(Random.Range(1f, 8f));
		int addative = 2;

		if (!top || !bottom || !right || !left)
		{
			randomN = (int)Mathf.Round(Random.Range(1f, 4f));
			addative = 2;
		}

		///Diferent sets of multiplications for different sizes of levels
		float[] multiplactionOdd = new float[] { 3.5f, 6f, 8f };
		float[] multiplcationNOdd = new float[] { 3.5f, 5.5f, 7.5f };
		float[] multiplcationEven = new float[] { 3f, 5f, 6.5f };
		var multiplication = new float[3];

		if ((tilesNumberZ % 2 == 0 && tilesNumberX % 2 != 0) || (tilesNumberZ % 2 != 0 && tilesNumberX % 2 == 0)) multiplication = multiplactionOdd;
		else if (tilesNumberZ % 2 != 0 && tilesNumberX % 2 != 0) multiplication = multiplcationNOdd;
		else multiplication = multiplcationEven;

		if (randomN == 1)
		{
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2f);
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[0]);
		}
		else if (randomN == 2)
		{
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2f);
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[0]);
		}
		else if (randomN == 3)
		{
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[0]);
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[1]);
		}
		else if (randomN == 4)
		{
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[0]);
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[1]);
		}
		else if (randomN == 5)
		{
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[2]);
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[1]);
		}
		else if (randomN == 6)
		{
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[2]);
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[1]);
		}
		else if (randomN == 7)
		{
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[2]);
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2f);
		}
		else if (randomN == 8)
		{
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[2]);
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2f);
		}
		//Debug.Log("random: " + randomN + "; " + "exitGap: " + exitGap + " nextLevelGap: " + nextLevelGap + " addative:" + addative); ////For debugging exits
	}


	/// <summary>
	/// returns row where no other obstacles exist
	/// </summary>
	/// <returns>index of free row</returns>
	private int GetRandomEmptyRow()
	{
		int row;
		do
		{
			row = Random.Range(1, tilesNumberZ);
		} while (occupiedRows[row]);

		return row;
	}


	/// <summary>
	/// returns cell where no other obstacles exist
	/// </summary>
	/// <returns>coordinates of free cell</returns>
	private Vector3 GetRandomCell()
	{
		int cellX, cellZ;
		do
		{
			cellX = Random.Range(0, tilesNumberX);
			cellZ = Random.Range(1, tilesNumberZ);
		} while (occupiedTiles[cellX][cellZ]);

		return new Vector3(cellX, 0, cellZ);
	}

	/// <summary>
	/// Spawns a key in a random position
	/// </summary>
	public void SpawnKeyRandom()
	{
		Vector3 tilemapOrigin = tilemap.transform.position;
		Vector3 obstaclePosition = GetRandomCell();
		occupiedTiles[(int)obstaclePosition.x][(int)obstaclePosition.z] = true;

		Vector3 worldPos = tilemapOrigin + new Vector3(obstaclePosition.x * tileSize, key.GetComponent<Renderer>().bounds.size.y / 2, obstaclePosition.z * tileSize);

		GameObject go = Instantiate(key, worldPos, Quaternion.identity);
		go.transform.SetParent(levelGameobject.transform, false);
	}
}
