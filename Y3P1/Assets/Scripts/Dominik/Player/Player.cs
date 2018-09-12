using Photon.Pun;
using UnityEngine;

namespace Y3P1
{
    public class Player : MonoBehaviourPunCallbacks
    {

        public static GameObject localPlayerObject;
        public static Player localPlayer;

        private PlayerController playerController;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public PlayerCamera playerCam;
        [HideInInspector] public Entity entity;
        [SerializeField] private GameObject playerUIPrefab;
        [SerializeField] private Vector3 playerUISpawnOffset = new Vector3(0, 3, 0.2f);
        [HideInInspector] private TestShoot testShoot;

        private void Awake()
        {
            playerController = GetComponentInChildren<PlayerController>();
            rb = GetComponentInChildren<Rigidbody>();
            playerCam = GetComponentInChildren<PlayerCamera>();
            entity = GetComponentInChildren<Entity>();
            testShoot = GetComponentInChildren<TestShoot>();

            CreatePlayerUI();

            if (!photonView.IsMine)
            {
                playerCam.gameObject.SetActive(false);
                playerController.enabled = false;
                testShoot.enabled = false;

                Destroy(rb);
                foreach (Collider col in GetComponentsInChildren<Collider>())
                {
                    col.enabled = false;
                }
                return;
            }

            localPlayerObject = gameObject;
            localPlayer = this;

            playerCam.Initialize();
            playerController.Initialise();

            DontDestroyOnLoad(gameObject);
        }

        private void CreatePlayerUI()
        {
            PlayerUI playerUI = Instantiate(playerUIPrefab, transform.position + playerUISpawnOffset, Quaternion.identity, transform).GetComponent<PlayerUI>();
            playerUI.Initialise(this);
        }
    }
}
