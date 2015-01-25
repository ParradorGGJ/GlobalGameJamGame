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
        [SerializeField]
        private float m_JumpForce = 10.0f;
        [SerializeField]
        private float m_Gravity = 9.81f;
        [SerializeField]
        private float m_TurnSpeed = 45.0f;

        private CharacterController m_Controller = null;
        private NetworkID m_NetworkID = null;
        private NetworkController m_NetworkController = null;

        private float m_LastHorizontalMotion = 0.0f;
        private float m_LastVerticalMotion = 0.0f;
        private float m_LastMouseMotion = 0.0f;

        private Vector3 m_ServerPosition = Vector3.zero;
        private Quaternion m_ServerRotation = Quaternion.identity;

        
        private Vector3 m_Velocity = Vector3.zero;

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
            m_NetworkID = GetComponent<NetworkID>();
        }


        

        private void Update()
        {
            if(m_NetworkID.isOwnerSelf)
            {
                //Move
                float motionH = Input.GetAxis("Horizontal");
                float motionV = Input.GetAxis("Vertical");
                float mouseX = Input.GetAxis("Mouse X");

                if(motionH != m_LastHorizontalMotion || motionV != m_LastVerticalMotion || mouseX != m_LastMouseMotion)
                {
                    m_LastHorizontalMotion = motionH;
                    m_LastVerticalMotion = motionV;
                    m_LastMouseMotion = mouseX;

                    m_NetworkController.SendUpdateClientMotion(motionH, motionV,mouseX);
                }

                Vector3 moveDirection = new Vector3(m_LastHorizontalMotion, 0.0f, m_LastVerticalMotion);
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= m_Speed;
                moveDirection.y = m_Velocity.y;
                if(!m_Controller.isGrounded)
                {
                    moveDirection.y = 0.0f;
                }

                moveDirection.y -= m_Gravity * Time.deltaTime;
                m_Velocity = moveDirection;
                m_Controller.Move(m_Velocity * Time.deltaTime);

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


    }

}

