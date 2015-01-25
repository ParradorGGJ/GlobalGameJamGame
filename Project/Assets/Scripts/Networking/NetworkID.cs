using UnityEngine;
using System.Collections;

namespace Parrador
{

    /// <summary>
    /// Acts as a form of object ownership.
    /// </summary>
    public class NetworkID : MonoBehaviour
    {
        /// <summary>
        /// The name of the player who owns this object.
        /// </summary>
        private string m_OwnerName = string.Empty;
        /// <summary>
        /// The index of the name of the player who owns this object from the server.
        /// </summary>
        private int m_OwnerNameIndex = 0;
        /// <summary>
        /// The id of this object within the game world.
        /// </summary>
        private string m_ObjectID = string.Empty;
        /// <summary>
        /// A reference to the name of the player who owns this object.
        /// </summary>
        private string m_Self = string.Empty;

   
        private void Start()
        {
            NetworkManager manager = NetworkManager.instance;
            if(manager != null)
            {
                //This should never fail.
                m_Self = manager.GetSelf().name;
            }
        }

        private void OnDestroy()
        {
            NetworkManager manager = NetworkManager.instance;
            if(manager != null)
            {
                manager.UnregisterSpawnedObject(gameObject);
            }
        }

        /// <summary>
        /// Server Only
        /// </summary>
        public void ReceiveServerInfo(NetworkViewID aViewID, string aOwnerName, int aOwnerNameIndex, string aObjectID)
        {
            if(Network.isServer && aViewID == networkView.viewID)
            {
                m_OwnerName = aOwnerName;
                m_OwnerNameIndex = aOwnerNameIndex;
                m_ObjectID = aObjectID;
            }
        }

        /// <summary>
        /// Client Only.
        /// </summary>
        [RPC]
        private void OnReceiveServerInfo(NetworkViewID aViewID, string aOwnerName, int aOwnerNameIndex, string aObjectID)
        {
            if(aViewID == networkView.viewID)
            {
                m_OwnerName = aOwnerName;
                m_OwnerNameIndex = aOwnerNameIndex;
                m_ObjectID = aObjectID;
            }

            if(Network.isClient)
            {
                ///Register all spawned objects with the network manager if were a client. 
                ///Otherwise we have no idea what objects are out there.
                ///The server does this internally in NetworkManager.
                NetworkManager manager = NetworkManager.instance;
                if(manager != null)
                {
                    manager.RegisterSpawnedObject(gameObject);
                }
            }
        }
        public string ownerName
        {
            get { return m_OwnerName; }
        }
        public int ownerNameIndex
        {
            get { return m_OwnerNameIndex; }
        }
        public string objectID
        {
            get { return m_ObjectID; }
        }
        public bool isOwnerSelf
        {
            get { return m_Self == m_OwnerName; }
        }
    }

}

