
using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSJson;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Control
{
    public class Controller
    {
        public delegate string receivedSpreadsheets(string[] spreadsheetNames);
        public event receivedSpreadsheets getSpreadsheets;

        public delegate void cellUpdated(CellUpdated c);
        public event cellUpdated editCell;

        SocketState serverConnection;

        private int ID;

        String username;
        public Controller()
        {
            ID = -1;
        }

        public void Connect(string name, String ipAddr)
        {
            username = name;
            String addr = ipAddr;
            int port = 1100;
            Networking.ConnectToServer(OnConnect, addr, port);
        }

        /// <summary>
        /// TCP connection created.
        /// </summary>
        /// <param name="socket"></param>
        private void OnConnect(SocketState socket)
        {
            serverConnection = socket;
            Networking.Send(socket.TheSocket, username + "\n");
            socket.OnNetworkAction = ReceiveAndSendSpreadsheets;
            Networking.GetData(socket);
        }

        /// <summary>
        /// Called when server sends all spreadsheet names.
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveAndSendSpreadsheets(SocketState state)
        {
            string[] spreadSheetNames = state.GetData().Split('\n');
            string name = getSpreadsheets(spreadSheetNames);
            state.ClearData();

            state.OnNetworkAction = ReceiveUpdatesLoop;
            Networking.Send(state.TheSocket, name + "\n");

            Networking.GetData(serverConnection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
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
                    ID = i;

                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                if (message[message.Length - 1] != '\n')
                    break;

                JObject x =JObject.Parse(message);
                if(((string)x["messageType"]).Equals("cellUpdated"))
                {
                    CellUpdated c = JsonConvert.DeserializeObject<CellUpdated>(message);
                    editCell(c);
                }
                else if(((string)x["messageType"]).Equals("cellSelected"))
                {

                }

                state.RemoveData(0, message.Length);

               
            }

            Networking.GetData(state);

        }

        public void SendUpdatesToServer(object o)
        {
            string message = JsonConvert.SerializeObject(o);
            Networking.Send(serverConnection.TheSocket, message + "\n");
        }

        public bool HasID()
        {
            return ID != -1;
        }
    }
}
