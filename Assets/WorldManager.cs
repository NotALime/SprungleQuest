using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public int mobCap = 200;

    public float spawnRadius = 300;

    public MobSpawn[] spawning;

    public LayerMask groundLayer;

    [Header("Time And Weather")]
    public float time;
    [Tooltip("In minutes")]
    public float dayTime = 12;
    public int days;


    public Material skyMaterial;

    public Light skyLight;
    public Gradient SkyColor;
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
                    spawnPos = hit.point + Vector3.up * 5 + Random.insideUnitSphere * 3;
                    Entity leader = Instantiate(spawn.mob, spawnPos, Quaternion.identity);
                    leader.mob.stats.level += (Random.Range(2, 4));

                    if (spawn.autoTarget)
                    {
                        leader.mob.target = GameSettings.player;
                    }
                    for (int i = 0; i < Random.Range(1, spawn.maxGroupSize); i++)
                    {
                        spawnPos = hit.point + Vector3.up * 5 + Random.insideUnitSphere * 3;
                        Entity mob = Instantiate(spawn.mob, spawnPos, Quaternion.identity);
                        mob.mob.leader = leader;

                        if (spawn.autoTarget)
                        {
                            mob.mob.target = GameSettings.player;
                        }
                    }
                }
            }
        }

        time += Time.deltaTime;
        skyMaterial.SetFloat("_DayTime", time / (dayTime * 60));

        skyLight.color = SkyColor.Evaluate(time / (dayTime * 60));
        skyLight.transform.rotation = Quaternion.Euler(new Vector3(((time / (dayTime * 60)) * 360) - 90, 50, 0));
        if (time >= dayTime * 60)
        {
            time = 0;
            days++;
        }
    }
}

[System.Serializable]
public class MobSpawn
{
    public Entity mob;
    public int maxGroupSize;
    public bool leader;
    public bool autoTarget;
    public float chanceToSpawn;
}
