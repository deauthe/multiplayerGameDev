using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;
    public GameObject loadingScreen;
    public GameObject menuButtons;
    public TMP_Text loadingScreenText;
    public TMP_InputField createRoomInput;
    public GameObject createRoomScreen;
    public GameObject roomScreen;
    public TMP_Text roomNameText;
    public GameObject errorCreateRoomScreen;
    public TMP_Text errorText;
    public GameObject roomBrowserScreen;
    public RoomButton theRoomButton;
    private List<RoomButton> allRoomButtons = new List<RoomButton>();
    public TMP_Text PlayerName;
    private List<TMP_Text> allPlayerNames = new List<TMP_Text>();
    public GameObject nameInputScreen;
    public TMP_InputField nameInput;
    private bool hasSetNick;
    public string levelToPlay;
    public GameObject startButton;
    public GameObject testButton;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        CloseMenus();
        loadingScreen.SetActive(true);
        loadingScreenText.text = "connecting to Network...";
        PhotonNetwork.ConnectUsingSettings();
#if UNITY_EDITOR
        testButton.SetActive(true);
#endif

    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        loadingScreenText.text = "joining lobby";
        PhotonNetwork.AutomaticallySyncScene = true;


    }
    public override void OnJoinedLobby()
    {
        CloseMenus();
        menuButtons.SetActive(true);
        if (!hasSetNick)
        {
            CloseMenus();
            nameInputScreen.SetActive(true);
        }
    }

    void Update()
    {
        
    }

    public void CloseMenus()
    {
        loadingScreen.SetActive(false);
        menuButtons.SetActive(false);
        createRoomScreen.SetActive(false);
        roomScreen.SetActive(false);
        errorCreateRoomScreen.SetActive(false);
        roomBrowserScreen.SetActive(false);
        nameInputScreen.SetActive(false);
        
    }
    public void OpenCreateRoom()
    {
        CloseMenus();
        createRoomScreen.SetActive(true);
    }
    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(createRoomInput.text))
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 8;

            PhotonNetwork.CreateRoom(createRoomInput.text, options);

            CloseMenus();
            loadingScreenText.text = "creating room";
            loadingScreen.SetActive(true);
        }
    }
    public override void OnJoinedRoom()
    {
        CloseMenus();
        roomScreen.SetActive(true);
        
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        PhotonNetwork.NickName = "Player " + Random.Range(0, 8).ToString();


        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }
    public void ListAllPlayers()
    {
        foreach(TMP_Text player in allPlayerNames)
        {
            Destroy(player.gameObject);
        }
        allPlayerNames.Clear();
        Player[] players = PhotonNetwork.PlayerList;
        for(int i = 0; i < players.Length; i++)
        {
            TMP_Text newPlayerLabel = Instantiate(PlayerName, PlayerName.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true);
            allPlayerNames.Add(newPlayerLabel);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer) //copied all the below lines from above
    {
        TMP_Text newPlayerLabel = Instantiate(PlayerName, PlayerName.transform.parent);
        newPlayerLabel.text = newPlayer.NickName;
        newPlayerLabel.gameObject.SetActive(true);
        allPlayerNames.Add(newPlayerLabel);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayers();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CloseMenus();
        errorCreateRoomScreen.SetActive(true);
        errorText.text = "Failed to create room: " + message + "  error code:" + returnCode;
        
    }
    public void BackToMenu()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenus();
        loadingScreenText.text = "leaving room";
        loadingScreen.SetActive(true);

    }
    public override void OnLeftRoom()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }
    public void OpenRoomBrowser()
    {
        CloseMenus();
        roomBrowserScreen.SetActive(true);

    }
    public void CloseRoomBrowser()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("new room");
        foreach(RoomButton rb in allRoomButtons)
        {
            Destroy(rb.gameObject);
        }
        allRoomButtons.Clear();
        theRoomButton.gameObject.SetActive(false);

        for(int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList)
            {
                RoomButton newButton = Instantiate(theRoomButton, theRoomButton.transform.parent);
                newButton.setButtonDetails(roomList[i]);
                newButton.gameObject.SetActive(true);
                allRoomButtons.Add(newButton);
                
            }
        }
        
    }
    public void JoinRoomMethod(RoomInfo inputInfo) //this fx is being called in the roomButton script with the required argument.
    {
        PhotonNetwork.JoinRoom(inputInfo.Name);
        loadingScreen.SetActive(true);
        loadingScreenText.text = "joining room";

    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void startGame()
    {
        PhotonNetwork.LoadLevel(levelToPlay);
    }
    public void QuickPlay()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;
        PhotonNetwork.CreateRoom("test",options);
        CloseMenus();
        loadingScreen.SetActive(true);
        loadingScreenText.text = "creating room";

    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (roomScreen.activeInHierarchy == true)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                startButton.SetActive(true);

            }
            else
            {
                startButton.SetActive(false);
            }
        }
        
    }
    public void setNickName()
    {
        if (!string.IsNullOrEmpty(nameInput.text))
        {
            PhotonNetwork.NickName = nameInput.text;
            hasSetNick = true;
            CloseMenus();
            menuButtons.SetActive(true);
        }
    }



}
