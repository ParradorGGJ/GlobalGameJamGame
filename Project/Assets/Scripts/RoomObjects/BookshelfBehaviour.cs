using UnityEngine;
using System.Collections;


namespace Parrador
{
    public class BookshelfBehaviour : GenericObjectBehaviour
    {

        // Use this for initialization
        void Start()
        {
            objectType = ObjectType.Bookshelf;
            
            //if (m_Light == null )
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
            //TODO: Whatever the bookshelf does
        }
    }
}