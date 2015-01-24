﻿using UnityEngine;
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

        [SerializeField]
        private ObjectType m_ObjectType;


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            UpdateState();
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

        #endregion

    }
}