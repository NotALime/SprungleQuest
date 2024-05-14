using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public int mobCap = 200;

    public float spawnRadius = 300;

    public MobSpawn[] spawning;

    public LayerMask groundLayer;

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (MobSpawn spawn in spawning)
        { 
            if(EvoUtils.PercentChance(spawn.chanceToSpawn, true))
            {
                Vector3 spawnPos = GameSettings.player.transform.position + Random.insideUnitSphere.normalized * spawnRadius;
                spawnPos.y = 50000;
                RaycastHit hit;
                if (Physics.Raycast(spawnPos, Vector3.down, out hit, Mathf.Infinity, groundLayer))
                {
                    for (int i = 0; i < Random.Range(1, spawn.maxGroupSize); i++)
                    {
                        spawnPos = hit.point + Vector3.up * 5 + Random.insideUnitSphere * 3;
                        Entity mob = Instantiate(spawn.mob, spawnPos, Quaternion.identity);
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class MobSpawn
{
    public Entity mob;
    public int maxGroupSize;
    public float chanceToSpawn;
}
