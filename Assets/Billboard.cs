using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool yAxisOnly;
    void Update()
    {
        if (yAxisOnly)
        {
            transform.LookAt(Camera.main.transform.position + Vector3.up, Vector3.up);
        }
        else
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
