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
    private bool isInitialised;
    private Canvas canvas;

    private Stats currentStats;
    private int averageILevel = 1;

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
        CalculateArmor();
    }

    IEnumerator Time()
    {
        yield return new WaitForSeconds(0.1f);
        CalculateArmor();
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

    public void AddItem(Item toAdd)
    {
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
        if (Input.GetKeyDown(KeyCode.L))
        {
            RemoveAllItems();
        }

        if(currentSlot != null)
        {
            int index = -1;
            for (int i = 0; i < allSlots.Count; i++)
            {
                if (currentSlot == allSlots[i])
                {
                    index = i;
                }
            }
            if(allItems[index] != null)
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
                if (Player.localPlayer.dwarfAnimController.CanEquipRanged())
                {
                    int index = 0;
                    for (int i = 0; i < allSlots.Count; i++)
                    {
                        if (currentSlot == allSlots[i])
                        {
                            index = i;
                        }
                    }


                    if (allItems[index] != null)
                    {
                        bool unEquip = false;
                        if (allSlots[index].slotType != InventorySlot.SlotType.all)
                        {
                            unEquip = true;
                        }



                        int newSlot = 0;
                        int type = 0;
                        if (allItems[index] is Weapon)
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
                        else if (allItems[index] is Helmet)
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
                        else if (allItems[index] is Trinket)
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
                                    AddItem(allItems[index]);
                                    RemoveItem(index);
                                }
                            }
                            else
                            {
                                Item temp = allItems[index];
                                allItems[index] = allItems[newSlot];
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
                            if (allItems[index] != null)
                            {
                                allSlots[index].EnableImage();
                                allSlots[index].SetImage(Database.hostInstance.allSprites[allItems[index].spriteIndex]);
                            }
                            else
                            {
                                allSlots[index].DisableImage();
                            }
                            drag = null;
                        }
                    }
                }
                CalculateArmor();
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
        }

        if (Input.GetButtonDown("Tab"))
        {
            OpenCloseInv();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            if(LootRandomizer.instance != null)
            {
                Item newItem = LootRandomizer.instance.DropLoot(averageILevel);
                if (newItem == null)
                {
                    return;
                }
                AddItem(newItem);
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

        int iLevel = 0;
        

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
        averageILevel = iLevel / 3;
        SetInfo();
    }


    public string[] Stats()
    {
        return new string[] { "Stamina: <color=#00A8FF>" + currentStats.stamina.ToString(), "Strenght: <color=#00A8FF>" + currentStats.strength.ToString(), "Agility: <color=#00A8FF>" + currentStats.agility.ToString(), "Willpower: <color=#00A8FF>" + currentStats.willpower.ToString(), "Defence: <color=#00A8FF>" + currentStats.defense.ToString() };
    }

    public string[] ILevel()
    {
        return new string[] { "Item Level: <color=#00A8FF>" + averageILevel.ToString() };
    }

    public void SetInfo()
    {
        StatsInfo.instance.SetPlayerStats(Stats(), ILevel());
    }

    public bool InventoryIsOpen()
    {
        return canvas.enabled;
    }

    public void DropNewItem(Vector3 loc)
    {
        Item newItem = LootRandomizer.instance.DropLoot(averageILevel);
        if(newItem == null)
        {
            return;
        }
        GameObject newObj = Database.hostInstance.allGameobjects[newItem.prefabIndex];
        SaveItem(newItem, newObj.name,loc);
    }
}
