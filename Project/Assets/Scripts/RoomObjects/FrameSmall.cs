using UnityEngine;
using System;
using System.Collections;

namespace Parrador
{

    public class FrameSmall : GenericObjectBehaviour
    {
        [SerializeField]
        private Transform m_Frame = null;

        [SerializeField]
        private Vector3 m_NormalPosition = Vector3.zero;
        [SerializeField]
        private Vector3 m_FlippedPosition = Vector3.zero;

        [SerializeField]
        private Vector3 m_NormalRotation = Vector3.zero;
        [SerializeField]
        private Vector3 m_FlippedRotation = Vector3.zero;

        protected override void Start()
        {
            base.Start();
            objectType = ObjectType.SmallFrame;
        }

        private void OnTriggerStay(Collider aCollider)
        {
            if (aCollider.CompareTag("Player") == false) { return; }

            if (Input.GetKeyUp(KeyCode.E))
            {
                Debug.Log("Use");
                NetworkWorld.SendObjectChange(networkID, !used);
            }
        }

        public override void ObjectSpecificStateUpate()
        {
            if (m_Frame == null)
            {
                return;
            }

            if(used)
            {
                m_Frame.position = m_FlippedPosition;
                m_Frame.rotation = Quaternion.Euler(m_FlippedRotation);
            }
            else
            {
                m_Frame.position = m_NormalPosition;
                m_Frame.rotation = Quaternion.Euler(m_NormalRotation);
            }
        }

        public override void OnStateChange(object aState)
        {
            try
            {
                used = (bool)aState;
                UpdateState();
            }
            catch (Exception aException)
            {
                Debug.LogException(aException);
            }
        }


    }
}

