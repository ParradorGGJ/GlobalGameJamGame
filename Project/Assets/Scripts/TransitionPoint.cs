using UnityEngine;
using System.Collections;


namespace Parrador
{
    public class TransitionPoint : MonoBehaviour
    {
        [SerializeField]
        private RoomType m_DestinationRoom;

        private CorridorController m_Corridor = null;

        private float m_TransitionDistanceBuffer = 0.5f;

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
                    //Debug.Log("getting corridor ref from gamemanager");
                    m_Corridor = GameManager.instance.corridorController;

                    if (m_Corridor == null)
                    {
                        Debug.Log("corridor ref from gamemanager found null");
                        return;
                    }
                }

                Transform movePoint = m_Corridor.GetRoomTransitionPoint(m_DestinationRoom);
                collider.transform.position = movePoint.position + (m_TransitionDistanceBuffer * movePoint.forward);
                collider.transform.rotation = movePoint.rotation;
            }
        }
    }
}