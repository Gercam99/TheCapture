using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using  TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;
using CTF.Lobby;
using CTF.Managers.Animations;
using CTF.PlayerController;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun.UtilityScripts;

namespace CTF.Managers
{
    public class MainMenuManager : MonoBehaviourPunCallbacks
    {
        public static MainMenuManager Instance;


        [Header("PANELS")] 
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject selectionPanel;
        [SerializeField] private GameObject createRoomPanel;
        [SerializeField] private GameObject loadingRoomPanel;
        [SerializeField] private GameObject roomListPanel;
        [SerializeField] private GameObject insideRoomPanel;
        [SerializeField] private GameObject networkErrorPanel;
        public GameObject mainCanvas;


        [Header("LOG IN")] [SerializeField] private TMP_InputField playerNameInput;
        [SerializeField] private Button connectBtn;

        [Header("SELECTION")] [SerializeField] private Button createRoomBtn;
        [SerializeField] private Button joinRandomRoomBtn;
        [SerializeField] private Button showListRoomsBtn;
        [SerializeField] private Button exitBtn;

        [Header("LOADING PANEL")] 
        [SerializeField] private TextMeshProUGUI textMessage;
        
        [Header("CREATE ROOM PANEL")] 
        [SerializeField] private Button backBtn;
        [SerializeField] private Button createNewRoomBtn;
        [SerializeField] private TMP_InputField roomNameInputField;
        [SerializeField] private string maxPlayersTypeGame;

        [Header("ROOM LIST PANEL")] [SerializeField]
        private GameObject roomListContent;

        [SerializeField] private Button backListBtn;
        [SerializeField] private GameObject roomListEntryPrefab;

        [Header("INSIDE ROOM PANEL")] [SerializeField]
        private Button startGameBtn;
        [SerializeField] private Button leaveGameBtn;
        [SerializeField] private Button changeTypeGameBtn;
        public Transform containerBlueTeam;
        public Transform containerRedTeam;
        [SerializeField] private TypeGameManager typeGameManager;
        [SerializeField] private TextMeshProUGUI nameRoomText;
        [SerializeField] private TextMeshProUGUI playersInRoomText;
        [SerializeField] private GameObject playerListEntryPrefab;

        [Header("NETWORK ERROR")] [SerializeField]
        private TextMeshProUGUI errorText;

        [SerializeField] private Button reconnectBtn;

        // DICTIONARIES
        private Dictionary<string, RoomInfo> cachedRoomInfos;
        private Dictionary<string, GameObject> roomListEntries;
        private Dictionary<int, GameObject> playerListEntries;

        public string playerName;

        private AnimationMainMenuManager animationMainMenuManager;

        [SerializeField,Range(1,20)] private byte limitToPlay = 1;



        #region Setup

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            PhotonNetwork.AutomaticallySyncScene = true;

            //Activamos el panel de inicio
            SetActivePanel(loginPanel.name);
            SetupButtons();

            // Dictionaries creation
            cachedRoomInfos = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, GameObject>();
            playerListEntries = new Dictionary<int, GameObject>();

            // Get saved playerName
            playerNameInput.text = PlayerPrefs.GetString("playerName");
        }

        private void Start()
        {
            // Events to call when player join and left a team, only if player is in room
            PhotonTeamsManager.PlayerJoinedTeam += OnJoinTeam;
            PhotonTeamsManager.PlayerLeftTeam += LeftTeam;

            // Declare animationMMM with singleton
            animationMainMenuManager = AnimationMainMenuManager.Instance;
            
        }

        public void RestartPlayer()
        {
            PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
            PhotonNetwork.LocalPlayer.CustomProperties.Clear();
            
            cachedRoomInfos = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, GameObject>();
            playerListEntries = new Dictionary<int, GameObject>();
        }
        
        /// <summary>
        /// Funcion encargada de asignar todos los listeners a los botones
        /// </summary>
        private void SetupButtons()
        {
            // LOGIN BTN
            connectBtn.onClick.AddListener(OnLoginButtonClicked);

            // SELECTION BTN
            createRoomBtn.onClick.AddListener(OnCreateRoomButtonClicked);
            joinRandomRoomBtn.onClick.AddListener(OnJoinRandomRoomButtonClicked);
            showListRoomsBtn.onClick.AddListener(OnRoomListButtonClicked);
            exitBtn.onClick.AddListener(OnExitToServer);

            // CREATE ROOM BTN
            backBtn.onClick.AddListener(OnBackButtonClicked);
            createNewRoomBtn.onClick.AddListener(OnCreateNewRoomClicked);

            // List Rooms BTN
            backListBtn.onClick.AddListener(OnBackButtonClicked);

            // INSIDE BTN
            leaveGameBtn.onClick.AddListener(OnLeaveGameButtonClicked);
            startGameBtn.onClick.AddListener(OnStartGameButtonClicked);
            changeTypeGameBtn.onClick.AddListener(() => { typeGameManager.panelTypeGame.SetActive(true); });

            // NETWORK ERROR
            reconnectBtn.onClick.AddListener(OnReconnectToServerButtonClicked);

        }

        #endregion

        #region PhotonEvents
        
        public override void OnConnectedToMaster()
        {
            Debug.LogError(PhotonNetwork.CloudRegion);
            SetActivePanel(selectionPanel.name);
        }
        

        #region ROOM FUNCTIONS

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            // Clear
            ClearRoomListView();

            // Update
            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }

        public override void OnJoinedRoom()
        {
            // Clear
            cachedRoomInfos.Clear();

            // Setup Initials
            SetActivePanel(insideRoomPanel.name);
            changeTypeGameBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient); // Change game if u are master client of the room
            animationMainMenuManager.InsideRoomAnimation(); // Play animation of canvas
            nameRoomText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
            
            // Initialize chat 
            ChatManager.Instance.actualChannelName = PhotonNetwork.CurrentRoom.Name; // Set chanel for chat
            ChatManager.Instance.Connection(); // Start chat connection

            playerListEntries ??= new Dictionary<int, GameObject>(); // If list it's null create one

            
            // Setup list of players VISUALIZE
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                // First we need to compare if p is not your player
                if (p != PhotonNetwork.LocalPlayer)
                {

                    GameObject entry = Instantiate(playerListEntryPrefab); // Instantiate prefab
                    
                    // COMPARE TEAM of P. Depend the team of p, the parent of instantiate go to correspond container
                    if (p.GetPhotonTeam().Name == "Red")
                    {
                        entry.transform.SetParent(containerRedTeam);
                        entry.transform.localScale = Vector3.one;
                    }
                    else
                    {
                        entry.transform.SetParent(containerBlueTeam);
                        entry.transform.localScale = Vector3.one;
                    }
                    
                    entry.transform.localScale = Vector3.one; // Reset scale for to prevent bugs 
                    entry.GetComponent<PlayerListCTF>().Initialize(p.ActorNumber, p.NickName); // Get this comp, and assign corresponding properties

                    // SETUP CUSTOM PROPERTIES. This step is necessary for all other player get this properties
                    object isPlayerReady;

                    if (p.CustomProperties.TryGetValue(PlayerCTF.PLAYER_READY, out isPlayerReady))
                    {
                        entry.GetComponent<PlayerListCTF>().SetPlayerReady((bool) isPlayerReady); // Cal function to assign this properties
                    }

                    // Add this new instance of player to dictionary
                    playerListEntries.Add(p.ActorNumber, entry);

                    startGameBtn.gameObject.SetActive(CheckPlayersReady()); // Start gameButton setup

                    continue; // Break at this line and next player on list
                }
                
                // P join to a team depend of members on teams
                p.JoinTeam(PhotonTeamsManager.Instance.GetTeamMembersCount("Blue") >
                           PhotonTeamsManager.Instance.GetTeamMembersCount("Red")
                    ? "Red"
                    : "Blue");
            }
        }

        public override void OnLeftRoom()
        {
            // Destroy objects and clear dictionary because you left the room
            foreach (GameObject entry in playerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            SetActivePanel(selectionPanel.name);

            playerListEntries.Clear();
            playerListEntries = null;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            #region OLD

            // GameObject entry = Instantiate(playerListEntryPrefab);
            // if (newPlayer.GetPhotonTeam().Name == "Red")
            // {
            //     entry.transform.SetParent(containerRedTeam);
            //     entry.transform.localScale = Vector3.one;
            // }
            // else
            // {
            //     entry.transform.SetParent(containerBlueTeam);
            //     entry.transform.localScale = Vector3.one;
            // }
            //
            // entry.transform.localScale = Vector3.one;
            // entry.GetComponent<PlayerListCTF>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);
            //
            // playerListEntries.Add(newPlayer.ActorNumber, entry);
            //

            #endregion

            startGameBtn.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // Find in dictionary the correct player left the room and destroy.
            GameObject value = gameObject;
            if (playerListEntries.TryGetValue(otherPlayer.ActorNumber, out value))
            {
                Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            }
            
            playerListEntries.Remove(otherPlayer.ActorNumber);

            startGameBtn.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                startGameBtn.gameObject.SetActive(CheckPlayersReady());
            }
        }
        
        #endregion
        

        #region LOBBY FUNCTIONS

        public override void OnJoinedLobby()
        {
            // Clear dictionary and setup visualized rooms.
            cachedRoomInfos.Clear();
            ClearRoomListView();
        }
        
        public override void OnLeftLobby()
        {
            // Clear dictionary and setup visualized rooms.
            cachedRoomInfos.Clear();
            ClearRoomListView();
        }

        #endregion

        
        #region TEAMS FUNCTIONS

        private void OnJoinTeam(Player p, PhotonTeam team)
        {
            Debug.Log(p.NickName + team.Name); // What player and what team it is.
            GameObject entry = Instantiate(playerListEntryPrefab); // Instantiate prefab

            // Compare if p is read o blue team. Depend of the team, the instance prefab go to the corresponding container
            if (p.GetPhotonTeam().Name == "Red")
            {
                entry.transform.SetParent(containerRedTeam);
                entry.transform.localScale = Vector3.one;

            }
            else
            {
                entry.transform.SetParent(containerBlueTeam);
                entry.transform.localScale = Vector3.one;
            }

            
            if (p == PhotonNetwork.LocalPlayer)
            {
                object numPlayerInTeam = null;

                if (p.GetPhotonTeam().Name == "Blue")
                {
                    numPlayerInTeam = PhotonTeamsManager.Instance.GetTeamMembersCount("Blue");
                }
                if (p.GetPhotonTeam().Name == "Red")
                {
                    numPlayerInTeam = PhotonTeamsManager.Instance.GetTeamMembersCount("Blue");
                }
                
                
                Hashtable props = new Hashtable
                {
                    {PlayerCTF.PLAYER_LOADED_LEVEL, false},
                    {PlayerCTF.PLAYER_NUM_TEAM, numPlayerInTeam}
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }

            // Get comp and call initialize whit corresponding parameters of p
            entry.GetComponent<PlayerListCTF>().Initialize(p.ActorNumber, p.NickName);

            // SETUP CUSTOM PROPERTIES. This step is necessary for all other player get this properties
            object isPlayerReady;

            if (p.CustomProperties.TryGetValue(PlayerCTF.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerListCTF>().SetPlayerReady((bool) isPlayerReady); // Cal function to assign this properties
            }

            Debug.Log(PhotonTeamsManager.Instance.GetTeamMembersCount("Blue") + "    " +
                      PhotonTeamsManager.Instance.GetTeamMembersCount("Red")); // Call to show members in each team
            
            // Add this new instance of player to dictionary
            playerListEntries.Add(p.ActorNumber, entry);

            startGameBtn.gameObject.SetActive(CheckPlayersReady());


        }

        private void LeftTeam(Player p, PhotonTeam team)
        {
            // What player left the team
            Debug.Log(p.NickName + " / " + team.Name);

            object teamNum;
            if(PhotonNetwork.LocalPlayer.GetPhotonTeam() == null) return;
            
            
            
            if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Name == "Blue" && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerCTF.PLAYER_NUM_TEAM, out teamNum))
            {
                if (int.Parse(teamNum.ToString()) != 1)
                {
                    // NEW Property to compare if player is inGame or not when finalized load level.
                    Hashtable props = new Hashtable
                    {
                        {PlayerCTF.PLAYER_LOADED_LEVEL, false},
                        {PlayerCTF.PLAYER_NUM_TEAM, int.Parse(teamNum.ToString()) - 1}
                    };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                }
            }
            
            if(PhotonNetwork.LocalPlayer.GetPhotonTeam().Name == "Red" && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerCTF.PLAYER_NUM_TEAM, out teamNum))
            {

                if (int.Parse(teamNum.ToString()) != 1)
                {
                    // NEW Property to compare if player is inGame or not when finalized load level.
                    Hashtable props = new Hashtable
                    {
                        {PlayerCTF.PLAYER_LOADED_LEVEL, false},
                        {PlayerCTF.PLAYER_NUM_TEAM, int.Parse(teamNum.ToString())-1}
                    };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                }

            }

        }

        #endregion

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            GameObject entry;
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;

                if (changedProps.TryGetValue(PlayerCTF.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListCTF>().SetPlayerReady((bool) isPlayerReady);
                }


            }

            
            startGameBtn.gameObject.SetActive(CheckPlayersReady());

        }
        
        #region FAILED FUNCTIONS
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            SetActivePanel(selectionPanel.name);
        }
        
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("FAILED");
            SetActivePanel(selectionPanel.name);
        }
        
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            string roomName = "Room " + Random.Range(1000, 10000);

            RoomOptions options = new RoomOptions {MaxPlayers = 8};

            PhotonNetwork.CreateRoom(roomName, options, null);
        }
        
        public override void OnDisconnected(DisconnectCause _cause)
        {
            Debug.LogWarningFormat("Disconnected from server due to: {0}", _cause);

            if (_cause == DisconnectCause.ClientTimeout || _cause == DisconnectCause.ServerTimeout ||
                _cause == DisconnectCause.ServerAddressInvalid ||
                _cause == DisconnectCause.DisconnectByServerReasonUnknown)
            {
                SetActivePanel(networkErrorPanel.name);
                errorText.text = $"<color=#D03A39>Network connection failed. </color>\n{_cause}";
            }
        }

        #endregion

        #endregion
        
        #region UI Events

        /// <summary>
        /// Funcion encargada de conectar a photon y registrar el usuario.
        /// </summary>
        private void OnLoginButtonClicked()
        {
            playerName = playerNameInput.text;
            PlayerPrefs.SetString("playerName", playerName);

            if (playerName != string.Empty)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
                Debug.Log("Your name in game is: " + PhotonNetwork.NickName);
                SetActivePanel(loadingRoomPanel.name);
                textMessage.text = "Connecting to server...";
            }
            else
            {
                Debug.LogError("Player name is invalid");
            }

        }
        
        
        /// <summary>
        /// Funcion encargada de abrir el panel de creación de la room customizada
        /// </summary>
        private void OnCreateRoomButtonClicked()
        {
            SetActivePanel(createRoomPanel.name);
            animationMainMenuManager.CreateRoomAnimation();
        }
        

        /// <summary>
        /// Función encargada de abrir el panel de todas las rooms que hay creadas
        /// </summary>
        private void OnRoomListButtonClicked()
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            SetActivePanel(roomListPanel.name);
            animationMainMenuManager.ShowRoomsAnimation();
        }
        

        /// <summary>
        /// Función encargada para salir y desconectar de los servidores de photon
        /// </summary>
        private void OnExitToServer()
        {
            PhotonNetwork.Disconnect();

            SetActivePanel(loginPanel.name);
        }
        
        
        /// <summary>
        /// Funcion encargada de salir de una room o creación de room o lobby y abrir el panel de selección
        /// </summary>
        private void OnBackButtonClicked()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SetActivePanel(selectionPanel.name);
        }
        
        
        /// <summary>
        /// Función encargada de seleccionar el tipo de juego cuando se crea la room
        /// </summary>
        /// <param name="value"></param>
        public void OnSelectTypeGame(int value)
        {
            switch (value)
            {
                case 0:
                    maxPlayersTypeGame = "2";
                    break;
                case 1:
                    maxPlayersTypeGame = "4";
                    break;
                case 2:
                    maxPlayersTypeGame = "20";
                    break;
            }

            if (PhotonNetwork.CurrentRoom != null)
            {
                byte maxPlayer;
                byte.TryParse(maxPlayersTypeGame, out maxPlayer);
                PhotonNetwork.CurrentRoom.MaxPlayers = maxPlayer;
            }
        }

        
        /// <summary>
        /// Función encargada de crear la room a partir de los parametros asignados manualmente y abrir el panel InsideRoom
        /// </summary>
        private void OnCreateNewRoomClicked()
        {
            string roomName = roomNameInputField.text;
            roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

            byte maxPlayers;
            byte.TryParse(maxPlayersTypeGame, out maxPlayers);
            maxPlayers = (byte) Mathf.Clamp(maxPlayers, 2, 20);

            RoomOptions options = new RoomOptions {MaxPlayers = maxPlayers, PlayerTtl = 1000};

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        
        /// <summary>
        /// Función encargada de crear una room random.
        /// </summary>
        private void OnJoinRandomRoomButtonClicked()
        {
            SetActivePanel(loadingRoomPanel.name);
            textMessage.text = "Creating or joining a room...";

            string roomName = "Room " + Random.Range(1000, 10000);

            int value = Random.Range(0, 2);
            OnSelectTypeGame(value);

            byte maxPlayers;
            byte.TryParse(maxPlayersTypeGame, out maxPlayers);
            maxPlayers = (byte) Mathf.Clamp(maxPlayers, 2, 20);

            RoomOptions options = new RoomOptions {MaxPlayers = maxPlayers, PlayerTtl = 1000};


            PhotonNetwork.JoinRandomOrCreateRoom(null, 0, MatchmakingMode.FillRoom, null, null, roomName, options,
                null);
        }

        
        /// <summary>
        /// Función encargada de salir del juego e ir al panel de seleccion
        /// </summary>
        private void OnLeaveGameButtonClicked()
        {
            PhotonNetwork.LocalPlayer.LeaveCurrentTeam();

            SetActivePanel(selectionPanel.name);
            animationMainMenuManager.SelectionMenuAnimation();
            ChatManager.Instance.Disconnect();
            PhotonNetwork.LeaveRoom();

        }
        
        
        /// <summary>
        /// Función encargada de iniciar el juego.
        /// </summary>
        private void OnStartGameButtonClicked()
        {

            foreach (var player in PhotonNetwork.PlayerList)
            {
                object numPlayerInTeam;
                if (player.CustomProperties.TryGetValue(PlayerCTF.PLAYER_NUM_TEAM, out numPlayerInTeam))
                {
                    Debug.LogError(player.NickName + "/" + player.GetPhotonTeam().Name + "/"+ int.Parse(numPlayerInTeam.ToString()) );
                }
            }

            
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.AutomaticallySyncScene = true;
            SetActivePanel(loadingRoomPanel.name);
            textMessage.text = "Loading Game... Please wait.";
            PhotonNetwork.LoadLevel("CaptureFlag");

        }


        /// <summary>
        /// Funcion encargada de reconectar con el server si te da error
        /// </summary>
        private void OnReconnectToServerButtonClicked()
        {
            string playerName = PlayerPrefs.GetString("playerName");

            if (playerName != string.Empty)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.Reconnect();
                Debug.Log("Your name in game is: " + PhotonNetwork.NickName);
            }
        }

        #endregion

        #region PRIVATE FUNCTIONS

        private void Update()
        {

            if (PhotonNetwork.CurrentRoom != null)
                playersInRoomText.text =
                    PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        }

        
        /// <summary>
        /// Funcion encargada de borrar el Dictionary y asi reiniciar la cache de la room
        /// </summary>
        private void ClearRoomListView()
        {
            foreach (GameObject entry in roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListEntries.Clear();
        }

        
        /// <summary>
        /// Funcion encargada de ver si todos los jugadores estan readys o no.
        /// </summary>
        /// <returns>Si la propiedad del jugador esta ready devolvera {TRUE} sino {false}.
        /// Forzamos que sea false si no es el master de la room</returns>
        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(PlayerCTF.PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool) isPlayerReady || PhotonNetwork.CurrentRoom.PlayerCount < limitToPlay)
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        
        /// <summary>
        /// Funcion encargada de activar y desactivar los demás paneles a partir del nombre
        /// </summary>
        /// <param name="activePanel">Nombre del panel que quieres activar</param>
        private void SetActivePanel(string activePanel)
        {
            loginPanel.SetActive(activePanel.Equals(loginPanel.name));
            selectionPanel.SetActive(activePanel.Equals(selectionPanel.name));
            createRoomPanel.SetActive(activePanel.Equals(createRoomPanel.name));
            loadingRoomPanel.SetActive(activePanel.Equals(loadingRoomPanel.name));
            roomListPanel.SetActive(activePanel.Equals(roomListPanel.name));
            insideRoomPanel.SetActive(activePanel.Equals(insideRoomPanel.name));
            networkErrorPanel.SetActive(activePanel.Equals(networkErrorPanel.name));
        }

        
        /// <summary>
        /// Funcion encargada de updatear el Dictionary  de la room cuando buscamos la room.
        /// </summary>
        /// <param name="roomList">Room</param>
        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomInfos.ContainsKey(info.Name))
                    {
                        cachedRoomInfos.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomInfos.ContainsKey(info.Name))
                {
                    cachedRoomInfos[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomInfos.Add(info.Name, info);
                }
            }
        }

        
        /// <summary>
        /// Funcion encargada de updatear si haya alguna room creada.
        /// </summary>
        private void UpdateRoomListView()
        {
            foreach (RoomInfo info in cachedRoomInfos.Values)
            {
                GameObject entry = Instantiate(roomListEntryPrefab);
                entry.transform.SetParent(roomListContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RoomListCTF>().Initialize(info.Name, (byte) info.PlayerCount, info.MaxPlayers);

                roomListEntries.Add(info.Name, entry);
            }
        }

        
        /// <summary>
        /// Funcion encargada de updatear las propiedades de tu jugador local
        /// </summary>
        public void LocalPlayerPropertiesUpdated()
        {
            startGameBtn.gameObject.SetActive(CheckPlayersReady());
        }

        #endregion

    }
}
