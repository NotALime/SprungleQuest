using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public Item item;

    public int slotIndex;

    public InventoryUI inv;

    public Button button;

    public TextMeshProUGUI amountText;

    public bool holdingItem = true;
    private void Start()
    {
        button = GetComponent<Button>();
    }

    public virtual void SelectItem()
    {
        if (inv.itemHeld != null)
        {
            Item invItem = inv.itemHeld;

            inv.itemHeld = item;
            item = invItem;

            if(slotIndex <= inv.inv.items.Length)
            inv.inv.items[slotIndex] = item;
        }
        else if (item != null)
        {
            inv.itemHeld = item;
            inv.inv.items[slotIndex] = null;
            inv.SetItemToInventory(item);
            amountText.text = "";
            item = null;
        }
    }

    private void Update()
    {
        if (item != null && holdingItem)
        {
            item.transform.position = transform.position;
            amountText.text = (item.stack > 1 ? item.stack.ToString() : "");
            item.transform.rotation = transform.rotation * Quaternion.Euler(item.rotationOffset);
        }
    }
}
