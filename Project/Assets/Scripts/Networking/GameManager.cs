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
        private float m_TimeLimit = 60.0f;
        private float m_TimeRemaining;

        //[SerializeField]

        private void Start()
        {
            if(NetworkManager.instance != null)
            {
                NetworkManager.instance.gameCallbackHandler = this;
                NetworkManager.instance.callbackHandler = this;
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
            instance.m_TimeRemaining += aBonusTime;
        }

        public void OnStateChange(NetworkID aObjectID, object aState)
        {
            GenericObjectBehaviour gob = aObjectID.GetComponent<GenericObjectBehaviour>();
            if(gob != null)
            {
                gob.OnStateChange(aState);
            }
        }

        public void OnGameStart()
        {
            //Gets called when the GameMode officially starts. (All players loaded).
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


        public CorridorController corridorController
        {
            get { return m_CorridorController; }
            set { m_CorridorController = value; }
        }

        public float timeRemaining
        {
            get { return m_TimeRemaining; }
            set { m_TimeRemaining = value; }
        }






        
    }
}