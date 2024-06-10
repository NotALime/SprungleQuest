using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFlight : MonoBehaviour
{
    public Item item;
    public float flightTime;
    public float currentFlightTime;
    public float flightForce;
    public float cooldownTime;
    float cooldown;
    public ParticleSystem floatParticles;
    void Start()
    {
        item = GetComponent<Item>();
    }

    public void Hover(Entity e)
    {
        floatParticles.transform.position = e.mob.orientation.position += Vector3.up * -2f;
        if (cooldown <= 0 && currentFlightTime <= flightTime)
        {
            if (e.mob.input.y > 0)
            {
                e.mob.rb.AddForce(Vector3.up * flightForce);
                floatParticles.enableEmission = true;
                currentFlightTime += Time.deltaTime;
            }
            else
            {
                floatParticles.enableEmission = false;
            }
        }
        else if(currentFlightTime > flightTime)
        {
            cooldown = cooldownTime;
            currentFlightTime = 0;
        }
        cooldown -= Time.deltaTime;
    }

    public void HoverAI(Inventory ai)
    {
        if (ai.owner.entity.mob.target != null)
        {
            if (EvoUtils.PercentChance(0.05f * ai.owner.entity.mob.stats.level, true))
            {
                ai.owner.entity.mob.input.y = 1;
            }

            if (ai.owner.entity.mob.input.y > 0 && currentFlightTime > flightTime * 0.2f)
            {
                ai.owner.entity.mob.primaryInput = true;
                ai.owner.entity.mob.input.y = 0;
            }
        }
    }
}  
