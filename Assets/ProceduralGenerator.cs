using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProceduralGenerator : MonoBehaviour
{

	public const float maxViewDst = 1000;
	public Transform viewer;
	public GameObject chunk;

	public static Vector2 viewerPosition;
	int chunkSize;
	int chunksVisibleInViewDst;

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

	void Start()
	{
		chunkSize = 64;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
	}

	void Update()
	{
		viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
		UpdateVisibleChunks();
	}

	void UpdateVisibleChunks()
	{

		for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
		{
			terrainChunksVisibleLastUpdate[i].SetVisible(false);
		}
		terrainChunksVisibleLastUpdate.Clear();

		int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
		{
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
			{
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
				{
					terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
					if (terrainChunkDictionary[viewedChunkCoord].IsVisible())
					{
						terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
					}
				}
				else
				{
					terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, chunk));
				}

			}
		}
	}

	public class TerrainChunk
	{

		GameObject meshObject;
		Vector2 position;
		Bounds bounds;


		public TerrainChunk(Vector2 coord, int size, Transform parent, GameObject chunk)
		{
			position = coord * size;
			bounds = new Bounds(position, Vector2.one * size);
			Vector3 positionV3 = new Vector3(position.x, 0, position.y);

			meshObject = Instantiate(chunk, positionV3, Quaternion.identity);
			//meshObject.transform.position = positionV3;
			meshObject.transform.localScale = new Vector3(1,1,1);
			//meshObject.transform.parent = parent;
			SetVisible(false);
		}

		public void UpdateTerrainChunk()
		{
			float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
			bool visible = viewerDstFromNearestEdge <= maxViewDst;
			SetVisible(visible);
		}

		public void SetVisible(bool visible)
		{
			meshObject.SetActive(visible);
		}

		public bool IsVisible()
		{
			return meshObject.activeSelf;
		}

	}
}