using UnityEngine;
using System;
using System.Collections;
namespace Parrador
{
    [Serializable]
    public class NetworkState
    {
        public float timeStamp { get; set; }
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
    
        public NetworkState()
        {
    
        }
        public NetworkState(float aTimeStamp, Vector3 aPosition, Quaternion aRotation)
        {
            timeStamp = aTimeStamp;
            position = aPosition;
            rotation = aRotation;
        }
    }

}

