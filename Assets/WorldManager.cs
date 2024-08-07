using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
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

    [Header("Peddled")]
    public List<MobSpawn> peddledSpawns = new List<MobSpawn>();
    public AudioClip peddleAmbience;
    [Header("Time And Weather")]
    public static float time;
    [Tooltip("In minutes")]
    public float worldTime = 12;
    public int days;
    public float timeScale = 1;

    public Material skyMaterial;
    public static float wetness;

    public GlobalKeyword wetKey;

    public Transform celestialRot;
    public Light sunLight;
    float initialSunIntensity;
    public Light moonLight;
    float initialMoonIntensity;

    public Volume flashbang;

    public RandomEncounter[] encounters;

    public float sunset = 10;
    public float sunrise = 2;

    public Material window;

    float weatherCooldown;

    [Header("Questing")]
    public static List<Quest> activeQuests;

    [HideInInspector]
    public static WorldManager instance;
    public Item currency;
    public static Item money;

    public float startTime = 6;
    private void Start()
    {
        instance = this;
        time = startTime * 60;

        initialMoonIntensity = moonLight.intensity;
        initialSunIntensity = sunLight.intensity;
        wetKey = Shader.globalKeywords[0];

        money = currency;

        SetWeather(defaultWeather);

        window.color = currentWeather.lightColor.Evaluate(time / (worldTime * 60));
        skyMaterial.SetFloat("_DayTime", time / (worldTime * 60));
        skyMaterial.SetColor("_HorizonColor", currentWeather.horizonColor.Evaluate(time / (worldTime * 60)));
        skyMaterial.SetColor("_ZenithColor", currentWeather.zenithColor.Evaluate(time / (worldTime * 60)));

        skyMaterial.SetFloat("_StarOpacity", currentWeather.starStrength);
     //   wetness = currentWeather.wetness;
     //   Shader.SetGlobalFloat("_Wetness", wetness);
        RenderSettings.ambientLight =currentWeather.lightColor.Evaluate(time / (worldTime * 60));
         sunLight.transform.rotation = Quaternion.Euler(new Vector3(((time / (worldTime * 60)) * 360) - 90, 50, 0));
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

        flashbang.weight = Mathf.Lerp(flashbang.weight, 0, 0.5f * Time.deltaTime);
    }


    public TextMeshProUGUI titleText;
    public void SendTitleMessage(string text)
    {
        titleText.text = text;
        Invoke("ResetTitle", 10);
    }
    public void FlashBang()
    {
        flashbang.weight = 1;
    }
    public void ResetTitle()
    {
        titleText.text = "";

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
                                DistanceEnabler.NewDistanceEnabler(mob.transform);
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
        if(EntityEffect.peddling)
        {
            foreach (MobSpawn spawn in peddledSpawns)
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
                                DistanceEnabler.NewDistanceEnabler(mob.transform);
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

        foreach (RandomEncounter e in encounters)
        {
            if (e.repeats || e.happened == false)
            {
                if (EvoUtils.PercentChance(e.chance, true))
                {
                    Instantiate(e.encounter, GameSettings.player.transform.position + Random.insideUnitSphere.normalized * spawnRadius + Vector3.up * 100, Quaternion.identity);
                    MusicManager.instance.StartSong(e.startSound);
                    WorldManager.instance.SendTitleMessage(e.startText);
                    e.happened = true;
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

        skyMaterial.SetFloat("_StarOpacity", Mathf.LerpUnclamped(skyMaterial.GetFloat("_StarOpacity"), currentWeather.starStrength, Time.deltaTime * timeScale));
        wetness = Mathf.LerpUnclamped(wetness, currentWeather.wetness, 20 * Time.deltaTime * timeScale);

        Shader.SetKeyword(wetKey, wetness > 0);


        if (fadingAudio)
        {
            ambientSound.volume = Mathf.Lerp(ambientSound.volume, -0.1f, Time.deltaTime * timeScale * 0.1f);

            if(ambientSound.volume <= 0)
            {
                fadingAudio = false;
            }
        }
        else
        {
            ambientSound.volume = Mathf.Lerp(ambientSound.volume, 0.3f, Time.deltaTime * timeScale * 0.1f);
        }

        if (!InteriorManager.playerOccupied)
        {
            RenderSettings.ambientLight = Color.LerpUnclamped(RenderSettings.ambientLight, currentWeather.lightColor.Evaluate(time / (worldTime * 60)), Time.deltaTime * timeScale);
            celestialRot.rotation = Quaternion.Euler(new Vector3(((time / (worldTime * 60)) * 360) - 90, 200, 0));

            sunLight.color = RenderSettings.ambientLight;
        }
        ambientSound.enabled = !InteriorManager.playerOccupied;

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
        ambientSound.clip = weather.ambience;
        ambientSound.Play();
        fadingAudio = true;
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

    bool fadingAudio;
    public AudioSource ambientSound;
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
public class RandomEncounter
{
    public GameObject encounter;
    public float chance;
    public bool repeats = true;
    public bool happened;
    public AudioClip startSound;
    public string startText;

    public MobSpawnConditions conditions;
}

[System.Serializable]
public class MobSpawnConditions
{
    public float minTime = 0;
    public float maxTime = 12;
}