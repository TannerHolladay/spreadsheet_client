using NetworkController;
using NetworkUtil;
using System;
using System.Collections.Generic;

namespace Control
{
    public class Controller
    {
        public delegate void receivedSpreadsheets(string[] spreadsheetNames);
        public event receivedSpreadsheets getSpreadsheets;

        SocketState serverConnection;

        String username;
        public Controller()
        {
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
            socket.OnNetworkAction = ReceiveSpreadsheets;
            Networking.GetData(socket);
        }

        /// <summary>
        /// Called when server sends all spreadsheet names.
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveSpreadsheets(SocketState socket)
        {
            string[] spreadSheetNames = socket.GetData().Split('\n');
            getSpreadsheets(spreadSheetNames);
            socket.ClearData();
        }

        /// <summary>
        /// Sends user selected spreadsheet name to servern and awaits spreadsheet data.
        /// </summary>
        /// <param name="name"></param>
        public void SendSelectedSpreadsheet(string name)
        {
            serverConnection.OnNetworkAction = ReceiveSpreadsheetData;
            Networking.Send(serverConnection.TheSocket, name + "\n");
            Networking.GetData(serverConnection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveSpreadsheetData(SocketState obj)
        {
            //TODO: Implement editing loop. Check 1.4 Entering the Editing and Updating Loop pg. 4
        }
    }
}
