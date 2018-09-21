using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using Y3P1;

public class Inventory : MonoBehaviourPunCallbacks
{
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
        allItems.Clear();
        for (int i = 0; i < allSlots.Count; i++)
        {
            allItems.Add(null);
        }
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
                if (allItems[i] == null)
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

    [PunRPC]
    private void DropItem(string toDrop,byte[] item)
    {
        NotificationManager.instance.NewNotification("test1");
        if (PhotonNetwork.IsMasterClient)
        {
            NotificationManager.instance.NewNotification("test2");
            GameObject insItem = PhotonNetwork.InstantiateSceneObject(toDrop, Player.localPlayer.transform.position, Quaternion.identity);
            NotificationManager.instance.NewNotification("test3");
            int id = insItem.GetComponent<PhotonView>().ViewID;
            photonView.RPC("RI", RpcTarget.AllBuffered, item, id);
        }
    }

    [PunRPC]
    private void RI(byte[] item, int id)
    {
            GameObject itemIG = PhotonNetwork.GetPhotonView(id).gameObject;
            itemIG.GetComponent<WeaponPrefab>().myItem = (Item)ByteArrayToObject(item);
            itemIG.GetComponent<WeaponPrefab>().Drop();
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

    void SaveItem(Item toSave, string objName)
    {
        print("test222222222222222222222222222222222222222222222222");
        byte[] saved = ObjectToByteArray(toSave);
        photonView.RPC("DropItem", RpcTarget.AllBuffered, objName, saved);
    }

    public void StopDragging()
    {
        if (drag == null || !CheckAvailability())
        {
            drag = null;
            return;
        }

        int lastSlotIndex = 0;
        for (int i = 0; i < allSlots.Count; i++)
        {
            if (allSlots[i] == lastSlot)
            {
                lastSlotIndex = i;
            }
        }
        if (currentSlot == null)
        {
            print("test111111111111111111111111111111111111111111111111111");
            if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.weapon)
            {
                UnequipWeapon(lastSlotIndex);
            }
            GameObject newObj = Database.hostInstance.allGameobjects[allItems[lastSlotIndex].prefabIndex];
            print(newObj);
            SaveItem(allItems[lastSlotIndex], newObj.name);

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

        if (allItems[currentSlotIndex] != null)
        {
            Item temp = allItems[currentSlotIndex];
            allItems[currentSlotIndex] = allItems[lastSlotIndex];
            allItems[lastSlotIndex] = temp;
            allSlots[currentSlotIndex].EnableImage();
            allSlots[lastSlotIndex].SetImage(Database.hostInstance.allSprites[allItems[lastSlotIndex].spriteIndex]);
            allSlots[currentSlotIndex].SetImage(Database.hostInstance.allSprites[allItems[currentSlotIndex].spriteIndex]);
        }
        else
        {
            if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.weapon)
            {
                UnequipWeapon(lastSlotIndex);
            }
            allItems[currentSlotIndex] = allItems[lastSlotIndex];
            allSlots[currentSlotIndex].EnableImage();
            allSlots[currentSlotIndex].SetImage(Database.hostInstance.allSprites[allItems[currentSlotIndex].spriteIndex]);
            RemoveItem(lastSlotIndex);
        }

        if (allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.weapon)
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
            if (allItems[currentSlotIndex] != null)
            {
                if (allItems[lastSlotIndex] is Weapon)
                {
                    return true;
                }
            }

            if (allItems[lastSlotIndex] is Weapon)
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
        else if (allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.all)
        {
            if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.weapon && allItems[currentSlotIndex] != null)
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
        return false;
    }

    private void UnequipWeapon(int slot)
    {
        allSlots[slot].EquipWeapon(null);
    }

    private void Awake()
    {
        OpenCloseInv();
        drag = null;
    }

    public void OpenCloseInv()
    {
        if (photonView.IsMine)
        {
            if (GetComponentInParent<Canvas>().enabled == false)
            {
                GetComponentInParent<Canvas>().enabled = true;
            }
            else
            {
                GetComponentInParent<Canvas>().enabled = false;
            }
        }
    }

    public void AddItem(Item toAdd)
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            if (allItems[i] == null && allSlots[i].CheckSlotType())
            {
                allItems[i] = toAdd;
                allSlots[i].SetImage(Database.hostInstance.allSprites[allItems[i].spriteIndex]);
                allSlots[i].EnableImage();
                break;
            }
        }
    }

    // for testing
    private void Update()
    {
        if (Input.GetButtonDown("Tab"))
        {
            OpenCloseInv();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Item test = LootRandomizer.instance.DropLoot();
            print(test.itemName);
            AddItem(LootRandomizer.instance.DropLoot());
        }

        print(drag);
        if (drag == null)
        {
            onMouse.enabled = false;
        }
        else
        {
            onMouse.enabled = true;
            onMouse.sprite = Database.hostInstance.allSprites[drag.spriteIndex];
            onMouse.transform.position = Input.mousePosition;
        }
    }

    public bool CheckFull()
    {
        bool check = true;
        for (int i = 0; i < allSlots.Count; i++)
        {
            if(allSlots[i].slotType == InventorySlot.SlotType.all)
            {
                if(allItems[i] == null)
                {
                    check = false;
                }
            }
        }
        return check;
    }
}
