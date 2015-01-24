using UnityEngine;
using System.Collections;

namespace Parrador
{

    public class InteractiveObject : MonoBehaviour
    {
        [SerializeField]
        private bool m_Used = false;

        [SerializeField]
        private Transform m_UsedState = null;

        [SerializeField]
        private Transform m_NormalState = null;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            UpdateState();
        }

        public void UpdateState()
        {
            if (m_Used)
            {
                transform.position = m_UsedState.position;
                transform.rotation = m_UsedState.rotation;
            }
            else
            {
                transform.position = m_NormalState.position;
                transform.rotation = m_NormalState.rotation;
            }

        }

    }
}