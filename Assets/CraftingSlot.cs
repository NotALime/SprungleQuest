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
                item = null;

                foreach (RecipeEntry r in recipe.recipe)
                {
                    inv.inv.TakeItem(inv.inv.GetItem(r.item), r.amount);
                }
            }
        }
    }

    public static void GenerateCraftingUI(Recipe[] recipes)
    {
        
    }
}

[System.Serializable]
public class Recipe
{
    public RecipeEntry[] recipe;
    public Item output;
}

[System.Serializable]
public class RecipeEntry
{
    public Item item;
    public int amount;
}
