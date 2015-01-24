using UnityEngine;
using System.Collections;

namespace Parrador
{

    public class InteractiveObject : MonoBehaviour
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

        public void UpdateState()
        {
            switch (m_ObjectType)
            {
                case ObjectType.Bookshelf:
                    //TODO: HANDLE CASE
                    break;
                case ObjectType.Chair:
                    //TODO: HANDLE CASE
                    break;
                case ObjectType.CoffeeTable:
                    //TODO: HANDLE CASE
                    break;
                case ObjectType.Couch:
                    //TODO: HANDLE CASE
                    break;
                case ObjectType.DigitalClock:
                    //TODO: HANDLE CASE
                    break;
                case ObjectType.Lamp:
                    //TODO: HANDLE CASE
                    break;
                case ObjectType.Picture:
                    //TODO: HANDLE CASE
                    break;
                case ObjectType.WasteBasket:
                    //TODO: HANDLE CASE
                    UpdateObjectPositionRotation();
                    break;
            }

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

    }
}