using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonRoom[] mainRooms;
    public DungeonRoom[] rooms;
    public int size = 20;
    public float roomSize;

    public int mainPathLength = 10;

    GameObject[,] grid;

    public Transform start;

    public int hallLength = 4;

    private void Start()
    {
        Vector3 offset = transform.position;
        Vector3 previousOffset = Vector3.zero;

        Vector3 dir = new Vector3(size * 0.5f, 0, 0);
        grid = new GameObject[size, size];

        for (int x = 0; x < mainPathLength; x++)
        {
            Vector3 dirAdd = (EvoUtils.PercentChance(0.5f)) ? Vector3.forward : Vector3.right * EvoUtils.NormalizeInt(Random.Range(-1f,1f));
            dir += dirAdd;
            GameObject room = Instantiate(mainRooms[Random.Range(0, mainRooms.Length)].obj, transform.position + dir * roomSize, Quaternion.Euler(new Vector3(-90,0,0)));
            if (x == 0)
            {
                start.position = room.transform.position;
            }
            room.transform.parent = transform;
            grid[(int)dir.x, (int)dir.z] = room;
        }



        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (grid[x,y] == null)
                {
                    DungeonRoom dungeonRoom = rooms[Random.Range(0, rooms.Length)];
                    GameObject room = Instantiate(dungeonRoom.obj, transform.position + new Vector3(x, 0, y) * roomSize, Quaternion.identity);
                    grid[x, y] = room;
                    room.transform.parent = transform;
                }
            }
        }
    }
}

[System.Serializable]
public class DungeonRoom
{
    public Vector3 size;
    public bool wall;

    public GameObject obj;
}