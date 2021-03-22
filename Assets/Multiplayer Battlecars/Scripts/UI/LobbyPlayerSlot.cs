using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Battlecars.Networking;

namespace Battlecars.UI
{
    public class LobbyPlayerSlot : MonoBehaviour
    {
        /// <summary>
        /// Assign a player to a slot.
        /// </summary>
        public bool isTaken => player != null;
        private BattlecarsPlayerNet player = null;

        #region UI Stuff
        [SerializeField] private TextMeshProUGUI nameDisplay;
        [SerializeField] private Button playerButton;
        #endregion

        // Set the player in this slot to the passed player
        public void AssignPlayer(BattlecarsPlayerNet _player) => player = _player;


        // Update is called once per frame
        void Update()
        {
            //If the slot is taken, the button shouldn't be interactable. 
            playerButton.interactable = isTaken;

            //If the player is set, then display their name, otherwise display "Awaiting Player"...
            nameDisplay.text = isTaken ? player.username : "Awaiting Player...";
        }
    }
}
