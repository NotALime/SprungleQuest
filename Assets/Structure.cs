using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public StructureObject[] structurePieces;
    public Entity[] structureMobs;

    public float level = 1;

    public float chanceToSpawnMob = 0.1f;
    public void Start()
    {
        Generate();
    }

    public void Generate()
    {
        foreach (StructureObject s in structurePieces)
        {
            foreach (GameObject obj in s.structure)
            {
                obj.SetActive(false);
                if (Random.Range(0f, 1f) <= s.chanceToSpawn)
                {
                    obj.SetActive(true);
                }
            }
        }
        foreach (Entity e in structureMobs)
        {
            e.gameObject.SetActive(true);
            DistanceEnabler.NewDistanceEnabler(e.transform);
        }
    }
}
[System.Serializable]
public class StructureObject
{
    public GameObject[] structure;

    public float chanceToSpawn = 1;
}

[System.Serializable]
public class StructureMob
{
    public Entity[] locations;

    public float chanceToSpawn = 1;
}
