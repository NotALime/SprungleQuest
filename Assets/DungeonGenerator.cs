using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public int dungeonWidth = 5;
    public int dungeonLength = 5;
    public float roomWidth = 5f;
    public float roomLength = 5f;
    public float wallHeight = 5f;
    public Material material;

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int z = 0; z < dungeonLength; z++)
            {
                Vector3 roomPosition = new Vector3(x * roomWidth, 0, z * roomLength);
                GenerateRoom(roomPosition);
            }
        }
    }

    void GenerateRoom(Vector3 position)
    {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        meshRenderer.material = material;

        // Vertices
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(-roomWidth / 2, 0, -roomLength / 2) + position); // Bottom Left
        vertices.Add(new Vector3(roomWidth / 2, 0, -roomLength / 2) + position); // Bottom Right
        vertices.Add(new Vector3(roomWidth / 2, 0, roomLength / 2) + position); // Top Right
        vertices.Add(new Vector3(-roomWidth / 2, 0, roomLength / 2) + position); // Top Left

        // Triangles
        List<int> triangles = new List<int>();
        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(1);
        triangles.Add(0);
        triangles.Add(3);
        triangles.Add(2);

        // Normals
        List<Vector3> normals = new List<Vector3>();
        for (int i = 0; i < 4; i++)
        {
            normals.Add(Vector3.up);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
    }
}