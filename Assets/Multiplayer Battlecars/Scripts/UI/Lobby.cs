using System.Collections.Generic;
using UnityEngine;
using Battlecars.Networking;
using TMPro;

namespace Battlecars.UI
{
    public class Lobby : MonoBehaviour
    {
        #region Variables and Properties
        private List<LobbyPlayerSlot> leftTeamSlots = new List<LobbyPlayerSlot>();
        private List<LobbyPlayerSlot> rightTeamSlots = new List<LobbyPlayerSlot>(); 
        
        //Adding server name 
        public string LobbyName => lobbyNameInput.text;

        [SerializeField] private GameObject leftTeamHolder, rightTeamHolder;
        [SerializeField] private TMP_InputField lobbyNameInput;

        //Flipping bool to determine which column the connected player will be added to.
        private bool assigningToLeft = true;

        private BattlecarsPlayerNet localPlayer;
        #endregion

        #region Methods
        public void AssignPlayerToLobbySlot(BattlecarsPlayerNet _player, bool _left, int _slotId)
        {
            //Get the correct slot list depending on the left param
            List<LobbyPlayerSlot> slots = _left ? leftTeamSlots : rightTeamSlots;

            //Assign the player to the relevant slot in the list
            slots[_slotId].AssignPlayer(_player);
        }

        public void OnPlayerConnected(BattlecarsPlayerNet _player)
        {
            bool assigned = false;
            List<LobbyPlayerSlot> slots = assigningToLeft ? leftTeamSlots : rightTeamSlots;
            if (_player.isLocalPlayer)
            {
                localPlayer = _player; 
            }

            //TRUE POWER OF LAMBDAS
            //Loop through each item in the list and run a lambda with the item at that index.
            slots.ForEach(slot =>
            {
                if (assigned)
                {
                    return;
                }
                else if (!slot.isTaken)
                {
                    //If we haven't already assigned the player to a slot and this slot hasn't been taken,
                    //assign this player to this slot and team
                    //as slot has been assigned
                    slot.AssignPlayer(_player);
                    assigned = true;

                    //Which slot is this?
                    //Get the correct slot index and tell the server to set the slot on every client
                    int slotId = slots.IndexOf(slot);
                    localPlayer.AssignPlayerToSlot(assigningToLeft, slotId, _player.playerId);
                }
            });

            for (int i = 0; i < leftTeamSlots.Count; i++)
            {
                LobbyPlayerSlot slot = leftTeamSlots[i];
                if (slot.isTaken)
                    localPlayer.AssignPlayerToSlot(slot.isLeft, i, slot.Player.playerId);
            }

            for (int i = 0; i < rightTeamSlots.Count; i++)
            {
                LobbyPlayerSlot slot = rightTeamSlots[i];
                if (slot.isTaken)
                    localPlayer.AssignPlayerToSlot(slot.isLeft, i, slot.Player.playerId);
            }

            //Flip the flag so that the next one will end up in the other list.
            assigningToLeft = !assigningToLeft;
        }

        void Start()
        {
            //Fill two lists with their slots
            leftTeamSlots.AddRange(leftTeamHolder.GetComponentsInChildren<LobbyPlayerSlot>());
            rightTeamSlots.AddRange(rightTeamHolder.GetComponentsInChildren<LobbyPlayerSlot>());
        }

        void Update()
        {

        }
        #endregion
    }
}
