using UnityEngine;
using System.Collections.Generic;

namespace Parrador
{
    public class GameManager : MonoBehaviour, IGameCallbackHandler, INetworkCallbackHandler
    {

        #region SINGLETON

        private static GameManager s_Instance = null;

        public static GameManager instance
        {
            get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        }

        private static void CreateInstance()
        {
            GameObject world = GameObject.Find("_World");
            if (world == null)
            {
                world = new GameObject("_World");
                world.transform.position = Vector3.zero;
                world.transform.rotation = Quaternion.identity;
            }
            s_Instance = world.GetComponent<GameManager>();
            if (s_Instance == null)
            {
                s_Instance = world.AddComponent<GameManager>();
            }
        }

        private static bool SetInstance(GameManager aInstance)
        {
            if (s_Instance != null && s_Instance != aInstance)
            {
                return false;
            }
            s_Instance = aInstance;
            return true;
        }

        private static void DestroyInstance(GameManager aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion

        [SerializeField]
        private CorridorController m_CorridorController;

        [SerializeField]
        private float m_TimeLimit = 420.0f;
        private float m_TimeRemaining;

        [SerializeField]
        private bool m_GameOver = false;
        private bool m_GameOverState = false; // true = win
        
        private bool m_GameRunning = false;

        private void Awake()
        {
            if(NetworkManager.instance != null)
            {
            Debug.Log("Setting Callbakcs");
                NetworkManager.instance.gameCallbackHandler = this;
                NetworkManager.instance.callbackHandler = this;
            }
        }
        
        void Update()
        {
        	if(m_GameRunning && NetworkWorld.IsHost())
        	{
        		m_TimeRemaining -= Time.deltaTime;
        		NetworkWorld.SendSetTime(m_TimeRemaining);
        		
        		if(m_TimeRemaining <= 0.0f && !m_GameOver)
        		{
        			NetworkWorld.SendGameOver();
        		}
        	}
        
        	
        }
        
        private void OnDestroy()
        {
            if(NetworkManager.instance != null)
            {
                NetworkManager.instance.gameCallbackHandler = null;
                NetworkManager.instance.callbackHandler = this;
            }
        }

        public void AddTime(float aBonusTime)
        {
            Player player = NetworkWorld.GetSelf();
            NetworkWorld.SendAddTime(player, 5.0f);
        }

        public void OnStateChange(NetworkID aObjectID, object aState)
        {
            GenericObjectBehaviour gob = aObjectID.GetComponent<GenericObjectBehaviour>();
            if(gob != null)
            {
                gob.OnStateChange(aState);
            }
        }

        public float GetGameTime()
        {
            return m_TimeRemaining;
        }
        public void SetGameTime(float aTime)
        {
            m_TimeRemaining = aTime;
        }

        public void OnGameStart()
        {
        Debug.Log("Sajkltja");
            //Gets called when the GameMode officially starts. (All players loaded).
            NetworkWorld.SendSetTime(m_TimeLimit);
            m_GameRunning = true;
        }

        public void OnGameObjectSpawned(string aObjectID, string aOwner)
        {
            //NetworkWorld.GetSelf();
            //NetworkWorld.GetPlayer(aOwner);
            //NetworkWorld.GetGameObject(aObjectID);
        }

        public void OnGameObjectDespawned(string aObjectID, string aOwner)
        {

        }
        
        public void OnGameOver()
        {
        	Debug.Log("Game Over");
        	m_GameRunning = false;
        	m_GameOver = true;
        	Application.LoadLevel("End_Game_Scene");
        }

        public CorridorController corridorController
        {
            get { return m_CorridorController; }
            set { m_CorridorController = value; }
        }

        public float timeRemaining
        {
            get { return m_TimeRemaining; }
            //set { m_TimeRemaining = value; }
        }

		public bool gameOver
		{
			get { return m_GameOver;  }
			set { m_GameOver = value; }
		}  
		
		public bool gameOverState
		{
			get { return m_GameOverState;	}
			set { m_GameOverState = value;	}
		}

        public float timeLimit
        {
            get { return m_TimeLimit; }
            set { m_TimeLimit = value; }
        }
    }
}