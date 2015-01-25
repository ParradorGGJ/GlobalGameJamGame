using UnityEngine;
using System;

namespace Parrador
{
    [Serializable]
    public class Player
    {
        public string name { get; set; }
        public string externalIP { get; set; }
        public int externalPort { get; set; }
        public string guid { get; set; }
        public string ipAddress { get; set; }
        public int port { get; set; }

        [NonSerialized]
        public NetworkPlayer networkPlayer = default(NetworkPlayer); //This is server only

        public static Player ToPlayer(string aName, NetworkPlayer aPlayer)
        {
            Player player = new Player();
            player.name = aName;
            player.externalIP = aPlayer.externalIP;
            player.externalPort = aPlayer.externalPort;
            player.guid = aPlayer.guid;
            player.ipAddress = aPlayer.ipAddress;
            player.port = aPlayer.port;
            player.networkPlayer = aPlayer;
            return player;
        }

    }
}


