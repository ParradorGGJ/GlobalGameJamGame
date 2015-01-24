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

        // Use this for initialization
        void Start()
        {
            m_TransitionScripts = new TransitionPoint[m_TransitionLocations.Length];

            for (int i = 0 ; i < m_TransitionLocations.Length ; i++)
            {
                m_TransitionScripts[i] = m_TransitionLocations[i].GetComponent<TransitionPoint>();
            }

            RandomizeTransitionPointDestinations();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public GameObject GetRandomTransitionLocation()
        {
            if (m_TransitionLocations == null) { return null; }

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

        //public 

    }
}