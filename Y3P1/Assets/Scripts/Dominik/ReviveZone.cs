using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Y3P1;

public class ReviveZone : MonoBehaviourPunCallbacks, IPunObservable
{

    private bool checkForInput;
    private bool reviving;

    private Player player;
    private int playerID;
    private Collider reviveCollider;

    [SerializeField] private float reviveSpeed;
    [SerializeField] private Image progressImage;
    [SerializeField] private GameObject progressPanel;
    [SerializeField] private GameObject interactIndicator;

    private void Awake()
    {
        player = transform.root.GetComponent<Player>();
        playerID = player.photonView.ViewID;
        reviveCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (checkForInput)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleRevive(true);
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                ToggleRevive(false);
            }
        }

        if (reviving)
        {
            progressImage.fillAmount += Time.deltaTime * reviveSpeed;

            if (progressImage.fillAmount == 1)
            {
                Revive();
            }
        }
        else
        {
            progressImage.fillAmount = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.root.GetComponent<Player>();
            if (player != this.player)
            {
                checkForInput = true;
                interactIndicator.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.root.GetComponent<Player>();
            if (player != this.player)
            {
                checkForInput = false;
                interactIndicator.SetActive(false);
            }
        }
    }

    public void ToggleRevive(bool b)
    {
        reviving = b;
        progressPanel.SetActive(b);
    }

    public void SetReviveCollider(bool b)
    {
        reviveCollider.enabled = b;
    }

    private void Revive()
    {
        reviving = false;
        photonView.RPC("SyncRevive", RpcTarget.All, playerID);
    }

    [PunRPC]
    private void SyncRevive(int toReviveID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView toRevive = PhotonView.Find(toReviveID);
            toRevive.GetComponent<Player>().Revive(false);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(reviveCollider.enabled);
        }
        else
        {
            reviveCollider.enabled = (bool)stream.ReceiveNext();
        }
    }
}
