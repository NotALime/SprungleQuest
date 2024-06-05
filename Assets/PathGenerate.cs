using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Splines;

public class PathGenerate : MonoBehaviour
{
    public int units = 20;
    public float unitSize = 100;
    public float spread = 100;

    public SplineInstantiate pathRender;

    public SplineContainer villagePath;

    public List<GameObject> POI;
    private void Start()
    {
        Generate();
    }
    public void Generate()
    {
        Vector3 dir = Random.insideUnitSphere.normalized;
        Vector3 pos = transform.position;
        for (int i = 0; i < units; i++)
        {
            pos = pos + dir * i * unitSize + Random.insideUnitSphere.normalized * spread;
            BezierKnot knot = new BezierKnot(new Unity.Mathematics.float3(pos.x, 3000, pos.z));
            villagePath.Spline.Add(knot);
        }
        GameObject obj = Instantiate(POI[Random.Range(0, POI.Count)], pos + Vector3.up * 5000, Quaternion.identity);
        obj.transform.parent = transform;
        pathRender.Randomize();
    }
}
