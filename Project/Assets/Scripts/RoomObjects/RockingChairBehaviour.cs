using UnityEngine;
using System;
using System.Collections;

namespace Parrador
{
    public class RockingChairBehaviour : GenericObjectBehaviour
    {
        private Animator m_Animator = null;

        protected override void Start()
        {
            Debug.Log(transform.position);
            base.Start();
            objectType = ObjectType.Chair;
            m_Animator = GetComponentInChildren<Animator>();
            used = false;
            UpdateState();
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
            m_Animator.SetBool("ChairState", used);
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