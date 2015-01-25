using UnityEngine;
using System;
using System.Collections.Generic;

namespace Parrador
{

    [Serializable]
    public struct InteractiveObjectInfo
    {
        [SerializeField]
        private Transform m_Location;
        [SerializeField]
        private string m_PrefabName;

        public Transform location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }
        public string prefabName
        {
            get { return m_PrefabName; }
            set { m_PrefabName = value; }
        }
    }

    public enum DeteriorationLevel
    {
        Prestine,
        LowGrunge,
        MediumGrunge,
        HeavyGrunge
    }

    public class Room : MonoBehaviour
    {
        [SerializeField]
        private RoomType m_UniqueRoomType;

        [SerializeField]
        private List<InteractiveObjectInfo> m_InteractiveObjects = new List<InteractiveObjectInfo>();

        [SerializeField]
        private Transform m_TransitionLocation;

        [SerializeField]
        private int m_TimesVisitedByOtherPlayer = 0;

        //[SerializeField]
        //private bool m_UpdateDeteriorationState = false;

        [SerializeField]
        private DeteriorationLevel m_Deterioration = DeteriorationLevel.Prestine;

        [SerializeField]
        private MeshRenderer m_RoomRenderer;

        [SerializeField]
        private Material[] m_RoomMaterials;

        // Use this for initialization
        void Start()
        {
            NetworkMode networkState = NetworkWorld.GetNetworkState();
            if(networkState == NetworkMode.GameClient || networkState == NetworkMode.GameServer)
            {
                foreach(InteractiveObjectInfo objInfo in m_InteractiveObjects)
                {
                    if(networkState == NetworkMode.GameServer)
                    {
                        int prefabIndex = NetworkWorld.GetPrefabIndex(objInfo.prefabName);
                        if (prefabIndex != -1)
                        {
                            if (objInfo.location != null)
                            {
                                NetworkWorld.SpawnObject(prefabIndex, objInfo.location.position, objInfo.location.rotation);
                            }
                            else
                            {
                                NetworkWorld.SpawnObject(prefabIndex, transform.position, Quaternion.identity);
                            }
                        }                    
                    }
                    Destroy(objInfo.location.gameObject);
                }


            }
            else
            {
                Debug.LogError("Created a room while not playing the game...");
            }
        }


        void Update()
        {
            //if (m_UpdateDeteriorationState)
            //{
            //    //testing
            //    int newState = (int)m_Deterioration + 1;
            //    if (newState > (int)DeteriorationLevel.HeavyGrunge)
            //    {
            //        newState = 0;
            //    }
            //    m_Deterioration = (DeteriorationLevel)newState;
            //    UpdateMaterial(newState);
            //    //end testing
            //    m_UpdateDeteriorationState = false;
            //}

        }

        private void UpdateMaterial(int aMaterialIndex)
        {
            if (m_RoomRenderer == null)
            {
                Debug.Log("renderer null :(");
                return;
            }
            if (aMaterialIndex >= m_RoomMaterials.Length)
            {
                Debug.Log("index out of range for materials");
                return;
            }

            m_RoomRenderer.material = m_RoomMaterials[aMaterialIndex];
        }

        public void UpdateRoomState()
        {
            float timePercentage = Mathf.Clamp01(GameManager.instance.timeRemaining / GameManager.instance.timeLimit);

            if (timePercentage < 0.25f)
            {
                m_Deterioration = DeteriorationLevel.HeavyGrunge;
            }
            else if (timePercentage < 0.5f)
            {
                m_Deterioration = DeteriorationLevel.MediumGrunge;
            }
            else if (timePercentage < 0.75f)
            {
                m_Deterioration = DeteriorationLevel.LowGrunge;
            }
            else
            {
                m_Deterioration = DeteriorationLevel.Prestine;
            }

            int currentState = (int)m_Deterioration;
            UpdateMaterial(currentState);

        }

        public RoomType uniqueID
        {
            get { return m_UniqueRoomType; }
        }

        public Transform transitionLocation
        {
            get { return m_TransitionLocation; }
        }
    }
}