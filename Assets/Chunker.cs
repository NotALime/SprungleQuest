using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunker : MonoBehaviour
{
    public const int renderDistance =  2000;
    public Transform player;
    public static Vector2 viewPos;
    public static float chunkSize = 50;
    int chunksVisibleInViewDistance;

    public GameObject playerStart;

    public WorldGen chunk;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    public List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    public static TerrainChunk currentTerrainChunk;

    public static int seed;

    void Start()
    {
      //  seed = Random.Range(-99999, 99999);
        WorldGen.chunk = chunk;
        chunkSize = chunk.gridWidth * chunk.tileSize;
        chunksVisibleInViewDistance = Mathf.RoundToInt(renderDistance / chunkSize);
     //   Invoke("SpawnHouse", 1f);
    }

    void SpawnHouse()
    {
        playerStart = Instantiate(playerStart, new Vector3(0, 5000, 0), Quaternion.identity);
    }

    private void Update()
    {
        viewPos = new Vector2(player.position.x, player.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewPos.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewPos.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if (terrainChunkDictionary[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }

                    if (new Vector2(currentChunkCoordX, currentChunkCoordY) * chunkSize == viewPos)
                    {
                        currentTerrainChunk = terrainChunkDictionary[viewedChunkCoord];
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, 5));
                }
            }
        }
    }

    public class TerrainChunk
    {
        Vector2 pos;
        public WorldGen meshObj;
        Bounds bounds;
        public TerrainChunk(Vector2 coord, float size, Transform parent, int LOD)
        {
            pos = coord * size - coord;
            bounds = new Bounds(pos, Vector2.one * size);

            Vector3 posV3 = new Vector3(pos.x, 0, pos.y);
            meshObj = Instantiate(WorldGen.chunk, posV3, Quaternion.identity);
            meshObj.generationOffset = pos;
            meshObj.LOD = 1;
          //  Debug.Log(pos.ToString());
            meshObj.transform.position = posV3;
            //  meshObj.transform.localScale = Vector3.one * size / 10f;
            meshObj.transform.parent = parent;
            SetVisible(false);
        }

        public void UpdateTerrainChunk()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewPos));
            bool visible = viewerDstFromNearestEdge <= renderDistance;
            SetVisible(visible);
        }

        public void SetVisible(bool vis)
        {
            meshObj.gameObject.SetActive(vis);
        }

        public bool IsVisible()
        {
            return meshObj.gameObject.activeSelf;
        }
    }

}
