using System;
using System.Net.Sockets;
using System.IO;

namespace NetworkController
{
    public class Networking
    {
        public static void ConnectToServer(String sendMessage, String addr, int portNumber)
        {
            try
            {
                String message = sendMessage;
                String ipAddr = addr;
                int port = portNumber;
                // Create a TCP connection
                TcpClient client = new TcpClient(ipAddr, port);

                // Encodes message into bytes that will be sent over TCP connection
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(message);

                // Stream used for reading and writing message to and from TCP connection
                NetworkStream stream = client.GetStream();

                // Send data to TCP server.
                stream.Write(data, 0, data.Length);

                // Store response in byte array
                data = new Byte[512];

                String receivedData = null;

                int receivedBytes = stream.Read(data, 0, data.Length);
                // Convert received bytes into string 
                receivedData = System.Text.Encoding.UTF8.GetString(data, 0, receivedBytes);
                Console.WriteLine("This is the received data: ", receivedData);

            }
            catch (SocketException e)
            {
                Console.WriteLine("Exception message: " + e);
            }

        }
    }

}
