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
    private bool solo;

    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject playMenuPanel;
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private GameObject connectionProgress;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        SetUpNameInputField();
        playMenuPanel.SetActive(true);
        roomPanel.SetActive(false);
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

    public void ConnectSolo()
    {
        solo = true;
        Connect();
    }

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            if (solo)
            {
                PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 1 }, null);
            }
            else
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        roomPanel.SetActive(true);
        connectionProgress.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void PlayButton()
    {
        if (string.IsNullOrEmpty(nameInputField.text) || nameInputField.text.All(char.IsWhiteSpace))
        {
            return;
        }

        playMenuPanel.SetActive(false);
        roomPanel.SetActive(true);
    }

    private void GetAvailableRooms()
    {

    }

    public void JoinSpecificRoomButton(TextMeshProUGUI roomName)
    {
        SetUpPanelsWhenConnecting();
        PhotonNetwork.JoinRoom(roomName.text);
    }

    public void QuickJoinButton()
    {
        SetUpPanelsWhenConnecting();
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 10 }, null);
    }

    public void CreateRoomButton(TMP_InputField roomName)
    {
        SetUpPanelsWhenConnecting();
        PhotonNetwork.CreateRoom(!string.IsNullOrEmpty(roomName.text) ? roomName.text : null);
    }

    private void SetUpPanelsWhenConnecting()
    {
        roomPanel.SetActive(false);
        connectionProgress.SetActive(true);
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
