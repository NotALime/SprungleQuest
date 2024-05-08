using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Humanoid))]
public class Inventory : MonoBehaviour
{
    public Humanoid owner;
    public Item[] items;

    public Item[] accessories;

    public Transform hand;

    public Animator handAnimator;

    float cooldown;

    public LayerMask itemLayer;

    [HideInInspector]
    public int hotbarIndex = -1;

    public float deathDecay;

    public Transform itemHolder;

    public Accessorization accessorization;

    public InventoryUI ui;

    private void Start()
    {
        owner = GetComponent<Humanoid>();

        foreach (Item item in items)
        {
            if (item != null)
            {
                item.transform.parent = itemHolder;
                item.gameObject.SetActive(false);
            }
        }

        foreach (Item item in accessories)
        {
            if (item != null)
            {
                VisualEquip(item);
            }
        }
        hotbarIndex = Random.Range(0, items.Length);
        if (items[hotbarIndex] != null)
        {
            items[hotbarIndex].gameObject.SetActive(false);
            HoldItem(items[hotbarIndex]);
        }

    }

    public void Update()
    {
        if (items[hotbarIndex] != null)
        {
            items[hotbarIndex].transform.position = hand.transform.position;
            items[hotbarIndex].transform.rotation = hand.transform.rotation;
            if (owner.entity.player == false && owner.entity.mob.aiEnabled)
            {
                items[hotbarIndex].ai.Invoke(this);
                foreach (Item item in accessories)
                {
                        item.ai.Invoke(this);
                }

                if (Random.Range(0f, 1f) < 0.01f * owner.entity.mob.stats.level)
                {
                    items[hotbarIndex].gameObject.SetActive(false);
                    hotbarIndex = Random.Range(0, items.Length);
                    HoldItem(items[hotbarIndex]);
                }
            }
            if (owner.entity.mob.aiEnabled)
            {
                if (items[hotbarIndex].cooldown < 0)
                {
                    if (owner.entity.mob.primaryInput)
                    {
                        items[hotbarIndex].onUsePrimary.Invoke(this);
                    }
                }
                if (owner.entity.mob.secondaryInput && items[hotbarIndex])
                {
                    items[hotbarIndex].onUseSecondary.Invoke(this);
                }
            }
        }

        handAnimator.SetBool("Holding", (items[hotbarIndex] != null));
    }

    private void FixedUpdate()
    {
        if (items[hotbarIndex] != null)
        {
            items[hotbarIndex].onIdle.Invoke(this);
        }
    }

    private void OnDestroy()
    {
        List<Item> selectedItems = new List<Item>();

        int numItemsToSelect = Mathf.RoundToInt(items.Length * deathDecay);

        List<int> indices = new List<int>();
        for (int i = 0; i < items.Length; i++)
        {
            indices.Add(i);
        }

        for (int i = 0; i < numItemsToSelect; i++)
        {
            int randomIndex = Random.Range(0, indices.Count);
            selectedItems.Add(items[indices[randomIndex]]);
            indices.RemoveAt(randomIndex);
        }

        // Print selected items for demonstration
        foreach (var item in selectedItems)
        {
            item.transform.parent = null;
            item.gameObject.SetActive(true);
        }
    }

    public Item GetItemLookedAt()
    {
        Ray ray = new Ray(owner.entity.mob.orientation.position, owner.entity.mob.orientation.transform.forward);
        RaycastHit hit;

        Debug.Log("Trying to get item looked at");

        if (Physics.Raycast(ray, out hit, owner.playerHeight, itemLayer))
        {
            if (hit.collider.gameObject.GetComponent<Item>())
            {
                Debug.Log("Obtained item " + hit.collider.gameObject.GetComponent<Item>().itemName);
                return hit.collider.gameObject.GetComponent<Item>();
            }
        }
        return null;
    }

    public void HoldItem(Item i)
    {
        i.gameObject.SetActive(true);
        if (i.rb != null)
        {
            i.rb.constraints = RigidbodyConstraints.FreezeAll;
            i.transform.SetParent(hand.transform, true);
            i.transform.position = hand.transform.position;
            i.transform.rotation = hand.transform.rotation;
        }
      //  if (owner.entity.player)
      //  {
      //      i.gameObject.layer = 8;
      //  }
      //  else
      //  {
            i.gameObject.layer = 6;
     //   }
    }
    public void DropItem(Item i)
    {
        if (i.rb != null)
        {
            i.rb.constraints = RigidbodyConstraints.None;
        }
        i.transform.parent = null;
        i.transform.position = hand.transform.position;
        i.transform.rotation = hand.transform.rotation;
        i.gameObject.layer = 7;
    }


    public void VisualEquip(Item i)
    {
        if (i.GetComponent<Accessory>())
        {
            Accessory a = i.GetComponent<Accessory>();

            if (a.bodyRenderer == null)
            {
                GameObject obj = Object.Instantiate(new GameObject(), accessorization.bodyVisual.transform);
                obj.name = i.itemName + " bodypart";
                SkinnedMeshRenderer renderer = obj.AddComponent<SkinnedMeshRenderer>();
                Debug.Log(renderer.ToString());
                Debug.Log(accessorization.rootBone.ToString());
                renderer.rootBone = accessorization.rootBone;
                renderer.bones = accessorization.bodyVisual.bones;
                renderer.sharedMesh = a.mesh;
                renderer.material = a.material;

                if (owner.entity.player)
                {
                    renderer.gameObject.layer = 8;
                }
                else
                {
                    renderer.gameObject.layer = 6;
                }

                a.bodyRenderer = renderer;
            }
        }
    }

    public void VisualUnequip(Item i)
    {
        if (i.GetComponent<Accessory>())
        {
            Destroy(i.GetComponent<Accessory>().bodyRenderer);
        }
    }
}

[System.Serializable]
public class Accessorization
{
    public Transform rootBone;
    public SkinnedMeshRenderer bodyVisual;
}

