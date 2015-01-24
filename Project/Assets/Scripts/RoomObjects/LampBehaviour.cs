using UnityEngine;
using System.Collections;


namespace Parrador
{
    public class LampBehaviour : GenericObjectBehaviour
    {

        [SerializeField]
        private Light m_Light = null;

        // Use this for initialization
        void Start()
        {
            objectType = ObjectType.Lamp;
            if (m_Light == null )
            {
                Debug.Log(objectType + " ObjectType not set up properly. ID: " + GetInstanceID());
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateState();  //only here while testing, will be called through interfact externally
        }

        public override void ObjectSpecificStateUpate()
        {
            m_Light.enabled = used;
        }

    }
}