using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHeal : MonoBehaviour
{
    Item item;
    public float healPerSecond;
    public float timeAfterDamage;

    public ParticleSystem healParticle;

    private void Awake()
    {
        item = GetComponent<Item>();
        item.itemDescription += "\n" + healPerSecond.ToString() + " HP/S regeneration ";
        if (timeAfterDamage > 0)
        {
            item.itemDescription += timeAfterDamage.ToString() + "seconds after being damaged";
        }
    }
    public void Heal(Entity e)
    {
        if (e.currentIframe <= -timeAfterDamage)
        {
            e.baseEntity.health += healPerSecond * Time.deltaTime;
        }
        healParticle.transform.position = Vector3.LerpUnclamped(healParticle.transform.position, e.mob.orientation.position, 10 * Time.deltaTime);
        healParticle.enableEmission = (e.currentIframe <= -timeAfterDamage) && e.baseEntity.health < e.baseEntity.maxHealth;
    }
}