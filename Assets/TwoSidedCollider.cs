using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoSidedCollider : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        Mesh originalMesh = meshFilter.mesh;

        // Create and assign the original collider
        meshCollider.sharedMesh = originalMesh;

        // Create and assign the inverted collider
        GameObject innerColliderObject = new GameObject("InnerCollider");
        innerColliderObject.layer = 3;
        innerColliderObject.transform.SetParent(transform, false);
        MeshCollider innerCollider = innerColliderObject.AddComponent<MeshCollider>();
        innerCollider.sharedMesh = InvertMesh(originalMesh);
    }

    private Mesh InvertMesh(Mesh mesh)
    {
        Mesh invertedMesh = new Mesh();
        invertedMesh.vertices = mesh.vertices;
        invertedMesh.normals = mesh.normals;
        invertedMesh.uv = mesh.uv;

        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i];
            triangles[i] = triangles[i + 1];
            triangles[i + 1] = temp;
        }

        invertedMesh.triangles = triangles;
        return invertedMesh;
    }
}