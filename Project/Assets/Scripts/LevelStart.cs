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


        //public GameObject m_Player = null;
        public string m_PrefabName = string.Empty;

        private Stack<string> m_SpawnedObjects = new Stack<string>();
        
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
               NetworkWorld.SpawnObject(NetworkWorld.GetPrefabIndex(m_PrefabName), spawnPosition, Quaternion.identity);
           }
           else if(Input.GetKeyDown(KeyCode.O))
           {
               if(m_SpawnedObjects.Count > 0)
               {
                   string objectID = m_SpawnedObjects.Pop();
                   NetworkWorld.DespawnObject(objectID);
               }
           }


        }

        public void OnGameStart()
        {

        }
        public void OnGameObjectSpawned(string aID, string aOwner)
        {
            Player self = NetworkWorld.GetSelf();
            Player owner = NetworkWorld.GetPlayer(aOwner);

            if(self != null && owner != null)
            {
                if(self.name == owner.name)
                {
                    m_SpawnedObjects.Push(aID);
                }
            }



            //NetworkManager manager = NetworkManager.instance;
            //if(manager != null && m_Player == null)
            //{
            //    string self = manager.GetSelf().name;
            //    if(self == aOwner)
            //    {
            //        m_Player = manager.GetSpawnedObject(aID);
            //        if(m_Player != null)
            //        {
            //            Camera.main.transform.position = m_Player.transform.position;
            //            Camera.main.transform.parent = m_Player.transform;
            //        }
            //    }
            //}

        }

        public void OnGameObjectDespawned(string aID, string aOwner)
        {

            //NetworkManager manager = NetworkManager.instance;
            //if (manager != null && m_Player != null)
            //{
            //    string self = manager.GetSelf().name;
            //    if (self == aOwner)
            //    {
            //        GameObject spawnedObject = manager.GetSpawnedObject(aID);
            //        if(spawnedObject == m_Player)
            //        {
            //            m_Player = null;
            //            Camera.main.transform.parent = null;
            //        }
            //    }
            //}
        }
    }

}

