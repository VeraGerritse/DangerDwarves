using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{

    private string gameVersion = "1.0.0";
    private static string playerNamePrefKey = "PlayerName";
    private bool isConnecting;

    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject playMenuPanel;
    [SerializeField] private GameObject connectionProgress;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        SetUpNameInputField();
        playMenuPanel.SetActive(true);
        connectionProgress.SetActive(false);
    }

    public void Connect()
    {
        // Player name is empty.
        if (string.IsNullOrEmpty(nameInputField.text) || nameInputField.text.All(char.IsWhiteSpace))
        {
            return;
        }

        isConnecting = true;

        playMenuPanel.SetActive(false);
        connectionProgress.SetActive(true);

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        playMenuPanel.SetActive(true);
        connectionProgress.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 10 }, null);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void SetUpNameInputField()
    {
        string defaultName = "";
        if (nameInputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                nameInputField.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;
    }

    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }
        PhotonNetwork.NickName = value;

        PlayerPrefs.SetString(playerNamePrefKey, value);
    }
}
