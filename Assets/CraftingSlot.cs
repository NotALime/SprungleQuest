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
                }
            }
            if (canCraft)
            {
                inv.itemHeld = item;
                inv.SetItemToInventory(item);
                item = null;

                foreach (RecipeEntry r in recipe.recipe)
                {
                    inv.inv.TakeItem(r.item, r.amount);
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
