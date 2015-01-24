using UnityEngine;
using System.Collections.Generic;

namespace Parrador
{
    public class LevelStart : MonoBehaviour , INetworkCallbackHandler
    {
        private static LevelStart s_Instance = null;
        public static LevelStart instance
        {
            get
            {
                return s_Instance;
            }
        }

        [SerializeField]
        private Vector3[] m_SpawnPositions = null;


        public GameObject m_Player = null;
        
        void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start() 
        {
            NetworkManager manager = NetworkManager.instance;
            if(manager == null)
            {
                Debug.LogError("Failed to find network manager");
            }
            else
            {
                manager.callbackHandler = this;
            }
	    }

        // Update is called once per frame
        void Update()
        {
            NetworkManager manager = NetworkManager.instance;
            if(manager != null)
            {
                if(manager.networkState == NetworkMode.GameClient || manager.networkState == NetworkMode.GameServer)
                {
                    HandleSpawning(manager);
                }
            }
        }

        void HandleSpawning(NetworkManager manager)
        {
            int playerIndex = manager.GetPlayerIndex(manager.GetSelf());
            Vector3 spawnPosition = Vector3.zero;
            if(playerIndex >= 0 && playerIndex <= m_SpawnPositions.Length)
            {
                spawnPosition = m_SpawnPositions[playerIndex];
            }

           if(Input.GetKeyDown(KeyCode.P))
           {
               if(m_Player == null)
               {
                   manager.SpawnObject(0, spawnPosition, Quaternion.identity);
               }
               else
               {
                   if(manager.DespawnObject(m_Player))
                   {
                       m_Player = null;
                   }
               }
           }


        }

        public void OnGameStart()
        {

        }
        public void OnGameObjectSpawned(string aID)
        {
            NetworkManager manager = NetworkManager.instance;
            if(manager != null && m_Player == null)
            {
                m_Player = manager.GetSpawnedObject(aID);
            }
        }
    }

}

