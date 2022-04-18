using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    #region Private Serialized Fields

    [Tooltip("The UI Panel to let the user enter player name and room name")]
    [SerializeField] private GameObject launcherPanel;
    
    [Tooltip("The Host UI Panel that displays players in current room.")]
    [SerializeField] private GameObject lobbyPanelHost;
    
    [Tooltip("The Host UI viewport that displays players in current room.")]
    [SerializeField] private Transform hostViewportContent;
    
    [Tooltip("The Client UI Panel that displays players in current room.")]
    [SerializeField] private GameObject lobbyPanelClient;
    
    [Tooltip("The Client UI viewport that displays players in current room.")]
    [SerializeField] private Transform clientViewportContent;

    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField] private GameObject connectingText;
    
    [SerializeField] private PlayerItem playerItem;

    #endregion


    #region Private Fields
    
    private string _gameVersion = "1";
    private string _roomName;
    
    private List<PlayerItem> _playerItemList = new List<PlayerItem>();

    #endregion


    #region MonoBehavior Callbacks
    
    private void Awake()
    {
        // all clients in the same room will automatically sync level
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        // sets the proper display panel/label
        launcherPanel.SetActive(true);
        lobbyPanelHost.SetActive(false);
        lobbyPanelClient.SetActive(false);
        connectingText.SetActive(false);
    }

    #endregion


    #region MonoBahaviorPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN");
        
        // join or create room
        PhotonNetwork.JoinOrCreateRoom(_roomName, new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() was called by PUN. Now this client is in a room.");
        
        // set proper UI panels
        launcherPanel.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        {
            lobbyPanelHost.SetActive(true);
            lobbyPanelClient.SetActive(false);
        }
        else
        {
            lobbyPanelHost.SetActive(false);
            lobbyPanelClient.SetActive(true);
        }
        connectingText.SetActive(false);
        
        // update player list
        UpdatePlayerList();
    }

    public override void OnJoinRoomFailed(short returnCode, string message )
    {
        Debug.Log("OnJoinRoomFailed() was called by PUN. So we create one.");

        // sets the proper display panel/label
        launcherPanel.SetActive(true);
        lobbyPanelHost.SetActive(false);
        lobbyPanelClient.SetActive(false);
        connectingText.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    #endregion


    #region Public Methods

    // start the connection process
    public void Connect()
    {
        // get the room name the player entered
        _roomName = launcherPanel.transform.GetChild(1).GetComponent<RoomNameInputField>().GetPlayerPrefRoomName();
        
        // sets the proper display panel/label
        launcherPanel.SetActive(false);
        lobbyPanelHost.SetActive(false);
        lobbyPanelClient.SetActive(false);
        connectingText.SetActive(true);

        // join room if connected to server, otherwise connect
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinOrCreateRoom(_roomName, new RoomOptions(), TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = _gameVersion;
        }
    }
    
    // starts the game
    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Unite 2015 Networked");
    }

    #endregion


    #region Private Methods

    private void UpdatePlayerList()
    {
        foreach(PlayerItem item in _playerItemList)
        {
            Destroy(item.gameObject);
        }
        _playerItemList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
            return;
        
        int i = 1;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayer;
            if (PhotonNetwork.IsMasterClient)
            {
                newPlayer = Instantiate(playerItem, hostViewportContent);
            }
            else
            {
                newPlayer = Instantiate(playerItem, clientViewportContent);
            }
            newPlayer.SetPlayerName(player.Value);
            _playerItemList.Add(newPlayer);
            i++;
        }
    }

    #endregion
}