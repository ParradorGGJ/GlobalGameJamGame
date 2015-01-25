using UnityEngine;
using System.Collections;


namespace Parrador
{
    public abstract class GenericObjectBehaviour : MonoBehaviour, InteractiveObject
    {

        [SerializeField]
        private bool m_Used = false;

        private ObjectType m_ObjectType;


        #region ACCESSORS
        public ObjectType objectType
        {
            get { return m_ObjectType; }
            set { m_ObjectType = value; }
        }

        public bool used
        {
            get { return m_Used; }
            set { m_Used = value; }
        }
        #endregion

        /// <summary>
        /// Object specific state update function
        /// </summary>
        public abstract void ObjectSpecificStateUpate();


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