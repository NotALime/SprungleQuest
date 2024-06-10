using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/Weather Event")]
public class WeatherEvent : ScriptableObject
{
    public Gradient horizonColor;
    public Gradient zenithColor;
    public Gradient lightColor;
    public float minTime = 6;
    public float maxTime = 24;
    public float fogValue = 0;
    public float starStrength;
    public float wetness;
    public float chanceToHappen = 0.01f;
    public MobSpawn[] spawnPool;

    public AudioClip ambience;
}
