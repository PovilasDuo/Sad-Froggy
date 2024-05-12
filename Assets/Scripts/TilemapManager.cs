using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [Header("Settings")]
    public PlayerSettings playerSettings;

    [Header("Tilemap")]
	public Tilemap tilemap;
	public int tilesNumberX = 11;
	public int tilesNumberZ = 11;
	public int tileSize = 10;

	[Header("Obstacles")]
	public GameObject roadTile;
	public GameObject riverTile;
	public GameObject grassTile;
	public GameObject tadpole;
	public GameObject car;
	public GameObject rock;
	public GameObject key;
	public GameObject border;

	[Header("Moving obstacles")]
	public GameObject leaf;

	[Header("Obstacle counts")]
	public int roadCount = 2;
	public int riverCount = 2;
	public int rockCount = 5;
    public int tadpoleCount = 7;

    [Header("Doors")]
	public GameObject exitDoor;
	public GameObject nextLevelDoor;
	public GameObject gates;

	private LinearObstacles[] occupiedRows;
	private bool[][] occupiedTiles;

	private int borderNumber = 0;
	private int borderCount = 0;
	private int exitGap, nextLevelGap;

	public GameObject levelGameobject;
	public float levelNr = 1;

	public bool destroying = false;

	public enum LinearObstacles
	{
		None,
		River,
		Road
	}

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
	///  Generates a map of default size
	/// </summary>
	/// <param name="top">If the top border should be emmitted</param>
	public void GenerateMap(bool top, bool camerMovement)
	{
		IterateDifficulity();
		ClearOccupation();
		SetTilemapSize(tilesNumberX, tilesNumberZ);
		SetTilemapPosition();

		CreateLinearObstacles(roadTile, roadCount);
		CreateLinearObstacles(riverTile, riverCount);
		CreateObstacles(rock, rockCount);
		CreateLeaves();
		levelGameobject.AddComponent<CarManager>();
		levelGameobject.GetComponent<CarManager>().occupiedRows = occupiedRows;

		CreateCollectibles(tadpole, tadpoleCount);
		SpawnKeyRandom();

		CreateBase();
		if (top)
		{
			levelGameobject.transform.position = new Vector3(levelGameobject.transform.position.x, 0, levelGameobject.transform.position.z + (tilesNumberZ + 1) * tileSize);
			if (camerMovement)
			{
				Camera.main.GetComponent<CameraVisibility>().enabled = false;
				float positionToMoveTo = (GameObject.Find("Level " + (levelNr - 2)).transform.position.z - (tilesNumberZ + 1) / 2 * tileSize) - Camera.main.transform.position.z;
				StartCoroutine(SmoothCameraMovement(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + positionToMoveTo), 0.5f));
				Camera.main.GetComponent<CameraVisibility>().enabled = true;
			}
			CreateBorders(true, false, true, true);
		}
		else CreateBorders(true, !top, true, true);

		PrepareNextLevel();
	}

	public void IterateDifficulity()
	{
		if (levelNr % 5 == 0)
		{
			roadCount++;
		}
		else if (levelNr % 2 == 0)
		{
			tilesNumberZ++;
			levelGameobject.transform.position = new Vector3(levelGameobject.transform.position.x, levelGameobject.transform.position.y, levelGameobject.transform.position.z - tileSize);
			rockCount++;
			tadpoleCount += playerSettings.tadpoleIncrease;
            Camera.main.GetComponent<CameraVisibility>().speed++;
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
		occupiedRows = Enumerable.Repeat(LinearObstacles.None, tilesNumberZ).ToArray();
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

			if (obstacle == roadTile) occupiedRows[obstaclePosition] = LinearObstacles.Road;
			else if (obstacle == riverTile) occupiedRows[obstaclePosition] = LinearObstacles.River;

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

			Vector3 worldPos = tilemapOrigin + new Vector3(obstaclePosition.x * tileSize, 0, obstaclePosition.z * tileSize);

			GameObject go = Instantiate(obstacle, worldPos, Quaternion.identity);
			go.transform.SetParent(levelGameobject.transform, false);
		}
	}

	/// <summary>
	/// Creates leaves on the river
	/// </summary>
	public void CreateLeaves()
	{
		var tilemapOrigin = tilemap.transform.position;

		for (int r = 0; r < occupiedRows.Length; r++)
		{
			if (occupiedRows[r] == LinearObstacles.River)
			{
				var positions = new List<int>();
				var leavesCount = UnityEngine.Random.Range(1, 3);

				for (int i = 0; i < leavesCount; i++)
				{
					int newPos;
					do
					{
						newPos = UnityEngine.Random.Range(0, tilesNumberX);
					} while (positions.Contains(newPos));
					positions.Add(newPos);

					Vector3 worldPos = tilemapOrigin + new Vector3(newPos * tileSize, 0.1f, r * tileSize);
					GameObject go = Instantiate(leaf, worldPos, Quaternion.identity);
					go.transform.SetParent(levelGameobject.transform, false);
				}
			}
		}
	}

	/// <summary>
	/// Creates cars on the roads
	/// </summary>
	/// <param name="levelGameobject"></param>
	public GameObject CreateCar(GameObject levelGameobject, LinearObstacles[] occupiedRows)
	{
		var tilemapOrigin = tilemap.transform.position;
		GameObject go = new GameObject();
		for (int r = 0; r < occupiedRows.Length; r++)
		{
			if (occupiedRows[r] == LinearObstacles.Road)
			{
				Vector3 worldPos = tilemapOrigin + new Vector3(-1 * tileSize, 1f, r * tileSize);
				bool backwards = false;
				if (Random.Range(0, 2) == 1)
				{
					worldPos = new Vector3(-worldPos.x, worldPos.y, worldPos.z);
					backwards = true;
				}
				go = Instantiate(car, worldPos, Quaternion.identity);
				go.transform.SetParent(levelGameobject.transform, false);
				if (backwards)
				{
					go.GetComponent<CarMovement>().backwards = true;
					go.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
				}
			}
		}
		return go;
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
			if (occupiedRows[z] == LinearObstacles.None)
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
		GetRandomGaps();

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
				CreateBorderTile(mapSize.x * tileSize, 0, z * tileSize, tilemapOrigin, new Vector3(0f, 90f, 0f), ShouldCreateGate(z)); // Right border
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
				CreateBorderTile(-tileSize, 0, z * tileSize, tilemapOrigin, new Vector3(0f, -90f, 0f), ShouldCreateGate(z)); // Left border
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
	private void CreateBorderTile(float x, float y, float z, Vector3 origin, Vector3 rotation, bool shouldCreateGate = false)
	{
		borderNumber++;
		Vector3 worldPos = new Vector3(x, y, z) + origin;
		GameObject go = Instantiate(grassTile, worldPos, Quaternion.identity);
		go.transform.SetParent(levelGameobject.transform, false);

		Vector3 localPos = go.transform.localPosition;
		if (shouldCreateGate)
		{
			worldPos.y += 3;
			go = Instantiate(gates, worldPos, Quaternion.Euler(new Vector3(0, 0, 0)));
		}
		else if (borderNumber == exitGap)
		{
			go = Instantiate(exitDoor, worldPos, Quaternion.Euler(rotation));
		}
		else if (borderNumber == nextLevelGap)
		{
			go = Instantiate(nextLevelDoor, worldPos, Quaternion.Euler(rotation));
		}
		else
		{
			go = Instantiate(border, worldPos, Quaternion.Euler(rotation));
		}
		go.transform.SetParent(levelGameobject.transform, false);
	}

	/// <summary>
	/// Check if a gate should be created in the position
	/// </summary>
	/// <param name="z">Position</param>
	/// <returns>True if it should be created</returns>
	private bool ShouldCreateGate(int z)
	{
		if (z >= 0 && z < tilesNumberZ && occupiedRows[z] == LinearObstacles.Road)
			return true;
		return false;
	}

	/// <summary>
	/// Creates random gaps for exit and next level doors
	/// </summary>
	private void GetRandomGaps()
	{
		exitGap = Mathf.CeilToInt((tilesNumberX + 2) / 2f);
		nextLevelGap = Mathf.CeilToInt((tilesNumberX + 2) / 2f + 1);
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
			row = UnityEngine.Random.Range(1, tilesNumberZ);
		} while (occupiedRows[row] != LinearObstacles.None);
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
			cellX = UnityEngine.Random.Range(0, tilesNumberX);
			cellZ = UnityEngine.Random.Range(1, tilesNumberZ);
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
