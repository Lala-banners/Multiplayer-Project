using UnityEngine;
using Battlecars.Networking;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Collections.Generic;

namespace Battlecars.UI
{
    public class ConnectionMenu : MonoBehaviour
    {
        [Header("Connect to server buttons")]
        [SerializeField] private Button hostButton;
        [SerializeField] private Button connectButton;
        [SerializeField] private TextMeshProUGUI ipText;
        [SerializeField] private DiscoGame gameTemplate;

        [Space]
        [Header("Network Manager")]
        [SerializeField] private BattlecarsNetworkManager networkManager;
        [SerializeField] private Transform foundGamesHolder;

        private Dictionary<IPAddress, DiscoGame> discoveredGames = new Dictionary<IPAddress, DiscoGame>();

        // Start is called before the first frame update
        void Start()
        {
            //Make host server button work
            hostButton.onClick.AddListener(() => networkManager.StartHost());
            connectButton.onClick.AddListener(OnClickJoin);

            //Ear
            networkManager.disco.onServerFound.AddListener(OnDetectServer);
            networkManager.disco.StartDiscovery();
        }

        private void OnClickJoin()
        {
            networkManager.networkAddress = ipText.text.Trim((char)8203);
            networkManager.StartClient();
        }

        private void OnDetectServer(DiscoveryResponse _response)
        {
            //Here we have received a server that is broadcasting on the network (visualisation of the ear)
            
            if (!discoveredGames.ContainsKey(_response.EndPoint.Address))
            {
                //We haven't already found a game with this IP, so make it
                DiscoGame game = Instantiate(gameTemplate, foundGamesHolder);
                game.gameObject.SetActive(true);

                //Setup the game using the response and add it to the list
                game.SetUp(_response, networkManager);
                discoveredGames.Add(_response.EndPoint.Address, game);
            }
            else
            {
                DiscoGame discoGame = discoveredGames[_response.EndPoint.Address];
                if(discoGame.GameName != _response.gameName)
                {
                    discoGame.UpdateResponse(_response);
                }
            }
        }
    }
}