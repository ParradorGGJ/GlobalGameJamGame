using UnityEngine;
using System.Collections.Generic;

namespace Parrador
{

    public class Room : MonoBehaviour
    {
        [SerializeField]
        private RoomType m_UniqueRoomType;

        [SerializeField]
        private InteractiveObject[] m_RoomObjects;

        [SerializeField]
        private List<NetworkID> m_InteractiveObjects = new List<NetworkID>();

        [SerializeField]
        private Transform m_TransitionLocation;

        [SerializeField]
        private int m_TimesVisitedByOtherPlayer = 0;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Null check results for safety
        /// </summary>
        /// <param name="aObjectType"></param>
        /// <returns></returns>
        public InteractiveObject GetObjectFromRoom(ObjectType aObjectType)
        {
            for (int i = 0; i < m_RoomObjects.Length; i++ )
            {
                if (m_RoomObjects[i].GetObjectType() == aObjectType)
                {
                    return m_RoomObjects[i];
                }
            }

            return null;
        }

        public InteractiveObject[] GetRoomObjects()
        {
            return m_RoomObjects;
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