using UnityEngine;
using System.Collections;

namespace Parrador
{
    public class NetworkController : MonoBehaviour
    {
        [SerializeField]
        private Transform m_Observed = null;
        [SerializeField]
        private float m_PingMargin = 0.2f;

        private NetworkInputClient m_ClientInput = null;
        private NetworkInputServer m_ServerInput = null;

        private float m_ClientPing = 0.0f;
        private NetworkState[] m_NetworkStates = new NetworkState[20];

        // -- Network Info
        [SerializeField]
        private string m_PlayerName = string.Empty;
        [SerializeField]
        private int m_PlayerIndex = 0;
        [SerializeField]
        private string m_ObjectID = string.Empty;
        private string m_Self = string.Empty;

        private void Start()
        {
            m_ClientInput = GetComponent<NetworkInputClient>();
            m_ServerInput = GetComponent<NetworkInputServer>();
            NetworkManager manager = NetworkManager.instance;
            if(manager != null)
            {
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

        public void SendUpdateClientMotion(float aHorizontal, float aVertical)
        {
            if(Network.isClient)
            {
                networkView.RPC("OnReceiveUpdateClientMotion", RPCMode.Server, aHorizontal, aVertical);
            }
            else if(Network.isServer)
            {
                OnReceiveUpdateClientMotion(aHorizontal, aVertical);
            }
            
        }

        [RPC]
        private void OnReceiveUpdateClientMotion(float aHorizontal, float aVertical)
        {
            if(!Network.isServer)
            {
                Debug.LogError("OnReceiveUpdateClientMotion was called on a machine that is not the server.");
                return;
            }
            m_ServerInput.UpdateClientMotion(aHorizontal, aVertical);
        }
        public void ReceiveServerInfo(NetworkViewID aViewID, string aPlayerName, int aPlayerIndex, string aObjectID)
        {
            if(Network.isServer && aViewID == networkView.viewID)
            {
                m_PlayerName = aPlayerName;
                m_PlayerIndex = aPlayerIndex;
                m_ObjectID = aObjectID;
            }
        }

        [RPC]
        private void OnReceiveServerInfo(NetworkViewID aViewID, string aPlayerName, int aPlayerIndex, string aObjectID)
        {
            if (aViewID == networkView.viewID)
            {
                m_PlayerName = aPlayerName;
                m_PlayerIndex = aPlayerIndex;
                m_ObjectID = aObjectID;
            }

            if(Network.isClient)
            {
                NetworkManager manager = NetworkManager.instance;
                if(manager != null)
                {
                    manager.RegisterSpawnedObject(gameObject);
                }
            }
        }

        private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo aInfo)
        {
            Vector3 pos = m_Observed.position;
            Quaternion rot = m_Observed.rotation;
            if(stream.isWriting)
            {
                stream.Serialize(ref pos);
                stream.Serialize(ref rot);
            }
            else
            {
                stream.Serialize(ref pos);
                stream.Serialize(ref rot);


                if(m_ClientInput != null)
                {
                    m_ClientInput.LerpToTarget(pos, rot);
                }

                for(int i = m_NetworkStates.Length -1; i >= 1; i--)
                {
                    m_NetworkStates[i] = m_NetworkStates[i - 1];
                }
                m_NetworkStates[0] = new NetworkState((float)aInfo.timestamp, pos, rot);

            }

        }

        private void Update()
        {
            //This moves the player for all peers - the server
            if(Network.player == m_ClientInput.owner || Network.isServer)
            {
                return;
            }

            m_ClientPing = (Network.GetAveragePing(Network.connections[0]) / 100.0f) + m_PingMargin;
            float interpolationTime = (float)Network.time - m_ClientPing;

            if(m_NetworkStates[0] == null)
            {
                m_NetworkStates[0] = new NetworkState(0.0f, transform.position, transform.rotation);
            }

            if(m_NetworkStates[0].timeStamp > interpolationTime)
            {
                for(int i = 0; i < m_NetworkStates.Length; i++)
                {
                    if(m_NetworkStates[i] == null)
                    {
                        continue;
                    }

                    if(m_NetworkStates[i].timeStamp <= interpolationTime || i == m_NetworkStates.Length-1)
                    {
                        NetworkState bestTarget = m_NetworkStates[Mathf.Max(i - 1, 0)];
                        NetworkState bestStart = m_NetworkStates[i];

                        float timediff = bestTarget.timeStamp - bestStart.timeStamp;
                        float lerpTime = 0.0f;

                        if(timediff > 0.0001f)
                        {
                            lerpTime = ((interpolationTime - bestStart.timeStamp) / timediff);
                        }

                        transform.position = Vector3.Lerp(bestStart.position, bestTarget.position, lerpTime);
                        transform.rotation = Quaternion.Slerp(bestStart.rotation, bestTarget.rotation, lerpTime);
                        return;
                    }
                }
            }
            else
            {
                NetworkState state = m_NetworkStates[0];
                transform.position = Vector3.Lerp(transform.position, state.position, 0.5f);
                transform.rotation = Quaternion.Slerp(transform.rotation, state.rotation, 0.5f);
            }
        }

        public string objectOwner
        {
            get { return m_PlayerName; }
        }
        public int objectOwnerIndex
        {
            get { return m_PlayerIndex; }
        }
        public string objectID
        {
            get { return m_ObjectID; }
        }
        public string self
        {
            get { return m_Self; }
        }
        public bool isOwner
        {
            get { return m_Self == m_PlayerName; }
        }
    }
}


