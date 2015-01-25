using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace Parrador
{
    public enum NetworkMode
    {
        Offline,
        MatchMaking, //Currently in match making
        LobbyServer, //Enter Upon starting server.
        LobbyClient, //Enter Upon connecting with Server.
        GameServer, //Entered the game as a server
        GameClient //Entered the game as a client
    }

    public interface INetworkCallbackHandler
    {
        void OnGameStart();
        void OnGameObjectSpawned(string aObjectID, string aOwner);
        void OnGameObjectDespawned(string aObjectID, string aOwner);
    }
    public interface IGameCallbackHandler
    {
        void OnStateChange(NetworkID aID, object aState);
        float GetGameTime();
        void SetGameTime(float aTime);
        void OnGameOver();
    }


    [RequireComponent(typeof(NetworkView))]
    public class NetworkManager : MonoBehaviour
    {
        public const int MAX_CONNECTIONS = 8;
        public const int MAX_PLAYERS = 3;
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
            Application.runInBackground = true;

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

        private NetworkMode m_CurrentState = NetworkMode.MatchMaking;

        private List<Player> m_CurrentPlayers = new List<Player>();
        private List<Player> m_RegisteredPlayers = new List<Player>();
        private List<Player> m_LoadedPlayers = new List<Player>();
        private bool m_LoadedTimerFinish = false;
        private float m_LoadTimer = 60.0f;

        private List<GameObject> m_ServerGameObjects = new List<GameObject>();
        private List<GameObject> m_PreloadedGameObject = new List<GameObject>();

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
        [SerializeField]
        private List<GameObject> m_Prefabs = new List<GameObject>();
        
        public string hostName
        {
            get { return m_HostName; }
            set { if (m_CurrentState == NetworkMode.MatchMaking) { m_HostName = value; } }
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
        public NetworkMode networkState
        {
            get { return m_CurrentState; }
            set { m_CurrentState = value; }
        }
        public INetworkCallbackHandler callbackHandler
        {
            get;
            set;
        }
        public IGameCallbackHandler gameCallbackHandler
        {
            get;
            set;
        }
        

        private void ResetState()
        {
            m_CurrentState = NetworkMode.MatchMaking;
            m_RegisteringUsers = 0;

            m_CurrentPlayers.Clear();
            m_RegisteredPlayers.Clear();
            m_LoadedPlayers.Clear();

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
            m_CurrentState = NetworkMode.LobbyClient;
            SendRegisterUser();
            Debug.Log("Connected");
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
                    m_CurrentState = NetworkMode.MatchMaking;
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

            //string playerName = m_CurrentPlayers.FirstOrDefault<NetworkPlayerInfo>(Element => Element.player == aPlayer).name;

            Player player = m_CurrentPlayers.FirstOrDefault<Player>(Element => Element.networkPlayer == aPlayer);

            if(player != null)
            {
                string playerName = player.name;
                if (!string.IsNullOrEmpty(playerName))
                {
                    foreach (GameObject gObject in m_ServerGameObjects)
                    {
                        NetworkID networkdID = gObject.GetComponent<NetworkID>();

                        if (networkdID != null)
                        {
                            if (networkdID.ownerName == playerName)
                            {
                                DespawnObject(networkdID.objectID);
                            }
                        }
                    }
                    StartCoroutine(PlayerDisconnectRoutine(playerName));
                }
            }

            

            
        }

        IEnumerator PlayerDisconnectRoutine(string aPlayerName)
        {
            yield return new WaitForSeconds(1.0f);

            m_RegisteredPlayers.RemoveAll(Element => Element.name == aPlayerName);
            m_CurrentPlayers.RemoveAll(Element => Element.name == aPlayerName);
            m_LoadedPlayers.RemoveAll(Element => Element.name == aPlayerName);

            SendUpdateList(m_CurrentPlayers);
        }

        private void OnServerInitialized()
        {
            Debug.Log("Initializing Server");

            Player player = Player.ToPlayer(m_HostName, Network.player);
            m_CurrentPlayers.Add(player);
            m_CurrentState = NetworkMode.LobbyServer;

            //NetworkPlayerInfo playerInfo = new NetworkPlayerInfo();
            //playerInfo.name = m_HostName;
            //playerInfo.player = Network.player;
            //m_CurrentPlayers.Add(playerInfo);
            //m_CurrentState = NetworkMode.LobbyServer;
        }

        private void OnLevelWasLoaded(int aLevelIndex)
        {
            string aLevelName = Application.loadedLevelName;
            if(aLevelName == LEVEL_NAME)
            {
                if(Network.isClient)
                {
                    Debug.Log("Loaded Level");
                    networkView.RPC(NetworkRPC.NETWORK_MANAGER_ON_REGISTER_GAME_LOADED, RPCMode.Server, m_HostName);
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
                networkView.RPC(NetworkRPC.NETWORK_MANAGER_ON_REGISTER_PLAYER, RPCMode.Server, m_HostName);
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

        private void SendUpdateList(List<Player> aInfo)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            
            formatter.Serialize(stream, aInfo);
            byte[] bytes = stream.ToArray();
            stream.Close();

            if (bytes == null || bytes.Length == 0)
            {
                Debug.LogError("Failed to serialize current player");
            }
            else
            {
                networkView.RPC(NetworkRPC.NETWORK_MANAGER_ON_UPDATE_CONNECTED_USERS,RPCMode.Others,bytes);
            }
        }

        [RPC]
        private void OnPlayerJoined(byte[] aPlayerBytes)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream(aPlayerBytes);
                Player joiningPlayer = formatter.Deserialize(stream) as Player;
            }
            catch(Exception aException)
            {
                Debug.LogException(aException);
            }
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
            if(networkState != NetworkMode.LobbyServer)
            {
                networkView.RPC(NetworkRPC.NETWORK_MANAGER_ON_RECEIVE_REGISTER_RESULT, aInfo.sender, false);
                return;
            }
            ///Fail condition 1
            if(m_RegisteringUsers + 1 >= MAX_PLAYERS)
            {
                Debug.LogWarning("Player (" + aPlayerName + ") was attempting to register but there are currently " + m_RegisteringUsers + " registering right now");
                networkView.RPC(NetworkRPC.NETWORK_MANAGER_ON_RECEIVE_REGISTER_RESULT, aInfo.sender, false);
                return;
            }
            ///Fail condition 2
            if(m_CurrentPlayers.Any<Player>(Element => Element.name == aPlayerName) || m_RegisteredPlayers.Any<Player>(Element => Element.name == aPlayerName))
            {
                Debug.LogWarning("Player (" + aPlayerName + ") was attempting to register but someone with that name already exists.");
                networkView.RPC(NetworkRPC.NETWORK_MANAGER_ON_RECEIVE_REGISTER_RESULT, aInfo.sender, false);
                return;
            }
            
            {
                Player player = Player.ToPlayer(aPlayerName, aInfo.sender);
                m_RegisteredPlayers.Add(player);
                m_RegisteringUsers++;
                networkView.RPC(NetworkRPC.NETWORK_MANAGER_ON_RECEIVE_REGISTER_RESULT, aInfo.sender, true);
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
                networkView.RPC(NetworkRPC.NETWORK_MANAGER_ON_REGISTER_COMPLETE, RPCMode.Server, m_HostName);
            }
            else
            {
                Debug.Log("Failed to register with server");
                CloseNetwork();
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
            Player player = m_RegisteredPlayers.FirstOrDefault<Player>(Element => Element.networkPlayer == aInfo.sender);


            if(!string.IsNullOrEmpty(player.name))
            {
                m_RegisteredPlayers.Remove(player);
                m_RegisteringUsers--;

                MemoryStream stream = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, player);
                stream.Close();
                byte[] playerBytes = stream.ToArray();

                ///Notify player join
                foreach(Player playerInfo in m_CurrentPlayers)
                {
                    if(playerInfo.networkPlayer == Network.player)
                    {
                        continue;
                    }
                    networkView.RPC(NetworkRPC.NETWORK_MANAGER_ON_PLAYER_JOINED, playerInfo.networkPlayer, playerBytes);
                }
                m_CurrentPlayers.Add(player);

                SendUpdateList(m_CurrentPlayers);
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
                    m_CurrentPlayers = (List<Player>)formatter.Deserialize(stream);
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
            //if(m_CurrentPlayers.Count != MAX_PLAYERS)
            //{
            //    Debug.Log("Need more players");
            //    return;
            //}

            networkView.RPC(NetworkRPC.NETWORK_MANAGER_ON_START_GAME, RPCMode.Others);
            OnStartGame();
            m_RegisteredPlayers.Clear();
        }

        [RPC]
        private void OnStartGame()
        {
            if(Network.isServer)
            {
                m_CurrentState = NetworkMode.GameServer;
            }
            else if(Network.isClient)
            {
                m_CurrentState = NetworkMode.GameClient;
            }
            Debug.Log("Start Game");
            m_LoadedTimerFinish = false;
            StartCoroutine(GameLoadRoutine());
            Application.LoadLevel(LEVEL_NAME);
        }

        IEnumerator GameLoadRoutine()
        {
            m_LoadedTimerFinish = false;
            yield return new WaitForSeconds(m_LoadTimer);
            m_LoadedTimerFinish = true;
        }

        [RPC]
        private void OnRegisterGameLoaded(string aPlayerName)
        {
            if(!Network.isServer)
            {
                return;
            }
            Player player = m_CurrentPlayers.FirstOrDefault<Player>(Element => Element.name == aPlayerName);

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
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();

                formatter.Serialize(stream, m_LoadedPlayers);
                byte[] loadedPlayerBytes = stream.ToArray();
                networkView.RPC("OnGameLoaded", RPCMode.Others,loadedPlayerBytes);
                OnGameLoaded(null);
            }

        }
        [RPC]
        private void OnGameLoaded(byte[] aLoadedPlayers)
        {
            Debug.Log("Game Loaded !!!!");
            if(Network.isClient)
            {
                m_CurrentState = NetworkMode.GameClient;
                try
                {
                    BinaryFormatter aFormatter = new BinaryFormatter();
                    MemoryStream stream = new MemoryStream(aLoadedPlayers);
                    m_LoadedPlayers = aFormatter.Deserialize(stream) as List<Player>;
                }
                catch(Exception aException)
                {
                    Debug.LogException(aException);
                }
                    
            }
            else
            {
                m_CurrentState = NetworkMode.GameServer;
            }
            if(callbackHandler != null)
            {
                callbackHandler.OnGameStart();
            }
        }

        public void SpawnObject(int aIndex)
        {
            SpawnObject(aIndex, Vector3.zero, Quaternion.identity);
        }

        public void SpawnObject(int aIndex, Vector3 aPosition, Quaternion aRotation)
        {
            if(Network.isClient)
            {
                networkView.RPC(NetworkRPC.NETWORK_MANAGER_ON_NETWORK_SPAWN_OBJECT, RPCMode.Server, aIndex, m_HostName, aPosition, aRotation);
            }
            else if(Network.isServer)
            {
                OnNetworkSpawnObject(aIndex, m_HostName, aPosition, aRotation);
            }
        }
        public void DespawnObject(string aObjectID)
        {
            if(Network.isClient)
            {
                networkView.RPC(NetworkRPC.NETWORK_MANAGER_ON_NETWORK_DESPAWN_OBJECT, RPCMode.Server, aObjectID, m_HostName);
            }
            else
            {
                OnNetworkDespawnObject(aObjectID, m_HostName);
            }
        }
        public void DespawnObject(GameObject aObject)
        {
            NetworkID id = aObject.GetComponent<NetworkID>();
            if(id != null)
            {
                DespawnObject(id.objectID);
            }
        }
        [RPC]
        private void OnNetworkSpawnObject(int aIndex, string aPlayer, Vector3 aPosition, Quaternion aRotation)
        {
            if(!Network.isServer)
            {
                Debug.LogError("Cannot spawn objects on the client. Use SpawnObject instead.");
                return;
            }

            GameObject prefab = GetPrefabByIndex(aIndex);
            Player player = GetPlayer(aPlayer);
            if(prefab == null)
            {
                Debug.LogError("Failed to SpawnObject, Missing prefab: index(" + aIndex + ").");
                return;
            }
            if(prefab.GetComponent<NetworkID>() == null)
            {
                Debug.LogError("Failed to SpawnObject, Missing NetworkID component");
                return;
            }
            if(player == null)
            {
                Debug.LogError("Failed to SpawnObject, Missing player: " + aPlayer);
                return;
            }
            int playerIndex = GetPlayerIndex(player);
            if(playerIndex == -1)
            {
                Debug.LogError("Failed to SpawnObject, Invalid PlayerIndex: " + aPlayer);
                return;
            }
            GameObject obj = Network.Instantiate(prefab, aPosition, aRotation, playerIndex) as GameObject;
            NetworkID netID = obj.GetComponent<NetworkID>();


            NetworkView netView = obj.networkView;
            if (netView != null && netID != null)
            {
                string objectID = Guid.NewGuid().ToString();
                netView.RPC(NetworkRPC.NETWORK_ID_ON_RECEIVE_SERVER_INFO, RPCMode.OthersBuffered, netView.viewID, player.name, playerIndex, objectID);
                netID.ReceiveServerInfo(netView.viewID, player.name, playerIndex, objectID);
                RegisterSpawnedObject(obj);
            }
            else if(netView == null)
            {
                Debug.LogWarning("Spawned a gameobject on the network but there is no network view associated with it.");
            }
            else if(netID == null)
            {
                Debug.LogWarning("Spawned a gameobject on the network but there is no network id associated with it.");
            }
        }

        [RPC]
        private void OnNetworkDespawnObject(string aObjectID, string aInvoker)
        {
            //Maybe check invoker if were only allowing the owners to delete their own objects
            //Maybe check admin status / server status etc...
            if(!Network.isServer)
            {
                return;
            }

            GameObject spawnedObject = GetSpawnedObject(aObjectID);
            if(spawnedObject != null)
            {
                networkView.RPC(NetworkRPC.NETWORK_MANAGER_ON_NETWORK_DESTROY_OBJECT, RPCMode.OthersBuffered, aObjectID);
                Destroy(spawnedObject);
            }
            else
            {
                Debug.LogError("Failed to destroy object. Object does not exist. ID = " + aObjectID);
            }
            

        }

        [RPC]
        private void OnNetworkDestroyObject(string aObjectID)
        {
            if(!Network.isClient)
            {
                Debug.LogError("Only clients are responsible for destroying their own scene objects. This should be invoked from the server.");
                return;
            }
            GameObject spawnedObject = GetSpawnedObject(aObjectID);
            if(spawnedObject != null)
            {
                Destroy(spawnedObject);
            }
            else
            {
                Debug.LogError("Failed to destroy object. Object does not exist. ID = " + aObjectID);
            }   
            
        }


        #endregion


        #region WORLD METHODS
        public void CloseNetwork()
        {
            Debug.Log("Closing Server");

            Network.Disconnect();
            ResetState();
        }



        #region PREFAB ACCESS

        public GameObject GetPrefabByName(string aName)
        {
            return m_Prefabs.FirstOrDefault<GameObject>(Element => Element.name == aName);
        }
        public GameObject GetPrefabByIndex(int aIndex)
        {
            if (aIndex >= 0 && aIndex <= m_Prefabs.Count)
            {
                return m_Prefabs[aIndex];
            }
            return null;
        }
        public int GetPrefabIndex(string aName)
        {
            return m_Prefabs.FindIndex(Element => Element.name == aName);
        }

        #endregion


        public Player GetPlayer(string aName)
        {
            return m_CurrentPlayers.FirstOrDefault<Player>(Element => Element.name == aName);
        }
        public Player GetPlayer(int aIndex)
        {
            if (aIndex >= 0 && aIndex < m_CurrentPlayers.Count)
            {
                return m_CurrentPlayers[aIndex];
            }
            return null;
        }
        public Player GetServerHost()
        {
            return m_CurrentPlayers.Count > 0 ? m_CurrentPlayers[0] : null;
        }
        public Player GetSelf()
        {
            return m_CurrentPlayers.FirstOrDefault<Player>(Element => Element.name == m_HostName);
        }
        public int GetPlayerIndex(string aName)
        {
            return m_CurrentPlayers.FindIndex(Element => Element.name == aName);
        }
        public int GetPlayerIndex(Player aInfo)
        {
            return m_CurrentPlayers.IndexOf(aInfo);
        }
        public void RegisterSpawnedObject(GameObject aObject)
        {
            m_ServerGameObjects.Add(aObject);
            NetworkID netID = aObject.GetComponent<NetworkID>();
            if(netID != null && callbackHandler != null)
            {
                callbackHandler.OnGameObjectSpawned(netID.objectID, netID.ownerName);
            }

        }
        public void UnregisterSpawnedObject(GameObject aObject)
        {
            bool exists = m_ServerGameObjects.Any<GameObject>(Element => Element == aObject);
            NetworkID netID = aObject.GetComponent<NetworkID>();
            if(netID != null && callbackHandler != null && exists)
            {
                callbackHandler.OnGameObjectDespawned(netID.objectID, netID.ownerName);
            }
            m_ServerGameObjects.Remove(aObject);
        }

        public GameObject GetSpawnedObject(string aObjectId)
        {
            foreach (GameObject gObject in m_ServerGameObjects)
            {
                NetworkID netID = gObject.GetComponent<NetworkID>();
                if(netID != null && netID.objectID == aObjectId)
                {
                    return gObject;
                }
            }
            Debug.LogError("Did not find object");
            return null;
        }


        

        public void SendObjectChange(NetworkID aNetworkID, object aState)
        {
            if(aNetworkID == null || aState == null)
            {
                Debug.LogError("Failed SendObjectChange: Bad Objects.");
                return;
            }
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            try
            {
                formatter.Serialize(stream,aState);
                if(Network.isClient)
                {
                    networkView.RPC("ChangeObjectState", RPCMode.Server, aNetworkID.objectID, stream.ToArray());
                }
                else
                {
                    ChangeObjectState(aNetworkID.objectID, stream.ToArray());
                }
            }
            catch(Exception aException)
            {
                Debug.LogException(aException);
            }

            stream.Close();
        }

        [RPC]
        private void ChangeObjectState(string aObjectID, byte[] aState)
        {
            if(!Network.isServer)
            {
                return;
            }

            networkView.RPC("OnChangeObjectState",RPCMode.OthersBuffered, aObjectID,aState);
            OnChangeObjectState(aObjectID, aState);
        }

        [RPC]
        private void OnChangeObjectState(string aObjectID, byte[] aState)
        {
            if(aState == null)
            {
                Debug.LogError("Missing State");
                return;
            }
            MemoryStream stream = new MemoryStream(aState);
            BinaryFormatter formatter = new BinaryFormatter();
            GameObject obj = GetSpawnedObject(aObjectID);
            
            if(obj != null)
            {
                NetworkID id = obj.GetComponent<NetworkID>();
                try
                {
                    object state = formatter.Deserialize(stream);
                    if(gameCallbackHandler != null)
                    {
                        gameCallbackHandler.OnStateChange(id, state);
                    }
                }
                catch (Exception aException)
                {
                    Debug.LogException(aException);
                }
            }
            

        }

        public void SendAddTime(Player aPlayer, float aTime)
        {
            if(aPlayer == null)
            {
                return;
            }

            if(Network.isClient)
            {
                networkView.RPC("OnAddTime",RPCMode.Server ,aPlayer.name, aTime);
            }
            else if(Network.isServer)
            {
                OnAddTime(aPlayer.name, aTime);
            }
        }
        public void SendSetTime(float aTime)
        {
            if(Network.isServer)
            {
				networkView.RPC("SendSynchronizeTime", RPCMode.OthersBuffered, aTime);
                SendSynchronizeTime(aTime);
            }
        }

        [RPC]
        public void OnAddTime(string aPlayer, float aTime)
        {
            if(!Network.isServer)
            {
                return;
            }

            if(gameCallbackHandler == null)
            {
                Debug.LogError("Missing Game Callback Handler");
                return;
            }

            //TODO: Get Game Time (gt + aTime)
            float totalTime = gameCallbackHandler.GetGameTime() + aTime;
			networkView.RPC("SendSynchronizeTime", RPCMode.OthersBuffered, totalTime);
            SendSynchronizeTime(totalTime);

        }
        [RPC]
        private void SendSynchronizeTime(float aTime)
        {
            //TODO: Set Game Time
            if(gameCallbackHandler != null)
            {
                gameCallbackHandler.SetGameTime(aTime);
            }
        }
        
        
        public void SendGameOver()
        {
        	if(Network.isServer)
        	{
        		networkView.RPC("OnGameOver",RPCMode.OthersBuffered);
        		OnGameOver();
        	}
        }
        
        [RPC]
        private void OnGameOver()
        {
        	gameCallbackHandler.OnGameOver();
        }

        
        

        #endregion

        #endregion

    }

}

