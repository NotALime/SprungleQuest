using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceEnabler : MonoBehaviour
{
    public Transform follow;
    public void Update()
    {
        if (follow != null)
        {
            transform.position = follow.position;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Activate()
    {
        follow.gameObject.SetActive(true);
    }
    public void Deactivate()
    {
            Destroy(gameObject);
     //       follow.gameObject.SetActive(false);
    }

    private void Start()
    {
        Deactivate();
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
