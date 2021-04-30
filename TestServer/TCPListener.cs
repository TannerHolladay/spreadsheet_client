// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class TCPListener
    {
        public static void Main()
        {
            TcpListener server = null;
            try
            {
                const int port = 1100;
                // Local host ip address
                var serverAddress = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(serverAddress, port);
                server.Start();

                // Buffer reading data
                var bytes = new byte[256];

                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");
                    var client = server.AcceptTcpClient();
                    Console.WriteLine("Connected");

                    var stream = client.GetStream();

                    int i;

                    // Loop until all of the data has been streamed into the byte buffer
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate read bytes into ASCII
                        string data = Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);


                        switch (data)
                        {
                            //Send response
                            case "Dude\n":
                                //Sends list of spreadsheet names with new line after each name.
                                //When last spreadsheet name is sent, sends an additional newline.
                                data = "abc\nSpreadsheetName\nxyz\n\n";
                                break;
                            case "SpreadsheetName\n":
                                //Sends spreadsheet data
                                data = "{ messageType: \"cellUpdated\" , cellName: \"A1\", contents: \"=1 + 2\" }\n";
                                break;
                        }

                        //Encoding message into bytes
                        byte[] response = Encoding.ASCII.GetBytes(data);


                        stream.Write(response, 0, response.Length);
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
                server?.Stop();
            }
        }
    }
}