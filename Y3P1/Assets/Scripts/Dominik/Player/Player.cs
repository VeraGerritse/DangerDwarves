using Photon.Pun;
using UnityEngine;

namespace Y3P1
{
    public class Player : MonoBehaviourPunCallbacks
    {

        public static GameObject localPlayerObject;
        public static Player localPlayer;

        [Header("UI")]

        [SerializeField] private GameObject playerUIPrefab;
        [SerializeField] private Vector3 playerUISpawnOffset = new Vector3(0, 3, 0.2f);
        [SerializeField] private GameObject playerUICam;
        [SerializeField] private GameObject characterCam;
        [SerializeField] private GameObject deathCanvas;

        #region Components
        [HideInInspector] public WeaponChargeCanvas weaponChargeCanvas;
        [HideInInspector] public AnimationEventsDwarf dwarfAnimEvents;
        [HideInInspector] public HeadTracking rangedWeaponLookAt;
        [HideInInspector] public PlayerController playerController;
        [HideInInspector] public WeaponSlot weaponSlot;
        [HideInInspector] public HelmetSlot helmetSlot;
        [HideInInspector] public TrinketSlot trinketSlot;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public PlayerCamera playerCam;
        [HideInInspector] public Entity entity;
        [HideInInspector] public Inventory myInventory;
        [HideInInspector] public DwarfAnimationsScript dwarfAnimController;
        [HideInInspector] public IKControl iKControl;
        [HideInInspector] public PlayerAppearance playerAppearance;
        #endregion

        private void Awake()
        {
            if (photonView.IsMine || !PhotonNetwork.IsConnected)
            {
                localPlayerObject = gameObject;
                localPlayer = this;
            }

            GatherPlayerComponents();
            Initialise();
        }

        private void GatherPlayerComponents()
        {
            playerController = GetComponentInChildren<PlayerController>();
            weaponChargeCanvas = GetComponentInChildren<WeaponChargeCanvas>();
            rb = GetComponentInChildren<Rigidbody>();
            playerCam = GetComponentInChildren<PlayerCamera>();
            entity = GetComponentInChildren<Entity>();
            weaponSlot = GetComponentInChildren<WeaponSlot>();
            helmetSlot = GetComponentInChildren<HelmetSlot>();
            trinketSlot = GetComponentInChildren<TrinketSlot>();
            myInventory = GetComponentInChildren<Inventory>();
            dwarfAnimController = GetComponentInChildren<DwarfAnimationsScript>();
            dwarfAnimEvents = GetComponentInChildren<AnimationEventsDwarf>();
            iKControl = GetComponentInChildren<IKControl>();
            playerAppearance = GetComponentInChildren<PlayerAppearance>();
        }

        private void Initialise()
        {
            playerCam.Initialise(IsConnectedAndMine());
            playerController.Initialise(IsConnectedAndMine());
            weaponChargeCanvas.Initialise(IsConnectedAndMine());
            dwarfAnimEvents.Initialise(IsConnectedAndMine());
            dwarfAnimController.Initialise(IsConnectedAndMine());
            rangedWeaponLookAt.Initialise(IsConnectedAndMine());
            myInventory.Initialise(IsConnectedAndMine());

            weaponSlot.Initialise(IsConnectedAndMine());
            helmetSlot.Initialise(IsConnectedAndMine());
            trinketSlot.Initialise(IsConnectedAndMine());

            playerUICam.SetActive(IsConnectedAndMine() ? true : false);
            characterCam.SetActive(IsConnectedAndMine() ? true : false);
            Destroy(IsConnectedAndMine() ? null : rb);

            if (PhotonNetwork.IsConnected)
            {
                CreatePlayerUI();
            }

            if (!IsConnectedAndMine())
            {
                SetLayer(transform, 14);

                foreach (Collider col in GetComponentsInChildren<Collider>())
                {
                    if (!col.GetComponent<Entity>())
                    {
                        col.enabled = false;
                    }
                }
            }
            else
            {
                playerAppearance.RandomizeAppearance();
                playerController.OnDodge += PlayerController_OnDodge;
                entity.OnDeath += () => Entity_OnDeath();
                DontDestroyOnLoad(gameObject);
            }
        }

        private void CreatePlayerUI()
        {
            PlayerUI playerUI = Instantiate(playerUIPrefab, transform.position + playerUISpawnOffset, Quaternion.identity, transform).GetComponent<PlayerUI>();
            playerUI.Initialise(this, IsConnectedAndMine());
        }

        private void PlayerController_OnDodge(bool dodgeStart)
        {
            rangedWeaponLookAt.enabled = !dodgeStart;
            entity.health.isInvinsible = dodgeStart;
        }

        private void Entity_OnDeath()
        {
            deathCanvas.SetActive(true);
            NotificationManager.instance.NewNotification("<color=red>" + PhotonNetwork.NickName + "</color> has been downed!");
        }

        public void Respawn(bool hub)
        {
            deathCanvas.SetActive(false);
            localPlayer.entity.Revive();

            if (hub)
            {
                transform.position = Vector3.up * 0.1f;
            }
        }

        public bool IsConnectedAndMine()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "WotrScene")
            {
                return true;
            }
            return PhotonNetwork.IsConnected && photonView.IsMine ? true : false;
        }

        public void SetLayer(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            foreach (Transform child in root)
            {
                SetLayer(child, layer);
            }
        }

        public override void OnDisable()
        {
            playerController.OnDodge -= PlayerController_OnDodge;
        }
    }
}
