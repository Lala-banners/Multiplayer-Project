using System;
using System.Net;
using Mirror;
using Mirror.Discovery;
using UnityEngine;
using UnityEngine.Events;

/*
	Discovery Guide: https://mirror-networking.com/docs/Guides/NetworkDiscovery.html
    Documentation: https://mirror-networking.com/docs/Components/NetworkDiscovery.html
    API Reference: https://mirror-networking.com/docs/api/Mirror.Discovery.NetworkDiscovery.html
*/

namespace Battlecars.Networking
{
    //Fine ill do it myself
    [Serializable] public class ServerFoundEvent : UnityEvent<DiscoveryResponse> {}

    /// <summary>
    /// Data to send to the client.
    /// </summary>
    public class DiscoveryRequest : NetworkMessage
    {
        // Add properties for whatever information you want sent by clients
        // in their broadcast messages that servers will consume.

        //The name of the game being sent
        public string gameName;
    }

    /// <summary>
    /// Anything we want converted from the client.
    /// </summary>
    public class DiscoveryResponse : NetworkMessage
    {
        #region Properties for whatever information you want the server to return to
        //The server that sent this message
        //this is a property so that it is not serialized but the clinet
        //fills this up after we receive it
        public IPEndPoint EndPoint { get; set; }

        /// <summary>
        /// A Uniform Resource Identifier is a unique sequence of characters that identifies 
        /// a logical or physical resource used by web technologies. 
        /// URIs may be used to identify anything, including real-world objects, 
        /// such as people and places, concepts, 
        /// or information resources such as web pages and books.
        /// </summary>
        public Uri uri;
        public long serverId;
        //The name of the game being sent
        public string gameName;
        #endregion

        #region Clients for them to display or consume for establishing a connection

        #endregion
    }

    public class BattlecarsNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
    {
        #region Server
        public long ServerId { get; private set; }

        [Tooltip("Transport to be advertised during discovery")]
        //allows the network messages to be sent across, sends the packets.
        public Transport transport; 

        [Tooltip("Invoked when a server is found")]
        //subscribe to this event to connect to server and add games to list
        public ServerFoundEvent onServerFound = new ServerFoundEvent();

        public override void Start()
        {
            ServerId = RandomLong();

            //if transport wasn't set in the inspector,
            //find the active one, activeTransport is set in awake()
            if (transport == null)
                transport = Transport.activeTransport;

            base.Start();
        }


        /// <summary>
        /// Process the request from a client
        /// </summary>
        /// <remarks>
        /// Override if you wish to provide more information to the clients
        /// such as the name of the host player
        /// </remarks>
        /// <param name="request">Request coming from client</param>
        /// <param name="endpoint">Address of the client that sent the request</param>
        /// <returns>A message containing information about this server</returns>
        protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint)
        {
            try
            {
                //This is just an example reply message,
                //you could add the game name here or game mode
                //if the player wants a specific game mode.
                return new DiscoveryResponse()
                {
                    serverId = ServerId,
                    uri = transport.ServerUri()
                };
            }
            catch(NotImplementedException _e)
            {
                //Someone dun goofed, so let us know what happened.
                Debug.LogError($"Transport {transport} does not support network disco");
                throw;
            }
        }
        #endregion

        #region Client
        /// <summary>
        /// Create a message that will be broadcasted on the network to discover servers
        /// </summary>
        /// <remarks>
        /// Override if you wish to include additional data in the discovery message
        /// such as desired game mode, language, difficulty, etc... </remarks>
        /// <returns>An instance of ServerRequest with data to be broadcasted</returns>
        protected override DiscoveryRequest GetRequest()
        {
            return new DiscoveryRequest();
        }

        /// <summary>
        /// Process the answer from a server
        /// </summary>
        /// <remarks>
        /// A client receives a reply from a server, this method processes the
        /// reply and raises an event
        /// </remarks>
        /// <param name="response">Response that came from the server</param>
        /// <param name="endpoint">Address of the server that replied</param>
        protected override void ProcessResponse(DiscoveryResponse _response, IPEndPoint _endpoint) 
        {
            //WE DONT FULLY UNDERSTAND THIS CODE BUT IT'S SOMETHING WE NEED TO DO WHICH IS IMPORTANT SO DEAL WITH IT
            //We received a message from the remote endpoint ()
            _response.EndPoint = _endpoint;

            //although we got a supposedly valid url we may not be able to resolve
            //the provided host/connection
            //however we know the real ip address of the server because we just
            //received a packet from it, so use that as host (convert to correct uri from url).
            UriBuilder realUri = new UriBuilder(_response.uri)
            {
                Host = _response.EndPoint.Address.ToString()
            };
            _response.uri = realUri.Uri;

            onServerFound.Invoke(_response);
        }
        #endregion
    }
}
