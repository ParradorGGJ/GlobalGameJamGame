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