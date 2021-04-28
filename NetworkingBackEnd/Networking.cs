/**
 * This class is designed to represent a Network.
 * 
 * A starting point for all methods was taken from Professor Kopta's PS7 Skeleton. Some methods, such as the beginning of ConnectToServer,
 * were patially implemented in the skeleton. We referenced code from recent lectures, recitations, and the Examples repository to help write our
 * own code.
 * 
 * Authors: Abbey Nelson & Tanner Holladay & Professor Kopta
 * 
 * Version 2.1 - Added comments
 * Version 2.0 - Finished implementing all methods from the PS7 Skeleton
 * Version 1.0 - Copied PS7 Skeleton from the Examples repository
 * November 6, 2020.
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkingBackEnd
{

    public static class Networking
    {
        /////////////////////////////////////////////////////////////////////////////////////////
        // Server-Side Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Starts a TcpListener on the specified port and starts an event-loop to accept new clients.
        /// The event-loop is started with BeginAcceptSocket and uses AcceptNewClient as the callback.
        /// AcceptNewClient will continue the event-loop.
        /// </summary>
        /// <param name="toCall">The method to call when a new connection is made</param>
        /// <param name="port">The the port to listen on</param>
        public static TcpListener StartServer(Action<SocketState> toCall, int port)
        {
            // If anything goes wrong with creating or starting the TcpListener, return null
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Tuple<Action<SocketState>, TcpListener> args = new Tuple<Action<SocketState>, TcpListener>(toCall, listener);
                listener.BeginAcceptSocket(AcceptNewClient, args);
                return listener;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// To be used as the callback for accepting a new client that was initiated by StartServer, and 
        /// continues an event-loop to accept additional clients.
        ///
        /// Uses EndAcceptSocket to finalize the connection and create a new SocketState. The SocketState's
        /// OnNetworkAction should be set to the delegate that was passed to StartServer.
        /// Then invokes the OnNetworkAction delegate with the new SocketState so the user can take action. 
        /// 
        /// If anything goes wrong during the connection process (such as the server being stopped externally), 
        /// the OnNetworkAction delegate should be invoked with a new SocketState with its ErrorOccured flag set to true 
        /// and an appropriate message placed in its ErrorMessage field. The event-loop should not continue if
        /// an error occurs.
        ///
        /// If an error does not occur, after invoking OnNetworkAction with the new SocketState, an event-loop to accept 
        /// new clients should be continued by calling BeginAcceptSocket again with this method as the callback.
        /// </summary>
        /// <param name="ar">The object asynchronously passed via BeginAcceptSocket. It must contain a tuple with 
        /// 1) a delegate so the user can take action (a SocketState Action), and 2) the TcpListener</param>
        private static void AcceptNewClient(IAsyncResult ar)
        {
            // Get all the information we need from ar
            Tuple<Action<SocketState>, TcpListener> args = (Tuple<Action<SocketState>, TcpListener>)ar.AsyncState;
            Action<SocketState> toCall = args.Item1;
            TcpListener listener = args.Item2;
            SocketState state = null;
            // Try to invoke the EndAcceptSocket method
            try
            {
                Socket theSocket = listener.EndAcceptSocket(ar);
                state = new SocketState(toCall, theSocket);
            }
            // If an error occurs, indicate the error as specified and return so OnNetworkAction can be invoked
            catch (Exception e)
            {
                state = new SocketState(toCall, null);
                state.ErrorOccured = true;
                state.ErrorMessage = e.Message;
                return;
            }
            // Invoke OnNetworkAction for the specified SocketState
            finally
            {
                state.OnNetworkAction(state);
            }
            // If everything works correctly, attempt to continue the event-loop to accept new clients.
            try
            {
                listener.BeginAcceptSocket(AcceptNewClient, ar.AsyncState);
            }
            // If anything goes wrong with this, indicate the error as specified and invoke OnNetworkAction
            catch (Exception e)
            {
                state = new SocketState(toCall, null);
                state.ErrorOccured = true;
                state.ErrorMessage = e.Message;
                state.OnNetworkAction(state);
            }

        }

        /// <summary>
        /// Stops the given TcpListener.
        /// </summary>
        public static void StopServer(TcpListener listener)
        {
            // Try to stop the TcpListener. If any errors occur, do nothing
            try
            {
                listener.Stop();
            }
            catch
            {
                ;
            }
        }

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

            // Establish the remote endpoint for the socket.
            IPHostEntry ipHostInfo;
            IPAddress ipAddress = IPAddress.None;

            SocketState state = null;

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
                    // Indicate an error to the user, as specified in the documentation
                    state = new SocketState(toCall, null);
                    state.ErrorOccured = true;
                    state.ErrorMessage = "No IPv4 address was found";
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
                    // Indicate an error to the user, as specified in the documentation
                    state = new SocketState(toCall, null);
                    state.ErrorOccured = true;
                    state.ErrorMessage = "The host name was not a valid IP address";
                    return;
                }
            }

            finally
            {
                // If an error occured when determining the address, invoke OnNetworkAction
                if (state != null)
                {
                    state.OnNetworkAction(state);
                }
            }

            // Create a TCP/IP socket.
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // This disables Nagle's algorithm (google if curious!)
            // Nagle's algorithm can cause problems for a latency-sensitive 
            // game like ours will be 
            socket.NoDelay = true;

            // Create a new SocketState with the correct delegate and socket
            state = new SocketState(toCall, socket);

            // Try to begin the connection process
            try
            {
                IAsyncResult result = socket.BeginConnect(ipAddress, port, ConnectedCallback, state);

                result.AsyncWaitHandle.WaitOne(3000, true);
            }
            // If an error occurs, indicate the error as specified and invoke OnNetworkAction
            catch
            {
                state.ErrorOccured = true;
                state.ErrorMessage = "Connection to server failed";
                state.OnNetworkAction(state);
                return;
            }

            // If a connection is not established within three seconds, close the socket
            if (!socket.Connected)
            {
                socket.Close();
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

            // Try to finalize hte connection with the EndConnect method
            try
            {
                state.TheSocket.EndConnect(ar);
            }
            // If an error occurs, indicate the error as specified and invoke OnNetworkAction
            catch
            {
                state.ErrorOccured = true;
                state.ErrorMessage = "Connection to server failed";
                state.OnNetworkAction(state);
                return;
            }

            // If an error doesn't occur, invoke OnNetworkAction with a new SocketState which represents the new connection
            SocketState newConnection = new SocketState(state.OnNetworkAction, state.TheSocket);
            newConnection.OnNetworkAction(newConnection);

        }


        /////////////////////////////////////////////////////////////////////////////////////////
        // Server and Client Common Code
        /////////////////////////////////////////////////////////////////////////////////////////

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
            // Try to begin the receving process
            try
            {
                state.TheSocket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, ReceiveCallback, state);
            }
            // If an error occurs, indicate the error as specified and invoke OnNetworkAction
            catch (Exception e)
            {
                state.ErrorOccured = true;
                state.ErrorMessage = e.Message;
                state.OnNetworkAction(state);
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

            // Finalize the receive process 
            try
            {
                numBytes = state.TheSocket.EndReceive(ar);

                // If we have a valid number of bytes, then read the characters and put them in the SocketState's data buffer
                if (numBytes > 0)
                {
                    string message = Encoding.UTF8.GetString(state.buffer,
                      0, numBytes);

                    // This must be thread-safe
                    lock (state.data)
                    {
                        state.data.Append(message);
                    }
                }
                // If we do not (numBytes == 0), indicate an error as specified by the documentation
                else
                {
                    state.ErrorOccured = true;
                    state.ErrorMessage = "The socket was closed during the receive operation";
                }
            }
            // If an error occurs, indicate the error as specified
            catch (Exception e)
            {
                state.ErrorOccured = true;
                state.ErrorMessage = e.Message;
            }
            // Invoke OnNetworkAction if the data is successfully or unsuccessfully received 
            finally
            {
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
            // If the socket is closed, does not attempt to send and returns false
            if (!socket.Connected)
            {
                return false;
            }
            // Tries to begin sending the data
            try
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(data);
                socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, SendCallback, socket);
            }
            // If an error occurs, close the socket and return false
            catch
            {
                socket.Close();
                return false;
            }

            // Everything worked, so return true
            return true;
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
            // Tries to finalize the send
            try
            {
                socket.EndSend(ar);
            }
            // If an error occurs, does not throw or do anything
            catch
            {
                ;
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
            // If the socket is closed, does not attempt to send and returns false
            if (!socket.Connected)
            {
                return false;
            }
            // Tries to begin sending the data
            try
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(data);
                socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, SendAndCloseCallback, socket);
            }
            // If an error occurs, close the socket and return false
            catch
            {
                socket.Close();
                return false;
            }
            return true;
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
            // Tries to finalize the send
            try
            {
                socket.EndSend(ar);
            }
            // If an error occurs, does not throw or do anything
            catch
            {
                ;
            }
            // Closes the socket whether or not the send is successful or not
            socket.Close();
        }

    }
}
