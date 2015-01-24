using UnityEngine;
using System.Collections;


namespace Parrador
{
    [RequireComponent(typeof(NetworkView))]
    public class NetworkManager : MonoBehaviour
    {
        private static NetworkManager s_Instance = null;
        public static NetworkManager instance
        {
            get { return s_Instance; }
        }

        void Awake()
        {
            if(s_Instance == null)
            {
                s_Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        void OnDestroy()
        {
            if(s_Instance == this)
            {
                s_Instance = null;
            }
        }



        #region SENDERS
        /// <summary>
        /// USE THESE METHODS TO SEND MESSAGES
        /// They wrap client / server functionality
        /// </summary>

        public void SendRoomStateChange(ObjectType aObjectType, RoomType aRoomType, bool aNewState)
        {
            int objectType = (int)aObjectType;
            int roomType = (int)aRoomType;
            if(Network.isServer)
            {
                networkView.RPC("OnChangeRoomStateBool", RPCMode.OthersBuffered, objectType, roomType, aNewState);
                OnChangeRoomStateBool(objectType, roomType, aNewState);
            }
            else if(Network.isClient)
            {
                networkView.RPC("OnRequestChangeRoomStateBool", RPCMode.Server, objectType, roomType, aNewState);
            }
            
        }
        public void SendRoomStateChange(ObjectType aObjectType, RoomType aRoomType, float aNewState )
        {
            int objectType = (int)aObjectType;
            int roomType = (int)aRoomType;
            if (Network.isServer)
            {
                networkView.RPC("OnChangeRoomStateFloat", RPCMode.OthersBuffered, objectType, roomType, aNewState);
                OnChangeRoomStateFloat(objectType, roomType, aNewState);
            }
            else if (Network.isClient)
            {
                networkView.RPC("OnRequestChangeRoomStateFloat", RPCMode.Server, objectType, roomType, aNewState);
            }
        }
        public void SendPlayerEnterRoom(RoomType aRoomType, string aPlayerName)
        {
            if(string.IsNullOrEmpty(aPlayerName))
            {
                Debug.LogError("Invalid player name entered for method SendPlayerEnterRoom");
                return;
            }
            int roomType = (int)aRoomType;
            if(Network.isServer)
            {
                networkView.RPC("OnPlayerEnterRoom", RPCMode.OthersBuffered, roomType, aPlayerName);
                OnPlayerEnterRoom(roomType, aPlayerName);
            }
            else
            {
                networkView.RPC("OnRequestPlayerEnterRoom", RPCMode.Server, roomType, aPlayerName);
            }
        }
        public void SendPlayerEnterCorridor(string aPlayerName)
        {
            if (string.IsNullOrEmpty(aPlayerName))
            {
                Debug.LogError("Invalid player name entered for method SendPlayerEnterRoom");
                return;
            }
            if (Network.isServer)
            {
                networkView.RPC("OnPlayerEnterCorridor", RPCMode.OthersBuffered, aPlayerName);
                OnPlayerEnterCorridor(aPlayerName);
            }
            else
            {
                networkView.RPC("OnRequestPlayerEnterCorridor", RPCMode.Server, aPlayerName);
            }
        }

        #endregion

        #region CLIENT_ONLY_RPC

        [RPC]
        void OnChangeRoomStateBool(int aObjectType, int aRoomType, bool aNewState)
        {
            ObjectType objectType = (ObjectType)aObjectType;
            RoomType roomType = (RoomType)aRoomType;

            //TODO: Search for room, search for object in room and change the state

        }

        [RPC]
        void OnChangeRoomStateFloat(int aObjectType, int aRoomType, float aNewState)
        {
            ObjectType objectType = (ObjectType)aObjectType;
            RoomType roomType = (RoomType)aRoomType;

            //TODO: Search for room, search for object in room and change the state

        }

        [RPC]
        void OnPlayerEnterRoom(int aRoomType, string aPlayerName)
        {
            //TODO: Update the players position to be within x room.

        }

        [RPC]
        void OnPlayerEnterCorridor(string aPlayerName)
        {
            //TODO: Update the players position to be within their corridor.
        }

        #endregion


        #region SERVER_ONLY_RPC

        [RPC]
        void OnRequestChangeRoomStateBool(int aObjectType, int aRoomType, bool aNewState)
        {
            if(!Network.isServer)
            {
                Debug.LogError("OnRequestChangeRoomStateBool was called on a machine that is not the server.");
                return;
            }
            ObjectType objectType = (ObjectType)aObjectType;
            RoomType roomType = (RoomType)aRoomType;
            SendRoomStateChange(objectType, roomType, aNewState);
        }
        [RPC]
        void OnRequestChangeRoomStateFloat(int aObjectType, int aRoomType, float aNewState)
        {
            if(!Network.isServer)
            {
                Debug.LogError("OnRequestChangeRoomStateFloat was called on a machine that is not the server.");
                return;
            }
            ObjectType objectType = (ObjectType)aObjectType;
            RoomType roomType = (RoomType)aRoomType;
            SendRoomStateChange(objectType, roomType, aNewState);
        }

        void OnRequestPlayerEnterRoom(int aRoomType, string aPlayerName)
        {
            if(!Network.isServer)
            {
                Debug.LogError("OnRequestPlayerEnterRoom was called on a machine that is not the server.");
                return;
            }
        }
        void OnRequestPlayerEnterCorridor(string aPlayerName)
        {
            if (!Network.isServer)
            {
                Debug.LogError("OnRequestPlayerEnterCorridor was called on a machine that is not the server.");
                return;
            }
        }

        #endregion
    }

}

