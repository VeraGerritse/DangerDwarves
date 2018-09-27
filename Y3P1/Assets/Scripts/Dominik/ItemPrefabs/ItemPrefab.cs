using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class ItemPrefab : MonoBehaviourPunCallbacks, IPunObservable
{

    protected Rigidbody rb;
    protected bool isDropped;
    protected DroppedItemLabel droppedItemLabel;

    public Item myItem;

    [SerializeField] protected GameObject interactCollider;
    [SerializeField] protected Collider objectCollider;
    [SerializeField] protected List<ParticleSystem> weaponRarityParticles = new List<ParticleSystem>();
    [SerializeField] protected Vector3 dropRotationAdjustment;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SetRarityParticleColors();
    }

    public virtual void Drop()
    {
        isDropped = true;

        interactCollider.SetActive(true);
        objectCollider.enabled = true;

        transform.eulerAngles += dropRotationAdjustment;

        SpawnDroppedItemLabel();
    }

    private void SpawnDroppedItemLabel()
    {
        droppedItemLabel = ObjectPooler.instance.GrabFromPool("DroppedItemLabel", transform.position + Vector3.up * 0.5f, Quaternion.identity).GetComponent<DroppedItemLabel>();
        droppedItemLabel.SetText(myItem.itemName, myItem.itemRarity);
    }

    public void PickUp()
    {
        isDropped = false;

        interactCollider.SetActive(false);
        objectCollider.enabled = false;

        Player.localPlayer.myInventory.AddItem(myItem);
        photonView.RPC("PickUpDestroy", RpcTarget.All);
    }

    [PunRPC]
    private void PickUpDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }

        droppedItemLabel.anim.SetTrigger("Pickup");
    }

    private void SetRarityParticleColors()
    {
        if (WeaponSlot.currentWeapon == null)
        {
            return;
        }

        Color rarityColor = new Color();
        switch (WeaponSlot.currentWeapon.itemRarity)
        {
            case Item.ItemRarity.common:

                rarityColor = Color.white;
                break;
            case Item.ItemRarity.rare:

                rarityColor = new Color(0, 0.835f, 1, 1);
                break;
            case Item.ItemRarity.epic:

                rarityColor = Color.magenta;
                break;
            case Item.ItemRarity.legendary:

                rarityColor = Color.yellow;
                break;
        }

        for (int i = 0; i < weaponRarityParticles.Count; i++)
        {
            ParticleSystem.MainModule psMainModule = weaponRarityParticles[i].main;
            psMainModule.startColor = rarityColor;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isDropped);
            stream.SendNext(interactCollider.activeInHierarchy);
            stream.SendNext(objectCollider.enabled);
        }
        else
        {
            isDropped = (bool)stream.ReceiveNext();
            interactCollider.SetActive((bool)stream.ReceiveNext());
            objectCollider.enabled = (bool)stream.ReceiveNext();
        }
    }
}
