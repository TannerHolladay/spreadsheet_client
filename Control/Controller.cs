using NetworkController;
using System;

namespace Control
{
    public class Controller
    {
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
        }

    }
}
