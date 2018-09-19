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
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject insItem = PhotonNetwork.InstantiateSceneObject(toDrop, Player.localPlayer.transform.position, Quaternion.identity);

            int id = insItem.GetComponent<PhotonView>().ViewID;
            photonView.RPC("RI", RpcTarget.AllBuffered, item, id);
        }
    }

    [PunRPC]
    private void RI(byte[] item, int id)
    {
            GameObject itemIG = PhotonNetwork.GetPhotonView(id).gameObject;
            //itemIG.GetComponent<WeaponPrefab>().Drop();
            NotificationManager.instance.NewNotification("testtesttesttesttesttesttesttesttesttesttest");
            RevertItem(item, itemIG);
    }

    private byte[] ConvertItemForSave(Item toSave)
    {
        byte[] saved = null;
        if(toSave.GetType() == typeof(Weapon_Ranged))
        {
            ItemSaveRanged savedItem = new ItemSaveRanged((Weapon_Ranged)toSave);
            saved = ObjectToByteArray(savedItem);
        }
        return saved;
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

    void RevertItem(byte[] toRevert,GameObject newObj)
    {
        Weapon_Ranged item = ScriptableObject.CreateInstance<Weapon_Ranged>();
        ItemSaveRanged iSR = (ItemSaveRanged)ByteArrayToObject(toRevert);

        item.itemName = iSR.name ;
        item.itemRarity = (Item.ItemRarity)iSR.rarity;
        //item.itemImage = iSR.image;

        item.myStats = ScriptableObject.CreateInstance<Stats>();
        item.myStats.stamina = iSR.stamina;
        item.myStats.strength = iSR.strenght;
        item.myStats.agility = iSR.agility;
        item.myStats.willpower = iSR.willpower;
        item.myStats.defense = iSR.defense;
        //item.itemPrefab = iSR.prefab;
        
        item.baseDamage = iSR.baseDamage;
        item.primaryFireRate = iSR.fireRate;
        
        item.secondaryProjectile = iSR.nameSec;
        item.secondaryFireRate = iSR.secFireRate;
        item.secondaryForce = iSR.secForce;
        item.secondaryAmountOfProjectiles = iSR.amountProj;
        item.secondaryConeOfFireInDegrees = iSR.coneDegrees;
        
        item.primaryProjectile = iSR.primaryName;
        item.force = iSR.priForce;
        item.amountOfProjectiles = iSR.priPro;
        item.coneOfFireInDegrees = iSR.priDeg;

        newObj.GetComponent<WeaponPrefab>().myItem = item;
    }

    void SaveItem(Item toSave,string objName)
    {
        byte[] saved = ConvertItemForSave(toSave);
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
            if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.weapon)
            {
                UnequipWeapon(lastSlotIndex);
            }
            GameObject newObj = allItems[lastSlotIndex].itemPrefab;

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
            allSlots[lastSlotIndex].SetImage(allItems[lastSlotIndex].itemImage);
            allSlots[currentSlotIndex].SetImage(allItems[currentSlotIndex].itemImage);
        }
        else
        {
            if (allSlots[lastSlotIndex].slotType == InventorySlot.SlotType.weapon)
            {
                UnequipWeapon(lastSlotIndex);
            }
            allItems[currentSlotIndex] = allItems[lastSlotIndex];
            allSlots[currentSlotIndex].EnableImage();
            allSlots[currentSlotIndex].SetImage(allItems[currentSlotIndex].itemImage);
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
        OpenCloseInv();
        StartCoroutine(AddStartingItems());
    }

    public void OpenCloseInv()
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

    IEnumerator AddStartingItems()
    {
        yield return new WaitForSeconds(0.2f);
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
        if (Input.GetButtonDown("Tab"))
        {
            OpenCloseInv();
        }
        if (Input.GetKeyDown(KeyCode.C) && LootRandomizer.instance != null)
        {
            for (int i = 0; i < allItems.Count; i++)
            {
                if (allItems[i] == null && allSlots[i].CheckSlotType())
                {
                    allItems[i] = LootRandomizer.instance.DropLoot();
                    allSlots[i].SetImage(allItems[i].itemImage);
                    allSlots[i].EnableImage();
                    break;
                }
            }
        }

        if (drag == null)
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
[System.Serializable]
public class ItemSaveRanged
{
    //item
    public string name;
    public int rarity;
    //public Sprite image;
    public int stamina;
    public int strenght;
    public int agility;
    public int willpower;
    public int defense;
   // public GameObject prefab;

    //weapon
    //primary attack
    public int baseDamage;
    public float fireRate;

    //secundary attack
    public string nameSec;
    public float secFireRate;
    public float secForce;
    public int amountProj;
    public int coneDegrees;

    //weapon_Ranged
    public string primaryName;
    public float priForce;
    public int priPro;
    public int priDeg;

    public ItemSaveRanged(Weapon_Ranged item)
    {
        name = item.itemName;
        rarity = (int)item.itemRarity;
       // image = item.itemImage;
        stamina = item.myStats.stamina;
        strenght = item.myStats.strength;
        agility = item.myStats.agility;
        willpower = item.myStats.willpower;
        defense = item.myStats.defense;
       // prefab = item.itemPrefab;

        baseDamage = item.baseDamage;
        fireRate = item.primaryFireRate;

        nameSec = item.secondaryProjectile;
        secFireRate = item.secondaryFireRate;
        secForce = item.secondaryForce;
        amountProj = item.secondaryAmountOfProjectiles;
        coneDegrees = item.secondaryConeOfFireInDegrees;

        primaryName = item.primaryProjectile;
        priForce = item.force;
        priPro = item.amountOfProjectiles;
        priDeg = item.coneOfFireInDegrees;
    }
}


