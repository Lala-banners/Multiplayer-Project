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
        [SerializeField] private TextMeshProUGUI serverName;
        public string GameName => response.gameName;

        private BattlecarsNetworkManager networkManager;
        private DiscoveryResponse response;

        public void SetUp(DiscoveryResponse _response, BattlecarsNetworkManager _networkMan)
        {
            UpdateResponse(_response);
            networkManager = _networkMan;

            Button button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(JoinGame);
        }

        public void UpdateResponse(DiscoveryResponse _response)
        {
            response = _response;
            ipDisplay.text = $"<b>{response.gameName}</b>\n{response.EndPoint.Address}";
        }

        private void JoinGame()
        {
            //When we touch the butt, connect to the server displayed on the butt
            networkManager.networkAddress = response.EndPoint.Address.ToString();
            networkManager.StartClient();
        }

        private void SetUpMatch(string _serverName, BattlecarsNetworkManager _networkMan)
        {
            serverName.text = _serverName;
        }
    }
}
