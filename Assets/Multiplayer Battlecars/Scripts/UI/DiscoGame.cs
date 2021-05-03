using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Battlecars.Networking;

namespace Battlecars.UI
{
    [RequireComponent(typeof(Button))]
    public class DiscoGame : MonoBehaviour
    {
        //Activate buttons, load game and connect to the game we selected, and start the client.
        [SerializeField] private TextMeshProUGUI ipDisplay;
        private BattlecarsNetworkManager networkManager;
        [SerializeField] private TextMeshProUGUI serverName;

        public void SetUp(string _address, BattlecarsNetworkManager _networkMan)
        {
            ipDisplay.text = _address;
            networkManager = _networkMan;
            Button button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(JoinGame);
        }

        private void JoinGame()
        {
            //When we touch the butt, connect to the server displayed on the butt
            networkManager.networkAddress = ipDisplay.text.Trim((char)8203);
            networkManager.StartClient();
        }

        private void SetUpMatch(string _serverName, BattlecarsNetworkManager _networkMan)
        {
            serverName.text = _serverName;

        }
    }
}
