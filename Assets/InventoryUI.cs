using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.Rendering;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public ItemSlot[] slotGrid;
    public ItemSlot[] hotbar;
    public ItemSlot[] accessorySlots;

    public ItemSlot slot;

    public GridLayoutGroup slotHolder;

    public int gridSizeX = 5;
    public int gridSizeY = 5;

    public Item itemHeld;

    public Canvas inventoryCanvas;
    public LayerMask uiLayer;

    public Camera renderCam;

    public RectTransform mouseObj;

    public Inventory inv;

    public static int slotWidth;
    public static int slotHeight;

    public GameObject inventory;

    public GameObject tooltip;
    public TextMeshProUGUI tooltipTitle;
    public TextMeshProUGUI tooltipDescription;
    // Start is called before the first frame update
    void Awake()
    {
        slotWidth = gridSizeX;
        slotHeight = gridSizeY;

     //   slotGrid = new ItemSlot[slotWidth, slotHeight];
        inv = GetComponent<Inventory>();

        //  for (int x = 0; x < slotWidth; x++)
        //  {
        //      for (int y = 0; y < slotHeight; y++)
        //      {
        //          ItemSlot itemSlot = Object.Instantiate(slot, slotHolder.transform.position, slotHolder.transform.rotation, slotHolder.transform);
        //          itemSlot.gridPos = new Vector2Int(x, y);
        //          itemSlot.inv = this;
        //
        //          slotGrid[x, y] = itemSlot;
        //      }
        //  }

        for (int i = 0; i < slotGrid.Length; i++)
        {
                slotGrid[i].inv = this;
                slotGrid[i].slotIndex = i;
        }
        for (int i = 0; i < accessorySlots.Length; i++)
        {
                accessorySlots[i].inv = this;
                slotGrid[i].slotIndex = slotGrid.Length + i;
        }

        inv.items = new Item[slotGrid.Length];
        inv.accessories = new Item[accessorySlots.Length];
    }

    bool invOpened = false;
    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = inventoryCanvas.planeDistance - 1;

        mouseObj.position = Camera.main.ScreenToWorldPoint(mousePos);

        if (itemHeld != null)
        {
            itemHeld.transform.position = Vector3.Lerp(itemHeld.transform.position, mouseObj.position, 50 * Time.deltaTime);
            itemHeld.transform.rotation = Quaternion.Slerp(itemHeld.transform.rotation, mouseObj.rotation * Quaternion.Euler(itemHeld.rotationOffset), 10 * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Cursor.lockState = !invOpened ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !invOpened ? false : true;

            inventory.transform.localPosition = !invOpened ? new Vector3(9999, 9999, 0) : new Vector3(0, 0, 0);

            invOpened = !invOpened;

            if (invOpened && itemHeld != null)
            {
                inv.DropItem(itemHeld);
                itemHeld = null;
            }
        }


        if (Input.GetKey(KeyCode.E))
        {
            if (inv.GetItemLookedAt() != null)
            {
                //   inv.hand.connectedBody = null;
                AddItem(inv.GetItemLookedAt());
            }
        }

        if (GetRectUnderCursor() != null && GetRectUnderCursor().GetComponent<ItemSlot>() && GetRectUnderCursor().GetComponent<ItemSlot>().item != null)
        {
                Item item = GetRectUnderCursor().GetComponent<ItemSlot>().item;
                tooltipTitle.text = item.itemName;
                tooltipDescription.text = item.itemDescription;
        }
        else if (inv.GetItemLookedAt() != null)
        {
            Item item = inv.GetItemLookedAt();
            tooltipTitle.text = item.itemName;
            tooltipDescription.text = item.itemDescription;
        }
        else
        {
            tooltipTitle.text = "";
            tooltipDescription.text = "";
        }

        if (!inv.owner.entity.mob.primaryInput && !inv.owner.entity.mob.primaryInput)
        {
            HotbarScrollLogic();
        }
    }
    public void HotbarScrollLogic()
    {
        if (Input.mouseScrollDelta.y * 100 > 0)
        {
            if (slotGrid[inv.hotbarIndex].item != null)
            {
                SetItemToInventory(slotGrid[inv.hotbarIndex].item, slotGrid[inv.hotbarIndex]);
            }
            slotGrid[inv.hotbarIndex].button.interactable = true;
            inv.hotbarIndex++;
            if (inv.hotbarIndex >= hotbar.Length)
            {
                inv.hotbarIndex = 0;
            }
            slotGrid[inv.hotbarIndex].button.interactable = false;
            if (slotGrid[inv.hotbarIndex].item != null)
            {
                slotGrid[inv.hotbarIndex].holdingItem = false;
                inv.HoldItem(slotGrid[inv.hotbarIndex].item);
            }
        }
        else if (Input.mouseScrollDelta.y * 100 < 0)
        {

            if (slotGrid[inv.hotbarIndex].item != null)
            {
                SetItemToInventory(slotGrid[inv.hotbarIndex].item, slotGrid[inv.hotbarIndex]);
            }
            slotGrid[inv.hotbarIndex].button.interactable = true;
            inv.hotbarIndex--;
            if (inv.hotbarIndex < 0)
            {
                inv.hotbarIndex = hotbar.Length - 1;
            }
            slotGrid[inv.hotbarIndex].button.interactable = false;
            if (slotGrid[inv.hotbarIndex].item != null)
            {
                slotGrid[inv.hotbarIndex].holdingItem = false;
                inv.HoldItem(slotGrid[inv.hotbarIndex].item);
            }
        }

    }

    //  public static bool IsPossibleSlot(ItemSlot slot, Item item)
    //  {
    //      return (item.size.x + slot.gridPos.x <= slotWidth && item.size.y + slot.gridPos.y <= slotHeight);
    //  }
    //
    //  public static List<ItemSlot> TakenSlots(ItemSlot slot, Item item)
    //  {
    //      List<ItemSlot> returnedSlots = new List<ItemSlot>();
    //      for (int x = 0; x < item.size.x; x++)
    //      {
    //          for (int y = 0; y < item.size.y; y++)
    //          {
    //              if (x + slot.gridPos.x < slotWidth && y + slot.gridPos.y < slotHeight)
    //              {
    //                  if (slotGrid[x + slot.gridPos.x, y + slot.gridPos.y] != null)
    //                  {
    //                      returnedSlots.Add(slotGrid[x + slot.gridPos.x, y + slot.gridPos.y]);
    //                  }
    //              }
    //          }
    //      }
    //      return returnedSlots;
    //  }

    //  public Vector3 GetCanvasPos(Vector3 pos)
    //  {
    //      // Convert the screen space position to local space of the canvas
    //      RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryCanvas.transform as RectTransform, pos, Camera.main, out Vector2 localPoint);
    //      return localPoint;
    //  }

    public void SetItemToInventory(Item i, ItemSlot slot = null)
    {
        i.rb.constraints = RigidbodyConstraints.FreezeAll;

        if (slot != null)
        {
            i.transform.position = slot.transform.position;
            i.transform.parent = slot.transform;
            slot.item = i;
            slot.holdingItem = true;
        }

        i.gameObject.layer = 5;

        foreach (Transform child in i.transform)
        {
            child.gameObject.layer = 5;
        }
    }

    public void AddItem(Item item)
    {
        bool added = false;
        //    for (int x = 0; x < slotWidth; x++)
        //    {
        //        for (int y = 0; y < slotHeight; y++)
        //        {
        //            if (!added)
        //            {
        //                if (slotGrid[x, y].item == null)
        //                {
        //                    SetItemToInventory(item, slotGrid[x, y]);
        //                    added = true;
        //                 //   break;
        //                }
        //            }
        //        }
        //    }

        for(int i = 0; i < slotGrid.Length; i++)
        {
                if (slotGrid[i].item == null && !added)
                {
                    inv.items[i] = item;
                    SetItemToInventory(item, slotGrid[i]);
                    added = true;
                    break;
                } 
        }
    }

    public GameObject GetObjectUnderCursor()
    {
        // Cast a ray from the camera to the mouse position
        Ray ray = renderCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits any collider in the specified layer mask
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, uiLayer))
        {
            Debug.Log(hit.collider.gameObject);
            return hit.collider.gameObject;
        }

        return null;
    }

    public GameObject GetRectUnderCursor()
    {
       // Check if the mouse is over any UI element
       if (EventSystem.current.IsPointerOverGameObject())
       {
           // Get the pointer data
           PointerEventData pointerData = new PointerEventData(EventSystem.current);
           pointerData.position = Input.mousePosition;
     
           // Perform the raycast
           List<RaycastResult> results = new List<RaycastResult>();
           EventSystem.current.RaycastAll(pointerData, results);
     
           // Get the first object hit
           if (results.Count > 0)
           {
               GameObject objectHit = results[0].gameObject;
               return objectHit;
           }
       }
      return null;
    }
}
