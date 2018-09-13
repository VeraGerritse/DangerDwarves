using Photon.Pun;
using UnityEngine;
using Y3P1;

public class TestWeaponSlot : MonoBehaviourPunCallbacks
{

    public static Weapon currentWeapon;
    private WeaponPrefab currentWeaponPrefab;

    public Weapon defaultWeapon;

    private void Awake()
    {
        if (defaultWeapon)
        {
            SetWeapon(defaultWeapon);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UsePrimary();
        }

        if (Input.GetMouseButtonDown(1))
        {
            UseSecondary();
        }
    }

    public void SetWeapon(Weapon weapon)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (currentWeaponPrefab)
        {
            PhotonNetwork.Destroy(currentWeaponPrefab.gameObject);
        }

        currentWeapon = weapon;
        currentWeaponPrefab = PhotonNetwork.Instantiate(currentWeapon.itemPrefab.name, Player.localPlayer.testWeaponSpawn.position, Player.localPlayer.testWeaponSpawn.rotation).GetComponent<WeaponPrefab>();
        currentWeaponPrefab.transform.SetParent(Player.localPlayer.testWeaponSpawn);
    }

    public void UsePrimary()
    {
        if (currentWeapon)
        {
            currentWeaponPrefab.UsePrimary();
            //photonView.RPC("UsePrimaryRPC", RpcTarget.All);
        }
    }

    public void UseSecondary()
    {
        if (currentWeapon)
        {
            currentWeaponPrefab.UseSecondary();
            //photonView.RPC("UseSecondaryRPC", RpcTarget.All);
        }
    }

    //[PunRPC]
    //private void UsePrimaryRPC()
    //{
    //    currentWeaponPrefab.UsePrimary();
    //}

    //[PunRPC]
    //private void UseSecondaryRPC()
    //{
    //    currentWeaponPrefab.UseSecondary();
    //}
}
