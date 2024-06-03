using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceEnabler : MonoBehaviour
{
    public Transform follow;
    public void Update()
    {
        transform.position = follow.position;
    }
    public void Activate()
    {
        follow.gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        follow.gameObject.SetActive(false);
    }

    public static DistanceEnabler NewDistanceEnabler(Transform follow)
    {
        GameObject obj = new GameObject();
        obj.name = follow.gameObject.name + " enabler";
        DistanceEnabler enabler = obj.AddComponent<DistanceEnabler>();
        enabler.follow = follow;
        BoxCollider collider = obj.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        obj.layer = 10;
        return enabler;
    }
}
