using UnityEngine;
using System.Collections;

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

        }

        public void OnGameStart()
        {

        }
    }

}

