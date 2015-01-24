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

        public bool redSpawned = false;
        public bool greenSpawned = false;
        public bool blueSpawned = false;

        public GameObject m_Red = null;
        public GameObject m_Blue = null;
        public GameObject m_Yellow = null;

        [SerializeField]
        public List<string> m_SpawnedObjectIDs = new List<string>();

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
                m_Red = manager.GetPrefabByIndex(0);
                m_Blue = manager.GetPrefabByIndex(1);
                m_Yellow = manager.GetPrefabByIndex(2);

                
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
            if(m_Red == null || m_Blue == null || m_Yellow == null)
                {
                    return;
                }

            if(Input.GetKeyDown(KeyCode.U))
            {
                manager.SpawnObject(m_Red.GetInstanceID());
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                manager.SpawnObject(m_Blue.GetInstanceID());
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                manager.SpawnObject(m_Yellow.GetInstanceID());
            }
            if(m_SpawnedObjectIDs.Count > 0 )
            {
                if (Input.GetKeyDown(KeyCode.J))
                {
                    manager.DespawnObject(m_SpawnedObjectIDs[m_SpawnedObjectIDs.Count - 1]);
                    m_SpawnedObjectIDs.RemoveAt(m_SpawnedObjectIDs.Count - 1);
                }
                if (Input.GetKeyDown(KeyCode.K))
                {
                    manager.DespawnObject(m_SpawnedObjectIDs[m_SpawnedObjectIDs.Count - 1]);
                    m_SpawnedObjectIDs.RemoveAt(m_SpawnedObjectIDs.Count - 1);
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    manager.DespawnObject(m_SpawnedObjectIDs[m_SpawnedObjectIDs.Count - 1]);
                    m_SpawnedObjectIDs.RemoveAt(m_SpawnedObjectIDs.Count - 1);
                }
            }
            


        }

        public void OnGameStart()
        {

        }
        public void OnGameObjectSpawned(string aID)
        {
            m_SpawnedObjectIDs.Add(aID);
        }
    }

}

