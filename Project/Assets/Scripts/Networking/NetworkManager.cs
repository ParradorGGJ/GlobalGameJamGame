using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace Parrador
{
    public enum NetworkState
    {
        Offline,
        MatchMaking,
        LobbyServer,
        LobbyClient,
        GameServer,
        GameClient
    }

    [RequireComponent(typeof(NetworkView))]
    public class NetworkManager : MonoBehaviour
    {
        public const int MAX_CONNECTIONS = 8;
        public const int MAX_PLAYERS = 2;
        public const string GAME_NAME = "Parrador_Online";

        private const string LEVEL_NAME = "Level";
        private const string MENU_NAME = "MatchMaking";

        private static NetworkManager s_Instance = null;
        public static NetworkManager instance
        {
            get { return s_Instance; }
        }

        void Awake()
        {
            if(s_Instance == null)
            {
                s_Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            if(s_Instance == this)
            {
                s_Instance = null;
                
            }
        }

        private NetworkState m_CurrentState = NetworkState.MatchMaking;
        private List<NetworkPlayerInfo> m_CurrentPlayers = new List<NetworkPlayerInfo>();
        //private List<NetworkPlayer> m_ConnectedPlayers = new List<NetworkPlayer>();
        private List<NetworkPlayerInfo> m_RegisteringPlayers = new List<NetworkPlayerInfo>();
        private List<NetworkPlayerInfo> m_LoadedPlayers = new List<NetworkPlayerInfo>();
        private int m_RegisteringUsers = 0;
        #region MEMBERS
        [SerializeField]
        private string m_HostName = string.Empty; //local player name
        [SerializeField]
        private string m_LobbyName = string.Empty; //lobby name -- applicable to server only
        [SerializeField]
        private string m_Comment = string.Empty; //comment to lobby -- applicable to server only
        [SerializeField]
        private int m_PortNumber = 25006; //the port for the server..
        
        public string hostName
        {
            get { return m_HostName; }
            set { m_HostName = value; }
        }
        public string lobbyName
        {
            get { return m_LobbyName; }
            set { m_LobbyName = value; }
        }
        public string comment
        {
            get { return m_Comment; }
            set { m_Comment = value; }
        }
        public int portNumber
        {
            get { return m_PortNumber; }
        }
        public int connectedPlayers
        {
            get { return m_CurrentPlayers.Count; }
        }
        public NetworkState networkState
        {
            get { return m_CurrentState; }
            set { m_CurrentState = value; }
        }
        public void CloseServer()
        {
            Network.Disconnect();
            ResetState();
        }
        #endregion

        private void ResetState()
        {
            m_CurrentState = NetworkState.MatchMaking;
            m_CurrentPlayers.Clear();
            m_RegisteringPlayers.Clear();
            m_RegisteringUsers = 0;
        }

        #region SENDERS
        /// <summary>
        /// USE THESE METHODS TO SEND MESSAGES
        /// They wrap client / server functionality
        /// </summary>

        public void SendRoomStateChange(ObjectType aObjectType, RoomType aRoomType, bool aNewState)
        {
            int objectType = (int)aObjectType;
            int roomType = (int)aRoomType;
            if(Network.isServer)
            {
                networkView.RPC("OnChangeRoomStateBool", RPCMode.OthersBuffered, objectType, roomType, aNewState);
                OnChangeRoomStateBool(objectType, roomType, aNewState);
            }
            else if(Network.isClient)
            {
                networkView.RPC("OnRequestChangeRoomStateBool", RPCMode.Server, objectType, roomType, aNewState);
            }
            
        }
        public void SendRoomStateChange(ObjectType aObjectType, RoomType aRoomType, float aNewState )
        {
            int objectType = (int)aObjectType;
            int roomType = (int)aRoomType;
            if (Network.isServer)
            {
                networkView.RPC("OnChangeRoomStateFloat", RPCMode.OthersBuffered, objectType, roomType, aNewState);
                OnChangeRoomStateFloat(objectType, roomType, aNewState);
            }
            else if (Network.isClient)
            {
                networkView.RPC("OnRequestChangeRoomStateFloat", RPCMode.Server, objectType, roomType, aNewState);
            }
        }
        public void SendPlayerEnterRoom(RoomType aRoomType, string aPlayerName)
        {
            if(string.IsNullOrEmpty(aPlayerName))
            {
                Debug.LogError("Invalid player name entered for method SendPlayerEnterRoom");
                return;
            }
            int roomType = (int)aRoomType;
            if(Network.isServer)
            {
                networkView.RPC("OnPlayerEnterRoom", RPCMode.OthersBuffered, roomType, aPlayerName);
                OnPlayerEnterRoom(roomType, aPlayerName);
            }
            else
            {
                networkView.RPC("OnRequestPlayerEnterRoom", RPCMode.Server, roomType, aPlayerName);
            }
        }
        public void SendPlayerEnterCorridor(string aPlayerName)
        {
            if (string.IsNullOrEmpty(aPlayerName))
            {
                Debug.LogError("Invalid player name entered for method SendPlayerEnterRoom");
                return;
            }
            if (Network.isServer)
            {
                networkView.RPC("OnPlayerEnterCorridor", RPCMode.OthersBuffered, aPlayerName);
                OnPlayerEnterCorridor(aPlayerName);
            }
            else
            {
                networkView.RPC("OnRequestPlayerEnterCorridor", RPCMode.Server, aPlayerName);
            }
        }

        #endregion

        #region CLIENT_ONLY_RPC

        [RPC]
        void OnChangeRoomStateBool(int aObjectType, int aRoomType, bool aNewState)
        {
            ObjectType objectType = (ObjectType)aObjectType;
            RoomType roomType = (RoomType)aRoomType;

            //TODO: Search for room, search for object in room and change the state

        }

        [RPC]
        void OnChangeRoomStateFloat(int aObjectType, int aRoomType, float aNewState)
        {
            ObjectType objectType = (ObjectType)aObjectType;
            RoomType roomType = (RoomType)aRoomType;

            //TODO: Search for room, search for object in room and change the state

        }

        [RPC]
        void OnPlayerEnterRoom(int aRoomType, string aPlayerName)
        {
            //TODO: Update the players position to be within x room.

        }

        [RPC]
        void OnPlayerEnterCorridor(string aPlayerName)
        {
            //TODO: Update the players position to be within their corridor.
        }

        #endregion


        #region SERVER_ONLY_RPC

        [RPC]
        void OnRequestChangeRoomStateBool(int aObjectType, int aRoomType, bool aNewState)
        {
            if(!Network.isServer)
            {
                Debug.LogError("OnRequestChangeRoomStateBool was called on a machine that is not the server.");
                return;
            }
            ObjectType objectType = (ObjectType)aObjectType;
            RoomType roomType = (RoomType)aRoomType;
            SendRoomStateChange(objectType, roomType, aNewState);
        }
        [RPC]
        void OnRequestChangeRoomStateFloat(int aObjectType, int aRoomType, float aNewState)
        {
            if(!Network.isServer)
            {
                Debug.LogError("OnRequestChangeRoomStateFloat was called on a machine that is not the server.");
                return;
            }
            ObjectType objectType = (ObjectType)aObjectType;
            RoomType roomType = (RoomType)aRoomType;
            SendRoomStateChange(objectType, roomType, aNewState);
        }

        void OnRequestPlayerEnterRoom(int aRoomType, string aPlayerName)
        {
            if(!Network.isServer)
            {
                Debug.LogError("OnRequestPlayerEnterRoom was called on a machine that is not the server.");
                return;
            }
        }
        void OnRequestPlayerEnterCorridor(string aPlayerName)
        {
            if (!Network.isServer)
            {
                Debug.LogError("OnRequestPlayerEnterCorridor was called on a machine that is not the server.");
                return;
            }
        }

        #endregion


#region LOBBY_HANDLING

        private void OnConnectedToServer()
        {
            //TODO: Tell Server To Register Me. (Send Name)
            m_CurrentState = NetworkState.LobbyClient;
            networkView.RPC("OnRegisterPlayer",RPCMode.Server,m_HostName);
            Debug.Log("Disconnected");
        }

        private void OnDisconnectedFromServer(NetworkDisconnection aInfo)
        {
            Debug.Log("Disconnected");
            if(Network.isServer)
            {
                //Local Server Disconnected
                Debug.Log("Local Disconnect");
                if(Application.loadedLevelName != MENU_NAME)
                {
                    Application.LoadLevel(MENU_NAME);
                }
            }
            else
            {
                if(aInfo == NetworkDisconnection.LostConnection)
                {
                    Debug.LogError("Lost Connection from the server.");
                }
                else if (aInfo == NetworkDisconnection.Disconnected)
                {
                    
                    //Successfully disconnected.
                    m_CurrentState = NetworkState.MatchMaking;
                    if(Application.loadedLevelName != MENU_NAME )
                    {
                        Application.LoadLevel(MENU_NAME);
                    }
                }
            }
        }

        private void OnFailedtoConnect(NetworkConnectionError aError)
        {
            Debug.LogError("Failed to connect to server for reason: " + aError);
        }

        private void OnNetworkInstantiate(NetworkMessageInfo aInfo)
        {

        }

        private void OnPlayerConnected(NetworkPlayer aPlayer)
        {
            Debug.Log("Player Connected");
        }

        private void OnPlayerDisconnected(NetworkPlayer aPlayer)
        {
            Debug.Log("Player Disconnected");



            m_RegisteringPlayers.RemoveAll(Element => Element.player == aPlayer);
            m_CurrentPlayers.RemoveAll(Element => Element.player == aPlayer);
            m_LoadedPlayers.RemoveAll(Element => Element.player == aPlayer);


            List<NetworkPlayerStreamInfo> streamInfo = new List<NetworkPlayerStreamInfo>();
            foreach (NetworkPlayerInfo netPlayer in m_CurrentPlayers)
            {
                streamInfo.Add(netPlayer.streamInfo);
            }

        }

        private void OnServerInitialized()
        {
            Debug.Log("Initializing Server");
            NetworkPlayerInfo playerInfo = new NetworkPlayerInfo();
            playerInfo.name = m_HostName;
            playerInfo.player = Network.player;
            m_CurrentPlayers.Add(playerInfo);
            m_CurrentState = NetworkState.LobbyServer;
        }

        private void OnLevelWasLoaded(int aLevelIndex)
        {
            string aLevelName = Application.loadedLevelName;
            if(aLevelName == LEVEL_NAME)
            {
                if(Network.isClient)
                {
                    Debug.Log("Loaded Level");
                    networkView.RPC("OnRegisterGameLoaded", RPCMode.Server, m_HostName);
                }
                else if(Network.isServer)
                {
                    Debug.Log("Loaded Level");
                    OnRegisterGameLoaded(m_HostName);
                }
            }
        }


        private void SendRegisterUser()
        {
            if(string.IsNullOrEmpty(m_HostName))
            {
                Debug.LogError("Failed to register user. Name was found empty");
                return;
            }

            if(Network.isClient)
            {
                networkView.RPC("OnRegisterPlayer", RPCMode.Server, m_HostName);
            }
        }

        private void SendUpdateList(List<NetworkPlayerStreamInfo> aInfo)
        {
            ///Send updated info to the clients.
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, aInfo);
            byte[] playerInfoStream = stream.ToArray();

            if (playerInfoStream == null || playerInfoStream.Length == 0)
            {
                Debug.LogError("Failed to serialize current player");
            }
            else
            {
                networkView.RPC("OnUpdateConnectedUsers", RPCMode.Others, playerInfoStream);
            }
        }

        [RPC]
        private void OnPlayerJoined(NetworkPlayer aPlayer, string aPlayerName)
        {
            //Gets sent to clients to notify them of the player joining.
        }
        [RPC]
        private void OnPlayerDisconnected(NetworkPlayer aPlayer, string PlayerName)
        {
            //Gets sent to clients to notify them of the player disconnecting.
        }

        [RPC]
        private void OnRegisterPlayer(string aPlayerName, NetworkMessageInfo aInfo)
        {
            //Gets sent to the server to cross reference the player name with existing playernames.
            ///Fails if registering users exceeds the max players
            ///Fails if player with name exists
            ///Fails if the user is already registered.
            if(!Network.isServer)
            {
                Debug.LogError("OnRegisterPlayer was called on a machine that is not the server.");
                return;
            }
            //Ignore players while the network is locked, if the network is locked its because the game has started.
            if(networkState != NetworkState.LobbyServer)
            {
                networkView.RPC("OnReceiveRegisterResult", aInfo.sender, false);
                return;
            }
            ///Fail condition 1
            if(m_RegisteringUsers + 1 >= MAX_PLAYERS)
            {
                Debug.LogWarning("Player (" + aPlayerName + ") was attempting to register but there are currently " + m_RegisteringUsers + " registering right now");
                networkView.RPC("OnReceiveRegisterResult", aInfo.sender, false);
                return;
            }
            ///Fail condition 2
            if(m_CurrentPlayers.Any<NetworkPlayerInfo>(Element => Element.name == aPlayerName) || m_RegisteringPlayers.Any<NetworkPlayerInfo>(Element => Element.name == aPlayerName))
            {
                Debug.LogWarning("Player (" + aPlayerName + ") was attempting to register but someone with that name already exists.");
                networkView.RPC("OnReceiveRegisterResult", aInfo.sender, false);
                return;
            }
            
            {
                NetworkPlayerInfo playerInfo = new NetworkPlayerInfo();
                playerInfo.name = aPlayerName;
                playerInfo.player = aInfo.sender;
                m_RegisteringPlayers.Add(playerInfo);
                m_RegisteringUsers++;
                networkView.RPC("OnReceiveRegisterResult", aInfo.sender, true);
            }
        }
        [RPC]
        private void OnReceiveRegisterResult(bool aResult)
        {
            ///Called on client to get feed back on the register request
            if (!Network.isClient)
            {
                Debug.LogError("OnReceiveRegisterResult was called on a machine that is not the server.");
                return;
            }
            if(aResult == true)
            {
                Debug.Log("Registered with server.");
                networkView.RPC("OnRegisterComplete", RPCMode.Server, m_HostName);
            }
            else
            {
                Debug.Log("Failed to register with server");
                Network.Disconnect();
            }
        }

        [RPC]
        private void OnRegisterComplete(string aPlayerName, NetworkMessageInfo aInfo)
        {
            ///The final handshake on the server. This will update the current player info list on server and clients.
            if(!Network.isServer)
            {
                Debug.LogError("OnRegisterComplete was called on a machine that is not the server.");
                return;
            }
            NetworkPlayerInfo player = m_RegisteringPlayers.FirstOrDefault<NetworkPlayerInfo>(Element => Element.player == aInfo.sender);
            if(!string.IsNullOrEmpty(player.name))
            {
                m_RegisteringPlayers.Remove(player);
                m_RegisteringUsers--;
                foreach(NetworkPlayerInfo playerInfo in m_CurrentPlayers)
                {
                    networkView.RPC("OnPlayerJoined", playerInfo.player, player.player, player.name);
                }
                m_CurrentPlayers.Add(player);
                

                List<NetworkPlayerStreamInfo> streamInfo = new List<NetworkPlayerStreamInfo>();
                foreach (NetworkPlayerInfo netPlayer in m_CurrentPlayers)
                {
                    streamInfo.Add(netPlayer.streamInfo);
                }

                SendUpdateList(streamInfo);

                
            }
            else
            {
                Debug.LogError("Failed third handshake. Player didnt exist, possible ghost.");
            }
            

        }

        [RPC]
        private void OnUpdateConnectedUsers(byte[] aUsers)
        {
            ///Recieve updated player info
            m_CurrentPlayers.Clear();
            if(aUsers != null && aUsers.Length > 0)
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream stream = new MemoryStream(aUsers);
                    List<NetworkPlayerStreamInfo> playerInfoStream =  (List<NetworkPlayerStreamInfo>)formatter.Deserialize(stream);
                    if(playerInfoStream != null)
                    {
                        foreach(NetworkPlayerStreamInfo streamInfo in playerInfoStream)
                        {
                            m_CurrentPlayers.Add(streamInfo.info);
                        }
                    }
                }
                catch(Exception aException)
                {
                    Debug.LogException(aException);
                }
                

            }
        }


#endregion


        #region GAME 
        public void StartGame()
        {
            if(!Network.isServer)
            {
                return;
            }
            if(m_CurrentPlayers.Count != MAX_PLAYERS)
            {
                Debug.Log("Need more players");
                return;
            }

            networkView.RPC("OnStartGame", RPCMode.Others);
            OnStartGame();
            m_RegisteringPlayers.Clear();
        }

        [RPC]
        private void OnStartGame()
        {
            Debug.Log("Start Game");
            Application.LoadLevel(LEVEL_NAME);
        }

        [RPC]
        private void OnRegisterGameLoaded(string aPlayerName)
        {
            if(!Network.isServer)
            {
                return;
            }
            NetworkPlayerInfo player = m_CurrentPlayers.FirstOrDefault<NetworkPlayerInfo>(Element => Element.name == aPlayerName);
            if(player.name != aPlayerName)
            {
                Debug.LogError("Failed to register game loaded for player: " + aPlayerName + ". Possible ghost");
            }
            else if(m_LoadedPlayers.Any(Element => Element.name == player.name))
            {
                Debug.LogError("Player " + aPlayerName + " has loaded twice");
            }
            else
            {
                m_LoadedPlayers.Add(player);
            }

            if(m_LoadedPlayers.Count == m_CurrentPlayers.Count)
            {
                networkView.RPC("OnGameLoaded", RPCMode.Others);
                OnGameLoaded();
            }

        }
        [RPC]
        private void OnGameLoaded()
        {
            Debug.Log("Game Loaded !!!!");
            if(Network.isClient)
            {
                m_CurrentState = NetworkState.GameClient;
            }
            else
            {
                m_CurrentState = NetworkState.GameServer;
            }
        }

        #endregion

    }

}

