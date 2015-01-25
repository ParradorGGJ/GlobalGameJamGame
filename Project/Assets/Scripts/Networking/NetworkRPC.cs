using UnityEngine;
using System.Collections;

public static class NetworkRPC 
{
    public const string NETWORK_ID_ON_RECEIVE_SERVER_INFO = "OnReceiveServerInfo";
    public const string NETWORK_MANAGER_ON_UPDATE_CONNECTED_USERS = "OnUpdateConnectedUsers";
    public const string NETWORK_MANAGER_ON_REGISTER_GAME_LOADED = "OnRegisterGameLoaded";
    public const string NETWORK_MANAGER_ON_PLAYER_JOINED = "OnPlayerJoined";
    public const string NETWORK_MANAGER_ON_PLAYER_DISCONNECTED = "OnPlayerDisconnected";
    public const string NETWORK_MANAGER_ON_RECEIVE_REGISTER_RESULT = "OnReceiveRegisterResult";
    public const string NETWORK_MANAGER_ON_REGISTER_COMPLETE = "OnRegisterComplete";
    public const string NETWORK_MANAGER_ON_REGISTER_PLAYER = "OnRegisterPlayer";
    public const string NETWORK_MANAGER_ON_START_GAME = "OnStartGame";
    public const string NETWORK_MANAGER_ON_NETWORK_SPAWN_OBJECT = "OnNetworkSpawnObject";
    public const string NETWORK_MANAGER_ON_NETWORK_DESPAWN_OBJECT = "OnNetworkDespawnObject";
    public const string NETWORK_MANAGER_ON_NETWORK_DESTROY_OBJECT = "OnNetworkDestroyObject";

}
