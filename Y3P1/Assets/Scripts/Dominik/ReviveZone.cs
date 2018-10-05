using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Y3P1;

public class ReviveZone : MonoBehaviourPunCallbacks, IPunObservable
{

    private Player player;
    private bool reviving;
    [SerializeField] private float reviveSpeed;
    [SerializeField] private Image progressImage;
    [SerializeField] private GameObject progressPanel;

    private void Awake()
    {
        player = transform.root.GetComponent<Player>();
    }

    private void Update()
    {
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

    public void ToggleRevive(bool b)
    {
        reviving = b;
        progressPanel.SetActive(b);
    }

    private void Revive()
    {
        reviving = false;
        player.Revive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gameObject.activeInHierarchy);
        }
        else
        {
            gameObject.SetActive((bool)stream.ReceiveNext());
        }
    }
}
