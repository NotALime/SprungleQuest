using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AccessorySlot : ItemSlot
{
    Item previousItem;
    public void ApplyVisual()
    {
        if (inv.itemHeld != null)
        {
            inv.inv.UnequipItem(inv.itemHeld);
            if (item != null)
            {
                inv.inv.EquipItem(item);
            }
        }
        else if (item != null)
        {
            inv.inv.EquipItem(item);
        }
    }
}
