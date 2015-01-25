using UnityEngine;
using System.Collections;


namespace Parrador
{
    [RequireComponent(typeof (Rigidbody))]
    public class MugBehaviour : GenericObjectBehaviour
    {

        [SerializeField]
        private Transform m_CarryAnchorPoint = null;

        private Rigidbody m_RigidBody = null;

        // Use this for initialization
        void Start()
        {
            objectType = ObjectType.Mug;

            m_RigidBody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_CarryAnchorPoint != null)
            {
                UpdateObjectPositionRotation();
            }
        }


        private void UpdateObjectPositionRotation()
        {
            transform.position = Vector3.Lerp(transform.position, m_CarryAnchorPoint.position, Time.deltaTime * 8.5f);
            transform.rotation = m_CarryAnchorPoint.rotation;
        }

        public override void ObjectSpecificStateUpate()
        {
            if (used == true)
            {
                m_RigidBody.useGravity = false;
            }
            else
            {
                m_CarryAnchorPoint = null;
                m_RigidBody.useGravity = true;
            }
        }

        public override void OnStateChange(object aState)
        {
            //TODO: Handle the state change. The object will be the same as was sent through SendObjectChange
        }

        void OnTriggerStay(Collider collider)
        {
            if (collider.CompareTag("Player") == false) { return; }

            if (Input.GetKeyUp(KeyCode.E))
            {
                used = !used;
                //Debug.Log("E pressed by player, used state = " + used);
                if (used == true)
                {
                    m_CarryAnchorPoint = collider.gameObject.transform.FindChild("CarryAnchor");
                    if (m_CarryAnchorPoint == null) { Debug.Log("anchor not found"); }
                    m_RigidBody.useGravity = false;
                }
                else
                {
                    m_CarryAnchorPoint = null;
                    m_RigidBody.useGravity = true;
                }

                UpdateState();

            }
        }
    }
}