using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageGenerator : MonoBehaviour
{
    public Building[] townHalls;
    public Building[] buildings;

    GameObject townHall;

    public float size;
    public float spread;

    public int maxBuildingAmount = 15;

    public float budget;

    List<GameObject> placedBuildings;
    private void Start()
    {
        PlaceBuilding(townHalls[Random.Range(0, townHalls.Length)], transform.position);

        List<Vector2> spots = new List<Vector2>();
        spots.AddRange(EvoUtils.GenerateVoronoi(maxBuildingAmount, spread, spread));
        while (budget > 0 || placedBuildings.Count >= maxBuildingAmount)
        {
            Building b = buildings[Random.Range(0, buildings.Length)];
            if (b.price >= budget)
            {
                int spot = Random.Range(0, spots.Count);
                PlaceBuilding(b, new Vector3(spots[spot].x, 1000, spots[spot].y));
                spots.Remove(spots[spot]);
            }
        }
    }

    public void PlaceBuilding(Building b, Vector3 pos)
    {
            GameObject built = Instantiate(b.building, pos, Quaternion.identity);
            budget -= b.price;
            placedBuildings.Add(built);
    }
}

[System.Serializable]
public class Building
{
    public GameObject building;
    public float price;
    [Tooltip("If perVillage == 0, no limit")]
    public int perVillage;
    int currentCount;
}
