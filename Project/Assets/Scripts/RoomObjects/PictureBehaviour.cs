using UnityEngine;
using System;
using System.Collections;


namespace Parrador
{
    public class PictureBehaviour : GenericObjectBehaviour
    {

        [SerializeField]
        private Transform m_Picture = null;
        [SerializeField]
        private Vector3 m_RotationUsed = Vector3.zero;
        [SerializeField]
        private Vector3 m_RotationNotUsed = Vector3.zero;
        // Use this for initialization
        protected override void Start()
        {
 	         base.Start();
             objectType = ObjectType.Picture;
             used = false;
             UpdateState();
        }

        void OnTriggerStay(Collider aCollider)
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
            if(m_Picture == null)
            {
                return;
            }
            if(used)
            {
                m_Picture.rotation = Quaternion.Euler(m_RotationUsed);
            }
            else
            {
                m_Picture.rotation = Quaternion.Euler(m_RotationNotUsed);
            }

        }
        public override void OnStateChange(object aState)
        {
            try
            {
                used = (bool)aState;
                UpdateState();
            }
            catch(Exception aException)
            {
                Debug.LogException(aException);
            }
        }
    }
}