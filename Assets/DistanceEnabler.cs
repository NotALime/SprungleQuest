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
}
