using UnityEngine;
using System.Collections;


namespace Parrador
{
    public abstract class GenericObjectBehaviour : MonoBehaviour, InteractiveObject
    {

        [SerializeField]
        private bool m_Used = false;
        [SerializeField]
        private NetworkID m_NetworkID = null;

        private ObjectType m_ObjectType = ObjectType.Bookshelf;

        protected virtual void Start()
        {
            m_NetworkID = GetComponent<NetworkID>();
        }

        

        #region ACCESSORS
        public bool used
        {
            get { return m_Used; }
            set { m_Used = value; }
        }
        public NetworkID networkID
        {
            get { return m_NetworkID; }
            set { m_NetworkID = value; }
        }
        public ObjectType objectType
        {
            get { return m_ObjectType; }
            set { m_ObjectType = value; }
        }
        #endregion

        /// <summary>
        /// Object specific state update function
        /// </summary>
        public abstract void ObjectSpecificStateUpate();
        public abstract void OnStateChange(object aState);

        #region INTERFACE
        public void UpdateState()
        {
            ObjectSpecificStateUpate();
        }
        
        public ObjectType GetObjectType()
        {
            return m_ObjectType;
        }

        public void SetState(bool aState)
        {
            m_Used = aState;
        }

        public bool GetState()
        {
            return m_Used;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void SetPosition(Vector3 aPosition)
        {
            transform.position = aPosition;
        }

        public Quaternion GetRotation()
        {
            return transform.rotation;
        }

        public void SetRotation(Quaternion aRotation)
        {
            transform.rotation = aRotation;
        }

        #endregion
    }
}