using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace Y3P1
{
    public class Player : MonoBehaviourPunCallbacks
    {

        public static GameObject localPlayerObject;
        public static Player localPlayer;

        [Header("Appearance")]

        [SerializeField] private List<GameObject> beardObjects = new List<GameObject>();
        [SerializeField] private List<Material> bodyMaterials = new List<Material>();
        [SerializeField] private List<Material> beardMaterials = new List<Material>();
        private SkinnedMeshRenderer dwarfRenderer;

        [Header("UI")]

        [SerializeField] private GameObject playerUIPrefab;
        [SerializeField] private Vector3 playerUISpawnOffset = new Vector3(0, 3, 0.2f);
        [SerializeField] private GameObject playerUICam;

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
            dwarfRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
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
            Destroy(IsConnectedAndMine() ? null : rb);

            if (!IsConnectedAndMine())
            {
                foreach (Collider col in GetComponentsInChildren<Collider>())
                {
                    col.enabled = false;
                }

                if (PhotonNetwork.IsConnected)
                {
                    CreatePlayerUI();
                }
            }
            else
            {
                RandomizeAppearance();
                playerController.OnDodge += PlayerController_OnDodge;
                DontDestroyOnLoad(gameObject);
            }
        }

        //private bool iKControlBeforeDodge;
        private void PlayerController_OnDodge(bool dodgeStart)
        {
            rangedWeaponLookAt.enabled = !dodgeStart;

            //if (dodgeStart)
            //{
            //    iKControlBeforeDodge = iKControl.enabled;

            //    if (iKControlBeforeDodge)
            //    {
            //        iKControl.enabled = false;
            //    }
            //}
            //else
            //{
            //    if (iKControlBeforeDodge)
            //    {
            //        iKControl.enabled = true;
            //    }
            //}
        }

        private void CreatePlayerUI()
        {
            PlayerUI playerUI = Instantiate(playerUIPrefab, transform.position + playerUISpawnOffset, Quaternion.identity, transform).GetComponent<PlayerUI>();
            playerUI.Initialise(this);
        }

        public bool IsConnectedAndMine()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "WotrScene")
            {
                return true;
            }
            return PhotonNetwork.IsConnected && photonView.IsMine ? true : false;
        }

        private void RandomizeAppearance()
        {
            int randomBeardModel = Random.Range(0, beardObjects.Count);
            int randomBeardMat = Random.Range(0, beardMaterials.Count);
            int randomDwarfMat = Random.Range(0, bodyMaterials.Count);

            photonView.RPC("SetAppearance", RpcTarget.AllBuffered, randomBeardModel, randomBeardMat, randomDwarfMat);
        }

        [PunRPC]
        private void SetAppearance(int beardModel, int beardMat, int bodyMat)
        {
            for (int i = 0; i < beardObjects.Count; i++)
            {
                if (i == beardModel)
                {
                    beardObjects[i].SetActive(true);
                    beardObjects[i].GetComponent<Renderer>().material = beardMaterials[beardMat];
                }
                else
                {
                    beardObjects[i].SetActive(false);
                }
            }

            dwarfRenderer.material = bodyMaterials[bodyMat];
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (photonView.IsMine)
                {
                    Player.localPlayer.entity.Hit(-10);
                }
            }
        }

        public override void OnDisable()
        {
            playerController.OnDodge -= PlayerController_OnDodge;
        }
    }
}
