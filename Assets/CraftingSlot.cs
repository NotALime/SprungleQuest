using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSlot : ItemSlot
{
    public Recipe recipe;

    public override void SelectItem()
    {
        if (item != null)
        {
            bool canCraft = true;
            foreach (RecipeEntry r in recipe.recipe)
            {
                if (!inv.inv.GetItem(r.item, r.amount))
                {
                    canCraft = false;
                    Debug.Log("Not enough resources to craft!");
                }
            }
            if (canCraft)
            {
                Debug.Log("Crafted " + item);
                Item givenItem = Instantiate(item);
                inv.itemHeld = givenItem;
                inv.SetItemToInventory(givenItem);

                if (recipe.amount != -1)
                {
                    item.stack--;
                    if (item.stack <= 0)
                    {
                        Destroy(item.gameObject);
                    }
                }

                foreach (RecipeEntry r in recipe.recipe)
                {
                    inv.inv.TakeItem(inv.inv.GetItem(r.item), r.amount);
                }
            }
        }
    }
}

[System.Serializable]
public class Recipe
{
    public RecipeEntry[] recipe;
    public int amount = -1;
    public Item output;
}

[System.Serializable]
public class RecipeEntry
{
    public Item item;
    public int amount;
}
