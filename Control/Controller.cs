using NetworkController;
using System;
using System.Collections.Generic;

namespace Control
{
    public class Controller
    {
        public delegate void receivedSpreadsheets(string[] spreadsheetNames);

        public event receivedSpreadsheets getSpreadsheets;

        String username;
        public Controller(String name)
        {
            username = name;
        }

        public void Connect(String ipAddr)
        {
            string message = username + "\n";
            String addr = ipAddr;
            int port = 1100;
            Networking.ConnectToServer(message, addr, port);

            getSpreadsheets(null);
        }
    }
}
