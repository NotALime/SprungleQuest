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
        item.itemDescription += "\n" + healPerSecond.ToString() + " HP/SEC regeneration ";
        if (timeAfterDamage > 0)
        {
            item.itemDescription += timeAfterDamage.ToString() + "seconds outside of danger";
        }
    }
    public void Heal(Inventory e)
    {
        if (e.owner.entity.currentIframe <= -timeAfterDamage)
        {
            e.owner.entity.baseEntity.health += healPerSecond * Time.deltaTime;
        }
        healParticle.transform.position = Vector3.LerpUnclamped(healParticle.transform.position, e.owner.entity.mob.orientation.position, 10 * Time.deltaTime);
        healParticle.enableEmission = (e.owner.entity.currentIframe <= -timeAfterDamage) && e.owner.entity.baseEntity.health < e.owner.entity.baseEntity.maxHealth;
    }
}