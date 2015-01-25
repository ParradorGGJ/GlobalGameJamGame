using UnityEngine;
using System.Collections;


namespace Parrador
{
    public class LampBehaviour : GenericObjectBehaviour
    {

        [SerializeField]
        private Light m_BottomLight = null;

        [SerializeField]
        private Light m_TopLight = null;

        // Use this for initialization
        void Start()
        {
            objectType = ObjectType.Lamp;
            if (m_BottomLight == null || m_TopLight == null)
            {
                Debug.Log(objectType + " ObjectType not set up properly. ID: " + GetInstanceID());
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public override void ObjectSpecificStateUpate()
        {
            m_BottomLight.enabled = used;
            m_TopLight.enabled = used;
        }

        void OnTriggerStay(Collider collider)
        {
            if (collider.CompareTag("Player") == false) { return; }

            if (Input.GetKeyUp(KeyCode.E))
            {
                used = !used;
                UpdateState();
                
            }
        }
        
    }
}