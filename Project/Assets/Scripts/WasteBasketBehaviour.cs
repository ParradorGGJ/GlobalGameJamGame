using UnityEngine;
using System.Collections;


namespace Parrador
{
    public class WasteBasketBehaviour : MonoBehaviour, InteractiveObject
    {

        [SerializeField]
        private bool m_Used = false;

        [SerializeField]
        private Transform m_UsedStateTransform = null;

        [SerializeField]
        private Transform m_NormalStateTransform = null;

        private ObjectType m_ObjectType;


        // Use this for initialization
        void Start()
        {
            m_ObjectType = ObjectType.WasteBasket;
            if ( (m_UsedStateTransform == null || m_NormalStateTransform == null) )
            {
                Debug.Log(m_ObjectType + " ObjectType not set up properly. ID: " + GetInstanceID());
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

            if (m_Used)
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


        #region INTERFACE
        public ObjectType GetObjectType()
        {
            return m_ObjectType;
        }

        public void UpdateState()
        {
            UpdateObjectPositionRotation();
        }

        public void SetState(bool aState)
        {
            m_Used = aState;
        }

        public bool GetState()
        {
            return m_Used;
        }

        #endregion
    }
}