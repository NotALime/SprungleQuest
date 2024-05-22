using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Item item;

    public int slotIndex;

    public InventoryUI inv;

    public Button button;

    public bool holdingItem = true;
    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void SelectItem()
    {
        if (inv.itemHeld != null)
        {
            Item invItem = inv.itemHeld;

            inv.itemHeld = item;
            item = invItem;

            if(slotIndex <= inv.inv.items.Length)
            holdingItem = true;
        }
        else if (item != null)
        {
            inv.itemHeld = item;
            inv.SetItemToInventory(item);
            item = null;
        }
    }

    private void Update()
    {
        if (item != null && holdingItem)
        {
            item.transform.position = transform.position;
            item.transform.rotation = transform.rotation * Quaternion.Euler(item.rotationOffset);
        }
    }
}
