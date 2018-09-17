using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
    public List<InventorySlot> allSlots = new List<InventorySlot>();
    [SerializeField] private List<Item> allItems = new List<Item>(); 
    public InventorySlot currentSlot;
    public InventorySlot lastSlot;
    public Item drag;
    private bool dragging;
    [SerializeField] private Image onMouse;
    [SerializeField] private List<Item> startingItems = new List<Item>();

    public void AddSlots()
    {
        //allItems.Clear();
        //for (int i = 0; i < allSlots.Count; i++)
        //{
        //    allItems.Add(null);
        //}
    }
    
    public void SetCurrentSlot(InventorySlot current)
    {
        currentSlot = current;
    }

    public void SetLastSlot(InventorySlot last)
    {
        lastSlot = last;
    }

    public void StartDragging()
    {
        for (int i = 0; i < allSlots.Count; i++)
        {
            if (allSlots[i] == currentSlot)
            {
                if(allItems[i] == null)
                {
                    return;
                }
                drag = allItems[i];
            }
        }
        dragging = true;

        lastSlot = currentSlot;
 
    }

    public void ExitInv()
    {
        GetComponentInParent<Canvas>().enabled = false;
    }

    public void StopDragging()
    {
        print("test123");
        if(drag == null || !CheckAvailability())
        {
            drag = null;
            return;
        }
        int lastSlotIndex = 0;
        for (int i = 0; i < allSlots.Count; i++)
        {
            if(allSlots[i] == lastSlot)
            {
                lastSlotIndex = i;
            }
        }
        int currentSlotIndex = 0;
        for (int i = 0; i < allSlots.Count; i++)
        {
            if (allSlots[i] == currentSlot)
            {
                currentSlotIndex = i;
            }
        }
        
        if(allItems[currentSlotIndex] != null)
        {
            Item temp = allItems[currentSlotIndex];
            allItems[currentSlotIndex] = allItems[lastSlotIndex];
            allItems[lastSlotIndex] = temp;
            allSlots[currentSlotIndex].EnableImage();
            allSlots[lastSlotIndex].SetImage(allItems[lastSlotIndex].itemImage);
            allSlots[currentSlotIndex].SetImage(allItems[currentSlotIndex].itemImage);
        }
        else
        {
            if(allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.weapon)
            {
                UnequipWeapon(lastSlotIndex);
            }
            allItems[currentSlotIndex] = allItems[lastSlotIndex];
            allSlots[currentSlotIndex].EnableImage();
            allSlots[currentSlotIndex].SetImage(allItems[currentSlotIndex].itemImage);
            allItems[lastSlotIndex] = null;
            allSlots[lastSlotIndex].DisableImage();


        }

        if(allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.weapon)
        {
            allSlots[currentSlotIndex].EquipWeapon(allItems[currentSlotIndex] as Weapon);
        }
        drag = null;
        dragging = false;

    }

    public bool IsDragging()
    {
        if (drag == null)
        {
            return false;
        }
        return true;
    }

    private bool CheckAvailability()
    {
        int currentSlotIndex = 0;
        for (int i = 0; i < allSlots.Count; i++)
        {
            if (allSlots[i] == currentSlot)
            {
                currentSlotIndex = i;
            }
        }
        int lastSlotIndex = 0;
        for (int i = 0; i < allSlots.Count; i++)
        {
            if (allSlots[i] == lastSlot)
            {
                lastSlotIndex = i;
            }
        }
        if (allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.weapon)
        {
            if(allItems[lastSlotIndex] is Weapon)
            {
                return true;
            }
        }
        else if(allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.all)
        {
            return true;
        }
        //else if(allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.armor && allItems[lastSlotIndex].GetType() == typeof(armor))
        //{
                //return true;
        //}
        return false;
    }

    private void UnequipWeapon(int slot)
    {
        allSlots[slot].EquipWeapon(null);
    }


    private void Awake()
    {
        for (int o = 0; o < startingItems.Count; o++)
        {
            for (int i = 0; i < allItems.Count; i++)
            {
                if (allItems[i] == null && allSlots[i].CheckSlotType())
                {
                    allItems[i] = startingItems[o];
                    allSlots[i].SetImage(allItems[i].itemImage);
                    allSlots[i].EnableImage();
                    break;
                }
            }
        }
    }
    // for testing
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if(GetComponentInParent<Canvas>().enabled == false)
            {
                GetComponentInParent<Canvas>().enabled = true;
            }
            else
            {
                GetComponentInParent<Canvas>().enabled = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.C) && LootRandomizer.instance != null)
        {
            for (int i = 0; i < allItems.Count; i++)
            {
                if(allItems[i] == null && allSlots[i].CheckSlotType())
                {
                    allItems[i] = LootRandomizer.instance.DropLoot();
                    allSlots[i].SetImage(allItems[i].itemImage);
                    allSlots[i].EnableImage();
                    break;
                }
            }
        }

        if(drag == null)
        {
            onMouse.enabled = false;
        }
        else
        {
            onMouse.enabled = true;
            onMouse.sprite = drag.itemImage;
            onMouse.transform.position = Input.mousePosition;
        }
    }
}
