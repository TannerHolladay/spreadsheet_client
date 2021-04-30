// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021

using System;
using System.Linq;
using System.Text.RegularExpressions;
using NetworkingBackEnd;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSJson;

namespace Control
{
    public class Controller
    {
        private const int Port = 1100;
        private SocketState _server;

        private string _username;

        public bool IsConnected { get; private set; }
        public event Action<string[]> GetSpreadsheets;
        public event Action<CellUpdated> EditCell;
        public event Action<CellSelected> SelectCell;
        public event Action<ServerShutdownError> ServerShutdown;
        public event Action<RequestError> RequestError;
        public event Action<Disconnected> ClientDisconnected;
        public event Action<string, string> Error;
        public event Action Connected;
        public event Action Disconnected;
        public event Action<int> IDReceive;

        /// <summary>
        ///     Connects client to server with username and begins Handshake with the server
        /// </summary>
        /// <param name="name">Username</param>
        /// <param name="serverIP">Ip to connect to</param>
        public void Connect(string name, string serverIP)
        {
            _username = name;
            Networking.ConnectToServer(OnConnect, serverIP, Port);
        }

        /// <summary>
        ///     Disconnects Client from the server
        /// </summary>
        private void Disconnect()
        {
            _server = null;
            IsConnected = false;
            Disconnected?.Invoke();
        }

        /// <summary>
        ///     TCP connection created. This then continues the handshake
        /// </summary>
        /// <param name="socket"></param>
        private void OnConnect(SocketState socket)
        {
            if (socket.ErrorOccured)
            {
                // inform the view
                Error?.Invoke(socket.ErrorMessage, "Error Connecting To The Server");
                Disconnect();
                return;
            }

            IsConnected = true;
            Connected?.Invoke();

            _server = socket;
            Networking.Send(socket.TheSocket, _username + "\n");
            socket.OnNetworkAction = ReceiveSpreadsheets;
            Networking.GetData(socket);
        }

        /// <summary>
        ///     Called when server sends all spreadsheet names.
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveSpreadsheets(SocketState state)
        {
            if (state.ErrorOccured)
            {
                // inform the view
                Error?.Invoke(state.ErrorMessage, "Error Receiving Spreadsheets");
                Disconnect();
                return;
            }

            string data = state.GetData();
            if (!data.EndsWith("\n\n"))
            {
                Networking.GetData(state);
                return;
            }

            string[] spreadSheetNames = data.Substring(0, data.Length - 2).Split('\n');
            GetSpreadsheets?.Invoke(spreadSheetNames.Where(sheet => !string.IsNullOrEmpty(sheet)).ToArray());
        }

        /// <summary>
        ///     Sends selected spreadsheet name to server and moves into the message loop
        /// </summary>
        /// <param name="spreadsheet">Name of Spreadsheet</param>
        public void SendSpreadsheetRequest(string spreadsheet)
        {
            _server.ClearData();

            Networking.Send(_server.TheSocket, $"{spreadsheet}\n");
            _server.OnNetworkAction = ReceiveUpdatesLoop;

            Networking.GetData(_server);
        }

        /// <summary>
        ///     Receives messages from server and handles them
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveUpdatesLoop(SocketState state)
        {
            if (state.ErrorOccured)
            {
                // inform the view
                Error?.Invoke(state.ErrorMessage, "Error In Event Loop");
                Disconnect();
                return;
            }

            string incomingMessages = state.GetData();
            string[] data = Regex.Split(incomingMessages, @"(?<=[\n])");

            foreach (string message in data.Where(message => message.Length != 0))
            {
                //Last Step of handshake if message is the id
                if (int.TryParse(message, out int i))
                {
                    IDReceive?.Invoke(i);
                    continue;
                }

                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens.
                if (message.Last() != '\n' || message[0] != '{')
                    continue;

                Console.WriteLine("Received:" + message);

                //Parse message as json object
                var x = JObject.Parse(message);
                switch (x["messageType"]?.ToString())
                {
                    case "cellUpdated":
                    {
                        if (EditCell is null) continue;
                        var updated = JsonConvert.DeserializeObject<CellUpdated>(message);
                        EditCell.Invoke(updated);
                        break;
                    }
                    case "cellSelected":
                    {
                        if (SelectCell is null) continue;
                        var selected = JsonConvert.DeserializeObject<CellSelected>(message);
                        SelectCell.Invoke(selected);
                        break;
                    }
                    case "serverError":
                    {
                        if (ServerShutdown is null) continue;
                        var error = JsonConvert.DeserializeObject<ServerShutdownError>(message);
                        ServerShutdown.Invoke(error);
                        break;
                    }
                    case "requestError":
                    {
                        if (RequestError is null) continue;
                        var error = JsonConvert.DeserializeObject<RequestError>(message);
                        RequestError.Invoke(error);
                        break;
                    }
                    case "disconnected":
                    {
                        var d = JsonConvert.DeserializeObject<Disconnected>(message);
                        ClientDisconnected?.Invoke(d);
                        break;
                    }
                }

                state.RemoveData(0, message.Length);
            }

            Networking.GetData(state);
        }

        /// <summary>
        ///     Sends a message to the server in Json Format
        /// </summary>
        /// <param name="o">Object to be serialized and sent</param>
        public void SendUpdatesToServer(object o)
        {
            string message = JsonConvert.SerializeObject(o);
            Console.WriteLine("Sent: " + message);
            Networking.Send(_server.TheSocket, message + "\n");
        }
    }
}