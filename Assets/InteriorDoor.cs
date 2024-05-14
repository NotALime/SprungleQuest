using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorDoor : MonoBehaviour
{
    public InteriorDoor exit;
    public InteriorManager interior;

    public bool insideDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Entity>())
        {
            if (insideDoor)
            {
                interior.ExitInterior(other.GetComponent<Entity>());
            }
            else
            {
                interior.EnterInterior(other.GetComponent<Entity>());
            }
        }
    }
}
