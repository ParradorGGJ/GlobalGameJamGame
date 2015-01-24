using UnityEngine;
using System.Collections;


namespace Parrador
{
    public class LampBehaviour : MonoBehaviour, InteractiveObject
    {

        [SerializeField]
        private bool m_Used = false;

        private ObjectType m_ObjectType;

        [SerializeField]
        private Light m_Light = null;

        // Use this for initialization
        void Start()
        {
            m_ObjectType = ObjectType.Lamp;
            if (m_Light == null )
            {
                Debug.Log(m_ObjectType + " ObjectType not set up properly. ID: " + GetInstanceID());
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateState();  //only here while testing, will be called through interfact externally
        }

        #region INTERFACE
        public ObjectType GetObjectType()
        {
            return m_ObjectType;
        }

        public void UpdateState()
        {
            m_Light.enabled = m_Used;
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