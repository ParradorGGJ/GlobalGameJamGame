using UnityEngine;
using System.Collections;


namespace Parrador
{
    public class TransitionPoint : MonoBehaviour
    {
        [SerializeField]
        private RoomType m_DestinationRoom;

        [SerializeField]
        private CorridorController m_Corridor = null;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public RoomType DestinationRoom
        {
            get { return m_DestinationRoom; }
            set { m_DestinationRoom = value; }
        }

        public CorridorController controller
        {
            set { m_Corridor = value; }
        }

        void OnTriggerStay(Collider collider)
        {
            if (collider.CompareTag("Player") == false) { return; }

            if (Input.GetKeyUp(KeyCode.E))
            {

                if (m_Corridor == null)
                {
                    Debug.Log("corridor ref null");
                    return;
                }

                Transform movePoint = m_Corridor.GetRoomTransitionPoint(m_DestinationRoom);
                collider.transform.position = movePoint.position;
                collider.transform.rotation = movePoint.rotation;
            }
        }
    }
}