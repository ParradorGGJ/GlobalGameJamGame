using UnityEngine;
using System.Collections;


namespace Parrador
{
    public class WasteBasketBehaviour : GenericObjectBehaviour
    {

        [SerializeField]
        private Transform m_UsedStateTransform = null;

        [SerializeField]
        private Transform m_NormalStateTransform = null;

        // Use this for initialization
        void Start()
        {
            objectType = ObjectType.WasteBasket;
            if ( (m_UsedStateTransform == null || m_NormalStateTransform == null) )
            {
                Debug.Log(objectType + " ObjectType not set up properly. ID: " + GetInstanceID());
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateState();  //only here while testing, will be called through interfact externally
        }


        private void UpdateObjectPositionRotation()
        {
            if (m_UsedStateTransform == null || m_NormalStateTransform == null) { return; }
            
            if (used)
            {
                transform.position = m_UsedStateTransform.position;
                transform.rotation = m_UsedStateTransform.rotation;
            }
            else
            {
                transform.position = m_NormalStateTransform.position;
                transform.rotation = m_NormalStateTransform.rotation;
            }
        }

        public override void ObjectSpecificStateUpate()
        {
            UpdateObjectPositionRotation();
        }

    }
}