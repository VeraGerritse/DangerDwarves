using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Y3P1;

public class ReviveZone : MonoBehaviourPunCallbacks, IPunObservable
{

    private bool checkForInput;
    private bool reviving;

    private Player player;
    private Collider reviveZoneCollider;

    [SerializeField] private GameObject reviveZoneObject;
    [SerializeField] private float reviveSpeed;
    [SerializeField] private Image progressImage;
    [SerializeField] private GameObject progressPanel;
    [SerializeField] private GameObject interactIndicator;

    private void Awake()
    {
        player = transform.root.GetComponent<Player>();
        reviveZoneCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (checkForInput && !photonView.IsMine)
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
        if (other.tag == "Player" && !photonView.IsMine)
        {
            checkForInput = true;
            interactIndicator.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !photonView.IsMine)
        {
            checkForInput = false;
            interactIndicator.SetActive(false);
            ToggleRevive(false);
        }
    }

    public void ToggleReviveZone(bool b)
    {
        reviveZoneObject.SetActive(b);
        reviveZoneCollider.enabled = b;
        interactIndicator.SetActive(false);
    }

    public void ToggleRevive(bool b)
    {
        if (!Player.localPlayer.entity.health.isDead)
        {
            reviving = b;
            progressPanel.SetActive(b);
        }
    }

    private void Revive()
    {
        reviving = false;
        photonView.RPC("SyncRevive", RpcTarget.All, player.photonView.ViewID);
    }

    [PunRPC]
    private void SyncRevive(int id)
    {
        ToggleReviveZone(false);

        if (Player.localPlayer.photonView.ViewID == id)
        {
            player.Respawn(false);
            NotificationManager.instance.NewNotification("<color=red>" + PhotonNetwork.NickName + "</color> has been revived!");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(reviveZoneObject.activeInHierarchy);
            stream.SendNext(reviveZoneCollider.enabled);
        }
        else
        {
            reviveZoneObject.SetActive((bool)stream.ReceiveNext());
            reviveZoneCollider.enabled = (bool)stream.ReceiveNext();
        }
    }
}