using NetworkController;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// Authors:  Jimmy, Noah, Tanner, Abbey, Sergio, Daniel Kopta
/// CS 3505 Spring 2021 
/// Professor: Daniel Kopta
/// Date: 4/13/2021
/// </summary>
namespace NetworkUtil
{
    public static class Networking
    {

        /////////////////////////////////////////////////////////////////////////////////////////
        // Client-Side Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Begins the asynchronous process of connecting to a server via BeginConnect, 
        /// and using ConnectedCallback as the method to finalize the connection once it's made.
        /// 
        /// If anything goes wrong during the connection process, toCall should be invoked 
        /// with a new SocketState with its ErrorOccured flag set to true and an appropriate message 
        /// placed in its ErrorMessage field. Between this method and ConnectedCallback, toCall should 
        /// only be invoked once on error.
        ///
        /// This connection process should timeout and produce an error (as discussed above) 
        /// if a connection can't be established within 3 seconds of starting BeginConnect.
        /// 
        /// </summary>
        /// <param name="toCall">The action to take once the connection is open or an error occurs</param>
        /// <param name="hostName">The server to connect to</param>
        /// <param name="port">The port on which the server is listening</param>
        public static void ConnectToServer(Action<SocketState> toCall, string hostName, int port)
        {
            //TODO: Find a way to pass in ip addresses

            // Establish the remote endpoint for the socket.
            IPHostEntry ipHostInfo;
            IPAddress ipAddress = IPAddress.None;

            // Determine if the server address is a URL or an IP
            try
            {
                ipHostInfo = Dns.GetHostEntry(hostName);
                bool foundIPV4 = false;
                foreach (IPAddress addr in ipHostInfo.AddressList)
                    if (addr.AddressFamily != AddressFamily.InterNetworkV6)
                    {
                        foundIPV4 = true;
                        ipAddress = addr;
                        break;
                    }
                // Didn't find any IPV4 addresses
                if (!foundIPV4)
                {
                    SendErrorMessage(new SocketState(toCall, new Socket(IPAddress.None.AddressFamily, SocketType.Stream, ProtocolType.Tcp)),
                        "Hostname Invalid.\nException Caught in ConnectToServer.");
                    return;
                }
            }
            catch (Exception)
            {
                // see if host name is a valid ipaddress
                try
                {
                    ipAddress = IPAddress.Parse(hostName);
                }
                catch (Exception)
                {
                    SendErrorMessage(new SocketState(toCall, new Socket(IPAddress.None.AddressFamily, SocketType.Stream, ProtocolType.Tcp)),
                        "Not a valid IP address.\nException Caught in ConnectToServer.");
                    return;
                }
            }

            // Create a TCP/IP socket.
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // This disables Nagle's algorithm (google if curious!)
            // Nagle's algorithm can cause problems for a latency-sensitive 
            // game like ours will be 
            socket.NoDelay = true;

            SocketState state = new SocketState(toCall, socket);

            //Connect
            IAsyncResult result = state.TheSocket.BeginConnect(ipAddress, port, ConnectedCallback, state);

            //Connection must establish within 3 seconds or error occurs
            result.AsyncWaitHandle.WaitOne(3000, true);

            if (!state.TheSocket.Connected)
            {
                SendErrorMessage(new SocketState(toCall, new Socket(IPAddress.None.AddressFamily, SocketType.Stream, ProtocolType.Tcp)),
                    "Took too long to connect.\nError Caught in ConnectToServer.");
                state.TheSocket.Close();
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a connection process that was initiated by ConnectToServer.
        ///
        /// Uses EndConnect to finalize the connection.
        /// 
        /// As stated in the ConnectToServer documentation, if an error occurs during the connection process,
        /// either this method or ConnectToServer (not both) should indicate the error appropriately.
        /// 
        /// If a connection is successfully established, invokes the toCall Action that was provided to ConnectToServer (above)
        /// with a new SocketState representing the new connection.
        /// 
        /// </summary>
        /// <param name="ar">The object asynchronously passed via BeginConnect</param>
        private static void ConnectedCallback(IAsyncResult ar)
        {
            SocketState state = (SocketState)ar.AsyncState;

            //Check if state is connected.
            if (!state.TheSocket.Connected)
            {
                //No Need to do anything else in this method.
                return;
            }

            try
            {
                state.TheSocket.EndConnect(ar);
            }
            catch (Exception e)
            {
                SendErrorMessage(new SocketState(state.OnNetworkAction, new Socket(IPAddress.None.AddressFamily, SocketType.Stream, ProtocolType.Tcp)),
                    e.Message + "\nError Caught in ConnectedCallback.");
                return;
            }

            state.OnNetworkAction(state);
        }


        /// <summary>
        /// Begins the asynchronous process of receiving data via BeginReceive, using ReceiveCallback 
        /// as the callback to finalize the receive and store data once it has arrived.
        /// The object passed to ReceiveCallback via the AsyncResult should be the SocketState.
        /// 
        /// If anything goes wrong during the receive process, the SocketState's ErrorOccured flag should 
        /// be set to true, and an appropriate message placed in ErrorMessage, then the SocketState's
        /// OnNetworkAction should be invoked. Between this method and ReceiveCallback, OnNetworkAction should only be 
        /// invoked once on error.
        /// 
        /// </summary>
        /// <param name="state">The SocketState to begin receiving</param>
        public static void GetData(SocketState state)
        {
            try
            {
                state.TheSocket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, ReceiveCallback, state);
            }
            catch (Exception e)
            {
                SendErrorMessage(state, e.Message + "\nError Caught in GetData.");
                return;
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a receive operation that was initiated by GetData.
        /// 
        /// Uses EndReceive to finalize the receive.
        ///
        /// As stated in the GetData documentation, if an error occurs during the receive process,
        /// either this method or GetData (not both) should indicate the error appropriately.
        /// 
        /// If data is successfully received:
        ///  (1) Read the characters as UTF8 and put them in the SocketState's unprocessed data buffer (its string builder).
        ///      This must be done in a thread-safe manner with respect to the SocketState methods that access or modify its 
        ///      string builder.
        ///  (2) Call the saved delegate (OnNetworkAction) allowing the user to deal with this data.
        /// </summary>
        /// <param name="ar"> 
        /// This contains the SocketState that is stored with the callback when the initial BeginReceive is called.
        /// </param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            SocketState state = (SocketState)ar.AsyncState;

            int numBytes;

            try
            {
                numBytes = state.TheSocket.EndReceive(ar);
                //If socket is closed during a receive operation then numBytes will be zero
                if (numBytes == 0)
                {
                    SendErrorMessage(state, "Socket Closed during Receive Operation.\nCaught in ReceiveCallback.");
                    return;
                }
            }
            catch (Exception e)
            {
                SendErrorMessage(state, e.Message + "\nError Caught in ReceiveCallback.");
                return;
            }

            lock (state)
            {
                string message = Encoding.UTF8.GetString(state.buffer, 0, numBytes);

                lock (state.data)
                {
                    state.data.Append(message);
                }

                state.OnNetworkAction(state);
            }


        }

        /// <summary>
        /// Begin the asynchronous process of sending data via BeginSend, using SendCallback to finalize the send process.
        /// 
        /// If the socket is closed, does not attempt to send.
        /// 
        /// If a send fails for any reason, this method ensures that the Socket is closed before returning.
        /// </summary>
        /// <param name="socket">The socket on which to send the data</param>
        /// <param name="data">The string to send</param>
        /// <returns>True if the send process was started, false if an error occurs or the socket is already closed</returns>
        public static bool Send(Socket socket, string data)
        {
            if (socket is null || !socket.Connected)
            {
                return false;
            }

            byte[] messageBytes = Encoding.UTF8.GetBytes(data);

            try
            {
                socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, SendCallback, socket);
                return true;
            }
            catch (Exception)
            {
                socket.Close();
                return false;
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a send operation that was initiated by Send.
        ///
        /// Uses EndSend to finalize the send.
        /// 
        /// This method must not throw, even if an error occured during the Send operation.
        /// </summary>
        /// <param name="ar">
        /// This is the Socket (not SocketState) that is stored with the callback when
        /// the initial BeginSend is called.
        /// </param>
        private static void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;

            try
            {
                socket.EndSend(ar);
            }
            catch (Exception)
            {
                //No Need to do anything else.
            }
        }

        /// <summary>
        /// Begin the asynchronous process of sending data via BeginSend, using SendAndCloseCallback to finalize the send process.
        /// This variant closes the socket in the callback once complete. This is useful for HTTP servers.
        /// 
        /// If the socket is closed, does not attempt to send.
        /// 
        /// If a send fails for any reason, this method ensures that the Socket is closed before returning.
        /// </summary>
        /// <param name="socket">The socket on which to send the data</param>
        /// <param name="data">The string to send</param>
        /// <returns>True if the send process was started, false if an error occurs or the socket is already closed</returns>
        public static bool SendAndClose(Socket socket, string data)
        {
            if (socket is null || !socket.Connected)
            {
                return false;
            }

            byte[] messageBytes = Encoding.UTF8.GetBytes(data);

            try
            {
                socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, SendAndCloseCallback, socket);
                return true;
            }
            catch (Exception)
            {
                socket.Close();
                return false;
            }

        }

        /// <summary>
        /// To be used as the callback for finalizing a send operation that was initiated by SendAndClose.
        ///
        /// Uses EndSend to finalize the send, then closes the socket.
        /// 
        /// This method must not throw, even if an error occured during the Send operation.
        /// 
        /// This method ensures that the socket is closed before returning.
        /// </summary>
        /// <param name="ar">
        /// This is the Socket (not SocketState) that is stored with the callback when
        /// the initial BeginSend is called.
        /// </param>
        private static void SendAndCloseCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;

            try
            {
                socket.EndSend(ar);
            }
            catch (Exception)
            {
                //No need to do anything else.
            }

            socket.Close();
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        // Helper Methods
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Sets the ErrorOccured flag on a given SocketState to true, sets the SocketState's error message,
        /// then, invokes the SocketState's OnNetworkAction.
        /// </summary>
        /// <param name="errorState">The state to send the error on</param>
        /// <param name="message">The error message</param>
        private static void SendErrorMessage(SocketState errorState, string message)
        {
            errorState.ErrorOccured = true;
            errorState.ErrorMessage = message;
            errorState.OnNetworkAction(errorState);
        }
    }
}