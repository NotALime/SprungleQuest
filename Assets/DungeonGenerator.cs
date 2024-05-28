using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonRoom[] rooms;
    public int size;
    public float roomAmount;

    GameObject[,,] grid;
    private void Start()
    {
        grid = new GameObject[size, size, size];
        Vector3 offset = transform.position;
        Vector3 previousOffset = Vector3.zero;
        for (int i = 0; i < roomAmount; i++)
        {
            DungeonRoom room = rooms[Random.Range(0, rooms.Length)];

            bool rotated = EvoUtils.PercentChance(0.5f);

            Vector3 roomOffset = new Vector3(room.size.x, 0, 0);
            offset += roomOffset * 0.5f + previousOffset * 0.5f;

            //  Vector3 orientation = new Vector3(0, Random.Range(0, 2) * 90, 0);
            //  Vector3 sizeRotated = EvoUtils.RotateVector(room.size, orientation);
            //  Vector3 rotateOffset = EvoUtils.RotateVector(new Vector3(0, 0, room.size.x), orientation);
            GameObject spawnedRoom = Instantiate(room.obj, offset, Quaternion.identity);
         //  for (int x = 0; x < room.size.x; x++)
         //  {
         //      for (int z = 0; z < sizeRotated.y; z++)
         //      {
         //          for (int y = 0; y < sizeRotated.y; y++)
         //          {
         //              grid[x, y, z] = spawnedRoom;
         //          }
         //      }
         //  }
            previousOffset = roomOffset;
        }
    }
}

[System.Serializable]
public class DungeonRoom
{
    public Vector3 size;
    public GameObject obj;
}