using UnityEngine;
using System.Collections;

namespace Parrador
{

    public class Room : MonoBehaviour
    {
        [SerializeField]
        private RoomType m_UniqueRoomType;

        [SerializeField]
        private InteractiveObject[] m_RoomObjects;

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