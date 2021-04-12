using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class TCPListener
    {
        public static void Main()
        {
            TcpListener server = null;
            try
            {
                int port = 10016;
                // Local host ip address
                IPAddress serverAddress = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(serverAddress, port);
                server.Start();

                // Buffer reading data
                Byte[] bytes = new byte[256];
                String data = null;

                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected");

                    // Reset data to null
                    data = null;

                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop until all of the data has been streamed into the byte buffer
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate read bytes into ASCII
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        data = data.ToUpper();

                        byte[] message = System.Text.Encoding.ASCII.GetBytes(data);

                        //Send response
                        stream.Write(message, 0, message.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }
                    // End connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
