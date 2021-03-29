using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

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

        private Dictionary<byte, BattlecarsPlayerNet> players = new Dictionary<byte, BattlecarsPlayerNet>();

        /// <summary>
        /// Runs only when connecting to an online scene as a Host.
        /// </summary>
        public override void OnStartHost()
        {
            isHost = true;
        }

        /// <summary>
        /// Attempts to return a player corresponding to the passed id.
        /// If no player is found, returns null (UNACCEPTABLEEEE).
        /// </summary>
        public BattlecarsPlayerNet GetPlayerForId(byte _playerId)
        {
            BattlecarsPlayerNet player;
            players.TryGetValue(_playerId, out player);
            return player;
        }

        /// <summary>
        /// Runs only on the server when the player has connected.
        /// This function is responsible for creating the player object and spawning into the game.
        /// Also responsible for ensuring the connection is aware of the player object.
        /// </summary>
        public override void OnServerAddPlayer(NetworkConnection _connection)
        {
            //Gives next spawn position depending on spawnMode
            Transform spawnPos = GetStartPosition();

            //Spawn a player and try to use spawnPos
            GameObject playerObj = spawnPos != null 
                ? Instantiate(playerPrefab, spawnPos.position, spawnPos.rotation)
                : Instantiate(playerPrefab);

            //Assign player id and add to server based on connection
            AssignPlayerID(playerObj);

            //Associates the player GO to the network connection on the server
            NetworkServer.AddPlayerForConnection(_connection, playerObj);
        }

        /// <summary>
        /// Removes the player with the corresponding id from the dictionary.
        /// </summary>
        public void RemovePlayer(byte _id)
        {
            //If the player is present in the dictionary, remove them
            if (players.ContainsKey(_id))
            {
                players.Remove(_id);
            }
        }

        /// <summary>
        /// Add players to clients.
        /// </summary>
        public void AddPlayer(BattlecarsPlayerNet _player)
        {
            if(!players.ContainsKey(_player.playerId))
            {
                players.Add(_player.playerId, _player);
            }
        }

        protected void AssignPlayerID(GameObject _playerObj)
        {
            //Find available ID in the players dictionary = loop through all keys (playerId) and increment
            byte id = 0;

            //Taken all the keys and sorted them in a sequential list. (System.Linq) 
            //Use sparingly because although powerful, is slow
            List<byte> playerIds = players.Keys.OrderBy(x => x).ToList(); 
            foreach (byte key in playerIds)
            {
                if (id == key)
                    id++;
            }

            //Get the playernet component from the go and assign id
            BattlecarsPlayerNet player = _playerObj.GetComponent<BattlecarsPlayerNet>();
            player.playerId = id;
            players.Add(id, player); //add to dictionary
        }
    }
}
