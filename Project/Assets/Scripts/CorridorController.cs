using UnityEngine;
using System.Collections;

namespace Parrador
{

    public class CorridorController : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] m_TransitionLocations = null;

        private TransitionPoint[] m_TransitionScripts = null;

        [SerializeField]
        private Room[] m_RoomList = null;

        [SerializeField]
        private Transform m_SpawnPoint = null;

        // Use this for initialization
        void Start()
        {
            m_TransitionScripts = new TransitionPoint[m_TransitionLocations.Length];

            for (int i = 0 ; i < m_TransitionLocations.Length ; i++)
            {
                m_TransitionScripts[i] = m_TransitionLocations[i].GetComponent<TransitionPoint>();
                m_TransitionScripts[i].controller = this;
            }

            RandomizeTransitionPointDestinations();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public GameObject GetRandomTransitionLocation()
        {
            if (m_TransitionLocations == null)
            {
                Debug.Log("m_transitionlocations is null on corridor controller");
                return null;
            }

            int randomRoom;
            randomRoom = Random.Range(0, m_TransitionLocations.Length);

            return m_TransitionLocations[randomRoom];
        }

        public void RandomizeTransitionPointDestinations()
        {
            for (int i = m_TransitionScripts.Length - 1; i > 0; i--)
            {
                int ran = Random.Range(0, i);
                RoomType temp = m_TransitionScripts[i].DestinationRoom;
                m_TransitionScripts[i].DestinationRoom = m_TransitionScripts[ran].DestinationRoom;
                m_TransitionScripts[ran].DestinationRoom = temp;
            }
            
        }

        public Transform GetRoomTransitionPoint(RoomType aRoomType)
        {
            Transform point = m_SpawnPoint;

            if (aRoomType == RoomType.Corridor)
            { 
                RandomizeTransitionPointDestinations();

                //point.position = GetRandomTransitionLocation().transform.position;
                //point.rotation = GetRandomTransitionLocation().transform.rotation;
                point = GetRandomTransitionLocation().transform;
                return point;
            }

            for (int i = 0; i < m_RoomList.Length; i++)
            {
                if (m_RoomList[i].uniqueID == aRoomType)
                {
                    point = m_RoomList[i].transitionLocation;
                    break;
                }
            }

            return point;
        }

        public Room[] rooms
        {
            get { return m_RoomList; }
            set { m_RoomList = value; }
        }
    }
}