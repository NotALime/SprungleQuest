using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldManager : MonoBehaviour
{
    public int mobCap = 200;

    public float spawnRadius = 300;

    public MobSpawn[] spawnPool;
    List<MobSpawn> spawns = new List<MobSpawn>();
    public WeatherEvent defaultWeather;
    WeatherEvent currentWeather;
    public WeatherEvent[] weather;

    public LayerMask groundLayer;

    [Header("Time And Weather")]
    public static float time;
    [Tooltip("In minutes")]
    public float worldTime = 12;
    public int days;
    public float timeScale = 1;

    public Material skyMaterial;
    public static float wetness;

    public Shader[] wetShader;

    public Light skyLight;

    public Material window;

    float weatherCooldown;

    private void Start()
    {
        SetWeather(defaultWeather);

        window.color = currentWeather.lightColor.Evaluate(time / (worldTime * 60));
        skyMaterial.SetFloat("_DayTime", time / (worldTime * 60));
        skyMaterial.SetColor("_HorizonColor", currentWeather.horizonColor.Evaluate(time / (worldTime * 60)));
        skyMaterial.SetColor("_ZenithColor", currentWeather.zenithColor.Evaluate(time / (worldTime * 60)));

        skyMaterial.SetFloat("StarOpacity", System.Convert.ToInt32(currentWeather.starsEnabled));
        wetness = currentWeather.wetness;
        Shader.SetGlobalFloat("_Wetness", wetness);
        RenderSettings.ambientLight =currentWeather.lightColor.Evaluate(time / (worldTime * 60));
        // skyLight.transform.rotation = Quaternion.Euler(new Vector3(((time / (worldTime * 60)) * 360) - 90, 50, 0));
        // RenderSettings.ambientLight = Color.LerpUnclamped(skyMaterial.GetColor("_ZenithColor"), currentWeather.zenithColor.Evaluate(time / (worldTime * 60)), Time.deltaTime);

        RenderSettings.fogDensity = currentWeather.fogValue;
        RenderSettings.fogColor = currentWeather.horizonColor.Evaluate(time / (worldTime * 60));
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Spawning();
        Lighting();
        ManageTime();
        CycleWeather();
    }
    public void Spawning()
    {
        foreach (MobSpawn spawn in spawns)
        {
            if (time > spawn.conditions.minTime * 60 || time < spawn.conditions.maxTime * 60)
            {
                if (EvoUtils.PercentChance(spawn.chanceToSpawn, true))
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
       }
    }

    public void ManageTime()
    {
        time += Time.deltaTime * timeScale;
        if (time >= worldTime * 60)
        {
            time = 0;
            days++;
        }
    }
    public void Lighting()
    {
        window.color = Color.LerpUnclamped(skyMaterial.GetColor("_ZenithColor"), currentWeather.lightColor.Evaluate(time / (worldTime * 60)), Time.deltaTime * timeScale);
        skyMaterial.SetFloat("_DayTime", time / (worldTime * 60));
        skyMaterial.SetColor("_HorizonColor", Color.LerpUnclamped(skyMaterial.GetColor("_HorizonColor"), currentWeather.horizonColor.Evaluate(time / (worldTime * 60)), Time.deltaTime * timeScale));
        skyMaterial.SetColor("_ZenithColor", Color.LerpUnclamped(skyMaterial.GetColor("_ZenithColor"), currentWeather.zenithColor.Evaluate(time / (worldTime * 60)), Time.deltaTime * timeScale));

        skyMaterial.SetFloat("StarOpacity", Mathf.LerpUnclamped(skyMaterial.GetFloat("_StarOpacity"), System.Convert.ToInt32(currentWeather.starsEnabled), Time.deltaTime * timeScale));
        wetness = Mathf.LerpUnclamped(wetness, currentWeather.wetness, Time.deltaTime * timeScale);
        Shader.SetGlobalFloat("_Wetness", wetness);
        RenderSettings.ambientLight = Color.LerpUnclamped(RenderSettings.ambientLight, currentWeather.lightColor.Evaluate(time / (worldTime * 60)), Time.deltaTime * timeScale);
       // skyLight.transform.rotation = Quaternion.Euler(new Vector3(((time / (worldTime * 60)) * 360) - 90, 50, 0));
       // RenderSettings.ambientLight = Color.LerpUnclamped(skyMaterial.GetColor("_ZenithColor"), currentWeather.zenithColor.Evaluate(time / (worldTime * 60)), Time.deltaTime);

        RenderSettings.fogDensity = Mathf.LerpUnclamped(RenderSettings.fogDensity, currentWeather.fogValue,  Time.deltaTime);
        RenderSettings.fogColor = Color.LerpUnclamped(RenderSettings.fogColor, currentWeather.horizonColor.Evaluate(time / (worldTime * 60)), Time.deltaTime);
    }

    public void CycleWeather()
    {
        if (weatherCooldown <= 0)
        {
            currentWeather = null;
            foreach (WeatherEvent w in weather)
            {
                if (EvoUtils.PercentChance(w.chanceToHappen))
                {
                    SetWeather(w);
                }
            }
            if (currentWeather == null)
            {
                SetWeather(defaultWeather);
            }
        }
        weatherCooldown -= Time.deltaTime * timeScale;
    }

    public void SetWeather(WeatherEvent weather)
    {
        currentWeather = weather;
        weatherCooldown = Random.Range(weather.minTime, weather.maxTime) * 60;
        spawns.Clear();
        foreach (MobSpawn spawn in spawnPool)
        {
            spawns.Add(spawn);
        }
        foreach (MobSpawn spawn in weather.spawnPool)
        {
            spawns.Add(spawn);
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

    public MobSpawnConditions conditions;
}

[System.Serializable]
public class MobSpawnConditions
{
    public float minTime = 0;
    public float maxTime = 12;
}