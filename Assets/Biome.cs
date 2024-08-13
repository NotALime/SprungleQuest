using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Terra.Terrain;
[CreateAssetMenu(fileName = "New Biome", menuName = "World/Biome")]
public class Biome : ScriptableObject
{
    public Texture2D floorMaterial;
    public float minTemp;
    public float maxTemp;
    public float minHumid;
    public float maxHumid;
    public float minHeight = -10000;
    public float maxHeight = 10000;

    public Feature[] features;

    public int index;
}

[System.Serializable]
public class Feature
{
    public GameObject[] varients;
    public float chance = 0.1f;
    public Vector3 rotateOffset;
    public Vector3 rotateRange;
}
