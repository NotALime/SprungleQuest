using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTake : MonoBehaviour
{
    public Item itemTake;
    public int amountTake = 1;
    public void TakeItem(Inventory inv)
    {
        if (inv.GetItem(itemTake, amountTake))
        { 
            inv.TakeItem(itemTake, amountTake);
        }
    }
}
