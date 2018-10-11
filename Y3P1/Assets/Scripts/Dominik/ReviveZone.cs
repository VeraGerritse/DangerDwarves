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

    public void ToggleReviveZone(bool b)
    {
        reviveZoneObject.SetActive(b);
        reviveZoneCollider.enabled = b;
    }

    public void ToggleRevive(bool b)
    {
        reviving = b;
        progressPanel.SetActive(b);
    }

    private void Revive()
    {
        reviving = false;
        player.Respawn(false);
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