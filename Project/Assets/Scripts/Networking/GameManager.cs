using UnityEngine;
using System.Collections;

namespace Parrador
{
    public class GameManager : MonoBehaviour
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


        public static void AddTime(float aBonusTime)
        {
            instance.m_TimeRemaining += aBonusTime;
        }


        public static CorridorController corridorController
        {
            get { return instance.m_CorridorController; }
            set { instance.m_CorridorController = value; }
        }

        public static float timeRemaining
        {
            get { return instance.m_TimeRemaining; }
            set { instance.m_TimeRemaining = value; }
        }


    }
}