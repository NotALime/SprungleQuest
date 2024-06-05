using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonRoom[] rooms;
    public int size;
    public float roomSize;

    GameObject[,] grid;
    private void Start()
    {
        grid = new GameObject[size, size];
        Vector3 offset = transform.position;
        Vector3 previousOffset = Vector3.zero;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                GameObject room = Instantiate(rooms[Random.Range(0, rooms.Length)].obj,transform.position + new Vector3(x,0,y)*roomSize, Quaternion.identity);
                room.transform.parent = transform;
            }
        }
    }
}

[System.Serializable]
public class DungeonRoom
{
    public Vector3 size;
    public GameObject obj;
}