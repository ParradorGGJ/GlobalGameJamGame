using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Parrador
{
    public class LobbyBrowser : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_Lobby = null;
        [SerializeField]
        private GameObject m_HostSettings = null;
        [SerializeField]
        private GameObject m_GameLobby = null;

        [SerializeField]
        private Text m_PlayerCount = null;
        [SerializeField]
        private RectTransform m_ContentPanel = null;

        [SerializeField]
        private GameObject m_PeerPanelPrefab = null;
        [SerializeField]
        private float m_PeerPanelMargin = 5.0f;
        [SerializeField]
        private float m_RefreshRate = 0.3f;
        private List<HostData> m_Hosts = new List<HostData>();
        private List<PeerPanel> m_Peers = new List<PeerPanel>();

        public void Start()
        {
            m_Lobby.SetActive(true);
            m_HostSettings.SetActive(false);
            m_GameLobby.SetActive(false);
        }

        public void Update()
        {
            NetworkManager manager = NetworkManager.instance;
            if(manager != null)
            {
                int playerCount =  manager.connectedPlayers;
                m_PlayerCount.text = playerCount.ToString();
            }
        }

        public void EnterPlayerName(string aName)
        {
            NetworkManager manager = NetworkManager.instance;
            if(manager != null)
            {
                manager.hostName = aName;
            }
        }
        public void EnterLobbyName(string aName)
        {
            NetworkManager manager = NetworkManager.instance;
            if (manager != null)
            {
                manager.lobbyName = aName;
            }
        }
        public void EnterLobbyComment(string aName)
        {
            NetworkManager manager = NetworkManager.instance;
            if (manager != null)
            {
                manager.comment = aName;
            }
        }

        public void HostSetup()
        {
            NetworkManager manager = NetworkManager.instance;
            if (manager != null && !string.IsNullOrEmpty(manager.hostName))
            {
                m_Lobby.SetActive(false);
                m_HostSettings.SetActive(true);
                m_GameLobby.SetActive(false);
            }
        }
        public void LobbySetup()
        {
            m_Lobby.SetActive(true);
            m_HostSettings.SetActive(false);
            m_GameLobby.SetActive(false);
        }
        public void GameLobbySetup()
        {
            NetworkManager manager = NetworkManager.instance;
            if(manager != null && !string.IsNullOrEmpty(manager.hostName) && !string.IsNullOrEmpty(manager.lobbyName))
            {
                m_Lobby.SetActive(false);
                m_HostSettings.SetActive(false);
                m_GameLobby.SetActive(true);
                OnHostGame();
            }
            
        }
        public void StartGame()
        {
            NetworkManager manager = NetworkManager.instance;
            if(manager != null)
            {
                manager.StartGame();
            }
        }


        public void RefreshHostList()
        {
            MasterServer.RequestHostList(NetworkManager.GAME_NAME);
            StartCoroutine(RefreshHostListRoutine());
        }



        void OnHostGame()
        {

            NetworkManager manager = NetworkManager.instance;
            if(manager == null)
            {
                return;
            }

            string lobbyName = manager.lobbyName;
            string comment = manager.comment;
            int portNumber = manager.portNumber;

            if (string.IsNullOrEmpty(lobbyName))
            {
                Debug.LogError("Failed to host server because the lobby name was left empty");
            }
            ///Try to initialize the server
            NetworkConnectionError error = Network.InitializeServer(NetworkManager.MAX_PLAYERS, portNumber, !Network.HavePublicAddress());
            if(error == NetworkConnectionError.NoError)
            {
                ///If server initialization works register the host with the master server.
                if (string.IsNullOrEmpty(comment))
                {
                    MasterServer.RegisterHost(NetworkManager.GAME_NAME, lobbyName);
                }
                else
                {
                    MasterServer.RegisterHost(NetworkManager.GAME_NAME, lobbyName, comment);
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
            yield return new WaitForSeconds(m_RefreshRate);
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


            float height = 0.0f;

            for (int i = 0; i < m_Hosts.Count; i++ )
            {
                HostData data = m_Hosts[i];

                GameObject gObject = Instantiate(m_PeerPanelPrefab) as GameObject;
                PeerPanel peerPanel = gObject.GetComponent<PeerPanel>();
                if (peerPanel != null)
                {
                    peerPanel.Start();
                    peerPanel.SetParent(m_ContentPanel);
                    peerPanel.joinButton.onClick.AddListener(() => OnJoin(peerPanel));
                    peerPanel.hostName = "Server Name: " + data.gameName;
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
                    height += peerPanel.height + m_PeerPanelMargin;
                    m_Peers.Add(peerPanel);
                }
                else
                {
                    Debug.LogError("Missing PeerPanel");
                }

            }

            m_ContentPanel.sizeDelta = new Vector2(m_ContentPanel.sizeDelta.x, height);
        }

        void OnJoin(PeerPanel aPanel)
        {
            if(aPanel != null)
            {
                HostData data = null;
                foreach(HostData hostData in m_Hosts)
                {
                    Debug.Log(hostData.gameName);
                    if (hostData.gameName == aPanel.hostName.Replace("Server Name: ", ""))
                    {
                        data = hostData;
                        break;
                    }
                }
                if(data != null)
                {
                    NetworkConnectionError error = Network.Connect(data);
                    if(error != NetworkConnectionError.NoError)
                    {
                        Debug.LogError("Failed to connect to server for reason: " + error);
                    }
                    else
                    {
                        m_HostSettings.SetActive(false);
                        m_Lobby.SetActive(false);
                        m_GameLobby.SetActive(true);
                    }
                }
                else
                {
                    Debug.Log("Failed to get host data. Searching for: " + aPanel.hostName);
                }
            }
            else
            {
                Debug.Log("No panel");
            }

            
        }

    }

}

