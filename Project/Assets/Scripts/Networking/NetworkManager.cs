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
        MatchMaking,
        LobbyServer,
        LobbyClient,
        GameServer,
        GameClient
    }

    public interface INetworkCallbackHandler
    {
        void OnGameStart();
        void OnGameObjectSpawned(string aObjectID, string aOwner);
        void OnGameObjectDespawned(string aObjectID, string aOwner);
    }

    [RequireComponent(typeof(NetworkView))]
    public class NetworkManager : MonoBehaviour
    {
        public const int MAX_CONNECTIONS = 8;
        public const int MAX_PLAYERS = 2;
        public const string GAME_NAME = "Parrador_Online";

        private const string LEVEL_NAME = "Proto_Level";
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

        private NetworkMode m_CurrentState = NetworkMode.MatchMaking;
        private List<NetworkPlayerInfo> m_CurrentPlayers = new List<NetworkPlayerInfo>();
        //private List<NetworkPlayer> m_ConnectedPlayers = new List<NetworkPlayer>();
        private List<NetworkPlayerInfo> m_RegisteringPlayers = new List<NetworkPlayerInfo>();
        private List<NetworkPlayerInfo> m_LoadedPlayers = new List<NetworkPlayerInfo>();

        private List<GameObject> m_ServerGameObjects = new List<GameObject>();

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
        public void CloseServer()
        {
            Network.Disconnect();
            ResetState();
        }
        public GameObject GetPrefabByName(string aName)
        {
            return m_Prefabs.FirstOrDefault<GameObject>(Element => Element.name == aName);
        }
        public GameObject GetPrefabByIndex(int aIndex)
        {
            if(aIndex >= 0 && aIndex <= m_Prefabs.Count)
            {
                return m_Prefabs[aIndex];
            }
            return null;
        }
        public GameObject GetPrefabByID(int aID)
        {
            return m_Prefabs.FirstOrDefault<GameObject>(Element => Element.GetInstanceID() == aID);
        }
        public NetworkPlayerInfo GetPlayer(string aName)
        {
            return m_CurrentPlayers.FirstOrDefault<NetworkPlayerInfo>(Element => Element.name == aName);
        }
        public NetworkPlayerInfo GetSelf()
        {
            return m_CurrentPlayers.FirstOrDefault<NetworkPlayerInfo>(Element => Element.name == m_HostName);
        }
        public int GetPlayerIndex(NetworkPlayerInfo aInfo)
        {
            if(string.IsNullOrEmpty(aInfo.name))
            {
                return -1;
            }
            for(int i = 0; i < m_CurrentPlayers.Count; i++)
            {
                if(m_CurrentPlayers[i].name == aInfo.name)
                {
                    return i;
                }
            }
            return -1;
        }
        public void RegisterSpawnedObject(GameObject aObject)
        {
            m_ServerGameObjects.Add(aObject);
            NetworkController controller = aObject.GetComponent<NetworkController>();
            if(controller != null && callbackHandler != null)
            {
                callbackHandler.OnGameObjectSpawned(controller.objectID, controller.objectOwner);
            }
            
        }
        public void UnregisterSpawnedObject(GameObject aObject)
        {
            bool exists = m_ServerGameObjects.Any<GameObject>(Element => Element == aObject);
            NetworkController controller = aObject.GetComponent<NetworkController>();
            if(callbackHandler != null && controller != null && exists)
            {
                callbackHandler.OnGameObjectDespawned(controller.objectID, controller.objectOwner);
            }
            m_ServerGameObjects.Remove(aObject);
        }

        public GameObject GetSpawnedObject(string aObjectId)
        {
            foreach (GameObject gObject in m_ServerGameObjects)
            {
                NetworkController controller = gObject.GetComponent<NetworkController>();
                if (controller.objectID == aObjectId)
                {
                    Debug.Log("Found Object");
                    return gObject;
                }
            }
            Debug.Log("Did not find object");
            return null;
        }
        #endregion

        private void ResetState()
        {
            m_CurrentState = NetworkMode.MatchMaking;
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
            m_CurrentState = NetworkMode.LobbyClient;
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
            m_CurrentState = NetworkMode.LobbyServer;
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
            if(networkState != NetworkMode.LobbyServer)
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
                m_CurrentState = NetworkMode.MatchMaking;
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
            //if(m_CurrentPlayers.Count != MAX_PLAYERS)
            //{
            //    Debug.Log("Need more players");
            //    return;
            //}

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
                m_CurrentState = NetworkMode.GameClient;
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
                networkView.RPC("OnNetworkSpawnObject", RPCMode.Server, aIndex, m_HostName, aPosition, aRotation);
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
                networkView.RPC("OnNetworkDespawnObject", RPCMode.Server, aObjectID, m_HostName);
            }
            else
            {
                OnNetworkDespawnObject(aObjectID, m_HostName);
            }
        }
        public void DespawnObject(GameObject aObject)
        {
            NetworkController controller = aObject.GetComponent<NetworkController>();
            if(controller != null)
            {
                DespawnObject(controller.objectID);
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
            NetworkPlayerInfo player = GetPlayer(aPlayer);
            if(prefab == null)
            {
                Debug.LogError("Failed to SpawnObject, Missing prefab: index(" + aIndex + ").");
                return;
            }
            if(string.IsNullOrEmpty(player.name))
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
            NetworkController netController = obj.GetComponent<NetworkController>();
            NetworkView netView = obj.networkView;
            if(netView != null && netController != null)
            {
                string objectID = Guid.NewGuid().ToString();


                Debug.Log("Created ID: " + objectID);

                netView.RPC("OnReceiveServerInfo", RPCMode.OthersBuffered, netView.viewID, player.name, playerIndex, objectID);
                netController.ReceiveServerInfo(netView.viewID, player.name, playerIndex, objectID);
                RegisterSpawnedObject(obj);
            }
            else if(netView == null)
            {
                Debug.LogWarning("Spawned a gameobject on the network but there is no network view associated with it.");
            }
            else if(netController == null)
            {
                Debug.LogWarning("Spawned a gameobject on the network but there is no network controller associated with it.");
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
                networkView.RPC("OnNetworkDestroyObject", RPCMode.OthersBuffered, aObjectID);
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

    }

}

