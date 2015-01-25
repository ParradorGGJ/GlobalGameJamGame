using UnityEngine;
using System;
using System.Collections;


namespace Parrador
{
    public class LampBehaviour : GenericObjectBehaviour
    {

        [SerializeField]
        private Light m_BottomLight = null;

        [SerializeField]
        private Light m_TopLight = null;

        [SerializeField]
        private AudioSource m_AudioSource = null;

        //[SerializeField]
        //private AudioClip m_AudioClip = null;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            objectType = ObjectType.Lamp;
            if (m_BottomLight == null || m_TopLight == null || m_AudioSource == null)
            {
                Debug.Log(objectType + " ObjectType not set up properly. ID: " + GetInstanceID());
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public override void ObjectSpecificStateUpate()
        {
            m_BottomLight.enabled = used;
            m_TopLight.enabled = used;
            m_AudioSource.Play();
        }

        void OnTriggerStay(Collider collider)
        {
            if (collider.CompareTag("Player") == false) { return; }

            if (Input.GetKeyUp(KeyCode.E))
            {

                NetworkWorld.SendObjectChange(networkID, !used);
                //used = !used;
                //UpdateState();

                //NetworkManager.instance.SendRoomStateChange(ObjectType.Lamp, RoomType.RoomB, !used);''

            }
        }
        
        public override void OnStateChange(object aState)
        {
            try
            {
                used = (bool)aState;
                UpdateState();
            }
            catch(Exception aException)
            {
                Debug.LogException(aException);
            }
        }
        
    }
}