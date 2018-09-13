using Photon.Pun;
using UnityEngine;

public class WeaponSlot : MonoBehaviourPunCallbacks
{

    public static Weapon currentWeapon;
    private WeaponPrefab currentWeaponPrefab;

    private float nextPrimaryTime;
    private float nextSecondaryTime;

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
        if (Input.GetMouseButton(0))
        {
            if (Time.time >= nextPrimaryTime)
            {
                nextPrimaryTime = Time.time + currentWeapon.primaryFireRate;
                UsePrimary();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (Time.time >= nextSecondaryTime)
            {
                nextSecondaryTime = Time.time + currentWeapon.secondaryFireRate;
                UseSecondary();
            }
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

    private void UsePrimary()
    {
        if (currentWeapon)
        {
            currentWeaponPrefab.UsePrimary();
        }
    }

    private void UseSecondary()
    {
        if (currentWeapon)
        {
            currentWeaponPrefab.UseSecondary();
        }
    }
}
