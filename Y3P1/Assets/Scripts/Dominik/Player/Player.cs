using Photon.Pun;
using UnityEngine;

namespace Y3P1
{
    public class Player : MonoBehaviourPunCallbacks
    {

        public static GameObject localPlayerObject;
        public static Player localPlayer;

        [SerializeField] private GameObject playerUIPrefab;
        [SerializeField] private Vector3 playerUISpawnOffset = new Vector3(0, 3, 0.2f);

        #region Components
        private PlayerController playerController;
        [HideInInspector] public WeaponSlot weaponSlot;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public PlayerCamera playerCam;
        [HideInInspector] public Entity entity;
        [HideInInspector] public Inventory myInventory;
        #endregion

        private void Awake()
        {
            GatherPlayerComponents();
            Initialise();
        }

        private void Initialise()
        {
            if (!photonView.IsMine)
            {
                playerCam.gameObject.SetActive(false);
                playerController.enabled = false;
                weaponSlot.enabled = false;
                myInventory.gameObject.SetActive(false);
                Destroy(rb);

                foreach (Collider col in GetComponentsInChildren<Collider>())
                {
                    col.enabled = false;
                }

                CreatePlayerUI();
                return;
            }

            localPlayerObject = gameObject;
            localPlayer = this;

            playerCam.Initialize();
            playerController.Initialise();

            DontDestroyOnLoad(gameObject);
        }

        private void GatherPlayerComponents()
        {
            playerController = GetComponentInChildren<PlayerController>();
            rb = GetComponentInChildren<Rigidbody>();
            playerCam = GetComponentInChildren<PlayerCamera>();
            entity = GetComponentInChildren<Entity>();
            weaponSlot = GetComponentInChildren<WeaponSlot>();
            myInventory = GetComponentInChildren<Inventory>();
        }

        private void CreatePlayerUI()
        {
            PlayerUI playerUI = Instantiate(playerUIPrefab, transform.position + playerUISpawnOffset, Quaternion.identity, transform).GetComponent<PlayerUI>();
            playerUI.Initialise(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                localPlayer.entity.Hit(-10);
            }
        }
    }
}
