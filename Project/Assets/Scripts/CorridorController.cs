using UnityEngine;
using System.Collections;

namespace Parrador
{

    public class CorridorController : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] m_TransitionPoints = null;


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public GameObject GetRandomTransitionPoint()
        {
            if (m_TransitionPoints == null) { return null; }

            int randomRoom;
            randomRoom = Random.Range(0, m_TransitionPoints.Length);

            return m_TransitionPoints[randomRoom];
        }

    }
}