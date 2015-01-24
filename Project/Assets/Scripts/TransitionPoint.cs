using UnityEngine;
using System.Collections;


namespace Parrador
{
    public class TransitionPoint : MonoBehaviour
    {
        [SerializeField]
        private RoomType m_DestinationRoom;

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


        void OnTriggerStay(Collider collider)
        {
            if (collider.CompareTag("Player") == false) { return; }

            if (Input.GetKeyUp(KeyCode.E))
            {
                Debug.Log("e pressed in door");
            }
        }
    }
}