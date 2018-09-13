using Photon.Pun;
using UnityEngine;
using Y3P1;

public class WeaponSlot : MonoBehaviourPunCallbacks
{

    public static Weapon currentWeapon;
    private WeaponPrefab currentWeaponPrefab;

    [SerializeField] private Transform weaponSpawn;
    [SerializeField] private Weapon defaultWeapon;

    [SerializeField] private Weapon testWeaponSwitch;

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

        if (Input.GetButtonDown("Jump"))
        {
            SetWeapon(currentWeapon == defaultWeapon ? testWeaponSwitch : defaultWeapon);
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
        currentWeaponPrefab = PhotonNetwork.Instantiate(currentWeapon.itemPrefab.name, weaponSpawn.position, weaponSpawn.rotation).GetComponent<WeaponPrefab>();
        currentWeaponPrefab.transform.SetParent(weaponSpawn);
    }

    public void UsePrimary()
    {
        if (currentWeapon)
        {
            currentWeaponPrefab.UsePrimary();
        }
    }

    public void UseSecondary()
    {
        if (currentWeapon)
        {
            currentWeaponPrefab.UseSecondary();
        }
    }
}
