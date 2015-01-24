using UnityEngine;
using System.Collections;

namespace Parrador
{
    public class NetworkInputServer : MonoBehaviour
    {
        [SerializeField]
        private float m_Speed = 5.0f;
        private CharacterController m_CharacterController = null;
        private NetworkController m_Controller = null;

        private float m_HorizontalMotion = 0.0f;
        private float m_VerticalMotion = 0.0f;

        // Use this for initialization
        void Start()
        {
            m_Controller = GetComponent<NetworkController>();
            if (Network.isServer)
            {
                m_CharacterController = GetComponent<CharacterController>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!Network.isServer)
            {
                return;
            }
            if(m_Controller.objectOwner == m_Controller.self)
            {
                //Use Client Input
                return;
            }

            if (m_CharacterController != null)
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


