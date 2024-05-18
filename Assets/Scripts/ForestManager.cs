using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class ForestManager : MonoBehaviour
{
	public Tilemap tilemap;
	public int tilesNumberX = 11;
	public int tilesNumberZ = 11;
	public int tileSize = 10;
	public GameObject grassTile;
	public List<GameObject> obstaclePrefabs; // List of obstacle prefabs
	public GameObject referenceObject; // The reference object for the offset
	private GameObject leftGO;
	private GameObject rightGO;

	public void CreateForest()
	{
		leftGO = new GameObject("leftGO");
		leftGO.transform.SetParent(referenceObject.transform);
		rightGO = new GameObject("rightGO");
		rightGO.transform.SetParent(referenceObject.transform);
		CreateBase();
		SpawnObstacles();
	}

	/// <summary>
	/// Calculates the tilemap origin based on the reference object's right edge.
	/// </summary>
	private Vector3 CalculateTilemapOriginRight()
	{
		return new Vector3(referenceObject.transform.position.x + (tilesNumberX * tileSize)/ 1.5f, referenceObject.transform.position.y, referenceObject.transform.position.z);
	}

	/// <summary>
	/// Calculates the tilemap origin based on the reference object's left edge.
	/// </summary>
	private Vector3 CalculateTilemapOriginLeft()
	{
		return new Vector3(referenceObject.transform.position.x - (tilesNumberX * tileSize) * 1.5f, referenceObject.transform.position.y, referenceObject.transform.position.z);
	}

	/// <summary>
	/// Creates base under obstacles (e.g., grass).
	/// </summary>
	public void CreateBase()
	{
		Vector3 tilemapOriginRight = CalculateTilemapOriginRight();
		Vector3 tilemapOriginLeft = CalculateTilemapOriginLeft();

		// Create base on the right side
		CreateBaseTiles(tilemapOriginRight, rightGO);

		// Create base on the left side
		CreateBaseTiles(tilemapOriginLeft, leftGO);
	}

	private void CreateBaseTiles(Vector3 tilemapOrigin, GameObject parent)
	{
		for (int z = 0; z < tilesNumberZ; z++)
		{
			for (int x = 0; x < tilesNumberX; x++)
			{
				Vector3 worldPos = tilemapOrigin + new Vector3(x * tileSize, 0, z * tileSize);
				GameObject go = Instantiate(grassTile, worldPos, Quaternion.identity);
				go.transform.SetParent(parent.transform, false);
			}
		}
	}

	/// <summary>
	/// Spawns obstacles in each tile.
	/// </summary>
	public void SpawnObstacles()
	{
		Vector3 tilemapOriginRight = CalculateTilemapOriginRight();
		Vector3 tilemapOriginLeft = CalculateTilemapOriginLeft();

		// Spawn obstacles on the right side
		SpawnObstacleTiles(tilemapOriginRight, rightGO);

		// Spawn obstacles on the left side
		SpawnObstacleTiles(tilemapOriginLeft, leftGO);
	}

	private void SpawnObstacleTiles(Vector3 tilemapOrigin, GameObject parent)
	{
		for (int z = 0; z < tilesNumberZ; z++)
		{
			for (int x = 0; x < tilesNumberX; x++)
			{
				Vector3 worldPos = tilemapOrigin + new Vector3(x * tileSize, 0, z * tileSize);
				// Pick a random obstacle prefab
				GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];

				// Instantiate the obstacle
				GameObject obstacle = Instantiate(obstaclePrefab, worldPos, Quaternion.identity);

				// Apply random rotation
				float randomRotationY = Random.Range(0f, 360f);
				obstacle.transform.rotation = Quaternion.Euler(0, randomRotationY, 0);

				// Apply random scale variation
				float randomScale = Random.Range(0.9f, 1.1f); // Example scale variation between 90% and 110%
				obstacle.transform.localScale *= randomScale;

				// Set the parent
				obstacle.transform.SetParent(parent.transform, false);
			}
		}
	}
}
