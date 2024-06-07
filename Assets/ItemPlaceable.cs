using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlaceable : MonoBehaviour
{
    public Item item;
    public GameObject placeable;

    public LayerMask groundLayer;

    private void Start()
    {
        item = GetComponent<Item>();
    }
    public void Place(Inventory inv)
    {
            Ray ray = new Ray(inv.owner.entity.mob.orientation.position, inv.owner.entity.mob.orientation.transform.forward);
            RaycastHit hit;

            Debug.Log("Trying to get item looked at");

            if (Physics.Raycast(ray, out hit, inv.owner.playerHeight * 2, groundLayer))
            {
                placed = Instantiate(placeable, hit.point, Quaternion.LookRotation(hit.normal, placeable.transform.forward));
                placed.transform.localScale = Vector3.one;
                inv.owner.entity.mob.primaryInput = false;
            }
    }

    GameObject placed;
    public void SetupAtNight(Inventory ai)
    {
        if (WorldManager.time > 9 && placed == null)
        {
            if (ai.owner.entity.mob.target == null)
            {
                ai.owner.entity.mob.orientation.Rotate(-40, 0, 0);
                Place(ai);
            }
        }
    }

    public void CampIdle(Entity ai)
    {

    }
}
