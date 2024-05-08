using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AccessorySlot : ItemSlot
{
    Item previousItem;
    public void ApplyVisual()
    {
        if (item != null)
        {
            if (previousItem != null)
            {
                inv.inv.VisualUnequip(previousItem);
            }
            previousItem = item;
            inv.inv.VisualEquip(item);
        }
        else if (previousItem != null)
        {
            inv.inv.VisualUnequip(previousItem);
            previousItem = null;
        }
    }

    private void FixedUpdate()
    {
        if (item != null)
        {
            item.onIdle.Invoke(inv.inv);

            if (item.cooldown < 0)
            {
                if (inv.inv.owner.entity.mob.primaryInput)
                {
                    item.onUsePrimary.Invoke(inv.inv);
                }
            }
            if (inv.inv.owner.entity.mob.secondaryInput )
            {
                item.onUseSecondary.Invoke(inv.inv);
            }
        }
    }
}
