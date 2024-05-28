using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;


public class VillageGenerator : MonoBehaviour
{
    public Building[] townHalls;
    public Building[] buildings;

    Spline villagePath = new Spline();
    public SplineInstantiate villagePathRender;
    GameObject townHall;

    public float size;
    public float spread;

    public int maxBuildingAmount = 15;

    public float budget;

    List<GameObject> placedBuildings = new List<GameObject>();
    private void Start()
    {
        PlaceBuilding(townHalls[Random.Range(0, townHalls.Length)], transform.position);

        List<Vector2> spots = new List<Vector2>();
        spots.AddRange(EvoUtils.GenerateVoronoi(maxBuildingAmount, spread, spread));

        villagePath.Clear();
        for (int i = 0; i < maxBuildingAmount; i++)
        {
            if (budget > 0)
            {
                Building b = buildings[Random.Range(0, buildings.Length)];
                int spot = Random.Range(0, spots.Count);
                PlaceBuilding(b, transform.position + new Vector3(spots[spot].x * size, 1000, spots[spot].y * size));
                BezierKnot knot = new BezierKnot(new Unity.Mathematics.float3(spots[spot].x, 1000, spots[spot].y));
                villagePath.Add(knot);
                spots.Remove(spots[spot]);
            }
            else
            {
                break;
            }
        }

        villagePathRender.Container.Spline = villagePath;
        villagePathRender.transform.localScale = Vector3.one * size; 
        villagePathRender.Randomize();
    }

    public void PlaceBuilding(Building b, Vector3 pos)
    {
            GameObject built = Instantiate(b.building, pos, Quaternion.identity);
        built.transform.parent = transform;
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
