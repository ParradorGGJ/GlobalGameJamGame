using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Parrador
{
    public class LobbyBrowser : MonoBehaviour
    {
        private const int MAX_CONNECTIONS = 2;
        private const string GAME_NAME = "Parrador_Online";
        
        [SerializeField]
        private string m_LobbyName = string.Empty;
        [SerializeField]
        private string m_Comment = string.Empty;
        [SerializeField]
        private int m_PortNumber = 25006;

        [SerializeField]
        private GameObject m_PeerPanelPrefab = null;
        [SerializeField]
        private float m_PeerPanelMargin = 5.0f;

        private List<HostData> m_Hosts = new List<HostData>();
        private List<PeerPanel> m_Peers = new List<PeerPanel>();
        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void HostGame()
        {
            OnHostGame();
        }

        public void RefreshHostList()
        {
            MasterServer.RequestHostList(GAME_NAME);
            StartCoroutine(RefreshHostListRoutine());
        }



        void OnHostGame()
        {
            if(string.IsNullOrEmpty(m_LobbyName))
            {
                Debug.LogError("Failed to host server because the lobby name was left empty");
            }

            ///Try to initialize the server
            NetworkConnectionError error = Network.InitializeServer(MAX_CONNECTIONS, m_PortNumber, !Network.HavePublicAddress());
            if(error == NetworkConnectionError.NoError)
            {
                ///If server initialization works register the host with the master server.
                if(string.IsNullOrEmpty(m_Comment))
                {
                    MasterServer.RegisterHost(GAME_NAME, m_LobbyName);
                }
                else
                {
                    MasterServer.RegisterHost(GAME_NAME, m_LobbyName, m_Comment);
                }
            }
            else
            {
                Debug.LogError("Failed to host to server because: " + error);
            }
           
        }

        

        /// <summary>
        /// Waits 3 seconds then polls the host list and updates the local host list.
        /// </summary>
        /// <returns></returns>
        IEnumerator RefreshHostListRoutine()
        {
            yield return new WaitForSeconds(3.0f);
            HostData[] hosts = MasterServer.PollHostList();
            m_Hosts.Clear();
            MasterServer.ClearHostList();
            for (int i = 0; i < m_Peers.Count; i++)
            {
                if (m_Peers[i] != null)
                {
                    Destroy(m_Peers[i]);
                }
            }
            m_Peers.Clear();
                

            if(hosts != null && hosts.Length > 0)
            {
                m_Hosts.AddRange(hosts);
            }

            //TODO: Update the UI

            for (int i = 0; i < m_Hosts.Count; i++ )
            {
                HostData data = m_Hosts[i];

                GameObject gObject = Instantiate(m_PeerPanelPrefab) as GameObject;
                PeerPanel peerPanel = gObject.GetComponent<PeerPanel>();
                if (peerPanel != null)
                {
                    peerPanel.Start();
                    peerPanel.joinButton.onClick.AddListener(() => OnJoin(peerPanel));
                    peerPanel.hostName = "Host Name: " + data.gameType;
                    peerPanel.comment = "Comment: " + data.comment;
                    if(i == 0)
                    {
                        peerPanel.position = Vector2.zero;
                    }
                    else
                    {
                        PeerPanel prev = m_Peers[i - 1];
                        peerPanel.position = prev.position + new Vector2(0.0f,prev.height + m_PeerPanelMargin);
                    }
                    m_Peers.Add(peerPanel);
                }
                else
                {
                    Debug.LogError("Missing PeerPanel");
                }

            }


        }

        void OnJoin(PeerPanel aPanel)
        {
            if(aPanel != null)
            {
                HostData data = m_Hosts.FirstOrDefault<HostData>(Element => Element.gameType == aPanel.hostName);
                if(data != null)
                {
                    NetworkConnectionError error = Network.Connect(data);
                    if(error != NetworkConnectionError.NoError)
                    {
                        Debug.LogError("Failed to connect to server for reason: " + error);
                    }
                }
            }

            
        }

    }

}

