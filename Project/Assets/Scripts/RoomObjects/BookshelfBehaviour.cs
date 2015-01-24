﻿using UnityEngine;
using System.Collections;


namespace Parrador
{
    public class BookshelfBehaviour : MonoBehaviour, InteractiveObject
    {

        [SerializeField]
        private bool m_Used = false;

        private ObjectType m_ObjectType;



        // Use this for initialization
        void Start()
        {
            m_ObjectType = ObjectType.Bookshelf;
            
            //if (m_Light == null )
            //{
            //    Debug.Log(m_ObjectType + " ObjectType not set up properly. ID: " + GetInstanceID());
            //}
        }

        // Update is called once per frame
        void Update()
        {
            UpdateState();  //only here while testing, will be called through interfact externally
        }

        #region INTERFACE
        public void UpdateState()
        {
            //TODO: however the bookshelf reacts
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