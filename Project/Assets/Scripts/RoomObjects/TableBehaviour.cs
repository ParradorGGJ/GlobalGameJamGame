using UnityEngine;
using System;
using System.Collections;


namespace Parrador
{
    public class TableBehaviour : GenericObjectBehaviour
    {
        [SerializeField]
        private Transform m_Table = null;
        [SerializeField]
        private Vector3 m_NormalRotation = Vector3.zero;
        [SerializeField]
        private Vector3 m_FlippedRotation = Vector3.zero;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            objectType = ObjectType.CoffeeTable;
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
            if(m_Table == null)
            {
                return;
            }

            if(used)
            {
                m_Table.rotation = Quaternion.Euler(m_NormalRotation);
            }
            else
            {
                m_Table.rotation = Quaternion.Euler(m_FlippedRotation);
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