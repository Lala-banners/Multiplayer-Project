using UnityEngine;
using Mirror;

namespace Battlecars.Networking
{
    public class BattlecarsNetworkManager : NetworkManager
    {
        /// <summary>
        /// A reference to the battlecars network manager version of the NetworkManager.
        /// </summary>
        public static BattlecarsNetworkManager Instance => singleton as BattlecarsNetworkManager;

        /// <summary>
        /// Host property (not static)
        /// Whether or not this NetworkManager is the Host.
        /// </summary>
        public bool isHost { get; private set; } = false;

        /// <summary>
        /// Runs only when connecting to an online scene as a Host.
        /// </summary>
        public override void OnStartHost()
        {
            isHost = true;
        }
    }
}
