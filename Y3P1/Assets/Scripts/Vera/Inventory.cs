using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using Y3P1;

public class Inventory : MonoBehaviourPunCallbacks {
    public List<InventorySlot> allSlots = new List<InventorySlot>();
    [SerializeField] private List<Item> allItems = new List<Item>(); 
    public InventorySlot currentSlot;
    public InventorySlot lastSlot;
    public Item drag;
    private bool dragging;
    [SerializeField] private Image onMouse;
    [SerializeField] private List<Item> startingItems = new List<Item>();
    GameObject testPlzWork;
    public void AddSlots()
    {
        allItems.Clear();
        for (int i = 0; i < allSlots.Count; i++)
        {
            allItems.Add(null);
        }
    }

    public void SetCurrentSlot(InventorySlot current)
    {

        currentSlot = current;
        if (currentSlot != null)
        {
            NotificationManager.instance.NewNotification(currentSlot.name + "entering");
        }
    }

    public void SetLastSlot(InventorySlot last)
    {

        lastSlot = last;
        NotificationManager.instance.NewNotification(last.name + "exiten");
    }

    public void StartDragging()
    {
        for (int i = 0; i < allSlots.Count; i++)
        {
            if (allSlots[i] == currentSlot)
            {
                if(allItems[i] == null)
                {
                    NotificationManager.instance.NewNotification("no item");
                    return;
                }
                drag = allItems[i];
            }
        }
        dragging = true;

        lastSlot = currentSlot;
        NotificationManager.instance.NewNotification(drag.name + " dragging");

    }

    public void ExitInv()
    {
        GetComponentInParent<Canvas>().enabled = false;
    }
    
    [PunRPC]
    private void DropItem(string toDrop)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            testPlzWork = PhotonNetwork.InstantiateSceneObject(toDrop, Player.localPlayer.transform.position, Quaternion.identity);
            InsertItem(testPlzWork);
        }
    }
    private byte[] ObjectToByteArray(object obj)
    {
        if (obj == null)
        {
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    private object ByteArrayToObject(byte[] bytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(bytes, 0, bytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        object obj = binForm.Deserialize(memStream);

        return obj;
    }

    public void InsertItem(GameObject objInW)
    {
        int lastSlotIndex = 0;
        for (int i = 0; i < allSlots.Count; i++)
        {
            if (allSlots[i] == lastSlot)
            {
                lastSlotIndex = i;
            }
        }
        objInW.GetComponent<ItemIW>().myItem = allItems[lastSlotIndex];
        //print("yay");
    }

    public void StopDragging()
    {
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
        if (currentSlot == null)
        {
            if(allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.weapon)
            {
                UnequipWeapon(lastSlotIndex);
            }
            GameObject newObj = allItems[lastSlotIndex].itemPrefab;
            print(newObj.name + "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAah");
            photonView.RPC( "DropItem", RpcTarget.AllBuffered, newObj.name);

            RemoveItem(lastSlotIndex);
            drag = null;
            return;
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
            RemoveItem(lastSlotIndex);
        }

        if(allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.weapon)
        {
            allSlots[currentSlotIndex].EquipWeapon(allItems[currentSlotIndex] as Weapon);
        }
        else if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.weapon)
        {
            allSlots[lastSlotIndex].EquipWeapon(allItems[lastSlotIndex] as Weapon);
        }
        drag = null;
        dragging = false;

    }

    void RemoveItem(int lSI)
    {
        allItems[lSI] = null;
        allSlots[lSI].DisableImage();
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
            print("weapon");
            if(allItems[currentSlotIndex] != null)
            {
                if (allItems[lastSlotIndex] is Weapon)
                {
                    return true;
                }
            }

            if(allItems[lastSlotIndex] is Weapon)
            {
                return true;
            }
        }
        else if (allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.helmet)
        {
            print("helmet");
            if (allItems[currentSlotIndex] != null)
            {
                if (allItems[lastSlotIndex] is Helmet)
                {
                    return true;
                }
            }

            if (allItems[lastSlotIndex] is Helmet)
            {
                return true;
            }
        }
        else if (allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.trinket)
        {
            print("trinket");
            if (allItems[currentSlotIndex] != null)
            {
                if (allItems[lastSlotIndex] is Trinket)
                {
                    return true;
                }
            }

            if (allItems[lastSlotIndex] is Trinket)
            {
                return true;
            }
        }
        else if(allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.all)
        {
            if(allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.weapon && allItems[currentSlotIndex] != null)
            {
                if (allItems[currentSlotIndex] is Weapon)
                {
                    return true;
                }
            }
            else if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.helmet && allItems[currentSlotIndex] != null)
            {
                if (allItems[currentSlotIndex] is Helmet)
                {
                    return true;
                }
            }
            else if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.trinket && allItems[currentSlotIndex] != null)
            {
                if (allItems[currentSlotIndex] is Trinket)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
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
            NotificationManager.instance.NewNotification(startingItems[o].name);
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
        if (Input.GetButtonDown("Tab"))
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
