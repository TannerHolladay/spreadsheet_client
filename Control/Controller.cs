
using System;
using System.Linq;
using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSJson;
using System.Text.RegularExpressions;

namespace Control
{
    public class Controller
    {
        private const int Port = 1100;
        public event Action<string[]> GetSpreadsheets;
        public event Action<CellUpdated> EditCell;
        public event Action<CellSelected> SelectCell;
        public event Action<ServerShutdownError> ServerShutdown;
        public event Action<RequestError> RequestError;
        public event Action<string, string> Error;
        public event Action Connected;
        public event Action Disconnected;
        public event Action<string> JoinSpreadsheet;
        private SocketState _server;
        private int _id;

        private string _username;
        
        public Controller()
        {
            _id = -1;
        }

        public void Connect(string name, string serverIP)
        {
            _username = name;
            Networking.ConnectToServer(OnConnect, serverIP, Port);
        }
        
        public void Disconnect()
        {
            _server = null;
            Disconnected?.Invoke();
        }

        /// <summary>
        /// TCP connection created.
        /// </summary>
        /// <param name="socket"></param>
        private void OnConnect(SocketState socket)
        {
            if (socket.ErrorOccured)
            {
                // inform the view
                Error?.Invoke(socket.ErrorMessage, "Error Connecting To The Server");
                return;
            }
            
            Connected?.Invoke();
            
            _server = socket;
            Networking.Send(socket.TheSocket, _username + "\n");
            socket.OnNetworkAction = ReceiveSpreadsheets;
            Networking.GetData(socket);
        }

        /// <summary>
        /// Called when server sends all spreadsheet names.
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveSpreadsheets(SocketState state)
        {
            string data = state.GetData();
            if (!data.EndsWith("\n\n"))
            {
                Networking.GetData(state);
                return;
            }
            string[] spreadSheetNames = data.Substring(0, data.Length-2).Split('\n');
            GetSpreadsheets?.Invoke(spreadSheetNames.Where(sheet => !string.IsNullOrEmpty(sheet)).ToArray());
        }

        public void SendSpreadsheetRequest(string spreadsheet)
        {
            _server.ClearData();

            Networking.Send(_server.TheSocket, $"{spreadsheet}\n");
            _server.OnNetworkAction = ReceiveUpdatesLoop;

            Networking.GetData(_server);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveUpdatesLoop(SocketState state)
        {
            //TODO: Implement editing loop. Check 1.4 Entering the Editing and Updating Loop pg. 4
            string incomingMessages = state.GetData();
            string[] data = Regex.Split(incomingMessages, @"(?<=[\n])");

            foreach (string message in data)
            {
                //ignore empty strings
                if (message.Length == 0)
                    continue;

                //Last Step of handshake if message is the id
                //SHOULD WE MAKE A SEPARATE LOOP FOR HANDSHAKE THEN USAGE????
                if (int.TryParse(message, out int i))
                    _id = i;

                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                if (message[message.Length - 1] != '\n')
                    break;

                JObject x = JObject.Parse(message);
                switch (x["messageType"]?.ToString())
                {
                    case "cellUpdated":
                    {
                        CellUpdated updated = JsonConvert.DeserializeObject<CellUpdated>(message);
                        EditCell?.Invoke(updated);
                        break;
                    }
                    case "cellSelected":
                    {
                        CellSelected selected = JsonConvert.DeserializeObject<CellSelected>(message);
                        SelectCell?.Invoke(selected);
                        break;
                    }
                    case "serverError":
                    {
                        ServerShutdownError error = JsonConvert.DeserializeObject<ServerShutdownError>(message);
                        ServerShutdown.Invoke(error);
                        break;
                    }
                    case "requestError":
                    {
                        RequestError error = JsonConvert.DeserializeObject<RequestError>(message);
                        RequestError.Invoke(error);
                        break;
                    }
                }

                state.RemoveData(0, message.Length);

               
            }

            Networking.GetData(state);

        }

        public void SendUpdatesToServer(object o)
        {
            string message = JsonConvert.SerializeObject(o);
            Networking.Send(_server.TheSocket, message + "\n");
        }

        public bool HasID()
        {
            return _id != -1;
        }
    }
}
