using UnityEngine;
using System.Collections;

namespace Parrador
{
    public class NetworkInputServer : MonoBehaviour
    {
        [SerializeField]
        private float m_Speed = 5.0f;
        private CharacterController m_CharacterController = null;
        private NetworkController m_NetworkController = null;

        private float m_HorizontalMotion = 0.0f;
        private float m_VerticalMotion = 0.0f;

        // Use this for initialization
        void Start()
        {
            m_NetworkController = GetComponent<NetworkController>();
            m_CharacterController = GetComponent<CharacterController>();
            //m_Controller = GetComponent<NetworkController>();
            //if (Network.isServer)
            //{
            //    m_CharacterController = GetComponent<CharacterController>();
            //}
        }

        // Update is called once per frame
        void Update()
        {
            if(!m_NetworkController.isOwner && Network.isServer)
            {
                m_CharacterController.Move(new Vector3(m_HorizontalMotion * m_Speed * Time.deltaTime, 0.0f, m_VerticalMotion * m_Speed * Time.deltaTime));
            }
        }



        public void UpdateClientMotion(float aHorizontal, float aVertical)
        {
            m_HorizontalMotion = aHorizontal;
            m_VerticalMotion = aVertical;
        }
    }
}


