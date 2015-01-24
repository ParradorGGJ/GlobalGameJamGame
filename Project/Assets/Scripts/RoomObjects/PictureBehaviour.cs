using UnityEngine;
using System.Collections;


namespace Parrador
{
    public class PictureBehaviour : GenericObjectBehaviour
    {

        // Use this for initialization
        void Start()
        {
            objectType = ObjectType.Picture;

            //TODO: check whatever is needed for Picture to function
            //if (false)
            //{
            //    Debug.Log(objectType + " ObjectType not set up properly. ID: " + GetInstanceID());
            //}
        }

        // Update is called once per frame
        void Update()
        {
            UpdateState();  //only here while testing, will be called through interfact externally
        }

        public override void ObjectSpecificStateUpate()
        {
            //TODO: Whatever the Picture does
        }
    }
}