using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteriorManager : MonoBehaviour
{
    public GameObject interior;

    public InteriorDoor entranceOutside;
    public InteriorDoor entranceInside;

    public UnityEvent eventOnExit;
    private void Awake()
    {
        interior.transform.position = new Vector3(transform.position.x, -2000, transform.position.z);
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
            RenderSettings.ambientLight = Color.black;
            WorldManager.instance.celestialRot.gameObject.SetActive(false);
            interior.SetActive(true);
            playerOccupied = true;
        }
        entity.interior = true;
        entity.transform.position = entranceInside.transform.position;
    }

    public static bool playerOccupied;
    public void ExitInterior(Entity entity)
    {
        if (entity.player)
        {
            playerOccupied = false;
            Invoke("InteriorDisableDelay", 20);
            WorldManager.instance.celestialRot.gameObject.SetActive(true);
            if (eventOnExit != null)
            {
                eventOnExit.Invoke();
            }
        }
        entity.interior = false;
        entity.transform.position = entranceOutside.transform.position;
        entity.transform.parent = null;
    }

    private void InteriorDisableDelay()
    {
        if(!playerOccupied)
        interior.SetActive(false);
    }
}
