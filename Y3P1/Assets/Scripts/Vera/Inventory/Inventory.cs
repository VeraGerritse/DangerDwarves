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
    public List<Item> allItems = new List<Item>();
    public List<InventorySlot> hotkeySlots = new List<InventorySlot>();
    public InventorySlot currentSlot;
    public InventorySlot lastSlot;
    public Item drag;
    private bool dragging;
    [SerializeField] private Image onMouse;
    [SerializeField] private List<Item> startingItems = new List<Item>();
    [SerializeField] private AverageItemLevel aIL;
    private bool isInitialised;
    private Canvas canvas;
    public int totalGoldAmount;

    private Stats currentStats;
    public int averageILevel = 1;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color common;
    [SerializeField] private Color rare;
    [SerializeField] private Color epic;
    [SerializeField] private Color Legendary;


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
    private void DropItem(string toDrop, byte[] item, Vector3 pos)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject insItem = PhotonNetwork.InstantiateSceneObject(toDrop, pos, Quaternion.identity);
            int id = insItem.GetComponent<PhotonView>().ViewID;
            photonView.RPC("RI", RpcTarget.AllBuffered, item, id);
        }
    }

    [PunRPC]
    private void AddGold(int amount)
    {
        UpdateGold(amount);
    }

    [PunRPC]
    private void RI(byte[] item, int id)
    {
        GameObject itemIG = PhotonNetwork.GetPhotonView(id).gameObject;
        itemIG.GetComponent<ItemPrefab>().myItem = (Item)ByteArrayToObject(item);
        itemIG.GetComponent<ItemPrefab>().Drop();
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

    void SaveItem(Item toSave, string objName, Vector3 loc)
    {
        byte[] saved = ObjectToByteArray(toSave);
        loc.y += 0.1f;
        photonView.RPC("DropItem", RpcTarget.AllBuffered, objName, saved, loc);
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
            if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.weapon)
            {
                allSlots[lastSlotIndex].EquipWeapon(null);
                CalculateArmor();
            }
            if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.helmet)
            {
                allSlots[lastSlotIndex].EquipHelmet(null);
                CalculateArmor();
            }
            if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.trinket)
            {
                allSlots[lastSlotIndex].EquipTrinket(null);
                CalculateArmor();
            }
            GameObject newObj = Database.hostInstance.allGameobjects[allItems[lastSlotIndex].prefabIndex];
            SaveItem(allItems[lastSlotIndex], newObj.name, Player.localPlayer.transform.position);

            RemoveItem(lastSlotIndex);
            drag = null;
            UpdateInventoryColor();
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
                allSlots[lastSlotIndex].EquipWeapon(null);
                CalculateArmor();
            }
            if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.helmet)
            {
                allSlots[lastSlotIndex].EquipHelmet(null);
                CalculateArmor();
            }
            if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.trinket)
            {
                allSlots[lastSlotIndex].EquipTrinket(null);
                CalculateArmor();
            }
            allItems[currentSlotIndex] = allItems[lastSlotIndex];
            allSlots[currentSlotIndex].EnableImage();
            allSlots[currentSlotIndex].SetImage(Database.hostInstance.allSprites[allItems[currentSlotIndex].spriteIndex]);
            RemoveItem(lastSlotIndex);
        }

        if (allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.weapon)
        {
            allSlots[currentSlotIndex].EquipWeapon(allItems[currentSlotIndex] as Weapon);
            CalculateArmor();
        }
        else if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.weapon)
        {
            allSlots[lastSlotIndex].EquipWeapon(allItems[lastSlotIndex] as Weapon);
            CalculateArmor();
        }

        if (allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.helmet)
        {
            allSlots[currentSlotIndex].EquipHelmet(allItems[currentSlotIndex] as Helmet);
            CalculateArmor();
        }
        else if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.helmet)
        {
            allSlots[lastSlotIndex].EquipHelmet(allItems[lastSlotIndex] as Helmet);
            CalculateArmor();
        }

        if (allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.trinket)
        {
            allSlots[currentSlotIndex].EquipTrinket(allItems[currentSlotIndex] as Trinket);
            CalculateArmor();
        }
        else if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.trinket)
        {
            allSlots[lastSlotIndex].EquipTrinket(allItems[lastSlotIndex] as Trinket);
            CalculateArmor();
        }
        drag = null;
        dragging = false;
        UpdateInventoryColor();
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
        else if (allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.all || allSlots[currentSlotIndex].slotType == InventorySlot.SlotType.hotkey)
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
        CalculateArmor();
        UpdateInventoryColor();
    }

    IEnumerator Time()
    {
        yield return new WaitForSeconds(0.15f);
        CalculateArmor();
        SafeManager.instance.LoadGame();
        CalculateArmor();
        Player.localPlayer.entity.health.ResetHealth(100);
        UpdateGold(0);
        UpdateInventoryColor();
        for (int i = 0; i < allSlots.Count; i++)
        {
            if(allSlots[i].slotType == InventorySlot.SlotType.hotkey)
            {
                hotkeySlots.Add(allSlots[i]);
            }
        }
        hotkeySlots.Reverse();
        OrderHotKeySlots();
    }

    private void OrderHotKeySlots()
    {
        List<InventorySlot> tempHK = hotkeySlots;
        for (int i = 0; i < tempHK.Count; i++)
        {
            if(tempHK[i].index < 1)
            {
                hotkeySlots[tempHK[i].index - 1] = tempHK[i];
            }
        }
    }

    private void EquipToHotkey(int toSlot)
    {
        int ind = GetIndex(currentSlot);
        if(currentSlot == null)
        {
            return;
        }
        if (currentSlot.slotType == InventorySlot.SlotType.hotkey )
        {
            return;
        }

        int slot = toSlot - 1;
        int index = GetIndex(hotkeySlots[slot]);

        if(allItems[ind] == null && allItems[index] != null)
        {
            allItems[ind] = allItems[index];
            allItems[index] = null;
            allSlots[index].DisableImage();
            allSlots[ind].SetImage(Database.hostInstance.allSprites[allItems[ind].spriteIndex]);
            allSlots[ind].EnableImage();
        }
        if(allItems[index] == null && allItems[ind] != null)
        {
            allItems[index] = allItems[ind];
            allItems[ind] = null;
            allSlots[ind].DisableImage();
            allSlots[index].SetImage(Database.hostInstance.allSprites[allItems[index].spriteIndex]);
            allSlots[index].EnableImage();
        }
        else
        {
            if(allSlots[ind].slotType == InventorySlot.SlotType.helmet && allItems[index] is Helmet || allSlots[ind].slotType == InventorySlot.SlotType.weapon && allItems[index] is Weapon|| allSlots[ind].slotType == InventorySlot.SlotType.trinket && allItems[index] is Trinket || allSlots[ind].slotType == InventorySlot.SlotType.all)
            {
                Item temp = allItems[ind];
                allItems[ind] = allItems[index];
                allItems[index] = temp;
                allSlots[ind].EnableImage();
                allSlots[index].SetImage(Database.hostInstance.allSprites[allItems[index].spriteIndex]);
                allSlots[ind].SetImage(Database.hostInstance.allSprites[allItems[ind].spriteIndex]);
            }

        }
        UpdateInventoryColor();
    }

    public void OpenCloseInv()
    {
        if (canvas.enabled == false)
        {
            canvas.enabled = true;
        }
        else
        {
            canvas.enabled = false;
        }
    }

    public void UpdateGold(int amount)
    {
        Player.localPlayer.myInventory.totalGoldAmount += amount;
        if (Player.localPlayer.myInventory.totalGoldAmount > 999999999)
        {
            Player.localPlayer.myInventory.totalGoldAmount = 999999999;
        }
        UIManager.instance.UpdateGold(Player.localPlayer.myInventory.totalGoldAmount);
        StatsInfo.instance.UpdateGold(Player.localPlayer.myInventory.totalGoldAmount);
    }

    public void AddItem(Item toAdd)
    {
        if(toAdd is Gold)
        {
            Gold tA = (Gold)toAdd;

            photonView.RPC("AddGold", RpcTarget.All, tA.amount);
            return;
        }
        for (int i = 0; i < allItems.Count; i++)
        {
            if (allItems[i] == null && allSlots[i].CheckSlotType())
            {
                allItems[i] = toAdd;
                if(Database.hostInstance.allSprites[allItems[i].spriteIndex] != null)
                {
                    allSlots[i].SetImage(Database.hostInstance.allSprites[allItems[i].spriteIndex]);
                    allSlots[i].EnableImage();
                }
                break;
            }
        }
        UpdateInventoryColor();
    }

    // for testing
    void RemoveAllItems()
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            if(allSlots[i].slotType == InventorySlot.SlotType.all)
            {
                allItems[i] = null;
                allSlots[i].DisableImage();
            }

        }
    }

    private void Update()
    {
        if (!isInitialised)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            photonView.RPC("AddGold", RpcTarget.All, Random.Range(500, 1000)); 
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            RemoveAllItems();
        }
        if (Input.GetButtonDown("One"))
        {
            if (InventoryIsOpen())
            {
                EquipToHotkey(1);
            }
            else
            {
                HotKeyAction(1);
            }
        }
        if (Input.GetButtonDown("Two"))
        {
            if (InventoryIsOpen())
            {
                EquipToHotkey(2);
            }
            else
            {
                HotKeyAction(2);
            }
        }
        if (Input.GetButtonDown("Three"))
        {
            if (InventoryIsOpen())
            {
                EquipToHotkey(3);
            }
            else
            {
                HotKeyAction(3);
            }
        }
        if (Input.GetButtonDown("Four"))
        {
            if (InventoryIsOpen())
            {
                EquipToHotkey(4);
            }
            else
            {
                HotKeyAction(4);
            }
        }
        if (Input.GetButtonDown("Five"))
        {
            if (InventoryIsOpen())
            {
                EquipToHotkey(5);
            }
            else
            {
                HotKeyAction(5);
            }
        }
        if (currentSlot != null)
        {
            int index = GetIndex(currentSlot);
            if (allItems[index] != null)
            {
                allItems[index].SendInfo();
            }
            else
            {
                StatsInfo.instance.DisablePanel();
            }
        }
        else
        {
            StatsInfo.instance.DisablePanel();
        }

        if (currentSlot != null)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                EquipItem(GetIndex(currentSlot));
                CalculateArmor();
                UpdateInventoryColor();
            }
        }

        if (Input.GetButtonDown("Fire3"))
        {
            if(currentSlot != null)
            {
                int index = 0;
                for (int i = 0; i < allSlots.Count; i++)
                {
                    if (currentSlot == allSlots[i])
                    {
                        index = i;
                    }
                }
                if(allItems[index] != null)
                {
                    if (allSlots[index].slotType == InventorySlot.SlotType.weapon)
                    {
                        allSlots[index].EquipWeapon(null);
                        CalculateArmor();
                    }
                    if (allSlots[index].slotType == InventorySlot.SlotType.helmet)
                    {
                        allSlots[index].EquipHelmet(null);
                        CalculateArmor();
                    }
                    if (allSlots[index].slotType == InventorySlot.SlotType.trinket)
                    {
                        allSlots[index].EquipTrinket(null);

                    }
                    GameObject newObj = Database.hostInstance.allGameobjects[allItems[index].prefabIndex];
                    SaveItem(allItems[index], newObj.name, Player.localPlayer.transform.position);

                    RemoveItem(index);
                    drag = null;
                }

            }
            CalculateArmor();
            UpdateInventoryColor();
        }

        if (Input.GetButtonDown("Tab"))
        {
            OpenCloseInv();
        }

        if (Input.GetKey(KeyCode.H))
        {
            if (Input.GetKey(KeyCode.B))
            {
                if (Input.GetKeyDown(KeyCode.N))
                {

                    if (LootRandomizer.instance != null)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Item newItem = LootRandomizer.instance.DropLoot(100 + averageILevel, 3);
                            if (newItem == null)
                            {
                                return;
                            }
                            AddItem(newItem);
                        }
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {

                if (LootRandomizer.instance != null)
                {
                    Item newItem = LootRandomizer.instance.DropLoot(10 + averageILevel,3);
                    if (newItem == null)
                    {
                        return;
                    }
                    AddItem(newItem);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            if(LootRandomizer.instance != null)
            {
                for (int i = 0; i < allSlots.Count; i++)
                {
                    Item newItem = LootRandomizer.instance.DropLoot(averageILevel, 3);
                    if (newItem == null)
                    {
                        return;
                    }
                    AddItem(newItem);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            CalculateArmor();
            SetInfo();
        }
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

    private void HotKeyAction(int ind)
    {

        int index = GetIndex(hotkeySlots[ind - 1]);
        if (allItems[index] == null)
        {

            return;
        }
        if(allItems[index] is Weapon|| allItems[index] is Helmet || allItems[index] is Trinket)
        {

            EquipItem(index);
            return;
        }
    }

    private int GetIndex(InventorySlot slot)
    {
        int index = -1;
        for (int i = 0; i < allSlots.Count; i++)
        {
            if (slot == allSlots[i])
            {
                index = i;
            }
        }

        return index;
    }

    private void EquipItem(int slotToEquip)
    {
        if (Player.localPlayer.dwarfAnimController.CanEquipRanged())
        {
            if (allItems[slotToEquip] != null)
            {
                bool unEquip = false;
                if (allSlots[slotToEquip].slotType != InventorySlot.SlotType.all && allSlots[slotToEquip].slotType != InventorySlot.SlotType.hotkey)
                {
                    unEquip = true;
                }



                int newSlot = 0;
                int type = 0;
                if (allItems[slotToEquip] is Weapon)
                {
                    for (int i = 0; i < allSlots.Count; i++)
                    {
                        if (allSlots[i].slotType == InventorySlot.SlotType.weapon)
                        {
                            newSlot = i;
                            type = 1;
                        }
                    }
                }
                else if (allItems[slotToEquip] is Helmet)
                {
                    for (int i = 0; i < allSlots.Count; i++)
                    {
                        if (allSlots[i].slotType == InventorySlot.SlotType.helmet)
                        {
                            newSlot = i;
                            type = 2;
                        }
                    }
                }
                else if (allItems[slotToEquip] is Trinket)
                {
                    for (int i = 0; i < allSlots.Count; i++)
                    {
                        if (allSlots[i].slotType == InventorySlot.SlotType.trinket)
                        {
                            newSlot = i;
                            type = 3;
                        }
                    }
                }
                if (type != 0)
                {
                    if (allItems[newSlot] == null)
                    {

                    }

                    if (unEquip)
                    {
                        if (!CheckFull())
                        {
                            if (type == 1)
                            {
                                allSlots[newSlot].EquipWeapon(null);
                                CalculateArmor();
                            }
                            if (type == 2)
                            {
                                allSlots[newSlot].EquipHelmet(null);
                                CalculateArmor();
                            }
                            if (type == 3)
                            {
                                allSlots[newSlot].EquipTrinket(null);
                                CalculateArmor();
                            }
                            AddItem(allItems[slotToEquip]);
                            RemoveItem(slotToEquip);
                        }
                    }
                    else
                    {
                        Item temp = allItems[slotToEquip];
                        allItems[slotToEquip] = allItems[newSlot];
                        allItems[newSlot] = temp;
                        if (type == 1)
                        {
                            allSlots[newSlot].EquipWeapon((Weapon)allItems[newSlot]);
                            CalculateArmor();
                        }
                        if (type == 2)
                        {
                            allSlots[newSlot].EquipHelmet((Helmet)allItems[newSlot]);
                            CalculateArmor();
                        }
                        if (type == 3)
                        {
                            allSlots[newSlot].EquipTrinket((Trinket)allItems[newSlot]);
                            CalculateArmor();
                        }
                    }

                    if (allItems[newSlot] != null)
                    {
                        allSlots[newSlot].EnableImage();
                        allSlots[newSlot].SetImage(Database.hostInstance.allSprites[allItems[newSlot].spriteIndex]);
                    }
                    else
                    {
                        allSlots[newSlot].DisableImage();
                    }
                    if (allItems[slotToEquip] != null)
                    {
                        allSlots[slotToEquip].EnableImage();
                        allSlots[slotToEquip].SetImage(Database.hostInstance.allSprites[allItems[slotToEquip].spriteIndex]);
                    }
                    else
                    {
                        allSlots[slotToEquip].DisableImage();
                    }
                    drag = null;
                }
            }
        }
        UpdateInventoryColor();
    }

    public bool CheckFull()
    {
        bool check = true;
        for (int i = 0; i < allSlots.Count; i++)
        {
            if (allSlots[i].slotType == InventorySlot.SlotType.all)
            {
                print(allItems[i]);
                if (allItems[i] == null)
                {
                    check = false;
                }
            }
        }
        return check;
    }

    public void Initialise(bool local)
    {
        canvas = GetComponentInParent<Canvas>();

        if (local)
        {
            AddSlots();
            OpenCloseInv();
            drag = null;
            StartCoroutine(Time());

            isInitialised = true;
        }
        else
        {
            OpenCloseInv();
            drag = null;
            enabled = false;
        }
    }

    public void CalculateArmor()
    {
        Stats together = new Stats();
        Stats helmet = null;
        Stats trinket = null;
        Stats weapon = null;

        float iLevel = 0;
        

        int helmetSlot = -1;
        int trinketSlot = -1;
        int weaponSlot = -1;

        for (int i = 0; i < allSlots.Count; i++)
        {
            if (allSlots[i].slotType == InventorySlot.SlotType.helmet)
            {
                helmetSlot = i;
            }
            if (allSlots[i].slotType == InventorySlot.SlotType.trinket)
            {
                trinketSlot = i;
            }
            if (allSlots[i].slotType == InventorySlot.SlotType.weapon)
            {
                weaponSlot = i;
            }
        }
        if(allItems[helmetSlot] != null)
        {
            helmet = allItems[helmetSlot].myStats;
            iLevel += allItems[helmetSlot].itemLevel;
        }
        else
        {
            iLevel += 1;
        }
        if (allItems[trinketSlot] != null)
        {
            trinket = allItems[trinketSlot].myStats;
            iLevel += allItems[trinketSlot].itemLevel;
        }
        else
        {
            iLevel += 1;
        }
        if (allItems[weaponSlot] != null)
        {
            weapon = allItems[weaponSlot].myStats;
            iLevel += allItems[weaponSlot].itemLevel;
        }
        else
        {
            iLevel += 1;
        }


        if (helmet!= null)
        {
            together.agility    += helmet.agility;
            together.defense    += helmet.defense;
            together.stamina    += helmet.stamina;
            together.strength   += helmet.strength;
            together.willpower  += helmet.willpower;
        }
        if (trinket != null)
        {
            together.agility   += trinket.agility;
            together.defense   += trinket.defense;
            together.stamina   += trinket.stamina;
            together.strength  += trinket.strength;
            together.willpower += trinket.willpower;
        }
        if (weapon != null)
        {
            together.agility    += weapon.agility;
            together.defense    += weapon.defense;
            together.stamina    += weapon.stamina;
            together.strength   += weapon.strength;
            together.willpower  += weapon.willpower;
        }

        currentStats = together;
        Player.localPlayer.entity.UpdateStats(together);
        averageILevel = Mathf.RoundToInt(iLevel / 3);
        SetInfo();
    }

    public string[] Stat()
    {
        return new string[] { "Stamina: <color=#00A8FF>" + currentStats.stamina.ToString(), "Strenght: <color=#00A8FF>" + currentStats.strength.ToString(), "Agility: <color=#00A8FF>" + currentStats.agility.ToString(), "Willpower: <color=#00A8FF>" + currentStats.willpower.ToString(), "Defence: <color=#00A8FF>" + currentStats.defense.ToString() };
    }

    public string[] ILevel()
    {
        return new string[] { "Item Level: <color=#00A8FF>" + averageILevel.ToString() };
    }

    public void SetInfo()
    {
        if(currentStats == null)
        {
            currentStats = new Stats();
        }
        StatsInfo.instance.SetPlayerStats(Stat(), ILevel());
        aIL.CalculateLevel();
    }

    public bool InventoryIsOpen()
    {
        return canvas.enabled;
    }

    public void DropNewItem(Vector3 loc , Entity.EntityType type)
    {
        Item newItem = null;
        if (type == Entity.EntityType.Humanoid)
        {
           newItem  = LootRandomizer.instance.DropLoot(averageILevel, 1);
        }
        else if (type == Entity.EntityType.Box)
        {
            newItem = LootRandomizer.instance.DropLoot(averageILevel, 2);
        }
        else if(type == Entity.EntityType.Chest)
        {
            newItem = LootRandomizer.instance.DropLoot(averageILevel, 3);
        }

        if(newItem == null)
        {
            return;
        }
        GameObject newObj = Database.hostInstance.allGameobjects[newItem.prefabIndex];
        SaveItem(newItem, newObj.name,loc);
    }

    public void LoadInventory(List<Item>saved,int gold, List<bool> isItem)
    {
        UpdateGold(gold);
        if (saved == null)
        {

                return;

        }
        for (int i = 0; i < allItems.Count; i++)
        {
            if(i < saved.Count)
            {
                //allItems[i] = saved[i];
                print(saved[i]);
                if(isItem[i])
                {
                    allItems[i] = saved[i];
                    allSlots[i].SetImage(Database.hostInstance.allSprites[allItems[i].spriteIndex]);
                    allSlots[i].EnableImage();
                    if (allSlots[i].slotType == InventorySlot.SlotType.weapon)
                    {
                        allSlots[i].EquipWeapon((Weapon)saved[i]);
                    }
                    if (allSlots[i].slotType == InventorySlot.SlotType.helmet)
                    {
                        allSlots[i].EquipHelmet((Helmet)saved[i]);
                    }
                    if (allSlots[i].slotType == InventorySlot.SlotType.trinket)
                    {
                        allSlots[i].EquipTrinket((Trinket)saved[i]);
                    }
                }

            }
        }
        UpdateInventoryColor();
    }

    private void UpdateInventoryColor()
    {
        print("Testing coooolors");
        for (int i = 0; i < allSlots.Count; i++)
        {
            if(allItems[i] != null)
            {
                if(allItems[i].itemRarity == Item.ItemRarity.common)
                {
                    allSlots[i].myOverlay.color = common;
                    allSlots[i].myOverlay.enabled = true;
                }
                else if (allItems[i].itemRarity == Item.ItemRarity.rare)
                {
                    allSlots[i].myOverlay.color = rare;
                    allSlots[i].myOverlay.enabled = true;
                }
                else if (allItems[i].itemRarity == Item.ItemRarity.epic)
                {
                    allSlots[i].myOverlay.color = epic;
                    allSlots[i].myOverlay.enabled = true;
                }
                else if (allItems[i].itemRarity == Item.ItemRarity.legendary)
                {
                    allSlots[i].myOverlay.color = Legendary;
                    allSlots[i].myOverlay.enabled = true;
                }
            }
            else
            {
                allSlots[i].myOverlay.enabled = false;
            }
        }
    }
}
