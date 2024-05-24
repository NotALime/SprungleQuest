using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Humanoid))]
public class Inventory : MonoBehaviour
{
    public Humanoid owner;
    public Item[] items;

    public Item[] accessories;
    public Item head;
    public Item body;
    public Item legs;
    public Item feet;

    public Transform hand;

    public Animator handAnimator;

    float cooldown;

    public LayerMask itemLayer;

    [HideInInspector]
    public int hotbarIndex = 0;

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
                HoldItem(item);
                item.transform.SetParent(itemHolder, true);
                item.gameObject.SetActive(false);
            }
        }

        foreach (Item item in accessories)
        {
            if (item != null)
            {
                item.transform.SetParent(itemHolder, true);
                item.gameObject.SetActive(false);
            }
        }
        hotbarIndex = Random.Range(0, items.Length);
        if (items[hotbarIndex] != null)
        {
            items[hotbarIndex].gameObject.SetActive(false);
            HoldItem(items[hotbarIndex]);
        }
    }

        void Update()
        {
            if (items[hotbarIndex] != null)
            {
                items[hotbarIndex].transform.position = hand.transform.position;
                items[hotbarIndex].transform.rotation = hand.transform.rotation;
            }
        }
        private void FixedUpdate()
        {
            if (items[hotbarIndex] != null)
            {
                items[hotbarIndex].onIdle.Invoke(this);
            }

         //   owner.entity.AI();
            if (items[hotbarIndex] != null)
            {
                if (owner.entity.player == false && owner.entity.mob.aiEnabled)
                {
                    items[hotbarIndex].ai.Invoke(this);
                    foreach (Item item in accessories)
                    {
                        if (item != null)
                        item.ai.Invoke(this);
                    }
                    if (EvoUtils.PercentChance(0.1f * owner.entity.mob.stats.level, true))
                    {
                        items[hotbarIndex].gameObject.SetActive(false);
                        hotbarIndex = Random.Range(0, items.Length);
                        HoldItem(items[hotbarIndex]);
                    }
                }
                if (owner.entity.mob.aiEnabled)
                {
                    if (items[hotbarIndex].cooldown < 0 && owner.entity.mob.primaryInput)
                    {
                    items[hotbarIndex].onUsePrimary.Invoke(this);
                    }
                    if (owner.entity.mob.secondaryInput && items[hotbarIndex])
                    {
                        items[hotbarIndex].onUseSecondary.Invoke(this);
                    }
                }
            }

            handAnimator.SetBool("Holding", (items[hotbarIndex] != null));
        }

    private void OnDestroy()
    {
        List<Item> selectedItems = new List<Item>();

        foreach (Item i in items)
        {
            if (EvoUtils.PercentChance(deathDecay))
            {
                selectedItems.Add(i);
            }
        }
        foreach (Item i in accessories)
        {
            if (EvoUtils.PercentChance(deathDecay))
            {
                selectedItems.Add(i);
            }
        }

        // Print selected items for demonstration
        foreach (var item in selectedItems)
        {
            if(item != null)
            DropItem(item);
        }
    }

    public Item GetItemLookedAt()
    {
        Ray ray = new Ray(owner.entity.mob.orientation.position, owner.entity.mob.orientation.transform.forward);
        RaycastHit hit;;

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

    Item currentItem = null;
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
            i.gameObject.layer = 6;
            foreach (Transform t in i.transform)
            {
                t.gameObject.layer = 6;
            }

        currentItem = i;
    }
    public void DropItem(Item i)
    {
        if (i.rb != null)
        {
            i.rb.constraints = RigidbodyConstraints.None;
        }
        i.transform.SetParent(null, true);
        i.transform.position = hand.transform.position + Vector3.up * 2;
        i.transform.rotation = hand.transform.rotation;
        i.gameObject.layer = 7;

        foreach (Transform t in i.transform)
        {
            t.gameObject.layer = 7;
        }
    }

    public void EquipItem(Item i)
    {
        if (i.GetComponent<StatModifier>())
        {
            Entity.ApplyStats(owner.entity, i.GetComponent<StatModifier>().modifier);
        }
        if (i.GetComponent<Accessory>())
        {
            VisualEquip(i.GetComponent<Accessory>());
        }
    }
    public void UnequipItem(Item i)
    {
        if (i.GetComponent<StatModifier>())
        {
            Entity.RemoveStats(owner.entity, i.GetComponent<StatModifier>().modifier);
        }
        if (i.GetComponent<Accessory>())
        {
            VisualUnequip(i.GetComponent<Accessory>());
        }
    }

    public void VisualEquip(Accessory a)
    {
            if (a.bodyRenderer == null)
            {
                GameObject obj = Object.Instantiate(new GameObject(), accessorization.bodyVisual.transform);
                obj.name = "bodypart";
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
    public void VisualUnequip(Accessory i)
    {
        Destroy(i.GetComponent<Accessory>().bodyRenderer);
    }
}


[System.Serializable]
public class Accessorization
{
    public Transform rootBone;
    public SkinnedMeshRenderer bodyVisual;
}

