﻿using UnityEngine;
using System.Collections;

namespace Parrador
{
    public static class NetworkWorld
    {
        // -- Start Game
        //Prefab Access
        // -- Name
        // -- Index
        //Player Info Access
        // -- Name
        // -- Index
        //Player Index Access
        // -- Name
        // -- Player Info
        //Self Player Info Access
        // -- GetSelf
        //GameObject Access
        // -- Spawn
        // -- Spawn (Position , Rotation)
        // -- Despawn (Object ID)
        // -- Despawn (Object Reference)
        // -- Register
        // -- Unregister
        // -- GetSpawnedObject
        // -- GetNetworkState
        // -- SendObjectChange
        // -- SendAddTime


        public static void CloseNetwork()
        {
            if(NetworkManager.instance != null)
            {
                NetworkManager.instance.CloseNetwork();
            }
        }
        public static void StartGame()
        {
            if(NetworkManager.instance != null)
            {
                NetworkManager.instance.StartGame();
            }
        }
        public static GameObject GetPrefab(string aName)
        {
            return NetworkManager.instance == null ? null : NetworkManager.instance.GetPrefabByName(aName);
        }
        public static GameObject GetPrefab(int aIndex)
        {
            return NetworkManager.instance == null ? null : NetworkManager.instance.GetPrefabByIndex(aIndex);
        }
        public static int GetPrefabIndex(string aName)
        {
            return NetworkManager.instance == null ? 0 : NetworkManager.instance.GetPrefabIndex(aName);
        }
        public static Player GetPlayer(string aName)
        {
            return NetworkManager.instance == null ? null : NetworkManager.instance.GetPlayer(aName);
        }
        public static Player GetPlayer(int aIndex)
        {
            return NetworkManager.instance == null ? null : NetworkManager.instance.GetPlayer(aIndex);
        }
        public static int GetPlayerIndex(string aName)
        {
            return NetworkManager.instance == null ? -1 : NetworkManager.instance.GetPlayerIndex(aName);
        }
        public static int GetPlayerIndex(Player aInfo)
        {
            return NetworkManager.instance == null ? -1 : NetworkManager.instance.GetPlayerIndex(aInfo);
        }
        
        public static bool IsHost()
        { 
        	return NetworkManager.instance == null ? false : GetSelf() == GetServerHost();
        }
        
        public static bool IsClient()
        {
        	return !IsHost();
        }
        
        public static Player GetServerHost()
        {
            return NetworkManager.instance == null ? null : NetworkManager.instance.GetServerHost();
        }

        public static Player GetSelf()
        {
            return NetworkManager.instance == null ? null : NetworkManager.instance.GetSelf();
        }
        public static void SpawnObject(int aIndex)
        {
            SpawnObject(aIndex, Vector3.zero, Quaternion.identity);
        }
        public static void SpawnObject(int aIndex, Vector3 aPosition, Quaternion aRotation)
        {
            if(NetworkManager.instance != null)
            {
                NetworkManager.instance.SpawnObject(aIndex, aPosition, aRotation);
            }
        }
        public static void DespawnObject(string aObjectID)
        {
            if(NetworkManager.instance != null)
            {
                NetworkManager.instance.DespawnObject(aObjectID);
            }
        }
        public static void DespawnObject(GameObject aObject)
        {
            if(NetworkManager.instance != null)
            {
                NetworkManager.instance.DespawnObject(aObject);
            }
        }
        public static void RegisterGameObject(GameObject aObject)
        {
            if(NetworkManager.instance != null)
            {
                NetworkManager.instance.RegisterSpawnedObject(aObject);
            }
        }
        public static void UnregisterGameObject(GameObject aObject)
        {
            if(NetworkManager.instance != null)
            {
                NetworkManager.instance.UnregisterSpawnedObject(aObject);
            }
        }
        public static GameObject GetGameObject(string aObjectID)
        {
            return NetworkManager.instance == null ? null : NetworkManager.instance.GetSpawnedObject(aObjectID);
        }
        public static NetworkMode GetNetworkState()
        {
            return NetworkManager.instance == null ? NetworkMode.Offline : NetworkManager.instance.networkState;
        }
        public static void SendObjectChange(NetworkID aID, object aNewState)
        {
            if(NetworkManager.instance != null)
            {
                NetworkManager.instance.SendObjectChange(aID, aNewState);
            }
        }
        public static void SendAddTime(Player aPlayer, float aTime)
        {
            if(NetworkManager.instance != null)
            {
                NetworkManager.instance.SendAddTime(aPlayer, aTime);
            }
        }

        public static void SendSetTime(float aTime)
        {
            if(NetworkManager.instance != null)
            {
                NetworkManager.instance.SendSetTime(aTime);
            }
        }
        
        public static void SendGameOver()
        {
        	if(NetworkManager.instance != null)
        	{
        		NetworkManager.instance.SendGameOver();
        	}
        }
    }
}


