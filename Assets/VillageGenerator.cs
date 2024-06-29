using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;


public class VillageGenerator : MonoBehaviour
{
    public Building[] townHalls;
    public Building[] buildings;

    public VillageProblem[] problems;
    public float problemRange = 1000;

    public List<NPCEmotion> population;

    public SplineInstantiate villagePathRender;
    GameObject townHall;

    public float size;
    public float spread;

    public int maxBuildingAmount = 15;

    public float budget;

    public GameObject path;

    List<GameObject> placedBuildings = new List<GameObject>();

    public float branchChance;

    public int branchLength;
    public int originBranchAmount = 4;

    public NPCEmotion villagerPrefab;
    public HumanoidSpecies[] villagerTypes;

    public VillageGenerator finalVillage;

    public static int villageIndex;

    public Entity horse;
    
    private void Start()
    {
        villageIndex++;
     //   if (villageIndex >= problems.Length)
     //   {
     //       Instantiate(finalVillage, transform.position, Quaternion.identity);
     //       Destroy(this.gameObject);
     //   }
        Vector3 origin = transform.position;
        PlaceBuilding(townHalls[Random.Range(0, townHalls.Length)], origin);

        Vector3 previousDir = Vector3.zero;
        Vector3 dir = Random.insideUnitSphere.normalized;
        dir.y = 0;

        for (int i = 0; i < originBranchAmount; i++)
        {
            GenerateBranch(origin);
        }

        StartCoroutine(GenerateProblem());
    }
    public void GenerateBranch(Vector3 origin)
    {
        Vector3 previousDir = origin;
        Vector3 dir = Random.insideUnitSphere.normalized;
        dir.y = 0;
        Spline villagePath = new Spline();
        SplineInstantiate pathRender = Instantiate(villagePathRender, Vector3.zero, Quaternion.identity);
        pathRender.transform.parent = transform;

        BezierKnot initialKnot = new BezierKnot(new Unity.Mathematics.float3(origin.x, 100, origin.z));
        villagePath.Add(initialKnot);

        bool branched = false;
        for (int i = 0; i < branchLength; i++)
        {
            if (budget > 0)
            {
                Vector3 placePos = previousDir += dir * size + Random.insideUnitSphere * spread;
                BezierKnot knot = new BezierKnot(new Unity.Mathematics.float3(placePos.x, placePos.y + 1000, placePos.z));
                Building b = buildings[Random.Range(0, buildings.Length)];
                Vector3 offset = Random.insideUnitSphere.normalized * 40;
                GameObject placed = PlaceBuilding(b, placePos + offset);
                placed.transform.parent = transform;
                villagePath.Add(knot);
                previousDir = placePos;

               if (EvoUtils.PercentChance(branchChance) && !branched)
               {
                    GenerateBranch(placePos);
                    branched = true;
                    i += (int)(branchLength * 0.3f);
               }
            }
            else
            {
                break;
            }
        }
        pathRender.Container.Spline = villagePath;
        pathRender.Randomize();
    }

    public IEnumerator GenerateProblem()
    {
        chosenProblem = problems[Random.Range(0, problems.Length)];

        chosenProblem.generated = true;
        chosenProblem.problem.transform.position = Random.insideUnitSphere.normalized * problemRange + Vector3.up * 2000;

        foreach (VillageProblem p in problems)
        {
            if (p != chosenProblem)
            {
                Destroy(p.problem);
            }
        }
        foreach (NPCEmotion ai in population)
        {
            ai.traits.Add(chosenProblem.before);
            ai.UpdatePersonality();
        }

        yield return new WaitUntil(() => chosenProblem.thingToDestroy == null);
        foreach (NPCEmotion ai in population)
        {
            ai.traits.Remove(chosenProblem.before);
            ai.traits.Add(chosenProblem.after);
            ai.UpdatePersonality();
        }
    }
    VillageProblem chosenProblem;

    public GameObject PlaceBuilding(Building b, Vector3 pos)
    {
        if (budget > 0)
        {
            GameObject built = Instantiate(b.building, pos + Vector3.up * 10, Quaternion.identity);
            NPCEmotion npc = Instantiate(villagerPrefab, pos + Random.insideUnitSphere.normalized * 10 + Vector3.up * 100, Quaternion.identity);
            DistanceEnabler.NewDistanceEnabler(npc.transform);
            population.Add(npc);
            npc.transform.parent = transform;
            npc.ai.mob.species = villagerTypes[Random.Range(0, villagerTypes.Length)];
            npc.ai.mob.targetPoint = pos;
            built.transform.parent = transform;
            budget -= b.price;
            placedBuildings.Add(built);

            return built;
        }
        return null;
    }
}

[System.Serializable]
public class Building
{
    public GameObject building;
    public float price;
    public int population = 1;
    [Tooltip("If perVillage == 0, no limit")]
    public int perVillage;
    int currentCount;
}
[System.Serializable]
public class VillageProblem
{
    public bool generated = false;
    public GameObject problem;
    public GameObject thingToDestroy;
    public EmotionTrait before;
    public EmotionTrait after;
}

