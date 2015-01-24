using UnityEngine;
using System;
using System.Collections;

namespace Parrador
{
    [Serializable]
    public struct NetworkPlayerInfo
    {
        public string name { get; set; }
        public NetworkPlayer player { get; set; }

        public NetworkPlayerStreamInfo streamInfo
        {
            get 
            {
                NetworkPlayerStreamInfo info = new NetworkPlayerStreamInfo();
                info.name = name;
                info.externalIP = player.externalIP;
                info.externalPort = player.externalPort;
                info.guid = player.guid;
                info.ipAddress = player.ipAddress;
                info.port = player.port;
                return info;
            }
        }
    }

    [Serializable]
    public struct NetworkPlayerStreamInfo
    {
        public string name;
        public string externalIP;
        public int externalPort;
        public string guid;
        public string ipAddress;
        public int port;

        public NetworkPlayerInfo info
        {
            get
            {
                NetworkPlayer player = new NetworkPlayer(ipAddress, port);
                NetworkPlayerInfo info = new NetworkPlayerInfo();
                info.name = name;
                info.player = player;
                return info;
            }
        }
    }

}

