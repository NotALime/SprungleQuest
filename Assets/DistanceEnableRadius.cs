using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceEnableRadius : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<DistanceEnabler>())
            other.GetComponent<DistanceEnabler>().Activate();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<DistanceEnabler>())
            other.GetComponent<DistanceEnabler>().Deactivate();
    }
}
