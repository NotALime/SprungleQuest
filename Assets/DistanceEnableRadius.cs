using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceEnableRadius : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<DistanceEnabler>().Activate();
    }
    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<DistanceEnabler>().Deactivate();
    }
}
