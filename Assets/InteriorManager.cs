using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorManager : MonoBehaviour
{
    public GameObject interior;

    public InteriorDoor entranceOutside;
    public InteriorDoor entranceInside;
    private void Awake()
    {
        interior.transform.position = new Vector3(transform.position.x, -5000, transform.position.z);
        interior.transform.rotation = Quaternion.identity;
        interior.SetActive(false);

        entranceOutside.interior = this;
        entranceOutside.exit = entranceInside;

        entranceInside.interior = this;
        entranceInside.insideDoor = true;
        entranceInside.exit = entranceOutside;

        foreach (Transform t in interior.transform)
        {
            if (t.GetComponent<Entity>())
            {
                t.GetComponent<Entity>().interior = true;
            }
        }
    }

    public void EnterInterior(Entity entity)
    {
        if (entity.player)
        {
            interior.SetActive(true);
            playerOccupied = true;
        }
        entity.interior = true;
        entity.transform.position = entranceInside.transform.position;
    }

    bool playerOccupied;
    public void ExitInterior(Entity entity)
    {
        if (entity.player)
        {
            playerOccupied = false;
            Invoke("InteriorDisableDelay", 20);
        }
        entity.interior = false;
        entity.transform.position = entranceOutside.transform.position;
    }

    private void InteriorDisableDelay()
    {
        if(!playerOccupied)
        interior.SetActive(false);
    }
}
