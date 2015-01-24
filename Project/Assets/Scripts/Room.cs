using UnityEngine;
using System.Collections;

namespace Parrador
{

    public class Room : MonoBehaviour
    {
        [SerializeField]
        private int m_UniqueID;

        [SerializeField]
        private GameObject[] m_RoomObjects;

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

        public GameObject[] GetRoomObjects()
        {
            return m_RoomObjects;
        }

        public int uniqueID
        {
            get { return m_UniqueID; }
        }

        //public void 
    }
}