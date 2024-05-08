using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFlyer : MonoBehaviour
{
    public Item item;

    public float speed;

    private void Start()
    {
        item = GetComponent<Item>();
    }
    public void Fly(Inventory inv)
    {
        if (inv.owner.entity.mob.input.z > 0)
        {
            inv.owner.entity.mob.rb.AddForce(inv.owner.entity.mob.orientation.forward * speed);
        }
        inv.owner.entity.mob.rb.useGravity = inv.owner.entity.mob.input.z > 0;
        inv.owner.movementEnabled = inv.owner.entity.mob.input.z > 0;
    }

    public void FlightAI(Inventory inv)
    {
        if (inv.owner.entity.mob.target != null)
        {
            if (Vector2.Distance(inv.owner.entity.mob.target.transform.position, inv.transform.position) < inv.owner.entity.mob.stats.visionRange && inv.transform.position.y < inv.owner.entity.mob.target.transform.position.y)
            {
                inv.owner.entity.mob.input.y = 1;
            }
            else 
            {
                inv.owner.entity.mob.input.z = 1;
            }
        }
    }
}
