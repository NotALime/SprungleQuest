using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUInstancer : MonoBehaviour
{
    public int instances;

    public Mesh mesh;

    public Material[] materials;

    public List<List<Matrix4x4>> batches = new List<List<Matrix4x4>>();

    private void Start()
    {

    }

    public void Update()
    {
        RenderBatches();
    }

    public void PlaceBatches()
    {
        int addedMatricies = 0;

        batches.Add(new List<Matrix4x4>());
        for (int i = 0; i < instances; i++)
        {
            if (addedMatricies < 1000)
            {
                batches[batches.Count - 1].Add(Matrix4x4.TRS(new Vector3(Random.Range(0, 50), Random.Range(0, 50), Random.Range(0, 50)), Random.rotation, Vector3.one));
            }
            else
            {
                batches.Add(new List<Matrix4x4>());
                addedMatricies = 0;
            }
        }
    }

    public void InstanceTerrainFeatures(Vector3[] positions, Vector3 rotation, Vector3 scale, int amount)
    {
        int addedMatricies = 0;

        batches.Add(new List<Matrix4x4>());
        for (int i = 0; i < instances; i++)
        {
            if (addedMatricies < 1000)
            {
                batches[batches.Count - 1].Add(Matrix4x4.TRS(new Vector3(Random.Range(0, 50), Random.Range(0, 50), Random.Range(0, 50)), Random.rotation, Vector3.one));
            }
            else
            {
                batches.Add(new List<Matrix4x4>());
                addedMatricies = 0;
            }
        }
    }
    private void RenderBatches()
    {
        foreach (List<Matrix4x4> batch in batches)
        {
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                Graphics.DrawMeshInstanced(mesh, i, materials[i], batch);
            }
        }
    }
}
