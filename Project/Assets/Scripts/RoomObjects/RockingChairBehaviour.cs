using UnityEngine;
using System.Collections;

namespace Parrador
{
    public class RockingChairBehaviour : GenericObjectBehaviour
    {
        [SerializeField]
        private float m_RotationRangeX = 17.0f;

        [SerializeField]
        private float m_RotationIncrement = 1.0f;

        [SerializeField]    // for visability
        private int m_RotationDirection = 1;

        [SerializeField]
        private float m_RotationFromOrigin = 0.0f;

        [SerializeField]
        private bool m_Resetting = false;

        // Use this for initialization
        void Start()
        {
            objectType = ObjectType.Chair;

            //TODO: check whatever is needed for chair to function
            //if (false)
            //{
            //    Debug.Log(objectType + " ObjectType not set up properly. ID: " + GetInstanceID());
            //}
        }

        // Update is called once per frame
        void Update()
        {
            //if (used)
            //{
            //    ObjectSpecificStateUpate();
            //}
            //
            //if (m_Resetting)
            //{
            //    UpdateResetting();
            //}
        }

        private void UpdateResetting()
        {
            //if (Mathf.Abs(m_RotationFromOrigin) < 0.01f)
            //{
            //    m_Resetting = false;
            //}
            //
            //if (Mathf.Sign(m_RotationFromOrigin) > 0)
            //{
            //    m_RotationDirection = -1;
            //}
            //else
            //{
            //    m_RotationDirection = 1;
            //}
            //
            //Quaternion updatedRotation = transform.rotation;
            //m_RotationFromOrigin = m_RotationIncrement * Time.deltaTime * m_RotationDirection;
            //updatedRotation.x += m_RotationFromOrigin;
            //transform.rotation = updatedRotation;

        }

        public override void ObjectSpecificStateUpate()
        {
            //TODO: Whatever the chair does
            //if (m_Resetting) { return; }
            //
            //if (used == false /*&& Mathf.Abs(transform.rotation.x) < Mathf.Epsilon*/) { return; }
            //
            //
            //Quaternion updatedRotation = transform.rotation;
            //m_RotationFromOrigin = m_RotationIncrement * Time.deltaTime * m_RotationDirection;
            //updatedRotation.x += m_RotationFromOrigin;
            //transform.rotation = updatedRotation;
            //
            //if (Mathf.Abs(m_RotationFromOrigin) > m_RotationRangeX)
            //{
            //    m_RotationDirection *= -1;
            //}

        }
    }
}