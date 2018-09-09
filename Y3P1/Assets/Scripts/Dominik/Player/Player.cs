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
        [HideInInspector] public Health health;
        [SerializeField] private GameObject playerUIPrefab;
        [HideInInspector] private TestShoot testShoot;


        private void Awake()
        {
            playerController = GetComponentInChildren<PlayerController>();
            rb = GetComponentInChildren<Rigidbody>();
            playerCam = GetComponentInChildren<PlayerCamera>();
            health = GetComponentInChildren<Health>();
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

            DontDestroyOnLoad(gameObject);
        }

        private void CreatePlayerUI()
        {
            PlayerUI playerUI = Instantiate(playerUIPrefab, transform.position, Quaternion.identity, transform).GetComponent<PlayerUI>();
            playerUI.Initialise(this);
        }
    }
}
