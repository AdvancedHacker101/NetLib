#define Validation_hard // Enable Hard, strict error cheking, may throw error even when the program will probably work fine
#if Validation_hard // When hard validation is enabled it will also enable soft validation
#define Validation_soft // Will throw the same errors but a with a little more userfriendly message as the program would without try catching would
#endif
using System;
using System.Text;
using System.Threading.Tasks;
using System.Net; // Basic networking objects
using System.Net.Sockets; // Sockets
using System.Security.Cryptography.X509Certificates; // Certificate parsing

/// <summary>
/// Network Library
/// </summary>
namespace NetLib
{
    /// <summary>
    /// Networking interfaces
    /// </summary>
    namespace Interfaces
    {
        /// <summary>
        /// Read messages from a socket connection
        /// </summary>
        interface INetworkReader
        {
            /// <summary>
            /// Read raw bytes from the stream
            /// </summary>
            /// <param name="size">Number of bytes to read</param>
            /// <returns>Bytes read from the stream</returns>
            byte[] DirectRead(int size);
            /// <summary>
            /// Read the stream until a line terminator
            /// </summary>
            /// <returns>The line read from the stream</returns>
            string ReadLine();
            /// <summary>
            /// Add an event listener for receiving bytes
            /// </summary>
            /// <param name="callback">The function to call when bytes are read</param>
            void AddEventDataReceived(Action<byte[]> callback);
            /// <summary>
            /// Add an event listener for receiving lines
            /// </summary>
            /// <param name="callback">The function to call when lines are received</param>
            void AddEventLineReceived(Action<string> callback);
            /// <summary>
            /// Set the encoding used to decode bytes back to string
            /// </summary>
            /// <param name="encoding">The encoding to use</param>
            void SetReceiveEncoding(Encoding encoding);
            /// <summary>
            /// Set the size of the buffer to read from the stream
            /// </summary>
            /// <param name="size">The size of the buffer</param>
            void SetReceiveSize(int size);
            /// <summary>
            /// Set the line terminator character for the readLine function
            /// </summary>
            /// <param name="newLine"></param>
            void SetReceiveNewLine(string newLine);
            /// <summary>
            /// Read butes from the stream asnyc
            /// </summary>
            /// <param name="maxSize">The size of the buffer to read from the stream</param>
            /// <returns>The bytes read from the stream</returns>
            Task<byte[]> DirectReadAsync(int maxSize);
            /// <summary>
            /// Read the stream until a line terminator async
            /// </summary>
            /// <returns>The line read from the stream</returns>
            Task<string> ReadLineAsync();
        }

        /// <summary>
        /// Write messages to a socket connection
        /// </summary>
        interface INetworkWriter
        {
            /// <summary>
            /// Write bytes to the stream
            /// </summary>
            /// <param name="buffer">The bytes to send to the stream</param>
            void DirectWrite(byte[] buffer);
            /// <summary>
            /// Write bytes to the stream with offset
            /// </summary>
            /// <param name="buffer">The bytes to send to the stream</param>
            /// <param name="offset">The offset to send the bytes from</param>
            void DirectWrite(byte[] buffer, int offset);
            /// <summary>
            /// ?Write bytes to the stream with offset and length
            /// </summary>
            /// <param name="buffer">The bytes to send to the stream</param>
            /// <param name="offset">The offset to send the bytes from</param>
            /// <param name="length">The number of bytes to send</param>
            void DirectWrite(byte[] buffer, int offset, int length);
            /// <summary>
            /// Write a line to the stream
            /// </summary>
            /// <param name="data">The line to write to the stream</param>
            void WriteLine(string data);
            /// <summary>
            /// Set the encoding to encode string into bytes when sending a new line
            /// </summary>
            /// <param name="encoding">The encoding to encode strings with</param>
            void SetSendEncoding(Encoding encoding);
            /// <summary>
            /// Set the line terminator to send whne sending a new line
            /// </summary>
            /// <param name="newLine">The line terminator character to send</param>
            void SetSendNewLine(string newLine);
        }
    }

    /// <summary>
    /// Socket Servers
    /// </summary>
    namespace Servers
    {
        /// <summary>
        /// Single Client Tcp Server
        /// </summary>
        public class SingleTcpServer : Interfaces.INetworkReader, Interfaces.INetworkWriter
        {
            /// <summary>
            /// The socket of the server
            /// </summary>
            protected Socket serverSocket;
            /// <summary>
            /// The connected client's socket
            /// </summary>
            protected Clients.TcpClient client;
            /// <summary>
            /// The endpoint to run the server on
            /// </summary>
            protected IPEndPoint serverEndPoint;
            /// <summary>
            /// The size of the buffer to receive from the client
            /// </summary>
            protected int recvSize;
            /// <summary>
            /// The encoding used when receiving new lines
            /// </summary>
            protected Encoding recvEncoder;
            /// <summary>
            /// The encoding use when sending new lines
            /// </summary>
            protected Encoding sendEncoder;
            /// <summary>
            /// The new line terminator to read until
            /// </summary>
            protected string readNewLine;
            /// <summary>
            /// The new line terminator to send
            /// </summary>
            protected string writeNewLine;
            /// <summary>
            /// Indicates if the server's running
            /// </summary>
            protected bool serverOffline = true;
            /// <summary>
            /// Indicates if the server should restart after the current client closes
            /// </summary>
            protected bool restartReading;

            /// <summary>
            /// Init the server
            /// </summary>
            /// <param name="restartRead">True to restart reading after the current client closes, otherwise false</param>
            public SingleTcpServer(bool restartRead)
            {
                ServerInit(restartRead); // Init the server
            }

            /// <summary>
            /// Init the server
            /// </summary>
            /// <param name="restartRead">True to restart reading after the current client closes, otherwise false</param>
            /// <param name="ep">The endpoint to run the server on</param>
            public SingleTcpServer(bool restartRead, IPEndPoint ep)
            {
                ServerInit(restartRead); // Init the server
                serverEndPoint = ep; // Set the endpoint to run on
            }

            /// <summary>
            /// Init the server
            /// </summary>
            /// <param name="restartRead">True to restart reading after the current client closes, otherwise false</param>
            private void ServerInit(bool restartRead)
            {
                client = null; // Set the current client to null
                serverEndPoint = null; // Reset the current endpoint
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a new server socket
                recvSize = 1024; // Init the read buffer size
                restartReading = restartRead; // Set the restart read variable
            }

            /// <summary>
            /// Start the server
            /// </summary>
            public void Start()
            {
                serverSocket.Bind(serverEndPoint); // Bind to the specified endpoint
                serverSocket.Listen(100); // Listen for max 100 pending connections
                Task t = new Task(() =>
                {
                    Socket clientSocket = serverSocket.Accept(); // Accept the pending client
                    client = new Clients.TcpClient(clientSocket); // Wrap the clinet in the NetLib tcp client class
                    // Set client data
                    client.SetReceiveEncoding(recvEncoder);
                    client.SetReceiveNewLine(readNewLine);
                    client.SetReceiveSize(recvSize);
                    client.SetSendEncoding(sendEncoder);
                    client.SetSendNewLine(writeNewLine);
                    client.AddEventClientStopped(() => { if (restartReading) Start(); });
                });
                t.Start(); // Start listening
                serverOffline = false; // Server's running
            }

            /// <summary>
            /// Wait for a client to connect
            /// </summary>
            private void WaitForClient()
            {
                while (client == null) // While the client isn't connected
                {
                    System.Threading.Thread.Sleep(100); // Sleep for 100ms
                }
            }

            /// <summary>
            /// Read bytes from the stream
            /// </summary>
            /// <param name="maxSize">The number of maximum bytes to read</param>
            /// <returns>The bytes read from the stream</returns>
            public byte[] DirectRead(int maxSize)
            {
                WaitForClient(); // Wait for the client to connect
                return client.DirectRead(maxSize); // Use the wrapped client to read from the stream
            }

            /// <summary>
            /// Read a line from the stram
            /// </summary>
            /// <returns>The line read from the stream</returns>
            public string ReadLine()
            {
                WaitForClient(); // Wait for the clinet to connect
                return client.ReadLine(); // Use the wrapped client to read new lines
            }

            /// <summary>
            /// Read bytes from the stream async
            /// </summary>
            /// <param name="maxSize">The number of maximum bytes to read from the stream</param>
            /// <returns>The bytes read from the stream</returns>
            public async Task<byte[]> DirectReadAsync(int maxSize)
            {
                return await client.DirectReadAsync(maxSize); // Wait for the function to complete and return
            }

            /// <summary>
            /// Read a line from the stream async
            /// </summary>
            /// <returns>The line read from the stream</returns>
            public async Task<string> ReadLineAsync()
            {
                return await client.ReadLineAsync(); // Wait for the function to complete
            }

            /// <summary>
            /// Close the client gracefully
            /// </summary>
            private void GracefulCloseClient()
            {
                if (client == null) return; // Check if the client is null
                client.GracefulStop(); // Stop the client
                client = null; // Reset the client
            }

            /// <summary>
            /// Close the client forcefully
            /// </summary>
            private void ForceCloseClient()
            {
                if (client == null) return; // Check if the client is null
                client.ForceStop(); // Close the client
                client = null; // Reset the client
            }

            /// <summary>
            /// Close the server
            /// </summary>
            private void ServerClose()
            {
                serverSocket.Close(); // Close the server socket
                serverSocket.Dispose(); // Dispose the server socket
                serverSocket = null; // Reset the server socket
            }

            /// <summary>
            /// Close the server gracefully
            /// </summary>
            public void GracefulStop()
            {
                if (serverOffline) return; // Check if the server's stopped
                GracefulCloseClient(); // Close the client
                ServerClose(); // Close the server
                serverOffline = true; // The server's stopped
                if (restartReading) Start(); // Restart if specified
            }

            /// <summary>
            /// Forcefully stop the server
            /// </summary>
            public void ForceStop()
            {
                if (serverOffline) return; // Check if the server's stopped
                ForceCloseClient(); // Force close the client
                ServerClose(); // Close the server
                serverOffline = true; // The server's stopped
                if (restartReading) Start(); // Restart server if specified
            }

            /// <summary>
            /// Add event listener for receiving bytes
            /// </summary>
            /// <param name="callback">The function to call when bytes are read from the stream</param>
            public void AddEventDataReceived(Action<byte[]> callback)
            {
                client.AddEventDataReceived(callback); // Use the wrapped client to listen for bytes
            }

            /// <summary>
            /// Add event listener for receiving new lines
            /// </summary>
            /// <param name="callback">The function to call when new lines are read from the stream</param>
            public void AddEventLineReceived(Action<string> callback)
            {
                client.AddEventLineReceived(callback); // Use the wrapped client to listen for new lines
            }

            /// <summary>
            /// Set the read encoding
            /// </summary>
            /// <param name="encoding">The encoding to decode bytes arrays with</param>
            public void SetReceiveEncoding(Encoding encoding)
            {
                recvEncoder = encoding; // Set the encoding
            }

            /// <summary>
            /// Set the maximum number of bytes to receive
            /// </summary>
            /// <param name="size">The number of bytes</param>
            public void SetReceiveSize(int size)
            {
                recvSize = size; // Set the size
            }

            /// <summary>
            /// Set the read line terminator
            /// </summary>
            /// <param name="newLine">The line terminator character</param>
            public void SetReceiveNewLine(string newLine)
            {
                readNewLine = newLine; // Set the line terminator
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write to the stream</param>
            public void DirectWrite(byte[] buffer)
            {
                DirectWrite(buffer, 0, buffer.Length); // Write the bytes to the stream
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write to the stream</param>
            /// <param name="offset">The offset to begin writing from</param>
            public void DirectWrite(byte[] buffer, int offset)
            {
                DirectWrite(buffer, offset, buffer.Length - offset); // Write bytes to the stream
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write to the stream</param>
            /// <param name="offset">The offset to begin writing from</param>
            /// <param name="length">The number of bytes to write out</param>
            public void DirectWrite(byte[] buffer, int offset, int length)
            {
                WaitForClient(); // Wait for the client to connect
                client.DirectWrite(buffer, offset, length); // Write the bytes to the stream
            }

            /// <summary>
            /// Write a line to the stream
            /// </summary>
            /// <param name="data">The line to write to the stream</param>
            public void WriteLine(string data)
            {
                byte[] buffer = sendEncoder.GetBytes(data + writeNewLine); // Convert the line and the terminator to a byte array
                DirectWrite(buffer); // Write the bytes to the stream
            }

            /// <summary>
            /// Set the encoding for writing new lines to the stream
            /// </summary>
            /// <param name="encoding">The encoding to use</param>
            public void SetSendEncoding(Encoding encoding)
            {
                sendEncoder = encoding; // Set the encoding
            }

            /// <summary>
            /// Set the new line terminator used when sending new lines
            /// </summary>
            /// <param name="newLine">The new line terminator to use</param>
            public void SetSendNewLine(string newLine)
            {
                writeNewLine = newLine;
            }
        }

        /// <summary>
        /// Single Client SSL Server
        /// </summary>
        public class SingleSSLServer : Interfaces.INetworkReader, Interfaces.INetworkWriter
        {
            /// <summary>
            /// The server socket
            /// </summary>
            protected Socket serverSocket;
            /// <summary>
            /// The client socket
            /// </summary>
            protected Socket clientSocket;
            /// <summary>
            /// The wrapped client socket
            /// </summary>
            protected Clients.SSLClient client;
            /// <summary>
            /// The endpoint to run the server on
            /// </summary>
            protected IPEndPoint serverEndPoint;
            /// <summary>
            /// The number of maximum bytes to read from the stream
            /// </summary>
            protected int recvSize;
            /// <summary>
            /// The encoding to use when reading new lines from the stream
            /// </summary>
            protected Encoding recvEncoder;
            /// <summary>
            /// The encoding to use when sending new lines to the stream
            /// </summary>
            protected Encoding sendEncoder;
            /// <summary>
            /// The new line terminator used when reading new lines from the stream
            /// </summary>
            protected string readNewLine;
            /// <summary>
            /// The new line terminator used when writing new lines to the stream
            /// </summary>
            protected string writeNewLine;
            /// <summary>
            /// Indicated if the server's running
            /// </summary>
            protected bool serverOffline = true;
            /// <summary>
            /// Event for client connection notification
            /// </summary>
            protected event Action ClientConnected;
            /// <summary>
            /// Indicates if the server should restart after it's client closes
            /// </summary>
            protected bool restartReading;
            /// <summary>
            /// The SSL Parameters of the server
            /// </summary>
            protected Utils.SSL.ServerSSLData sslParams;

            /// <summary>
            /// Init the server
            /// </summary>
            /// <param name="restartRead">True to restart reading after the current client closes, otherwise false</param>
            /// <param name="sslParameters">SSL Socket Server Parameters</param>
            public SingleSSLServer(bool restartRead, Utils.SSL.ServerSSLData sslParameters)
            {
                ServerInit(restartRead, sslParameters); // Init the server
            }

            /// <summary>
            /// Init the server
            /// </summary>
            /// <param name="restartRead">True to restart reading after the current client closes, otherwise false</param>
            /// <param name="ep">The endpoint to run the server on</param>
            /// <param name="sslParameters">SSL Socket Server Parameters</param>
            public SingleSSLServer(bool restartRead, IPEndPoint ep, Utils.SSL.ServerSSLData sslParameters)
            {
                ServerInit(restartRead, sslParameters);
                serverEndPoint = ep;
            }
            /// <summary>
            /// Init the server
            /// </summary>
            /// <param name="restartRead">True to restart reading after the current client closes, otherwise false</param>
            /// <param name="ep">The endpoint to run the server on</param>
            /// <param name="certFilePath">The certification file to load into ssl sockets</param>
            /// <param name="serverAddress">The address of this server (domain name specified in the cert file)</param>
            /// <param name="protocols">The SSL Protocol to use during the handshake</param>
            public SingleSSLServer(bool restartRead, IPEndPoint ep, string certFilePath, string serverAddress, System.Security.Authentication.SslProtocols protocols)
            {
                ServerInit(restartRead, Utils.SSL.ParseServerData(certFilePath, serverAddress, protocols));
                serverEndPoint = ep;
            }

            /// <summary>
            /// Init the server
            /// </summary>
            /// <param name="restartRead">True to restart reading after the current client closes, otherwise false</param>
            /// <param name="certFilePath">The certification file to load into ssl sockets</param>
            /// <param name="serverAddress">The address of this server (domain name specified in the cert file)</param>
            /// <param name="protocols">The SSL Protocol to use during the handshake</param>
            public SingleSSLServer(bool restartRead, string certFilePath, string serverAddress, System.Security.Authentication.SslProtocols protocols)
            {
                ServerInit(restartRead, Utils.SSL.ParseServerData(certFilePath, serverAddress, protocols));
            }

            /// <summary>
            /// Init the server
            /// </summary>
            /// <param name="restartRead">True to restart reading after the current client closes, otherwise false</param>
            /// <param name="ep">The endpoint to run the server on</param>
            /// <param name="certFilePath">The certification file to load into ssl sockets</param>
            /// <param name="serverAddress">The address of this server (domain name specified in the cert file)</param>
            public SingleSSLServer(bool restartRead, IPEndPoint ep, string certFilePath, string serverAddress)
            {
                ServerInit(restartRead, Utils.SSL.ParseServerData(certFilePath, serverAddress, Utils.SSL.defaultProtocols));
                serverEndPoint = ep;
            }

            /// <summary>
            /// Init the server
            /// </summary>
            /// <param name="restartRead">True to restart reading after the current client closes, otherwise false</param>
            /// <param name="certFilePath">The certification file to load into ssl sockets</param>
            /// <param name="serverAddress">The address of this server (domain name specified in the cert file)</param>
            public SingleSSLServer(bool restartRead, string certFilePath, string serverAddress)
            {
                ServerInit(restartRead, Utils.SSL.ParseServerData(certFilePath, serverAddress, Utils.SSL.defaultProtocols));
            }

            /// <summary>
            /// Init the server
            /// </summary>
            /// <param name="restartRead">True to restart reading after the current client closes, otherwise false</param>
            /// <param name="sslParameters">SSL Socket Server Parameters</param>
            private void ServerInit(bool restartRead, Utils.SSL.ServerSSLData sslParameters)
            {
                client = null; // Reset the client
                serverEndPoint = null; // Reset the endpoint
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a new server socket
                recvSize = 1024; // Set the default maximum number of bytes to read
                restartReading = restartRead; // Set the restart flag
                sslParams = sslParameters; // Set the ssl connection parameters
            }

            /// <summary>
            /// Start the server
            /// </summary>
            public void Start()
            {
                serverSocket.Bind(serverEndPoint); // Bind the server to the specified endpoint
                serverSocket.Listen(100); // Listen for connections with maximum 100 pending clients
                Task t = new Task(() =>
                {
                    try
                    {
                        Socket clientSocket = serverSocket.Accept(); // Accept the pending client
                        NetworkStream clientNS = new NetworkStream(clientSocket); // Get the network stream of the client
                        System.Net.Security.SslStream clientStream = new System.Net.Security.SslStream(clientNS); // Wrap the stream inside the ssl stream class
                        clientStream.AuthenticateAsServer(sslParams.certificate, false, sslParams.protocols, true); // Authenticate as the server
                        client = new Clients.SSLClient(clientStream); // Wrap the client in the SSL Client class
                        // Setup the client
                        client.SetReceiveEncoding(recvEncoder);
                        client.SetReceiveNewLine(readNewLine);
                        client.SetReceiveSize(recvSize);
                        client.SetSendEncoding(sendEncoder);
                        client.SetSendNewLine(writeNewLine);
                        client.AddEventClientStopped(() => { if (restartReading) Start(); });
                        ClientConnected?.Invoke(); // Notify the executing assembly of the connection
                    }
                    catch (Exception ex) // Something went wrong
                    {
                        if (serverSocket == null) // The server stopped before a connection is made, no worries
                        {
                            Console.WriteLine("Server stop generated error, can be ignored");
                            return;
                        }

#if Validation_soft // This error needs to be thrown, it's important
                        throw new Exception("Server connection accept failed", ex);
#endif
                    }
                });
                t.Start(); // Start listening
                serverOffline = false; // The server's running
            }

            /// <summary>
            /// Add event listener when the client's connected
            /// </summary>
            /// <param name="callback">The function to call when a client connects</param>
            public void AddEventClientConnected(Action callback)
            {
                ClientConnected += callback; // Add the callback
            }

            /// <summary>
            /// Wait for a client to connect
            /// </summary>
            private void WaitForClient()
            {
                while (client == null) // While the client isn't connected
                {
                    System.Threading.Thread.Sleep(100); // Wait for 100 ms
                }
            }

            /// <summary>
            /// Directly read bytes from the stream
            /// </summary>
            /// <param name="maxSize">The maximum size of bytes to read</param>
            /// <returns>The bytes read from the stream</returns>
            public byte[] DirectRead(int maxSize)
            {
                WaitForClient(); // Wait for a connection
                return client.DirectRead(maxSize); // Return the read bytes, using the wrapped client
            }

            /// <summary>
            /// Read a new line from the stream
            /// </summary>
            /// <returns>The new line read from the stream</returns>
            public string ReadLine()
            {
                WaitForClient(); // Wait for a connection
                return client.ReadLine(); // Return the new line using the wrapped client
            }

            /// <summary>
            /// Read bytes directly from the stream async
            /// </summary>
            /// <param name="maxSize">The maximum number of bytes to read</param>
            /// <returns>The bytes read from the stream</returns>
            public async Task<byte[]> DirectReadAsync(int maxSize)
            {
                return await client.DirectReadAsync(maxSize); // Wait for the function to complete
            }

            /// <summary>
            /// Read lines from the stream async
            /// </summary>
            /// <returns>The new line read from the stream</returns>
            public async Task<string> ReadLineAsync()
            {
                return await client.ReadLineAsync(); // Wait for the function to complete
            }

            /// <summary>
            /// Close the wrapped client gracefully
            /// </summary>
            private void GracefulCloseClient()
            {
                if (client == null) return; // Check if the client's null
                client.GracefulStop(); // Stop the client
                client = null; // Reset the client
            }

            /// <summary>
            /// Close the wrapped client forcefully
            /// </summary>
            private void ForceCloseClient()
            {
                if (client == null) return; // Check if the client's null
                client.ForceStop(); // Stop the client
                client = null; // Reset the client
            }

            /// <summary>
            /// Stop the server
            /// </summary>
            private void ServerClose()
            {
                serverSocket.Close(); // Close the server socket
                serverSocket.Dispose(); // Dispose the server socket
                serverSocket = null; // Reset the server socket
            }

            /// <summary>
            /// Gracefully stop the server
            /// </summary>
            public void GracefulStop()
            {
                if (serverOffline) return; // Check if the server's stopped
                GracefulCloseClient(); // Close the client
                ServerClose(); // Close the server
                serverOffline = true; // The server's now stopped
                if (restartReading) Start(); // Restart the server if specified
            }

            /// <summary>
            /// Forcefully stop the server
            /// </summary>
            public void ForceStop()
            {
                if (serverOffline) return; // Check if the server's stopped
                ForceCloseClient(); // Close the client
                ServerClose(); // Close the server
                serverOffline = true; // The server's now stopped
                if (restartReading) Start(); // Restart the server if specified
            }

            /// <summary>
            /// Add event listener for reading bytes from the stream
            /// </summary>
            /// <param name="callback">The function to call when new bytes are read</param>
            public void AddEventDataReceived(Action<byte[]> callback)
            {
                client.AddEventDataReceived(callback); // Listen for bytes, using the wrapped client
            }

            /// <summary>
            /// Add event listener for reading new lines from the stream
            /// </summary>
            /// <param name="callback">The function to call when new line are read</param>
            public void AddEventLineReceived(Action<string> callback)
            {
                client.AddEventLineReceived(callback); // Listen for new lines using the wrapped client
            }

            /// <summary>
            /// Set the encoding to read new lines with
            /// </summary>
            /// <param name="encoding">The encoding to read new lines with</param>
            public void SetReceiveEncoding(Encoding encoding)
            {
                recvEncoder = encoding; // Set the encoding
            }

            /// <summary>
            /// Set the maximum number of bytes to read from the stream
            /// </summary>
            /// <param name="size">The number of maximum bytes to read from the stream</param>
            public void SetReceiveSize(int size)
            {
                recvSize = size; // Set the size
            }

            /// <summary>
            /// Set the read new line terminator
            /// </summary>
            /// <param name="newLine">The line terminator</param>
            public void SetReceiveNewLine(string newLine)
            {
                readNewLine = newLine; // Set the line terminator
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write</param>
            public void DirectWrite(byte[] buffer)
            {
                DirectWrite(buffer, 0, buffer.Length); // Send bytes to the stream
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write</param>
            /// <param name="offset">The offset to begin writing from</param>
            public void DirectWrite(byte[] buffer, int offset)
            {
                DirectWrite(buffer, offset, buffer.Length - offset); // Send bytes to the stream
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write</param>
            /// <param name="offset">The offset to begin writing from</param>
            /// <param name="length">The number of bytes to write</param>
            public void DirectWrite(byte[] buffer, int offset, int length)
            {
                WaitForClient(); // Wait for a connection
                client.DirectWrite(buffer, offset, length); // Send the bytes to the stream
            }

            /// <summary>
            /// Write a new line to the stream
            /// </summary>
            /// <param name="data">The new line to write</param>
            public void WriteLine(string data)
            {
                byte[] buffer = sendEncoder.GetBytes(data + writeNewLine); // Convert the line and the line terminator to a byte array
                DirectWrite(buffer); // Send bytes to the stream
            }

            /// <summary>
            /// Set the encoding for sending new lines
            /// </summary>
            /// <param name="encoding">The encoding to use</param>
            public void SetSendEncoding(Encoding encoding)
            {
                sendEncoder = encoding; // Set the encoding
            }

            /// <summary>
            /// Set the new line terminator for sending new lines
            /// </summary>
            /// <param name="newLine">The new line terminator character</param>
            public void SetSendNewLine(string newLine)
            {
                writeNewLine = newLine; // Set the line terminator
            }
        }
    }

    /// <summary>
    /// Socket Clients
    /// </summary>
    namespace Clients
    {
        /// <summary>
        /// Basic TCP Client
        /// </summary>
        public class TcpClient : Interfaces.INetworkReader, Interfaces.INetworkWriter
        {
            /// <summary>
            /// The endpoint to connect to
            /// </summary>
            protected IPEndPoint clientEndPoint;
            /// <summary>
            /// The client socket
            /// </summary>
            protected Socket client;
            /// <summary>
            /// Encoding for reading new lines
            /// </summary>
            protected Encoding recvEncoder;
            /// <summary>
            /// Encoding for sending new lines
            /// </summary>
            protected Encoding sendEncoder;
            /// <summary>
            /// Maximum receive buffer size
            /// </summary>
            protected int recvSize;
            /// <summary>
            /// New line terminator for reading new lines
            /// </summary>
            protected string readNewLine;
            /// <summary>
            /// New line terminator for sending new lines
            /// </summary>
            protected string writeNewLine;
            /// <summary>
            /// Event for receiving byte data
            /// </summary>
            protected event Action<byte[]> DataReceived;
            /// <summary>
            /// Event for receiving new lines
            /// </summary>
            protected event Action<string> LineReceived;
            /// <summary>
            /// Enabled receive events
            /// </summary>
            protected Utils.NetworkIO.ReadEventCodes receiveCallbackMode;
            /// <summary>
            /// Indicates if the client's running
            /// </summary>
            protected bool clientOffline = true;
            /// <summary>
            /// Event for client disconnect notifications
            /// </summary>
            protected event Action ClientStopped;

            /// <summary>
            /// Init the client
            /// </summary>
            public TcpClient()
            {
                ClientInit(); // Init the client
            }

            /// <summary>
            /// Init the client
            /// </summary>
            private void ClientInit()
            {
                clientEndPoint = null; // Reset the endpoint
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a new client socket
                recvEncoder = Encoding.UTF8; // Set the receive encoder
                sendEncoder = Encoding.UTF8; // Set the send encoder
                recvSize = 1024; // Set the receive buffer size
                readNewLine = Environment.NewLine; // Set the reading line terminator
                writeNewLine = Environment.NewLine; // Set the sending line terminator
                receiveCallbackMode = Utils.NetworkIO.ReadEventCodes.Stopped; // Set the receive event mode
            }

            /// <summary>
            /// Init the client
            /// </summary>
            /// <param name="address">The IP Address or hostname to connect to</param>
            /// <param name="portNumber">The port to connect to</param>
            public TcpClient(string address, int portNumber)
            {
                ClientInit(); // Init the client
                Tuple<bool, IPAddress> ipResult = Utils.IPv4.IsValidDestination(address); // Check if the address is valid, and resolve dns if needed
#if Validation_hard // Not important check, socket connection will throw anyways if any of the data is wrong
                if (!ipResult.Item1) throw new ArgumentException($"The following destination is not valid: {address}"); // Invalid address or hostname not resolved
                if (!Utils.IPv4.IsPortValid(portNumber)) throw new ArgumentException($"The following port number is not valid {portNumber}"); // Invalid port number
#endif
                clientEndPoint = new IPEndPoint(ipResult.Item2, portNumber); // Set the endpoint to connect to
            }

            /// <summary>
            /// Init the client
            /// </summary>
            /// <param name="ep">The endpoint to connect to</param>
            public TcpClient(IPEndPoint ep)
            {
                ClientInit(); // Init the client
                clientEndPoint = ep; // Set the endpoint
            }

            /// <summary>
            /// Init the client
            /// </summary>
            /// <param name="clientSocket">The existing socket to wrap around</param>
            public TcpClient(Socket clientSocket)
            {
                ClientInit(); // Init the client
                client = clientSocket; // Set the socket
                clientEndPoint = (IPEndPoint)client.LocalEndPoint; // Get the endpoint
                if (client.Connected) clientOffline = false; // Set the started flag if connection is alive
            }

            /// <summary>
            /// Start the client
            /// </summary>
            public void Start()
            {
#if Validation_soft // Important check
                if (clientEndPoint == null) throw new NullReferenceException("The EndPoint to connect to is null"); // Check if the endpoint isn't set
#endif
                client.Connect(clientEndPoint); // Connect to the endpoint
                clientOffline = false; // The client is running now
            }

            /// <summary>
            /// Directly read bytes from the stream
            /// </summary>
            /// <param name="maxSize">The maximum number of bytes to read</param>
            /// <returns>The byte array read from the connection</returns>
            public byte[] DirectRead(int maxSize)
            {
#if Validation_soft // Important check
                if (client == null || !client.Connected) throw new InvalidOperationException("Cannot read from an offline client"); // Check if the client's online
#endif
                byte[] buffer = new byte[maxSize]; // Define the receive buffer
                int bytesRead = client.Receive(buffer, 0, maxSize, SocketFlags.None); // Read bytes from the stream
                byte[] dataBuffer = new byte[bytesRead]; // Define data buffer
                Array.Copy(buffer, dataBuffer, bytesRead); // Copy the bytes read from the stream

                return dataBuffer; // Return the bytes read from the stream
            }

            /// <summary>
            /// Read a new line from the stream
            /// </summary>
            /// <returns>The next new line from the connection</returns>
            public string ReadLine()
            {
#if Validation_soft // Important check
                if (client == null || !client.Connected) throw new InvalidOperationException("Cannot read from an offline client"); // Check if the client's running
#endif

                StringBuilder result = new StringBuilder(); // The final result string buffer

                while (true) // Inifinite loop
                {
                    byte[] buffer = new byte[recvSize]; // Define receive buffer
                    int bytesRead = client.Receive(buffer, 0, recvSize, SocketFlags.None); // Read bytes from the stream
                    string subResult = recvEncoder.GetString(buffer, 0, bytesRead); // Decode bytes read into strings
                    result.Append(subResult); // Append to the total result
                    if (subResult.EndsWith(readNewLine)) break; // If the data ends with a new line, return it
                }

                return result.ToString(); // Return the constructed line from the connection
            }

            /// <summary>
            /// Set the encoding to read new lines
            /// </summary>
            /// <param name="encoding">The encoding to use</param>
            public void SetReceiveEncoding(Encoding encoding)
            {
                recvEncoder = encoding; // Set the encoding
            }

            /// <summary>
            /// Set the size of the receive buffer
            /// </summary>
            /// <param name="size">The maximum size of the received bytes at once</param>
            public void SetReceiveSize(int size)
            {
                recvSize = size; // Set the limit
            }

            /// <summary>
            /// Set the new line terminator for reading new lines
            /// </summary>
            /// <param name="newLine">The new line terminator character</param>
            public void SetReceiveNewLine(string newLine)
            {
                readNewLine = newLine; // Set the line terminator
            }

            /// <summary>
            /// Invoke the data received event
            /// </summary>
            /// <param name="buffer">The byte array read from the stream</param>
            protected void DataReceivedMethod(byte[] buffer)
            {
#if Validation_hard // Not so important check
                if (receiveCallbackMode != Utils.NetworkIO.ReadEventCodes.Both && receiveCallbackMode != Utils.NetworkIO.ReadEventCodes.DataRecv) throw new InvalidOperationException("Can't invoke read line event, while receive data is stopped"); // Check if the receive method is started
#endif
                DataReceived?.Invoke(buffer); // Invoke the event with the specified data
            }

            /// <summary>
            /// Invoke the new line received event
            /// </summary>
            /// <param name="data">The new line read from the stream</param>
            protected void LineReceivedMethod(string data)
            {
#if Validation_hard // Not so important check
                if (receiveCallbackMode != Utils.NetworkIO.ReadEventCodes.Both && receiveCallbackMode != Utils.NetworkIO.ReadEventCodes.LineRecv) throw new InvalidOperationException("Can't invoke read line event, while read line is stopped"); // Check if the receiving is started
#endif
                LineReceived?.Invoke(data); // Invoke the event
            }

            /// <summary>
            /// Start the callback for event listener data receiving
            /// </summary>
            /// <param name="enableCode">The callback to enable</param>
            private void StartCallbackRead(Utils.NetworkIO.ReadEventCodes enableCode)
            {
                if (receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.Stopped) // Currently stopped callback
                {
                    receiveCallbackMode = enableCode; // Set the code to the caller
                    Utils.NetworkIO.ClientReadObject readObject = new Utils.NetworkIO.ClientReadObject() // Define a new read object
                    {
                        buffer = new byte[recvSize], // Byte buffer
                        lineRecvResult = new StringBuilder() // String buffer
                    };

                    client.BeginReceive(readObject.buffer, 0, recvSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), readObject); // Begin reading the stream
                }
                else if (receiveCallbackMode != Utils.NetworkIO.ReadEventCodes.Both && receiveCallbackMode != enableCode) receiveCallbackMode = enableCode; // Enable the other callback too
            }

            /// <summary>
            /// Read bytes directly from the stream async
            /// </summary>
            /// <param name="maxSize">The maximum size of the buffer to read from the stream</param>
            /// <returns>The task with the resulting bytes</returns>
            public async Task<byte[]> DirectReadAsync(int maxSize)
            {
                return await Task<byte[]>.Factory.StartNew(() => DirectRead(maxSize)).ConfigureAwait(false); // Wait for the task to finish
            }

            /// <summary>
            /// Read new lines from the stream async
            /// </summary>
            /// <returns>The task with the resulting new line</returns>
            public async Task<string> ReadLineAsync()
            {
                return await Task<string>.Factory.StartNew(ReadLine).ConfigureAwait(false); // Wait for the task to finish
            }

            /// <summary>
            /// Read data from the stream
            /// </summary>
            /// <param name="ar">Async Result</param>
            private void ReceiveCallback(IAsyncResult ar)
            {
                int bytesRead = 0; // Number of bytes read from the stream
                Utils.NetworkIO.ClientReadObject readObject = (Utils.NetworkIO.ClientReadObject)ar.AsyncState; // Current read object
                try
                {
                    bytesRead = client.EndReceive(ar); // Read bytes from the stream
                }
                catch (Exception ex) // Something went wrong
                {
                    ForceStop(); // Stop the client
                    return; // Return
                }

                if (bytesRead > 0) // If we read bytes from the stream
                {
                    byte[] dataBuffer = new byte[bytesRead]; // Define data buffer
                    Array.Copy(readObject.buffer, dataBuffer, bytesRead); // Copy the bytes read to the data buffer

                    if (receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.Both || receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.DataRecv) DataReceived?.Invoke(dataBuffer); // Invoke the data received event
                    if (receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.Both || receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.LineRecv) // Invoke the line received event
                    {
                        string subResult = recvEncoder.GetString(dataBuffer, 0, bytesRead); // Decode the data
                        readObject.lineRecvResult.Append(subResult); // Append the result to the string buffer
                        if (subResult.EndsWith(readNewLine)) // Check for line ending
                        {
                            LineReceived?.Invoke(readObject.lineRecvResult.ToString()); // Invoke the callback
                            readObject.lineRecvResult.Clear(); // Clear out the string buffer
                        }
                    }
                }

                Array.Clear(readObject.buffer, 0, recvSize); // Clear the receive buffer

                try
                {
                    client.BeginReceive(readObject.buffer, 0, recvSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), readObject); // Try reading the stream again
                }
                catch (Exception) // Something went wrong
                {
                    ForceStop(); // Stop the client
                }
            }

            /// <summary>
            /// Add an event listener for receiving bytes from the stream
            /// </summary>
            /// <param name="callback">The function to call when new bytes are available</param>
            public void AddEventDataReceived(Action<byte[]> callback)
            {
                DataReceived += callback; // Add the function to the event
                StartCallbackRead(Utils.NetworkIO.ReadEventCodes.DataRecv); // Start the callback
            }

            /// <summary>
            /// Add an event listener for receiving new lines from the stream
            /// </summary>
            /// <param name="callback">The function to call when new lines are available</param>
            public void AddEventLineReceived(Action<string> callback)
            {
                LineReceived += callback; // Add the function to the event
                StartCallbackRead(Utils.NetworkIO.ReadEventCodes.LineRecv); // Enable the callback
            }

            /// <summary>
            /// Add an event listener for when the client is stopped
            /// </summary>
            /// <param name="callback">The function to call when the client is disconnected</param>
            public void AddEventClientStopped(Action callback)
            {
                ClientStopped += callback; // Add the function to the event
            }

            /// <summary>
            /// Stop the client gracefully
            /// </summary>
            public void GracefulStop()
            {
                if (clientOffline) return; // Return if the client isn't running
#if Validation_soft // Important check
                if (client == null) throw new InvalidOperationException("Cannot close an offline client"); // The client isn't started
#endif

                client.Shutdown(SocketShutdown.Both); // Shutdown read and write sockets
                client.Disconnect(false); // Disconnect from the server
                client.Close(); // Close the client
                client.Dispose(); // Dispose the client
                client = null; // Reset the client
                clientOffline = true; // The client isn't running
                ClientStopped?.Invoke(); // Invoke the client stopped event
            }

            /// <summary>
            /// Stop the client forcefully
            /// </summary>
            public void ForceStop()
            {
                if (clientOffline) return; // Check if the client isn't running
#if Validation_soft // Important check
                if (client == null) throw new InvalidOperationException("Cannot close an offline client"); // Client isn't running
#endif
                client.Close(); // Close the client
                client.Dispose(); // Dispose the client
                client = null; // Reset the client
                clientOffline = true; // The client isn't running
                ClientStopped?.Invoke(); // Invoke the client stopped event
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write</param>
            public void DirectWrite(byte[] buffer)
            {
                DirectWrite(buffer, 0, buffer.Length); // Write the bytes to the stream
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write to the stream</param>
            /// <param name="offset">The offset to begin writing from</param>
            public void DirectWrite(byte[] buffer, int offset)
            {
                DirectWrite(buffer, offset, buffer.Length - offset); // Write the bytes to the stream
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write to the stream</param>
            /// <param name="offset">The offset to begin writing from</param>
            /// <param name="length">The number of bytes to write</param>
            public void DirectWrite(byte[] buffer, int offset, int length)
            {
#if Validation_soft // Important check
                if (client == null || !client.Connected) throw new InvalidOperationException("Cannot send data from an offline client"); // Check if the client's offline
#endif
                client.Send(buffer, offset, length, SocketFlags.None); // Send the bytes to the stream
            }

            /// <summary>
            /// Write a new line to the stream
            /// </summary>
            /// <param name="data">The line to write to the stream</param>
            public void WriteLine(string data)
            {
                byte[] sendBuffer = sendEncoder.GetBytes(data + writeNewLine); // Convert the line and the terminator to bytes
                DirectWrite(sendBuffer, 0, sendBuffer.Length); // Write the bytes to the stream
            }

            /// <summary>
            /// Set the send encoding to encode new lines with
            /// </summary>
            /// <param name="encoding">The encoding to use</param>
            public void SetSendEncoding(Encoding encoding)
            {
                sendEncoder = encoding; // Set the encoding
            }

            /// <summary>
            /// Set the new line terminator for when sending new lines
            /// </summary>
            /// <param name="newLine">The line terminator to use</param>
            public void SetSendNewLine(string newLine)
            {
                writeNewLine = newLine; // Set the line terminator
            }
        }

        /// <summary>
        /// SSL Network Client
        /// </summary>
        public class SSLClient : TcpClient
        {
            /// <summary>
            /// SSL Stream to wrap the plain text stream in
            /// </summary>
            private System.Net.Security.SslStream sslStream;
            /// <summary>
            /// Indicates if the certificate warning should be ignored
            /// </summary>
            private bool IgnoreCertificateWarning = false;
            /// <summary>
            /// The domain or address of the server to connect to
            /// </summary>
            private string serverDestination;

            /// <summary>
            /// Init the client
            /// </summary>
            /// <param name="certWarn">True to ignore certificate warnings, otherwise false</param>
            public SSLClient(bool certWarn) : base()
            {
                ClientInit(certWarn); // Init the client
            }

            /// <summary>
            /// Init the clinet
            /// </summary>
            /// <param name="crtWrn">True to ignore certificate warnings, otherwise false</param>
            private void ClientInit(bool crtWrn)
            {
                sslStream = null; // Reset the stream
                serverDestination = ""; // Reset the destination
                IgnoreCertificateWarning = crtWrn; // Set the certificate ignoration flag
            }

            /// <summary>
            /// Init the client
            /// </summary>
            /// <param name="address">The hostname or address of the server</param>
            /// <param name="port">The port to connect to</param>
            /// <param name="certWarn">True to ignore certificate warnings, otherwise false</param>
            public SSLClient(string address, int port, bool certWarn) : base(address, port)
            {
                ClientInit(certWarn); // Init the client
                serverDestination = address; // Set the destination
            }

            /// <summary>
            /// Init the client
            /// </summary>
            /// <param name="ep">The endpoint to connect to</param>
            /// <param name="certWarn">True to ignore certificate warnings, otherwise false</param>
            public SSLClient(IPEndPoint ep, bool certWarn) : base(ep)
            {
                ClientInit(certWarn); // Init the client
                serverDestination = ep.Address.ToString(); // Set the server destination
            }

            /// <summary>
            /// Init the client
            /// </summary>
            /// <param name="stream">The SSL Stream to wrap around</param>
            public SSLClient(System.Net.Security.SslStream stream)
            {
                ClientInit(false); // Init the client
                sslStream = stream; //Set the ssl stream
            }

            /// <summary>
            /// Manually set the server's certificate hostname to use
            /// </summary>
            /// <param name="certificateHostName">The domain name or address in the server's certificate</param>
            public void SetServerDestination(string certificateHostName)
            {
                serverDestination = certificateHostName; // Set the hostname
            }

            /// <summary>
            /// Start the client
            /// </summary>
            public new void Start()
            {
                if (IgnoreCertificateWarning) // Ignore certificate warnings
                {
                    //Define a new callback for validation
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback((sender, cert, chain, errors) => {
                        return true; // Return true, without any validation
                    });
                }
                base.Start(); // Start the base TCP Client
                NetworkStream ns = new NetworkStream(client); // Get the network stream of the client
                sslStream = new System.Net.Security.SslStream(ns); // Wrap the stream inside ssl stream
                sslStream.AuthenticateAsClient(serverDestination); // Authenticate to the server
            }

            /// <summary>
            /// Enable an event listener callback
            /// </summary>
            /// <param name="enableCode">The callback to enable</param>
            private void StartCallbackRead(Utils.NetworkIO.ReadEventCodes enableCode)
            {
                if (receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.Stopped) // All callbacks stopped
                {
                    receiveCallbackMode = enableCode; // Set the caller callback
                    Utils.NetworkIO.ClientReadObject readObject = new Utils.NetworkIO.ClientReadObject() // Define new read object
                    {
                        buffer = new byte[recvSize], // Define the bytes buffer
                        lineRecvResult = new StringBuilder() // Define the string buffer
                    };

                    sslStream.BeginRead(readObject.buffer, 0, recvSize, new AsyncCallback(ReceiveCallbackSSL), readObject); // Begin reading from the stream
                }
                else if (receiveCallbackMode != Utils.NetworkIO.ReadEventCodes.Both && receiveCallbackMode != enableCode) receiveCallbackMode = enableCode; // Enable the other callback too
            }

            /// <summary>
            /// SSL Receive callback fro events
            /// </summary>
            /// <param name="ar">Async Result</param>
            private void ReceiveCallbackSSL(IAsyncResult ar)
            {
                int bytesRead = 0; // The number of bytes read from the stream
                Utils.NetworkIO.ClientReadObject readObject = (Utils.NetworkIO.ClientReadObject)ar.AsyncState; // Get the current read object
                try
                {
                    bytesRead = sslStream.EndRead(ar); // Read from the stream
                }
                catch (Exception ex) // Something went wrong
                {
                    ForceStop(); // Stop the client
                    return; // Return
                }

                if (bytesRead > 0) // Bytes are available
                {
                    byte[] dataBuffer = new byte[bytesRead]; // Define the data buffer
                    Array.Copy(readObject.buffer, dataBuffer, bytesRead); // Copy the bytes read to the data buffer

                    if (receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.Both || receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.DataRecv) DataReceivedMethod(dataBuffer); // Invoke the data received event
                    if (receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.Both || receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.LineRecv) // Invoke the line received event
                    {
                        string subResult = recvEncoder.GetString(dataBuffer, 0, bytesRead); // Encode the bytes to string
                        readObject.lineRecvResult.Append(subResult); // Appnend the data to the string buffer
                        if (subResult.EndsWith(readNewLine)) // Check if the buffer ends with the new line
                        {
                            LineReceivedMethod(readObject.lineRecvResult.ToString()); // Invoke the new line event
                            readObject.lineRecvResult.Clear(); // Clear the string buffer
                        }
                    }
                }

                Array.Clear(readObject.buffer, 0, recvSize); // Clear the receive buffer

                try
                {
                    sslStream.BeginRead(readObject.buffer, 0, recvSize, new AsyncCallback(ReceiveCallbackSSL), readObject); // Re-Read from the stream
                }
                catch (Exception) // Something went wrong
                {
                    ForceStop(); // Stop the client
                }
            }

            /// <summary>
            /// Add an event listener for when new bytes are available
            /// </summary>
            /// <param name="callback">The function to call when new bytes are available</param>
            public new void AddEventDataReceived(Action<byte[]> callback)
            {
                DataReceived += callback; // Add the function to the event
                StartCallbackRead(Utils.NetworkIO.ReadEventCodes.DataRecv); // Start the callback
            }

            /// <summary>
            /// Add and event listener for when new lines are available
            /// </summary>
            /// <param name="callback">The function to call when new lines are available</param>
            public new void AddEventLineReceived(Action<string> callback)
            {
                LineReceived += callback; // Add the function to the event
                StartCallbackRead(Utils.NetworkIO.ReadEventCodes.LineRecv); // Enable the callback
            }

            /// <summary>
            /// Directly read bytes from the stream
            /// </summary>
            /// <param name="maxSize">The maximum number of bytes to read</param>
            /// <returns>The array of bytes read from the stream</returns>
            public new byte[] DirectRead(int maxSize)
            {
#if Validation_soft // Important check
                if (sslStream == null || client == null || !client.Connected) throw new InvalidOperationException("Cannot read from stream, when client is offline"); // Check if the base client is running
#endif
#if Validation_hard // Not so important check
                if (sslStream.CanRead) throw new InvalidOperationException("Cannot read from stream, SSL client can't read"); // Check if the sslStream object can read from the stream
#endif
                byte[] buffer = new byte[maxSize]; // Define the receive buffer
                int bytesRead = sslStream.Read(buffer, 0, maxSize); // Read from the stream
                byte[] dataBuffer = new byte[bytesRead]; // Define the data buffer
                Array.Copy(buffer, dataBuffer, bytesRead); // Copy the bytes read to the data buffer
                return dataBuffer; // Return the data buffer
            }

            /// <summary>
            /// Read a new line from the stream
            /// </summary>
            /// <returns>The next new line read from the stream</returns>
            public new string ReadLine()
            {
#if Validation_soft // Important check
                if (sslStream == null || client == null || !client.Connected) throw new InvalidOperationException("Cannot read from stream, when client is offline"); // Check if the base client is running
#endif
#if Validation_hard // Not so important check
                if (sslStream.CanRead) throw new InvalidOperationException("Cannot read from stream, SSL client can't read"); // Check if the sslStream can read
#endif
                StringBuilder result = new StringBuilder(); // Define string buffer

                while (true) // Loop infinitely
                {
                    byte[] buffer = new byte[recvSize]; // Define the receive buffer
                    int bytesRead = sslStream.Read(buffer, 0, recvSize); // Read bytes from the stream
                    string subResult = recvEncoder.GetString(buffer, 0, bytesRead); // Convert the bytes read to string
                    result.Append(subResult); // Append the string to the string buffer
                    if (subResult.EndsWith(readNewLine)) break; // Stop if a line has ended
                }

                return result.ToString(); // Return the current new line
            }

            /// <summary>
            /// Read bytes directly from the stream async
            /// </summary>
            /// <param name="maxSize">The number of bytes to read from the stream</param>
            /// <returns>The task with the bytes read from the stream</returns>
            public async new Task<byte[]> DirectReadAsync(int maxSize)
            {
                return await Task<byte[]>.Factory.StartNew(() => DirectRead(maxSize)).ConfigureAwait(false); // Wait for the task to complete
            }

            /// <summary>
            /// Read a new line from the stream async
            /// </summary>
            /// <returns>The task with the new line read from the stream</returns>
            public async new Task<string> ReadLineAsync()
            {
                return await Task<string>.Factory.StartNew(() => ReadLine()).ConfigureAwait(false); // Wait for the task to complete
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write</param>
            /// <param name="offset">The offset to begin writing from</param>
            /// <param name="length">The number of bytes to write</param>
            public new void DirectWrite(byte[] buffer, int offset, int length)
            {
                sslStream.Write(buffer, offset, length); // Write the bytes to the stream
            }

            /// <summary>
            /// Write a new line to the stream
            /// </summary>
            /// <param name="data">The line to write out</param>
            public new void WriteLine(string data)
            {
                byte[] buffer = sendEncoder.GetBytes(data + writeNewLine); // Convert the line and the terminator to a byte array
                DirectWrite(buffer, 0, buffer.Length); // Write the bytes to the stream
            }

            /// <summary>
            /// Gracefully stop the client
            /// </summary>
            public new void GracefulStop()
            {
                if (clientOffline) return; // Check if the client's running
                sslStream.Close(); // Close the stream
                sslStream.Dispose(); // Dispose the stream
                sslStream = null; // Reset the steam
                base.GracefulStop(); // Call stop on the base client
            }
        }
    }

    /// <summary>
    /// Utilities for basic networking tasks
    /// </summary>
    namespace Utils
    {
        /// <summary>
        /// Network input/output utils
        /// </summary>
        public class NetworkIO
        {
            /// <summary>
            /// Callback event codes
            /// </summary>
            public enum ReadEventCodes
            {
                DataRecv, // Receive only data
                LineRecv, // Receive only new lines
                Both, // Receive both
                Stopped // Receive nothing
            }

            /// <summary>
            /// Async Callback read object
            /// </summary>
            public struct ClientReadObject
            {
                /// <summary>
                /// Byte buffer for reading bytes
                /// </summary>
                public byte[] buffer;
                /// <summary>
                /// String buffer for reading lines
                /// </summary>
                public StringBuilder lineRecvResult;
            }
        }

        /// <summary>
        /// SSL Utils
        /// </summary>
        public class SSL
        {
            /// <summary>
            /// Default SSL Protocols
            /// </summary>
            public const System.Security.Authentication.SslProtocols defaultProtocols = System.Security.Authentication.SslProtocols.Tls | System.Security.Authentication.SslProtocols.Ssl3;

            /// <summary>
            /// SSL Server Initialization data
            /// </summary>
            public struct ServerSSLData
            {
                /// <summary>
                /// The certificate to serve
                /// </summary>
                public X509Certificate2 certificate;
                /// <summary>
                /// The domain or address in the certificate file
                /// </summary>
                public string serverAddress;
                /// <summary>
                /// The protocols to accept
                /// </summary>
                public System.Security.Authentication.SslProtocols protocols;
            }

            /// <summary>
            /// Parse a certification file to a certificate object
            /// </summary>
            /// <param name="filePath">The path of the file to parse</param>
            /// <returns>The parsed certificate</returns>
            public static X509Certificate2 ParseCertificationFile(string filePath)
            {
#if Validation_hard // Not so important check
                if (!System.IO.File.Exists(filePath) || (new System.IO.FileInfo(filePath)).Length == 0) throw new ArgumentException($"The specified file is not valid: {filePath}"); // Check if the file exists and isn't empty
#endif
                return new X509Certificate2(filePath); // Parse and return the certificate
            }

            /// <summary>
            /// Parse SSL Data for the SSL Server
            /// </summary>
            /// <param name="serverAddress">The domain name or addres in the server's certificate file</param>
            /// <param name="protocols">The ssl protocols to accept</param>
            /// <returns>The SSL Data for the server</returns>
            public static ServerSSLData ParseServerData(string serverAddress, System.Security.Authentication.SslProtocols protocols)
            {
                return new ServerSSLData()
                {
                    protocols = protocols, // Set the protocols
                    serverAddress = serverAddress // Set the address
                };
            }

            /// <summary>
            /// Parse SSL Data for the SSL Server
            /// </summary>
            /// <param name="filePath">The path of the certification file</param>
            /// <param name="serverAddress">The domain name or address of the server's certificate</param>
            /// <param name="protocols">The ssl protocols to accept</param>
            /// <returns>The SSL Data for the SSL Server</returns>
            public static ServerSSLData ParseServerData(string filePath, string serverAddress, System.Security.Authentication.SslProtocols protocols)
            {
                return new ServerSSLData()
                {
                    certificate = ParseCertificationFile(filePath), // Set the certification
                    protocols = protocols, // Set the protocols
                    serverAddress = serverAddress // Set the server address
                };
            }
        }

        /// <summary>
        /// IPv4 Utils
        /// </summary>
        public class IPv4
        {
            /// <summary>
            /// Check if a port number is valid
            /// </summary>
            /// <param name="portNumber">The port number to check</param>
            /// <returns>True if the specified port number is valid, otherwise false</returns>
            public static bool IsPortValid(int portNumber)
            {
                const int maxPortNumber = 65535; // Define the max port number
                const int minPortNumber = 1; // Define the min port number
                return portNumber >= minPortNumber && portNumber <= maxPortNumber; // Check if the port is between them
            }

            /// <summary>
            /// Check if the destination is valid, and resolve it if it's a hostname
            /// </summary>
            /// <param name="destination">The address or hostname to check</param>
            /// <returns>A boolean indicating if it's a valid address and an IPAddress, with the specified destination</returns>
            public static Tuple<bool, IPAddress> IsValidDestination(string destination)
            {
                Tuple<bool, IPAddress> ipCheck = IsValidAddress(destination); // Check if the IP is valid
                if (ipCheck.Item1) return ipCheck; // If IP is valid return the IP and true
                else return Dns.IsValidHostname(destination); // Try DNS resolving
            }

            /// <summary>
            /// Check if and IP Address is valid
            /// </summary>
            /// <param name="ipAddress">The IP Address to check</param>
            /// <returns>A boolean indicating if the ip is valid and the IP Address</returns>
            public static Tuple<bool, IPAddress> IsValidAddress(string ipAddress)
            {
                return new Tuple<bool, IPAddress>(IPAddress.TryParse(ipAddress, out IPAddress address), address); // Return the results of the check and the IP Address
            }
        }

        /// <summary>
        /// Dns Utils
        /// </summary>
        public class Dns
        {
            /// <summary>
            /// Check if a hostname is valid
            /// </summary>
            /// <param name="hostname">The hostname to check</param>
            /// <returns>A boolean indicating if the hostname is valid and the resolved IP Address</returns>
            public static Tuple<bool, IPAddress> IsValidHostname(string hostname)
            {
                try
                {
                    IPAddress[] result = System.Net.Dns.GetHostAddresses(hostname); // Resolve the hostname
                    return new Tuple<bool, IPAddress>(true, result[0]); // Return the IP Address of the hostname and true
                }
                catch (ArgumentException ex) // The hostname isn't valid
                {
                    return new Tuple<bool, IPAddress>(false, null); // Return false and null for the IP Address
                }
            }
        }
    }
}
