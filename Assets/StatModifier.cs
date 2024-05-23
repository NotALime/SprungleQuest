using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModifier : MonoBehaviour
{
    public MobStats modifier;
    Item item;

    private void Start()
    {
        item = GetComponent<Item>();
        if (modifier.defense != 0)
        {
            item.itemDescription += "\n" + modifier.defense.ToString() + " defense";
        }
        if (modifier.damage != 0)
        {
            item.itemDescription += "\n" + (modifier.damage * 100).ToString() + "% damage";
        }
        if (modifier.health != 0)
        {
            item.itemDescription += "\n" + (modifier.health * 100).ToString() + "% health";
        }
        if (modifier.moveSpeed != 0)
        {
            item.itemDescription += "\n" + (modifier.moveSpeed * 100).ToString() + "% movement speed";
        }
        if (modifier.attackSpeed != 0)
        {
            item.itemDescription += "\n" + (modifier.attackSpeed * 100).ToString() + "% attack speed";
        }
    }
}

