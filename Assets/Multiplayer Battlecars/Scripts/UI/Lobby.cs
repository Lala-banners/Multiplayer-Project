using System.Collections.Generic;
using UnityEngine;
using Battlecars.Networking;

namespace Battlecars.UI
{
    public class Lobby : MonoBehaviour
    {
        private List<LobbyPlayerSlot> leftTeamSlots = new List<LobbyPlayerSlot>();
        private List<LobbyPlayerSlot> rightTeamSlots = new List<LobbyPlayerSlot>();

        [SerializeField] private GameObject leftTeamHolder, rightTeamHolder;
        
        //Flipping bool to determine which column the connected player will be added to.
        private bool assigningToLeft = true;

        public void OnPlayerConnected(BattlecarsPlayerNet _player)
        {
            bool assigned = false;

            if (assigningToLeft)
            {
                //TRUE POWER OF LAMBDAS
                //Loop through each item in the list and run a lambda with the item at that index.
                leftTeamSlots.ForEach(slot => 
                { 
                    if(assigned)
                    {
                        return;
                    }
                    else if(!slot.isTaken)
                    {
                        slot.AssignPlayer(_player);
                        assigned = true;
                    }
                });
            }
            else
            {

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
    }
}
