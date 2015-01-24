using UnityEngine;
using System.Collections;

namespace Parrador
{
    public class NetworkInputClient : MonoBehaviour
    {
        [SerializeField]
        private float m_PositionErrorThreshHold = 0.2f;
        [SerializeField]
        private float m_Speed = 5.0f;
        private CharacterController m_Controller = null;
        private NetworkController m_NetworkController = null;

        private NetworkPlayer m_Owner = default(NetworkPlayer);
        private float m_LastHorizontalMotion = 0.0f;
        private float m_LastVerticalMotion = 0.0f;


        private Vector3 m_ServerPosition = Vector3.zero;
        private Quaternion m_ServerRotation = Quaternion.identity;

        void Awake()
        {
            //if (Network.isClient)
            //{
            //    enabled = false;
            //}
        }

        void Start()
        {
            m_Controller = GetComponent<CharacterController>();
            m_NetworkController = GetComponent<NetworkController>();
        }


        

        private void Update()
        {

            if(m_NetworkController.isOwner)
            {
                float motionH = Input.GetAxis("Horizontal");
                float motionV = Input.GetAxis("Vertical");

                if (motionH != m_LastHorizontalMotion || motionV != m_LastVerticalMotion)
                {
                    if (m_NetworkController != null)
                    {
                        m_NetworkController.SendUpdateClientMotion(motionH, motionV);
                    }
                    m_LastHorizontalMotion = motionH;
                    m_LastVerticalMotion = motionV;

                    if (m_Controller != null)
                    {
                        m_Controller.Move(new Vector3(m_LastHorizontalMotion * m_Speed * Time.deltaTime, 0.0f, m_LastVerticalMotion * m_Speed * Time.deltaTime));
                    }
                }
            }

        }

        public void LerpToTarget(Vector3 aServerPosition, Quaternion aServerRotation)
        {
            m_ServerPosition = aServerPosition;
            m_ServerRotation = aServerRotation;

            float distance = Vector3.Distance(transform.position, m_ServerPosition);

            if (distance >= m_PositionErrorThreshHold)
            {
                float lerp = ((1.0f / distance) * m_Speed) / 100.0f;

                transform.position = Vector3.Lerp(transform.position, m_ServerPosition, lerp);
                transform.rotation = Quaternion.Slerp(transform.rotation, m_ServerRotation, lerp);
            }
        }

        public NetworkPlayer owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

    }

}

