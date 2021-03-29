using UnityEngine;

using UnityEngine.SceneManagement;

using Mirror;

using System.Collections;

using Battlecars.UI;

namespace Battlecars.Networking
{
    public class BattlecarsPlayerNet : NetworkBehaviour
    {
        //Network Time Booyah!
        //byte - Unsigned (positive) number between 0-255
        [SyncVar]
        public byte playerId;
        //SyncVar allows data to be synced across the network
        [SyncVar]
        public string username = "";

        private Lobby lob;
        private bool hasJoinedLobby = false;

        public void SetUsername(string _name)
        {
            if (isLocalPlayer)
            {
                //Only localPlayers can call Commands as they are the only ones who have the authority to talk to the server.
                CmdSetUsername(_name);
            }
        }

        public void AssignPlayerToSlot(bool _left, int _slotId, byte _playerId)
        {
            if (isLocalPlayer)
            {
                CmdAssignPlayerToLobbySlot(_left, _slotId, _playerId);
            }
        }

        #region Commands
        //Always must be public, void and begin with Cmd
        [Command]
        public void CmdSetUsername(string _name) => username = _name;
        [Command]
        public void CmdAssignPlayerToLobbySlot(bool _left, int _slotId, byte _playerId) => RcpAssignPlayerToLobbySlot(_left, _slotId, _playerId);
        #endregion

        #region RPCs (Remote Procedure Calls)
        [ClientRpc]
        public void RcpAssignPlayerToLobbySlot(bool _left, int _slotId, byte _playerId) //This is assigning rcp to player 0/host object
        {
            //If this is running on the host client, don't need to set player
            //to the slot, so ignore this call.
            if (BattlecarsNetworkManager.Instance.isHost)
                return;

            //Find the Lobby in the scene and set the player to the correct slot
            StartCoroutine(AssignPlayerToLobbySlotDelayed(BattlecarsNetworkManager.Instance.GetPlayerForId(_playerId), _left, _slotId));
        }
        #endregion

        #region Coroutines
        private IEnumerator AssignPlayerToLobbySlotDelayed(BattlecarsPlayerNet _player, bool _left, int _slotId)
        {
            Lobby lob = FindObjectOfType<Lobby>();

            while(lob == null)
            {
                yield return null;

                lob = FindObjectOfType<Lobby>();
            }

            //Lobby successfully got, so assign the player
            lob.AssignPlayerToLobbySlot(_player, _left, _slotId);
        }
        #endregion

        private void Update()
        {
            if (BattlecarsNetworkManager.Instance.isHost)
            {
                //Attempt to get Lobby if haven't already joined lobby
                if (lob == null && !hasJoinedLobby)
                    lob = FindObjectOfType<Lobby>();


                //Attempt to join the lobby if haven't already and lobby is set
                if (lob != null && !hasJoinedLobby)
                {
                    hasJoinedLobby = true;
                    lob.OnPlayerConnected(this);
                }
            }
        }

        public override void OnStartClient()
        {
            BattlecarsNetworkManager.Instance.AddPlayer(this);
        }

        /// <summary>
        /// Runs when the object is connected and if is local player.
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            //Load in game menus with the lobby
            SceneManager.LoadSceneAsync("InGameMenus", LoadSceneMode.Additive);
        }

        /// <summary>
        /// Runs when the client is disconnected from the server.
        /// </summary>
        public override void OnStopClient()
        {
            //Remove the playerID from the server
            BattlecarsNetworkManager.Instance.RemovePlayer(playerId);
        }
    }
}
