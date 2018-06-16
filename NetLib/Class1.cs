﻿#define Validation_hard // Enable Hard, strict error cheking, may throw error even when the program will probably work fine
#define Logging_verbose // Enable verbose logging of errors
#if Validation_hard // When hard validation is enabled it will also enable soft validation
#define Validation_soft // Will throw the same errors but a with a little more userfriendly message as the program would without try catching would
#endif
using System;
using System.Text;
using System.Threading.Tasks;
using System.Net; // Basic networking objects
using System.Net.Sockets; // Sockets
using System.Security.Cryptography.X509Certificates; // Certificate parsing
using System.Collections.Generic;
using NetLib.Interfaces;

namespace NetLib
{
    namespace Interfaces
    {
        /// <summary>
        /// Class for building augmentations
        /// </summary>
        public class Augmentation
        {
            /// <summary>
            /// Disable default ctonsole messages
            /// </summary>
            protected bool disableConsole = true;

            /// <summary>
            /// Override this method to modify bytes before sending them
            /// </summary>
            /// <param name="data">The bytes to be sent currently</param>
            /// <param name="offset">The offset to start sending the bytes from</param>
            /// <param name="length">The number of bytes to send</param>
            /// <returns>The bytes to send (default: no changes)</returns>
            public virtual Tuple<byte[], int, int> OnBeforeSendBytes(byte[] data, int offset, int length)
            {
                if (!disableConsole) Console.WriteLine("Before sending bytes");
                return new Tuple<byte[], int, int>(data, offset, length);
            }

            /// <summary>
            /// Override this method to inspect sent bytes
            /// </summary>
            /// <param name="data">The bytes sent</param>
            /// <param name="offset">The offset to start sending the bytes from</param>
            /// <param name="length">The number of bytes to send</param>
            public virtual void OnAfterSendBytes(byte[] data, int offset, int length)
            {
                if (!disableConsole) Console.WriteLine("After bytes sent");
            }

            /// <summary>
            /// Override this method to modify string before sending them
            /// </summary>
            /// <param name="data">The string to be sent currently</param>
            /// <param name="encoding">The encoding used to construct the string</param>
            /// <returns>The string to send (default: no changes)</returns>
            public virtual string OnBeforeSendString(string data, Encoding encoding)
            {
                if (!disableConsole) Console.WriteLine("Before sending string");
                return data;
            }

            /// <summary>
            /// Override this method to inspect sent strings
            /// </summary>
            /// <param name="data">The string sent</param>
            /// <param name="encoding">The encoding used to construct the string</param>
            public virtual void OnAfterSendString(string data, Encoding encoding)
            {
                if (!disableConsole) Console.WriteLine("After string sent");
            }

            /// <summary>
            /// Override this method to receive the socket start event
            /// </summary>
            public virtual void OnStart()
            {
                if (!disableConsole) Console.WriteLine("Socket started");
            }

            /// <summary>
            /// Override this method to receive the socket stop event
            /// </summary>
            public virtual void OnStop()
            {
                if (!disableConsole) Console.WriteLine("Socket stopped");
            }

            /// <summary>
            /// Override this method to receive the augmentation install event
            /// <param name="socket">The socket that's installing the augmentation</param>
            /// </summary>
            public virtual void OnInstalled(INetworkSocket socket)
            {
                if (!disableConsole) Console.WriteLine("Augment installed");
            }

            /// <summary>
            /// Override this method to receive the augmentation uninstall event
            /// </summary>
            public virtual void OnUninstalled()
            {
                if (!disableConsole) Console.WriteLine("Augment Uninstalled");
            }

            /// <summary>
            /// Override this method to modify the bytes received before processing them
            /// </summary>
            /// <param name="data">The bytes received</param>
            /// <returns>The bytes to process (default: no changes)</returns>
            public virtual byte[] OnBeforeReceiveBytes(byte[] data)
            {
                if (!disableConsole) Console.WriteLine("Before reading bytes");
                return data;
            }

            /// <summary>
            /// Override this method to instpect the bytes received
            /// </summary>
            /// <param name="data">The bytes received</param>
            public virtual void OnAfterReceiveBytes(byte[] data)
            {
                if (!disableConsole) Console.WriteLine("After reading bytes");
            }

            /// <summary>
            /// Override this method to modify the string received before processing them
            /// </summary>
            /// <param name="data">The string received</param>
            /// <param name="encoding">The encoding used to construct the string</param>
            /// <returns>The string to process</returns>
            public virtual string OnBeforeReceiveString(string data, Encoding encoding)
            {
                if (!disableConsole) Console.WriteLine("Before reading string");
                return data;
            }

            /// <summary>
            /// Override this method to inspect the string received
            /// </summary>
            /// <param name="data">The received string</param>
            /// <param name="encoding">The encoding used to construct the string</param>
            public virtual void OnAfterReceiveString(string data, Encoding encoding)
            {
                if (!disableConsole) Console.WriteLine("After reading string");
            }
        }

        /// <summary>
        /// Implement augmentation functionallity
        /// </summary>
        interface IAugmentable
        {
            /// <summary>
            /// Install the augmentation
            /// </summary>
            void InstallAugmentation(Augmentation a);
            /// <summary>
            /// Uninstall the augmentation
            /// </summary>
            void UninstallAugmentation(Augmentation a);
        }

        /// <summary>
        /// Basic network connection functionallity
        /// </summary>
        public interface INetworkSocket
        {
            /// <summary>
            /// Start the socket connection
            /// </summary>
            void Start();
            /// <summary>
            /// Gracefully stop the socket connection
            /// </summary>
            void GracefulStop();
            /// <summary>
            /// Forcefully stop the socket connection
            /// </summary>
            void ForceStop();
            /// <summary>
            /// The encoding used for sending string data
            /// </summary>
            Encoding SendEncoder { get; set; }
            /// <summary>
            /// The encoding used for reading string data
            /// </summary>
            Encoding RecvEncoder { get; set; }
            /// <summary>
            /// The new line terminator to use when sending strings
            /// </summary>
            string WriteNewLine { get; set; }
            /// <summary>
            /// The new line terminator to use when receiving strings
            /// </summary>
            string ReadNewLine { get; set; }
            /// <summary>
            /// The maximum size of the receive buffer
            /// </summary>
            int RecvSize { get; set; }
        }

        /// <summary>
        /// Basic Server Functionallity
        /// </summary>
        interface INetworkServer
        {
            /// <summary>
            /// Add an event listener for client connects
            /// </summary>
            /// <param name="callback">The function to call when a new client connects</param>
            void AddEventClientConnected(Action callback);
            /// <summary>
            /// Add an event listener for client disconnects
            /// </summary>
            /// <param name="callback">The function to call when a client disconnects</param>
            void AddEventClientDisconnected(Action callback);
        }

        /// <summary>
        /// Basic Multi Socket Server Functionallity
        /// </summary>
        interface INetworkMultiServer
        {
            /// <summary>
            /// List the IDs of connected clients
            /// </summary>
            /// <returns>A string[] filled with the ID of the connected clients</returns>
            string[] ListClients();
            /// <summary>
            /// Add an event listener for new client connections
            /// </summary>
            /// <param name="callback">The function to call when a new client connects</param>
            void AddEventClientConnected(Action<string> callback);
            /// <summary>
            /// Add an event listener for client disconnections
            /// </summary>
            /// <param name="callback">The function to call when a client disconnects</param>
            void AddEventClientDisconnected(Action<string> callback);
        }

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
        }

        /// <summary>
        /// Write messages to a multi socket server connection
        /// </summary>
        interface INetworkMultiWriter
        {
            /// <summary>
            /// Write bytes to the stream
            /// </summary>
            /// <param name="buffer">The bytes to send to the stream</param>
            /// <param name="clientid">The ID of the client to send the data to</param>
            void DirectWrite(byte[] buffer, string clientid);
            /// <summary>
            /// Write bytes to the stream with offset
            /// </summary>
            /// <param name="buffer">The bytes to send to the stream</param>
            /// <param name="offset">The offset to send the bytes from</param>
            /// <param name="clientid">The ID of the client to send the data to</param>
            void DirectWrite(byte[] buffer, int offset, string clientid);
            /// <summary>
            /// ?Write bytes to the stream with offset and length
            /// </summary>
            /// <param name="buffer">The bytes to send to the stream</param>
            /// <param name="offset">The offset to send the bytes from</param>
            /// <param name="length">The number of bytes to send</param>
            /// <param name="clientid">The ID of the client to send the data to</param>
            void DirectWrite(byte[] buffer, int offset, int length, string clientid);
            /// <summary>
            /// Write a line to the stream
            /// </summary>
            /// <param name="data">The line to write to the stream</param>
            /// <param name="clientid">The ID of the client to send the new line to</param>
            void WriteLine(string data, string clientid);
        }

        /// <summary>
        /// Read messages from a multi socket server connection
        /// </summary>
        interface INetworkMultiReader
        {
            /// <summary>
            /// Read raw bytes from the stream
            /// </summary>
            /// <param name="size">Number of bytes to read</param>
            /// <param name="clientid">The ID of the client to read bytes from</param>
            /// <returns>Bytes read from the stream</returns>
            byte[] DirectRead(int size, string clientid);
            /// <summary>
            /// Read the stream until a line terminator
            /// </summary>
            /// <param name="clientid">The ID of the client to read a new line from</param>
            /// <returns>The line read from the stream</returns>
            string ReadLine(string clientid);
            /// <summary>
            /// Add an event listener for receiving bytes
            /// </summary>
            /// <param name="callback">The function to call when bytes are read</param>
            /// <param name="clientid">The ID of the client to add the event to</param>
            void AddEventDataReceived(Action<byte[]> callback, string clientid);
            /// <summary>
            /// Add an event listener for receiving lines
            /// </summary>
            /// <param name="callback">The function to call when lines are received</param>
            /// <param name="clientid">The ID of the client to add the event to</param>
            void AddEventLineReceived(Action<string> callback, string clientid);
            /// <summary>
            /// Read butes from the stream asnyc
            /// </summary>
            /// <param name="maxSize">The size of the buffer to read from the stream</param>
            /// <param name="clientid">The ID of the client to read bytes from</param>
            /// <returns>The bytes read from the stream</returns>
            Task<byte[]> DirectReadAsync(int maxSize, string clientid);
            /// <summary>
            /// Read the stream until a line terminator async
            /// </summary>
            /// <param name="clientid">The ID of the client to read a new line from</param>
            /// <returns>The line read from the stream</returns>
            Task<string> ReadLineAsync(string clientid);
        }
    }

    namespace Augmentations
    {
        /// <summary>
        /// Basic I/O Bytes Traffic counter
        /// </summary>
        public class Counter : Augmentation
        {
            /// <summary>
            /// The total number of bytes sent since the start/resume/reset of this counter
            /// </summary>
            public int TotalBytesSent { get; private set; } = 0;
            /// <summary>
            /// The total number of bytes received since the start/resume/reset of this counter
            /// </summary>
            public int TotalBytesReceived { get; private set; } = 0;
            /// <summary>
            /// Indicates if the counter's paused
            /// </summary>
            protected bool isPaused = false;

            /// <summary>
            /// Create a new counter
            /// </summary>
            public Counter()
            {
                disableConsole = true; // Set the console to disabled
                Console.WriteLine("Counter constructed"); // Wrtie debug message to the console
            }

            /// <summary>
            /// Decimal trim a float without rounding it
            /// </summary>
            /// <param name="numberOfDecimals">The number of decimal places to keep</param>
            /// <param name="value">The float to format</param>
            /// <returns>The formatted string value of the float</returns>
            private string GetLastDecimals(int numberOfDecimals, float value)
            {
                string v = value.ToString(); // Convert the float to string
                string separator = ""; // Define the decimal separator
                if (v.Contains(".")) separator = "."; // Dot is the separator
                else if (v.Contains(",")) separator = ","; // Colon is the separator
                else return v; // No separator, round number, return it
                int indexOf = v.IndexOf(separator); // Get the index of the separator
                return v.Substring(0, indexOf + 1 + numberOfDecimals); // Return the formatted float
            }

            /// <summary>
            /// Format raw byte count to string
            /// </summary>
            /// <param name="input">The input number of bytes</param>
            /// <returns>The formatted string</returns>
            public string FormatBytes(int input)
            {
                const float kb = 1024; // KB bottom limit
                const float mb = kb * 1024; // MB bottom limit
                const float gb = mb * 1024; // GB bottom limit
                float bytes = input; // Float version of the input

                if (bytes < kb) // Under KB
                {
                    return $"{bytes} B"; // Return as it is
                }
                else if (bytes >= kb && bytes < mb) // In KB Range
                {
                    return $"{GetLastDecimals(2, bytes / kb)} KB"; // Return the KB formatted string
                }
                else if (bytes >= mb && bytes < gb) // In MB Range
                {
                    return $"{GetLastDecimals(2, bytes / mb)} MB"; // Return the MB formatted string
                }
                else // Out of default range, assuming GB (not hangling TB, PB)
                {
                    return $"{GetLastDecimals(2, bytes / gb)} GB"; // Return the GB formatted string
                }
            }

            /// <summary>
            /// Count sent bytes
            /// </summary>
            /// <param name="sentBytes">The sent bytes</param>
            /// <param name="offset">The offset to start sending the bytes from</param>
            /// <param name="length">The number of bytes to send from the array</param>
            public override void OnAfterSendBytes(byte[] sentBytes, int offset, int length)
            {
                if (!isPaused) TotalBytesSent += sentBytes.Length; // Add the bytes
            }

            /// <summary>
            /// Count read bytes
            /// </summary>
            /// <param name="readBytes">The read bytes</param>
            public override void OnAfterReceiveBytes(byte[] readBytes)
            {
                if (!isPaused) TotalBytesReceived += readBytes.Length; // Add the bytes
            }

            /// <summary>
            /// Get the bytes length for string values
            /// </summary>
            /// <param name="value">The string to get the byte length of</param>
            /// <param name="e">The encoding of the string</param>
            /// <returns>The length of the byte array generated from the string</returns>
            private int GetStringBytesLenght(string value, Encoding e)
            {
                return e.GetByteCount(value); // Return the number of bytes in the string
            }

            /// <summary>
            /// Count read strings
            /// </summary>
            /// <param name="readString">The read string</param>
            /// <param name="encoding">The encoding of the string</param>
            public override void OnAfterReceiveString(string readString, Encoding encoding)
            {
                if (!isPaused) TotalBytesReceived += GetStringBytesLenght(readString, encoding); // Add the bytes
            }

            /// <summary>
            /// Count sent strings
            /// </summary>
            /// <param name="sentString">The sent string</param>
            /// <param name="encoding">The encoding of the string</param>
            public override void OnAfterSendString(string sentString, Encoding encoding)
            {
                if (!isPaused) TotalBytesSent += GetStringBytesLenght(sentString, encoding); // Add the bytes
            }

            /// <summary>
            /// Reset both counters back to 0
            /// </summary>
            public void ResetCounter()
            {
                TotalBytesReceived = 0; // Reset the read counter
                TotalBytesSent = 0; // Reset the send counter
            }

            /// <summary>
            /// Pause counting the bytes
            /// </summary>
            public void PauseCounter()
            {
                isPaused = true; // Pause the counting
            }

            /// <summary>
            /// Resume counting the bytes
            /// </summary>
            public void ResumeCounter()
            {
                isPaused = false;
            }
        }

        /// <summary>
        /// Ensures string message integrity using byte type message (client-server has to install the same augmentation)
        /// </summary>
        public class ByteIntegrity : Augmentation
        {
            /// <summary>
            /// Event listener for when a new message is available
            /// </summary>
            public event Action<string> OnDataRead;
            /// <summary>
            /// Event listener for when a new message on a multi socket server is available
            /// </summary>
            public event Action<string, string> OnDataMultiRead;
            /// <summary>
            /// The separator used for splitting message content and header
            /// </summary>
            private const string separator = "?!%!?";
            /// <summary>
            /// Writer object for clients and single socket servers
            /// </summary>
            private INetworkWriter write;
            /// <summary>
            /// Reader object for clients and single socket servers
            /// </summary>
            private INetworkReader read;
            /// <summary>
            /// Writer object for multi socket servers
            /// </summary>
            private INetworkMultiWriter mWrite;
            /// <summary>
            /// Reader object for multi socket servers
            /// </summary>
            private INetworkMultiReader mRead;

            /// <summary>
            /// Init the integrity keeper
            /// </summary>
            public ByteIntegrity()
            {
                disableConsole = true; // Disable default messges
            }

            /// <summary>
            /// Give an integer 0 prefix
            /// </summary>
            /// <param name="number">The number to give the prefix for</param>
            /// <param name="prefix">The length of the total prefixed string</param>
            /// <returns>The formatted int as a string</returns>
            private string FormatInt(int number, int prefix)
            {
                string n = number.ToString(); // Convert the number to string
                string result = ""; // Define the result
                for (int i = 0; i < prefix - n.Length; i++) // Loop through the remaining 0s
                {
                    result += "0"; // Append the 0 to the front of the result
                }

                return result + n; // Return the full result
            }

            /// <summary>
            /// Format a message for sending through the protocol
            /// </summary>
            /// <param name="message">The message to format</param>
            /// <returns>The formatted message</returns>
            private string FormatMessage(string message)
            {
                return $"{FormatInt(message.Length, 10)}{separator}{message}"; // Return the formatted message
            }

            /// <summary>
            /// Event listener for when the augmentation is installed
            /// </summary>
            /// <param name="socket">The installling socket object</param>
            public override void OnInstalled(INetworkSocket socket)
            {
                if (socket as INetworkMultiServer != null) // Multi socket server
                {
                    // Set the read and write object
                    mWrite = socket as INetworkMultiWriter;
                    mRead = socket as INetworkMultiReader;
                }
                else // Single socket server or client
                {
                    // Set the read and write object
                    read = socket as INetworkReader;
                    write = socket as INetworkWriter;
                }
            }

            /// <summary>
            /// Send data from a client or single socket server
            /// </summary>
            /// <param name="message">The message to send</param>
            public void SendData(string message)
            {
                message = FormatMessage(message); // Format the message
                Encoding e = ((INetworkSocket)write).SendEncoder; // Get the send encoding of the socket
                byte[] data = e.GetBytes(message); // Get the bytes of the message
                write.DirectWrite(data, 0, data.Length); // Send the bytes
            }

            /// <summary>
            /// Send data from a multi socket server
            /// </summary>
            /// <param name="clientID">The id of the client to send the message to</param>
            /// <param name="message">The message to send</param>
            public void SendData(string clientID, string message)
            {
                message = FormatMessage(message); // Format the message
                Encoding e = ((INetworkSocket)mWrite).SendEncoder; // Get the encoding to send bytes
                byte[] data = e.GetBytes(message); // Get the bytes of the message
                mWrite.DirectWrite(data, 0, data.Length, clientID); // Write the bytes to the socket
            }

            /// <summary>
            /// Parse the blob of data into a message
            /// </summary>
            /// <returns>A parsed string message</returns>
            private string ParseMessage(ref string fullString, ref int realLength)
            {
                if (realLength == 0) // Check if the length of current message isn't set
                {
                    if (fullString.Contains(separator)) // Check if the blob contains the separator
                    {
                        string length = fullString.Split(new string[] { separator }, StringSplitOptions.None)[0]; // Get the length of the real message
                        if (!int.TryParse(length, out realLength)) throw new InvalidOperationException("The current message length invalid characters!"); // Try to parse the length to int
                    }
                }

                if (fullString.Length >= realLength && realLength != 0 && fullString.Length != 0) // Check if the message is in the blob
                {
                    string realMessage = fullString.Substring(15, realLength); // Extract the messsage from the blob
                    fullString = fullString.Substring(realLength + 15); // Extract the block from the blob
                    realLength = 0; // Reset the current message length
                    return realMessage; // Return the real messsage
                }
                else return null; // Return null result
            }

            /// <summary>
            /// Begin reading data from the socket
            /// </summary>
            public void BeginReadData()
            {
                Encoding e = ((INetworkSocket)read).RecvEncoder; // Get the read encoding
                string fullString = ""; // The blob of the received messages from the client
                int realLength = 0; // The length of the current message to parse

                read.AddEventDataReceived((data) => // Check for new data
                {
                    string msg = e.GetString(data); // Get the string message
                    fullString += msg; // Append it to the blob
                    for (int i = 0; i < 10; i++) // Spin 10 times, for larger network loads to clear
                    {
                        string result = ParseMessage(ref fullString, ref realLength); // Try to get the next message
                        if (result != null) OnDataRead?.Invoke(result); // Signal the message
                        else break; // No more full messages
                    }
                });
            }

            /// <summary>
            /// Begin reading data from a multi socket server
            /// </summary>
            /// <param name="clientID">The ID of the client to send the message to</param>
            public void BeginReadData(string clientID)
            {
                Encoding e = ((INetworkSocket)mRead).RecvEncoder; // Get the read encoding
                string fullString = ""; // The blob of the receive message from the client
                int realLength = 0; // The length of the current message to parse
                mRead.AddEventDataReceived((data) => // Check for new data
                {
                    string msg = e.GetString(data); // Get the string message
                    fullString += msg; // Append it to the blob
                    for (int i = 0; i < 10; i++) // Spin 10 times, for larger network loads to clear
                    {
                        string result = ParseMessage(ref fullString, ref realLength); // Try to get the next message
                        if (result != null) OnDataMultiRead?.Invoke(result, clientID); // Signal the message
                        else break; // No more full messages
                    }
                }, clientID);
            }
        }

        /// <summary>
        /// Basic chat server augmentation
        /// </summary>
        public class Chat : Augmentation
        {
            /// <summary>
            /// Message receive event arguments
            /// </summary>
            public class MessageReceivedEventArgs
            {
                /// <summary>
                /// Create a new event argument object
                /// </summary>
                /// <param name="msg">The message the client sent</param>
                /// <param name="user">The name of the user who sent the message</param>
                public MessageReceivedEventArgs(string msg, string user)
                {
                    Message = msg; // Set the message
                    Username = user; // Set the username
                }

                /// <summary>
                /// The message the user sent
                /// </summary>
                public string Message { get; private set; }
                /// <summary>
                /// The name of the user who sent the message
                /// </summary>
                public string Username { get; private set; }
            }

            /// <summary>
            /// Instance of the server
            /// </summary>
            private INetworkMultiServer chatServer;
            /// <summary>
            /// Instance of the client
            /// </summary>
            private INetworkSocket chatClient;
            /// <summary>
            /// The name of the client user
            /// </summary>
            private string userName;
            /// <summary>
            /// ByteIntegrity object of the client
            /// </summary>
            private ByteIntegrity clientBI = null;
            /// <summary>
            /// A list of users connected to the server
            /// </summary>
            private List<Tuple<string, string>> users = new List<Tuple<string, string>>();
            /// <summary>
            /// Event listener for when failed to send a direct message
            /// </summary>
            public event Action<string> DirectMessageFailed;
            /// <summary>
            /// Event listener for when failed to broadcast a message
            /// </summary>
            public event Action<string> BroadcastFailed;
            /// <summary>
            /// Event listener for when the selected username is already taken
            /// </summary>
            public event Action<string> UsernameSelectionFailed;
            /// <summary>
            /// Event listener for when a new message is received
            /// </summary>
            public event Action<MessageReceivedEventArgs> MessageReceived;
            /// <summary>
            /// The separator string to use with message headers
            /// </summary>
            private const string messageHeaderSeparator = ":a!a:";

            /// <summary>
            /// Init the chat augmentation
            /// <param name="username">The name of the user to use for chatting (ignore if installing on server)</param>
            /// </summary>
            public Chat(string username = null)
            {
                disableConsole = true; // Disable default messages
                userName = username; // Set the username
            }

            /// <summary>
            /// Augmentation installed on the socket
            /// </summary>
            /// <param name="socket"></param>
            public override void OnInstalled(INetworkSocket socket)
            {
                if (socket as INetworkMultiServer != null) // Chat Server binding
                {
                    chatServer = (INetworkMultiServer)socket; // Set the server socket
                    ServerSetup(); // Setup the server with chat functions
                }
                else if (socket as INetworkServer != null) // Invalid binding (single socket server)
                {
#if Validation_hard
                    throw new InvalidOperationException("Can't augment chat to a single socket server. Use MultiTcpServer or MultiSSLServer instead");
#else
                    return;
#endif
                }
                else // Chat Client binding
                {
                    chatClient = socket; // Set the client socket
                    ClientSetup(); // Setup the client with the chat functions
                }
            }

            /// <summary>
            /// Check if the specified client has a username
            /// </summary>
            /// <param name="clientID">The ID of the client to check</param>
            /// <returns>True if the client has a username otherwise false</returns>
            private bool IsUsernameSet(string clientID)
            {
                Tuple<string, string> result = users.Find((user) => clientID == user.Item1); // Search for the client
                return result != null; // Return the result
            }

            private string GetUsername(string clientID)
            {
                Tuple<string, string> result = users.Find((user) => clientID == user.Item1); // Search for the client
                if (result == null) return null; // Check if the resul's null
                return result.Item2; // Return the name of the client
            }

            /// <summary>
            /// Check if a user already exists with the specified username
            /// </summary>
            /// <param name="username">The username to check</param>
            /// <returns>True if the username isn't available, otherwise false</returns>
            private bool UsernameTaken(string username)
            {
                foreach (Tuple<string, string> clientData in users) // Loop through the registered users
                {
                    if (clientData.Item2 == username) return true; // Return true if the username's found
                }

                return false; // Return false, if the username isn't found
            }

            /// <summary>
            /// Send a message to a specified username
            /// </summary>
            /// <param name="user">The user to send the message to</param>
            /// <param name="message">The message to send</param>
            /// <param name="server">The server instance to send the messge with</param>
            /// <param name="sender">The sender of the message</param>
            /// <returns>True if message sent, otherwise false</returns>
            private bool SendTo(string user, string message, ByteIntegrity server, string sender)
            {
                Tuple<string, string> result = users.Find((u) => user == u.Item2); // Find the target user
                if (result == null) return false; // Return false if user not found
                string id = result.Item1; // Get the clientID of the user
                server.SendData(id, $"{sender}{messageHeaderSeparator}{message}"); // Send the message to the user
                return true; // Return true, successful message sending
            }

            /// <summary>
            /// Broadcast a message to everyone
            /// </summary>
            /// <param name="sender">The sender of the message</param>
            /// <param name="message">The message</param>
            /// <param name="server">The server instance to send the messag with</param>
            private void Broadcast(string sender, string message, ByteIntegrity server)
            {
                List<Tuple<string, string>> toRemove = new List<Tuple<string, string>>(); // Define a list of client to remove

                foreach (Tuple<string, string> clientData in users) // Loop through the users
                {
                    if (clientData.Item2 == sender) continue; // Don't send the message if the user is the sender
                    try
                    {
                        server.SendData(clientData.Item1, $"{sender}{messageHeaderSeparator}{message}"); // Send the message to the current user
                    }
                    catch (InvalidOperationException ex) // Current user disconnected
                    {
#if Logging_verbose // Do the logging
                        Console.WriteLine("Supressed client error");
                        Console.WriteLine(ex);
#endif
                        toRemove.Add(clientData); // Add disconnected client to the list of clients to remove
                    }
                }

                toRemove.ForEach((item) => users.Remove(item)); // Remove the disconnected clients from the list
            }

            /// <summary>
            /// Setup the server for chat functions
            /// </summary>
            private void ServerSetup()
            {
                IAugmentable augmentable = chatServer as IAugmentable; // Get the augmentation handler of the server
                ByteIntegrity bi = new ByteIntegrity(); // Create the integrity augmentation
                augmentable.InstallAugmentation(bi); // Install the augmentation
                chatServer.AddEventClientConnected((id) => // Event: new client connected
                {
                    bi.OnDataMultiRead += (message, clientID) => // Event: client sent data
                    {
                        if (clientID != id) return; // If the client id's don't match -> return
#if Logging_verbose
                        Console.WriteLine($"[{clientID}]: {message}"); // Debugging message
#endif
                        if (message.StartsWith("user-info")) // Set the username
                        {
                            string name = message.Split(new string[] { messageHeaderSeparator }, StringSplitOptions.None)[1]; // Get the username parameter from the message
                            if (UsernameTaken(name)) // Check if the username if taken already
                            {
                                bi.SendData(clientID, $"userfailed{messageHeaderSeparator}{name}"); // Notify the client
                                return; // Return
                            }
                            users.Add(new Tuple<string, string>(id, name)); // Add the user to the user list
                        }
                        else if (message.StartsWith($"dm{messageHeaderSeparator}")) // Send direct message
                        {
                            string[] data = message.Split(new string[] { messageHeaderSeparator }, StringSplitOptions.None); // Get the parameter array
                            string user = data[1]; // Get the target user
                            string msg = data[2]; // Get the message
                            if (!IsUsernameSet(id)) // Check if the client set a username
                            {
                                bi.SendData(clientID, $"dmfailed{messageHeaderSeparator}{user}{messageHeaderSeparator}{msg}"); // Send an error to the sender
                            }
                            else
                            {
                                string sender = GetUsername(id); // Get the username of the sender
                                if (!SendTo(user, msg, bi, sender)) // Try to send the message to the target client
                                {
                                    bi.SendData(clientID, $"dmfailed{messageHeaderSeparator}{user}{messageHeaderSeparator}{msg}"); // Send the error message to the sender client
                                }
                            }
                        }
                        else // Broadcast message
                        {
                            if (!IsUsernameSet(id)) // Check if the client set a username
                            {
                                bi.SendData(clientID, $"broadcastfailed{messageHeaderSeparator}{message}"); // Send the error message to the client
                            }
                            else
                            {
                                string sender = GetUsername(id); // Get the username of the sender
                                Broadcast(sender, message, bi); // Broadcast the message to every user
                            }
                        }
                    };
                    bi.BeginReadData(id); // Start reading from the client
                });
            }

            /// <summary>
            /// Set a new username if the current one's taken
            /// </summary>
            /// <param name="user">The new username to set</param>
            public void ReloadUsername(string user)
            {
                userName = user; // Set the global username
                clientBI.SendData($"user-info{messageHeaderSeparator}{user}"); // Set the username on the server
            }

            /// <summary>
            /// Setup the client for chat functions
            /// </summary>
            private void ClientSetup()
            {
                IAugmentable augmentable = chatClient as IAugmentable; // Get the augmentation handler of the client
                clientBI = new ByteIntegrity(); // Create a new byteintegrity augmentation
                augmentable.InstallAugmentation(clientBI); // Install the augmentation to the client
                if (userName == null) throw new InvalidOperationException("Can't start chatting without setting a username!"); // Check if the username is set
                clientBI.SendData($"user-info{messageHeaderSeparator}{userName}"); // Set the username on the server
                clientBI.OnDataRead += (message) => // Event: server sent data
                {
                    const string dmFailed = "dmfailed" + messageHeaderSeparator; // Direct messgae failed header
                    const string broadcastFailed = "broadcastfailed" + messageHeaderSeparator; // Broadcast failed header
                    const string usernameFailed = "userfailed" + messageHeaderSeparator; // Username set failed header
                    if (message.StartsWith(dmFailed)) // Check if a direct message failed
                    {
                        DirectMessageFailed?.Invoke(message.Split(new string[] { messageHeaderSeparator }, StringSplitOptions.None)[1]); // Fire the event
                    }
                    else if (message.StartsWith(broadcastFailed)) // Check if a broadcast failed
                    {
                        BroadcastFailed?.Invoke(message.Split(new string[] { messageHeaderSeparator }, StringSplitOptions.None)[1]); // Fire the event
                    }
                    else if (message.StartsWith(usernameFailed)) // Check if the failed to set the username
                    {
                        UsernameSelectionFailed?.Invoke(message.Split(new string[] { messageHeaderSeparator }, StringSplitOptions.None)[1]); // Fire the event
                    }
                    else // Successful
                    {
                        string[] msgData = message.Split(new string[] { messageHeaderSeparator }, StringSplitOptions.None); // Get the contents of the message
                        string userName = msgData[0]; // Get the username
                        string msg = msgData[1]; // Get the message
                        MessageReceivedEventArgs e = new MessageReceivedEventArgs(msg, userName); // Create the event data
                        MessageReceived?.Invoke(e); // Invoke the event
                    }
                };
                clientBI.BeginReadData(); // Begin reading from the client
            }

            /// <summary>
            /// Send a message to everyone
            /// </summary>
            /// <param name="message">The message to send</param>
            public void SendMessage(string message)
            {
                if (clientBI == null) throw new InvalidOperationException("Only chat clients can send message, the server is only a forwarder for client messages"); // Check if we're a chat client
                clientBI.SendData(message); // Broadcast the message
            }

            /// <summary>
            /// Send a Direct Message to a user
            /// </summary>
            /// <param name="username">The user to send the message to</param>
            /// <param name="message">The message to send</param>
            public void SendMessage(string username, string message)
            {
                if (clientBI == null) throw new InvalidOperationException("Only chat clients can send message, the server is only a forwarder for client messages"); // Check if we're a chat client
                clientBI.SendData($"dm{messageHeaderSeparator}{username}{messageHeaderSeparator}{message}"); // Broadcast the message
            }
        }
    }

    namespace Servers
    {
        /// <summary>
        /// Single Client Tcp Server
        /// </summary>
        public class SingleTcpServer : INetworkReader, INetworkWriter, INetworkSocket, INetworkServer, IAugmentable
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
            public int RecvSize { get; set; }
            /// <summary>
            /// The encoding used when receiving new lines
            /// </summary>
            public Encoding RecvEncoder { get; set; }
            /// <summary>
            /// The encoding use when sending new lines
            /// </summary>
            public Encoding SendEncoder { get; set; }
            /// <summary>
            /// The new line terminator to read until
            /// </summary>
            public string ReadNewLine { get; set; }
            /// <summary>
            /// The new line terminator to send
            /// </summary>
            public string WriteNewLine { get; set; }
            /// <summary>
            /// Indicates if the server's running
            /// </summary>
            protected bool serverOffline = true;
            /// <summary>
            /// Indicates if the server should restart after the current client closes
            /// </summary>
            protected bool restartReading;
            /// <summary>
            /// Event listener for client disconnection
            /// </summary>
            protected event Action ClientDisconnected;
            /// <summary>
            /// Event listener for client connection
            /// </summary>
            protected event Action ClientConnected;
            /// <summary>
            /// List of installed augmentations
            /// </summary>
            protected List<Augmentation> augmentations = new List<Augmentation>();

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
                RecvSize = 1024; // Init the read buffer size
                ReadNewLine = "\n"; // Init the read new line terminator
                WriteNewLine = "\n"; // Init the write new line terminator
                SendEncoder = Encoding.ASCII; // Init the send string encoding
                RecvEncoder = Encoding.ASCII; // Init the read string encoding
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
                    augmentations.ForEach((aug) => client.InstallAugmentation(aug));
                    // Set client data
                    client.RecvEncoder = RecvEncoder;
                    client.ReadNewLine = ReadNewLine;
                    client.RecvSize = RecvSize;
                    client.SendEncoder = SendEncoder;
                    client.WriteNewLine = WriteNewLine;
                    client.AddEventClientStopped(() => { if (restartReading) Start(); });
                    client.AddEventClientStopped(() => ClientDisconnected?.Invoke());
                    ClientConnected?.Invoke();
                });
                t.Start(); // Start listening
                serverOffline = false; // Server's running
                augmentations.ForEach((aug) => aug.OnStart()); // Signal the start event to installed augmentations
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
                return client.DirectRead(maxSize); // Use the wrapped client to read from the stream and return the bytes read
            }

            /// <summary>
            /// Read a line from the stram
            /// </summary>
            /// <returns>The line read from the stream</returns>
            public string ReadLine()
            {
                WaitForClient(); // Wait for the clinet to connect
                return client.ReadLine(); // Use the wrapped client to read new lines and return them
            }

            /// <summary>
            /// Read bytes from the stream async
            /// </summary>
            /// <param name="maxSize">The number of maximum bytes to read from the stream</param>
            /// <returns>The bytes read from the stream</returns>
            public async Task<byte[]> DirectReadAsync(int maxSize)
            {
                return await client.DirectReadAsync(maxSize); // Wait for the function to complete and return the bytes
            }

            /// <summary>
            /// Read a line from the stream async
            /// </summary>
            /// <returns>The line read from the stream</returns>
            public async Task<string> ReadLineAsync()
            {
                return await client.ReadLineAsync(); // Wait for the function to complete and return the new line
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
                augmentations.ForEach((aug) => aug.OnStop()); // Signal the stop event to the augmentations
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
                augmentations.ForEach((aug) => aug.OnStop()); // Signal the stop event to the augmentations
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
                byte[] buffer = SendEncoder.GetBytes(data + WriteNewLine); // Convert the line and the terminator to a byte array
                DirectWrite(buffer); // Write the bytes to the stream
            }

            /// <summary>
            /// Add an event listener for when the client disconnects
            /// </summary>
            /// <param name="callback">The function to call when the client disconnects</param>
            public void AddEventClientDisconnected(Action callback)
            {
                ClientDisconnected += callback; // Add the callback to the event
            }

            /// <summary>
            /// Add an event listener for when the client connects
            /// </summary>
            /// <param name="callback">The function to call when the new client connects</param>
            public void AddEventClientConnected(Action callback)
            {
                ClientConnected += callback; // Add the callback to the event
            }

            /// <summary>
            /// Install an augmentation to the socket
            /// </summary>
            /// <param name="augmentation">The augmentation to install</param>
            public void InstallAugmentation(Augmentation augmentation)
            {
                augmentations.Add(augmentation); // Add the augmentation to the list
                augmentation.OnInstalled(this); // Notify the augmentation of the installation
                if (client != null) client.InstallAugmentation(augmentation); // Proxy to the client
            }

            /// <summary>
            /// Unintall an installed augmentation from the socket
            /// </summary>
            /// <param name="augmentation">The augmentation to uninstall</param>
            public void UninstallAugmentation(Augmentation augmentation)
            {
                augmentations.Remove(augmentation); // Remote the augmentation from the list
                augmentation.OnUninstalled(); // Notify the augmentation of the installation
                if (client != null) client.UninstallAugmentation(augmentation); // Proxy to the client
            }
        }

        /// <summary>
        /// Single Client SSL Server
        /// </summary>
        public class SingleSSLServer : INetworkReader, INetworkWriter, INetworkSocket, INetworkServer, IAugmentable
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
            public int RecvSize { get; set; }
            /// <summary>
            /// The encoding to use when reading new lines from the stream
            /// </summary>
            public Encoding RecvEncoder { get; set; }
            /// <summary>
            /// The encoding to use when sending new lines to the stream
            /// </summary>
            public Encoding SendEncoder { get; set; }
            /// <summary>
            /// The new line terminator used when reading new lines from the stream
            /// </summary>
            public string ReadNewLine { get; set; }
            /// <summary>
            /// The new line terminator used when writing new lines to the stream
            /// </summary>
            public string WriteNewLine { get; set; }
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
            /// Event listener when the client disconnects
            /// </summary>
            protected event Action ClientDisconnected;
            /// <summary>
            /// A list to keep installed augmentations in
            /// </summary>
            protected List<Augmentation> augmentations = new List<Augmentation>();

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
            /// <param name="sslParameters">SSL Socket Server Parameters</param>
            private void ServerInit(bool restartRead, Utils.SSL.ServerSSLData sslParameters)
            {
                client = null; // Reset the client
                serverEndPoint = null; // Reset the endpoint
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a new server socket
                RecvSize = 1024; // Set the default maximum number of bytes to read
                ReadNewLine = "\n"; // Init the read new line terminator
                WriteNewLine = "\n"; // Init the write new line terminator
                SendEncoder = Encoding.ASCII; // Init the send string encoding
                RecvEncoder = Encoding.ASCII; // Init the read string encoding
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
                        augmentations.ForEach((aug) => client.InstallAugmentation(aug)); // Install existing augmentations to the new client
                        // Setup the client
                        client.RecvEncoder = RecvEncoder;
                        client.ReadNewLine = ReadNewLine;
                        client.RecvSize = RecvSize;
                        client.SendEncoder = SendEncoder;
                        client.WriteNewLine = WriteNewLine;
                        client.AddEventClientStopped(() => { if (restartReading) Start(); });
                        client.AddEventClientStopped(() => ClientDisconnected?.Invoke());
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
                augmentations.ForEach((aug) => aug.OnStart()); // Signal the start event to the augmentations
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
                augmentations.ForEach((aug) => aug.OnStop()); // Signal the stop event to the augmentations
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
                augmentations.ForEach((aug) => aug.OnStop()); // Signal the stop event to the augmentations
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
                byte[] buffer = SendEncoder.GetBytes(data + WriteNewLine); // Convert the line and the line terminator to a byte array
                DirectWrite(buffer); // Send bytes to the stream
            }

            /// <summary>
            /// Add an event listener for when a client disconnects
            /// </summary>
            /// <param name="callback">The function to call when the client disconnects</param>
            public void AddEventClientDisconnected(Action callback)
            {
                ClientDisconnected += callback; // Add callback to function
            }

            /// <summary>
            /// Install an augmentation to the socket
            /// </summary>
            /// <param name="augmentation">The augmentation to install</param>
            public void InstallAugmentation(Augmentation augmentation)
            {
                augmentations.Add(augmentation); // Add the augmentation to the list
                augmentation.OnInstalled(this); // Notify the augmentation of the installation
                if (client != null) client.InstallAugmentation(augmentation); // Proxy to the client
            }

            /// <summary>
            /// Unintall an installed augmentation from the socket
            /// </summary>
            /// <param name="augmentation">The augmentation to uninstall</param>
            public void UninstallAugmentation(Augmentation augmentation)
            {
                augmentations.Remove(augmentation); // Remote the augmentation from the list
                augmentation.OnUninstalled(); // Notify the augmentation of the installation
                if (client != null) client.UninstallAugmentation(augmentation); // Proxy to the client
            }
        }

        /// <summary>
        /// Multi socket Tcp Server
        /// </summary>
        public class MultiTcpServer : INetworkMultiReader, INetworkMultiWriter, INetworkSocket, INetworkMultiServer, IAugmentable
        {
            /// <summary>
            /// The socket of the server
            /// </summary>
            protected Socket serverSocket;
            /// <summary>
            /// The connected client's socket
            /// </summary>
            protected List<Tuple<string, Clients.TcpClient>> clients = new List<Tuple<string, Clients.TcpClient>>();
            /// <summary>
            /// The endpoint to run the server on
            /// </summary>
            protected IPEndPoint serverEndPoint;
            /// <summary>
            /// The size of the buffer to receive from the client
            /// </summary>
            public int RecvSize { get; set; }
            /// <summary>
            /// The encoding used when receiving new lines
            /// </summary>
            public Encoding RecvEncoder { get; set; }
            /// <summary>
            /// The encoding use when sending new lines
            /// </summary>
            public Encoding SendEncoder { get; set; }
            /// <summary>
            /// The new line terminator to read until
            /// </summary>
            public string ReadNewLine { get; set; }
            /// <summary>
            /// The new line terminator to send
            /// </summary>
            public string WriteNewLine { get; set; }
            /// <summary>
            /// Indicates if the server's running
            /// </summary>
            protected bool serverOffline = true;
            /// <summary>
            /// Event listener for when a new client is connected
            /// </summary>
            protected event Action<string> ClientConnected;
            /// <summary>
            /// Event listener for when a client is disconnected
            /// </summary>
            protected event Action<string> ClientDisconnected;
            /// <summary>
            /// A list of installed augmentations
            /// </summary>
            protected List<Augmentation> augmentations = new List<Augmentation>();

            /// <summary>
            /// Init the server
            /// </summary>
            /// <param name="ep">The endpoint to run the server on</param>
            public MultiTcpServer(IPEndPoint ep)
            {
                ServerInit(); // Init the server
                serverEndPoint = ep; // Set the endpoint to run on
            }

            /// <summary>
            /// Init the server
            /// </summary>
            private void ServerInit()
            {
                clients.Clear(); // Clear the current client list
                serverEndPoint = null; // Reset the current endpoint
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a new server socket
                RecvSize = 2048; // Init the read buffer size
                ReadNewLine = "\n"; // Init the read new line terminator
                WriteNewLine = "\n"; // Init the write new line terminator
                SendEncoder = Encoding.ASCII; // Init the send string encoding
                RecvEncoder = Encoding.ASCII; // Init the read string encoding
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
                    while (!serverOffline)
                    {
                        Socket clientSocket = null;
                        try
                        {
                            clientSocket = serverSocket.Accept(); // Accept the pending client
                            if (serverOffline) break;
                        }
                        catch (Exception ex)
                        {
                            if (serverSocket == null)
                            {
                                Console.WriteLine("Error, when shutting down server, ignorable most likely");
                                return;
                            }

#if Validation_soft // Important check
                            throw new Exception("Failed to accept socket connection!", ex);
#endif
                        }
                        Clients.TcpClient client = new Clients.TcpClient(clientSocket); // Wrap the clinet in the NetLib tcp client class
                        augmentations.ForEach((aug) => client.InstallAugmentation(aug)); // Install the current augmentation to the new client
                        string clientID = Utils.NetworkIO.GenerateID(); // Generate the runtime unique ID of the client
                        // Set client data
                        client.RecvEncoder = RecvEncoder;
                        client.ReadNewLine = ReadNewLine;
                        client.RecvSize = RecvSize;
                        client.SendEncoder = SendEncoder;
                        client.WriteNewLine = WriteNewLine;
                        // Construct the clientData
                        Tuple<string, Clients.TcpClient> clientData = new Tuple<string, Clients.TcpClient>(clientID, client);
                        // Add the client to the list
                        clients.Add(clientData);
                        client.AddEventClientStopped(() => ClientDisconnected?.Invoke(clientID));
                        ClientConnected?.Invoke(clientID); // Fire the connection event 
                    }
                });
                serverOffline = false; // Server's running
                t.Start(); // Start listening
                augmentations.ForEach((aug) => aug.OnStart()); // Signal the start event to the augmentations
            }

            /// <summary>
            /// Add event listener for when a new client is connected
            /// </summary>
            /// <param name="callback">The function to call when a new client is connected</param>
            public void AddEventClientConnected(Action<string> callback)
            {
                ClientConnected += callback; // Add the function to the event
            }

            /// <summary>
            /// List the IDs of connected clients
            /// </summary>
            /// <returns>A string array filled with the ID of connected clients</returns>
            public string[] ListClients()
            {
                List<string> localClients = new List<string>(); // Define a new list for the return value

                foreach (Tuple<string, Clients.TcpClient> client in clients) // Go through the clients
                {
                    localClients.Add(client.Item1); // Add the ID of the clients
                }

                return localClients.ToArray(); // Return the client ID array
            }

            /// <summary>
            /// Get the NetLib TcpClient instance from a client ID
            /// </summary>
            /// <param name="clientID">The ID of the client</param>
            /// <returns>The NetLib TcpClient associated with the given ID</returns>
            private Clients.TcpClient GetClientByID(string clientID)
            {
                Tuple<string, Clients.TcpClient> tuple = clients.Find((data) => data.Item1 == clientID); // Find the matching client
                if (tuple == null) // If not found
                {
#if Validation_hard // Not so important check
                    throw new ArgumentException("The specified clientID doesn't exist");
#else
                    return null;
#endif
                }

                return tuple.Item2; // Return the client instance
            }

            /// <summary>
            /// Read bytes from the stream
            /// </summary>
            /// <param name="maxSize">The number of maximum bytes to read</param>
            /// <param name="clientID">The ID of the client to read from</param>
            /// <returns>The bytes read from the stream</returns>
            public byte[] DirectRead(int maxSize, string clientID)
            {
                Clients.TcpClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) return new byte[] { }; // Return if the client isn't found and error's suppressed
                return client.DirectRead(maxSize); // Use the wrapped client to read from the stream
            }

            /// <summary>
            /// Read a line from the stram
            /// </summary>
            /// <param name="clientID">The ID of the client to read from</param>
            /// <returns>The line read from the stream</returns>
            public string ReadLine(string clientID)
            {
                Clients.TcpClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) return ""; // Return if the client isn't found and error's suppressed
                return client.ReadLine(); // Use the wrapped client to read new lines
            }

            /// <summary>
            /// Read bytes from the stream async
            /// </summary>
            /// <param name="maxSize">The number of maximum bytes to read from the stream</param>
            /// <param name="clientID">The ID of the client to read from</param>
            /// <returns>The bytes read from the stream</returns>
            public async Task<byte[]> DirectReadAsync(int maxSize, string clientID)
            {
                Clients.TcpClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) return null; // Return if the client isn't found and error's suppressed
                return await client.DirectReadAsync(maxSize); // Wait for the function to complete and return
            }

            /// <summary>
            /// Read a line from the stream async
            /// </summary>
            /// <param name="clientID">The ID of the client to read from</param>
            /// <returns>The line read from the stream</returns>
            public async Task<string> ReadLineAsync(string clientID)
            {
                Clients.TcpClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) return null; // Return if the client isn't found and error's suppressed
                return await client.ReadLineAsync(); // Wait for the function to complete
            }

            /// <summary>
            /// Close the client gracefully
            /// <param name="client">The client to stop the connection with</param>
            /// </summary>
            private void GracefulCloseClient(Clients.TcpClient client)
            {
                if (client == null) return; // Check if the client is null
                client.GracefulStop(); // Stop the client
                client = null; // Reset the client
            }

            /// <summary>
            /// Close the client forcefully
            /// <param name="client">The tcp client to close the connection with</param>
            /// </summary>
            private void ForceCloseClient(Clients.TcpClient client)
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
                augmentations.ForEach((aug) => aug.OnStop()); // Signal the stop event to the augmentations
                if (serverOffline) return; // Check if the server's stopped
                foreach (Tuple<string, Clients.TcpClient> client in clients) // Go through the connected clients
                {
                    GracefulCloseClient(client.Item2); // Close the client
                }
                ServerClose(); // Close the server
                serverOffline = true; // The server's stopped
            }

            /// <summary>
            /// Forcefully stop the server
            /// </summary>
            public void ForceStop()
            {
                augmentations.ForEach((aug) => aug.OnStop()); // Signal the stop event to the augmentations
                if (serverOffline) return; // Check if the server's stopped
                foreach (Tuple<string, Clients.TcpClient> client in clients) // Go through the connected clients
                {
                    ForceCloseClient(client.Item2); // Force close the client
                }
                ServerClose(); // Close the server
                serverOffline = true; // The server's stopped
            }

            /// <summary>
            /// Add event listener for receiving bytes
            /// </summary>
            /// <param name="callback">The function to call when bytes are read from the stream</param>
            /// <param name="clientID">The ID of the client to read from</param>
            public void AddEventDataReceived(Action<byte[]> callback, string clientID)
            {
                Clients.TcpClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) throw new InvalidOperationException("Your can't start reading from a non-existent client"); // Return if the client isn't found and error's suppressed
                client.AddEventDataReceived(callback); // Use the wrapped client to listen for bytes
            }

            /// <summary>
            /// Add event listener for receiving new lines
            /// </summary>
            /// <param name="callback">The function to call when new lines are read from the stream</param>
            /// <param name="clientID">The ID of the client to read from</param>
            public void AddEventLineReceived(Action<string> callback, string clientID)
            {
                Clients.TcpClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) throw new InvalidOperationException("Your can't start reading from a non-existent client"); // Return if the client isn't found and error's suppressed
                client.AddEventLineReceived(callback); // Use the wrapped client to listen for new lines
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write to the stream</param>
            /// <param name="clientID">The ID of the client to read from</param>
            public void DirectWrite(byte[] buffer, string clientID)
            {
                DirectWrite(buffer, 0, buffer.Length, clientID); // Write the bytes to the stream
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write to the stream</param>
            /// <param name="offset">The offset to begin writing from</param>
            /// <param name="clientID">The ID of the client to read from</param>
            public void DirectWrite(byte[] buffer, int offset, string clientID)
            {
                DirectWrite(buffer, offset, buffer.Length - offset, clientID); // Write bytes to the stream
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write to the stream</param>
            /// <param name="offset">The offset to begin writing from</param>
            /// <param name="length">The number of bytes to write out</param>
            /// <param name="clientID">The ID of the client to read from</param>
            public void DirectWrite(byte[] buffer, int offset, int length, string clientID)
            {
                Clients.TcpClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) return; // Check if the client is null
                client.DirectWrite(buffer, offset, length); // Write the bytes to the stream
            }

            /// <summary>
            /// Write a line to the stream
            /// </summary>
            /// <param name="data">The line to write to the stream</param>
            /// <param name="clientID">The ID of the client to read from</param>
            public void WriteLine(string data, string clientID)
            {
                Clients.TcpClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) return; // Check if the client is null
                byte[] buffer = SendEncoder.GetBytes(data + WriteNewLine); // Convert the line and the terminator to a byte array
                DirectWrite(buffer, clientID); // Write the bytes to the stream
            }

            /// <summary>
            /// Add an event listener for when a client is disconnected
            /// </summary>
            /// <param name="callback">The function to call when the client disconnects</param>
            public void AddEventClientDisconnected(Action<string> callback)
            {
                ClientDisconnected += callback; // Add the callback to the event
            }

            /// <summary>
            /// Install an augmentation to the socket
            /// </summary>
            /// <param name="augmentation">The augmentation to install</param>
            public void InstallAugmentation(Augmentation augmentation)
            {
                augmentations.Add(augmentation); // Add the augmentation to the list
                augmentation.OnInstalled(this); // Notify the augmentation of the installation

                foreach (Tuple<string, Clients.TcpClient> clientData in clients) // Go through the connected clients
                {
                    clientData.Item2.InstallAugmentation(augmentation); // Install the augmentation to the current client
                }
            }

            /// <summary>
            /// Unintall an installed augmentation from the socket
            /// </summary>
            /// <param name="augmentation">The augmentation to uninstall</param>
            public void UninstallAugmentation(Augmentation augmentation)
            {
                augmentations.Remove(augmentation); // Remote the augmentation from the list
                augmentation.OnUninstalled(); // Notify the augmentation of the installation

                foreach (Tuple<string, Clients.TcpClient> clientData in clients) // Go through the connected clients
                {
                    clientData.Item2.UninstallAugmentation(augmentation); // Uninstall the augmentation from the current client
                }
            }
        }

        /// <summary>
        /// Multi socket Tcp Server
        /// </summary>
        public class MultiSSLServer : INetworkMultiReader, INetworkMultiWriter, INetworkSocket, INetworkMultiServer, IAugmentable
        {
            /// <summary>
            /// The socket of the server
            /// </summary>
            protected Socket serverSocket;
            /// <summary>
            /// The connected client's socket
            /// </summary>
            protected List<Tuple<string, Clients.SSLClient>> clients = new List<Tuple<string, Clients.SSLClient>>();
            /// <summary>
            /// The endpoint to run the server on
            /// </summary>
            protected IPEndPoint serverEndPoint;
            /// <summary>
            /// The size of the buffer to receive from the client
            /// </summary>
            public int RecvSize { get; set; }
            /// <summary>
            /// The encoding used when receiving new lines
            /// </summary>
            public Encoding RecvEncoder { get; set; }
            /// <summary>
            /// The encoding use when sending new lines
            /// </summary>
            public Encoding SendEncoder { get; set; }
            /// <summary>
            /// The new line terminator to read until
            /// </summary>
            public string ReadNewLine { get; set; }
            /// <summary>
            /// The new line terminator to send
            /// </summary>
            public string WriteNewLine { get; set; }
            /// <summary>
            /// Indicates if the server's running
            /// </summary>
            protected bool serverOffline = true;
            /// <summary>
            /// Event listener for when a new client is connected
            /// </summary>
            protected event Action<string> ClientConnected;
            /// <summary>
            /// Event listener for when a client is disconnected
            /// </summary>
            protected event Action<string> ClientDisconnected;
            /// <summary>
            /// A list of installed augmentations
            /// </summary>
            protected List<Augmentation> augmentations = new List<Augmentation>();
            /// <summary>
            /// The SSL Parameters of the server
            /// </summary>
            protected Utils.SSL.ServerSSLData sslParams;

            /// <summary>
            /// Init the server
            /// </summary>
            /// <param name="ep">The endpoint to run the server on</param>
            /// <param name="sslParameters">SSL Socket Server Parameters</param>
            public MultiSSLServer(IPEndPoint ep, Utils.SSL.ServerSSLData sslParameters)
            {
                ServerInit(sslParameters);
                serverEndPoint = ep;
            }
            /// <summary>
            /// Init the server
            /// </summary>
            /// <param name="ep">The endpoint to run the server on</param>
            /// <param name="certFilePath">The certification file to load into ssl sockets</param>
            /// <param name="serverAddress">The address of this server (domain name specified in the cert file)</param>
            /// <param name="protocols">The SSL Protocol to use during the handshake</param>
            public MultiSSLServer(IPEndPoint ep, string certFilePath, string serverAddress, System.Security.Authentication.SslProtocols protocols)
            {
                ServerInit(Utils.SSL.ParseServerData(certFilePath, serverAddress, protocols));
                serverEndPoint = ep;
            }

            /// <summary>
            /// Init the server
            /// </summary>
            /// <param name="ep">The endpoint to run the server on</param>
            /// <param name="certFilePath">The certification file to load into ssl sockets</param>
            /// <param name="serverAddress">The address of this server (domain name specified in the cert file)</param>
            public MultiSSLServer(IPEndPoint ep, string certFilePath, string serverAddress)
            {
                ServerInit(Utils.SSL.ParseServerData(certFilePath, serverAddress, Utils.SSL.defaultProtocols));
                serverEndPoint = ep;
            }

            /// <summary>
            /// Init the server
            /// </summary>
            private void ServerInit(Utils.SSL.ServerSSLData sslParameters)
            {
                clients.Clear(); // Clear the current client list
                serverEndPoint = null; // Reset the current endpoint
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a new server socket
                RecvSize = 2048; // Init the read buffer size
                sslParams = sslParameters; // Set the ssl protocol parameters
                ReadNewLine = "\n"; // Init the read new line terminator
                WriteNewLine = "\n"; // Init the write new line terminator
                SendEncoder = Encoding.ASCII; // Init the send string encoding
                RecvEncoder = Encoding.ASCII; // Init the read string encoding
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
                    while (!serverOffline)
                    {
                        Socket clientSocket = null;
                        try
                        {
                            clientSocket = serverSocket.Accept(); // Accept the pending client
                            if (serverOffline) break;
                        }
                        catch (Exception ex)
                        {
                            if (serverSocket == null)
                            {
                                Console.WriteLine("Error, when shutting down server, ignorable most likely");
                                return;
                            }

#if Validation_soft // Important check
                            throw new Exception("Failed to accept socket connection!", ex);
#endif
                        }
                        NetworkStream ns = new NetworkStream(clientSocket); // Get the network stream of the socket
                        System.Net.Security.SslStream sslStream = new System.Net.Security.SslStream(ns); // Wrap the stream in an ssl context
                        sslStream.AuthenticateAsServer(sslParams.certificate, false, sslParams.protocols, true); // Authenticate as the server
                        Clients.SSLClient client = new Clients.SSLClient(sslStream); // Wrap the clinet in the NetLib ssl client class
                        augmentations.ForEach((aug) => client.InstallAugmentation(aug)); // Install the current augmentation to the new client
                        string clientID = Utils.NetworkIO.GenerateID(); // Generate the runtime unique ID of the client
                        // Set client data
                        client.RecvEncoder = RecvEncoder;
                        client.ReadNewLine = ReadNewLine;
                        client.RecvSize = RecvSize;
                        client.SendEncoder = SendEncoder;
                        client.WriteNewLine = WriteNewLine;
                        // Construct the clientData
                        Tuple<string, Clients.SSLClient> clientData = new Tuple<string, Clients.SSLClient>(clientID, client);
                        // Add the client to the list
                        clients.Add(clientData);
                        client.AddEventClientStopped(() => ClientDisconnected?.Invoke(clientID));
                        ClientConnected?.Invoke(clientID); // Fire the connection event 
                    }
                });
                serverOffline = false; // Server's running
                t.Start(); // Start listening
                augmentations.ForEach((aug) => aug.OnStart()); // Signal the start event to the augmentations
            }

            /// <summary>
            /// Add event listener for when a new client is connected
            /// </summary>
            /// <param name="callback">The function to call when a new client is connected</param>
            public void AddEventClientConnected(Action<string> callback)
            {
                ClientConnected += callback; // Add the function to the event
            }

            /// <summary>
            /// List the IDs of connected clients
            /// </summary>
            /// <returns>A string array filled with the ID of connected clients</returns>
            public string[] ListClients()
            {
                List<string> localClients = new List<string>(); // Define a new list for the return value

                foreach (Tuple<string, Clients.SSLClient> client in clients) // Go through the clients
                {
                    localClients.Add(client.Item1); // Add the ID of the clients
                }

                return localClients.ToArray(); // Return the client ID array
            }

            /// <summary>
            /// Get the NetLib SSLClient instance from a client ID
            /// </summary>
            /// <param name="clientID">The ID of the client</param>
            /// <returns>The NetLib SSL Client associated with the given ID</returns>
            private Clients.SSLClient GetClientByID(string clientID)
            {
                Tuple<string, Clients.SSLClient> tuple = clients.Find((data) => data.Item1 == clientID); // Find the matching client
                if (tuple == null) // If not found
                {
#if Validation_hard // Not so important check
                    throw new ArgumentException("The specified clientID doesn't exist");
#else
                    return null;
#endif
                }

                return tuple.Item2; // Return the client instance
            }

            /// <summary>
            /// Read bytes from the stream
            /// </summary>
            /// <param name="maxSize">The number of maximum bytes to read</param>
            /// <param name="clientID">The ID of the client to read from</param>
            /// <returns>The bytes read from the stream</returns>
            public byte[] DirectRead(int maxSize, string clientID)
            {
                Clients.SSLClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) return new byte[] { }; // Return if the client isn't found and error's suppressed
                return client.DirectRead(maxSize); // Use the wrapped client to read from the stream
            }

            /// <summary>
            /// Read a line from the stram
            /// </summary>
            /// <param name="clientID">The ID of the client to read from</param>
            /// <returns>The line read from the stream</returns>
            public string ReadLine(string clientID)
            {
                Clients.SSLClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) return ""; // Return if the client isn't found and error's suppressed
                return client.ReadLine(); // Use the wrapped client to read new lines
            }

            /// <summary>
            /// Read bytes from the stream async
            /// </summary>
            /// <param name="maxSize">The number of maximum bytes to read from the stream</param>
            /// <param name="clientID">The ID of the client to read from</param>
            /// <returns>The bytes read from the stream</returns>
            public async Task<byte[]> DirectReadAsync(int maxSize, string clientID)
            {
                Clients.SSLClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) return null; // Return if the client isn't found and error's suppressed
                return await client.DirectReadAsync(maxSize); // Wait for the function to complete and return
            }

            /// <summary>
            /// Read a line from the stream async
            /// </summary>
            /// <param name="clientID">The ID of the client to read from</param>
            /// <returns>The line read from the stream</returns>
            public async Task<string> ReadLineAsync(string clientID)
            {
                Clients.SSLClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) return null; // Return if the client isn't found and error's suppressed
                return await client.ReadLineAsync(); // Wait for the function to complete
            }

            /// <summary>
            /// Close the client gracefully
            /// <param name="client">The client to stop the connection with</param>
            /// </summary>
            private void GracefulCloseClient(Clients.SSLClient client)
            {
                if (client == null) return; // Check if the client is null
                client.GracefulStop(); // Stop the client
                client = null; // Reset the client
            }

            /// <summary>
            /// Close the client forcefully
            /// <param name="client">The ssl client to close the connection with</param>
            /// </summary>
            private void ForceCloseClient(Clients.SSLClient client)
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
                augmentations.ForEach((aug) => aug.OnStop()); // Signal the stop event to the augmentations
                if (serverOffline) return; // Check if the server's stopped
                foreach (Tuple<string, Clients.SSLClient> client in clients) // Go through the connected clients
                {
                    GracefulCloseClient(client.Item2); // Close the client
                }
                ServerClose(); // Close the server
                serverOffline = true; // The server's stopped
            }

            /// <summary>
            /// Forcefully stop the server
            /// </summary>
            public void ForceStop()
            {
                augmentations.ForEach((aug) => aug.OnStop()); // Signal the stop event to the augmentations
                if (serverOffline) return; // Check if the server's stopped
                foreach (Tuple<string, Clients.SSLClient> client in clients) // Go through the connected clients
                {
                    ForceCloseClient(client.Item2); // Force close the client
                }
                ServerClose(); // Close the server
                serverOffline = true; // The server's stopped
            }

            /// <summary>
            /// Add event listener for receiving bytes
            /// </summary>
            /// <param name="callback">The function to call when bytes are read from the stream</param>
            /// <param name="clientID">The ID of the client to read from</param>
            public void AddEventDataReceived(Action<byte[]> callback, string clientID)
            {
                Clients.SSLClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) throw new InvalidOperationException("Your can't start reading from a non-existent client"); // Return if the client isn't found and error's suppressed
                client.AddEventDataReceived(callback); // Use the wrapped client to listen for bytes
            }

            /// <summary>
            /// Add event listener for receiving new lines
            /// </summary>
            /// <param name="callback">The function to call when new lines are read from the stream</param>
            /// <param name="clientID">The ID of the client to read from</param>
            public void AddEventLineReceived(Action<string> callback, string clientID)
            {
                Clients.SSLClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) throw new InvalidOperationException("Your can't start reading from a non-existent client"); // Return if the client isn't found and error's suppressed
                client.AddEventLineReceived(callback); // Use the wrapped client to listen for new lines
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write to the stream</param>
            /// <param name="clientID">The ID of the client to read from</param>
            public void DirectWrite(byte[] buffer, string clientID)
            {
                DirectWrite(buffer, 0, buffer.Length, clientID); // Write the bytes to the stream
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write to the stream</param>
            /// <param name="offset">The offset to begin writing from</param>
            /// <param name="clientID">The ID of the client to read from</param>
            public void DirectWrite(byte[] buffer, int offset, string clientID)
            {
                DirectWrite(buffer, offset, buffer.Length - offset, clientID); // Write bytes to the stream
            }

            /// <summary>
            /// Directly write bytes to the stream
            /// </summary>
            /// <param name="buffer">The array of bytes to write to the stream</param>
            /// <param name="offset">The offset to begin writing from</param>
            /// <param name="length">The number of bytes to write out</param>
            /// <param name="clientID">The ID of the client to read from</param>
            public void DirectWrite(byte[] buffer, int offset, int length, string clientID)
            {
                Clients.SSLClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) return; // Check if the client is null
                client.DirectWrite(buffer, offset, length); // Write the bytes to the stream
            }

            /// <summary>
            /// Write a line to the stream
            /// </summary>
            /// <param name="data">The line to write to the stream</param>
            /// <param name="clientID">The ID of the client to read from</param>
            public void WriteLine(string data, string clientID)
            {
                Clients.SSLClient client = GetClientByID(clientID); // Get the client by the specified ID
                if (client == null) return; // Check if the client is null
                byte[] buffer = SendEncoder.GetBytes(data + WriteNewLine); // Convert the line and the terminator to a byte array
                DirectWrite(buffer, clientID); // Write the bytes to the stream
            }

            /// <summary>
            /// Add an event listener for when a client is disconnected
            /// </summary>
            /// <param name="callback">The function to call when the client disconnects</param>
            public void AddEventClientDisconnected(Action<string> callback)
            {
                ClientDisconnected += callback; // Add the callback to the event
            }

            /// <summary>
            /// Install an augmentation to the socket
            /// </summary>
            /// <param name="augmentation">The augmentation to install</param>
            public void InstallAugmentation(Augmentation augmentation)
            {
                augmentations.Add(augmentation); // Add the augmentation to the list
                augmentation.OnInstalled(this); // Notify the augmentation of the installation

                foreach (Tuple<string, Clients.SSLClient> clientData in clients) // Go through the connected clients
                {
                    clientData.Item2.InstallAugmentation(augmentation); // Install the augmentation to the current client
                }
            }

            /// <summary>
            /// Unintall an installed augmentation from the socket
            /// </summary>
            /// <param name="augmentation">The augmentation to uninstall</param>
            public void UninstallAugmentation(Augmentation augmentation)
            {
                augmentations.Remove(augmentation); // Remote the augmentation from the list
                augmentation.OnUninstalled(); // Notify the augmentation of the installation

                foreach (Tuple<string, Clients.SSLClient> clientData in clients) // Go through the connected clients
                {
                    clientData.Item2.UninstallAugmentation(augmentation); // Uninstall the augmentation from the current client
                }
            }
        }
    }

    namespace Clients
    {
        /// <summary>
        /// Basic TCP Client
        /// </summary>
        public class TcpClient : INetworkReader, INetworkWriter, INetworkSocket, IAugmentable
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
            public Encoding RecvEncoder { get; set; }
            /// <summary>
            /// Encoding for sending new lines
            /// </summary>
            public Encoding SendEncoder { get; set; }
            /// <summary>
            /// Maximum receive buffer size
            /// </summary>
            public int RecvSize { get; set; }
            /// <summary>
            /// New line terminator for reading new lines
            /// </summary>
            public string ReadNewLine { get; set; }
            /// <summary>
            /// New line terminator for sending new lines
            /// </summary>
            public string WriteNewLine { get; set; }
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
            /// A list of installed augmentations
            /// </summary>
            protected List<Augmentation> augmentations = new List<Augmentation>();
            /// <summary>
            /// Indicates if a server socket created this client
            /// </summary>
            protected bool fromServer = false;

            /// <summary>
            /// Empty constructor for SSL clients (don't use this constructor)
            /// </summary>
            public TcpClient()
            {
                ClientInit(); // Init the tcp client
            }

            /// <summary>
            /// Init the client
            /// </summary>
            private void ClientInit()
            {
                clientEndPoint = null; // Reset the endpoint
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a new client socket
                RecvSize = 1024; // Set the receive buffer size
                ReadNewLine = "\n"; // Init the read new line terminator
                WriteNewLine = "\n"; // Init the write new line terminator
                SendEncoder = Encoding.ASCII; // Init the send string encoding
                RecvEncoder = Encoding.ASCII; // Init the read string encoding
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
                fromServer = true;
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
                if (!fromServer) augmentations.ForEach((aug) => aug.OnStart()); // Signal the start event to augmentations if we're not created from a server class
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

                augmentations.ForEach((aug) => dataBuffer = aug.OnBeforeReceiveBytes(dataBuffer)); // Let the augmentations modify the data
                augmentations.ForEach((aug) => aug.OnAfterReceiveBytes(dataBuffer)); // Let the augmentation inspect the data

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
                    byte[] buffer = new byte[RecvSize]; // Define receive buffer
                    int bytesRead = client.Receive(buffer, 0, RecvSize, SocketFlags.None); // Read bytes from the stream
                    string subResult = RecvEncoder.GetString(buffer, 0, bytesRead); // Decode bytes read into strings
                    result.Append(subResult); // Append to the total result
                    if (subResult.EndsWith(ReadNewLine)) break; // If the data ends with a new line, return it
                }
                string res = result.ToString();

                augmentations.ForEach((aug) => res = aug.OnBeforeReceiveString(res, RecvEncoder)); // Let the augmentations modify the data
                augmentations.ForEach((aug) => aug.OnAfterReceiveString(res, RecvEncoder)); // Let the augmentations inspect the data

                return res; // Return the constructed line from the connection
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
                        buffer = new byte[RecvSize], // Byte buffer
                        lineRecvResult = new StringBuilder() // String buffer
                    };

                    client.BeginReceive(readObject.buffer, 0, RecvSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), readObject); // Begin reading the stream
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
                return await Task<byte[]>.Factory.StartNew(() => DirectRead(maxSize)).ConfigureAwait(false); // Wait for the task to finish and return it
            }

            /// <summary>
            /// Read new lines from the stream async
            /// </summary>
            /// <returns>The task with the resulting new line</returns>
            public async Task<string> ReadLineAsync()
            {
                return await Task<string>.Factory.StartNew(ReadLine).ConfigureAwait(false); // Wait for the task to finish and return it
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
#if Logging_verbose
                    Console.WriteLine("Stopping client due to a read error from the socket");
                    Console.WriteLine(ex);
#endif
                    ForceStop(); // Stop the client
                    return; // Return
                }

                if (bytesRead > 0) // If we read bytes from the stream
                {
                    byte[] dataBuffer = new byte[bytesRead]; // Define data buffer
                    Array.Copy(readObject.buffer, dataBuffer, bytesRead); // Copy the bytes read to the data buffer

                    if (receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.Both || receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.DataRecv)
                    {
                        augmentations.ForEach((aug) => dataBuffer = aug.OnBeforeReceiveBytes(dataBuffer)); // Let the augmentations modify the data
                        augmentations.ForEach((aug) => aug.OnAfterReceiveBytes(dataBuffer)); // Let the augmentation inspect the data
                        DataReceived?.Invoke(dataBuffer); // Invoke the data received event
                    }

                    if (receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.Both || receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.LineRecv) // Invoke the line received event
                    {
                        string subResult = RecvEncoder.GetString(dataBuffer, 0, bytesRead); // Decode the data
                        readObject.lineRecvResult.Append(subResult); // Append the result to the string buffer
                        if (subResult.EndsWith(ReadNewLine)) // Check for line ending
                        {
                            string res = readObject.lineRecvResult.ToString();
                            augmentations.ForEach((aug) => res = aug.OnBeforeReceiveString(res, RecvEncoder)); // Let the augmentations modify the data
                            augmentations.ForEach((aug) => aug.OnAfterReceiveString(res, RecvEncoder)); // Let the augmentations inspect the data
                            LineReceived?.Invoke(res); // Invoke the callback
                            readObject.lineRecvResult.Clear(); // Clear out the string buffer
                        }
                    }
                }

                Array.Clear(readObject.buffer, 0, RecvSize); // Clear the receive buffer

                try
                {
                    client.BeginReceive(readObject.buffer, 0, RecvSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), readObject); // Try reading the stream again
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
                if (!fromServer) augmentations.ForEach((aug) => aug.OnStop()); // Signal the stop event to the augmentations if we're not created from server
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
                if (!fromServer) augmentations.ForEach((aug) => aug.OnStop()); // Signal the stop event to the augmentations if we're not created from server
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
                augmentations.ForEach((aug) =>
                {
                    Tuple<byte[], int, int> result = aug.OnBeforeSendBytes(buffer, offset, length); // Let the augmentations modify the data
                    // Unpack the tuple
                    buffer = result.Item1;
                    offset = result.Item2;
                    length = result.Item3;
                });

                augmentations.ForEach((aug) => aug.OnAfterSendBytes(buffer, offset, length)); // Let the augmentations inspect the data
                client.Send(buffer, offset, length, SocketFlags.None); // Send the bytes to the stream
            }

            /// <summary>
            /// Write a new line to the stream
            /// </summary>
            /// <param name="data">The line to write to the stream</param>
            public void WriteLine(string data)
            {
                augmentations.ForEach((aug) => data = aug.OnBeforeSendString(data, SendEncoder)); // Let the augmentations modify the data
                augmentations.ForEach((aug) => aug.OnAfterSendString(data, SendEncoder)); // Let the augmentations inspect the data
                byte[] sendBuffer = SendEncoder.GetBytes(data + WriteNewLine); // Convert the line and the terminator to bytes
                DirectWrite(sendBuffer, 0, sendBuffer.Length); // Write the bytes to the stream
            }

            /// <summary>
            /// Install an augmentation to the socket
            /// </summary>
            /// <param name="augmentation">The augmentation to install</param>
            public void InstallAugmentation(Augmentation augmentation)
            {
                augmentations.Add(augmentation); // Add the augmentation to the list
                if (!fromServer) augmentation.OnInstalled(this); // Notify the augmentation of the installation
            }

            /// <summary>
            /// Unintall an installed augmentation from the socket
            /// </summary>
            /// <param name="augmentation">The augmentation to uninstall</param>
            public void UninstallAugmentation(Augmentation augmentation)
            {
                augmentations.Remove(augmentation); // Remote the augmentation from the list
                if (!fromServer) augmentation.OnUninstalled(); // Notify the augmentation of the installation
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
                fromServer = true;
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
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback((sender, cert, chain, errors) =>
                    {
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
                        buffer = new byte[RecvSize], // Define the bytes buffer
                        lineRecvResult = new StringBuilder() // Define the string buffer
                    };

                    sslStream.BeginRead(readObject.buffer, 0, RecvSize, new AsyncCallback(ReceiveCallbackSSL), readObject); // Begin reading from the stream
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
#if Logging_verbose
                    Console.WriteLine("Stopping client due to a read error from the socket");
                    Console.WriteLine(ex);
#endif
                    ForceStop(); // Stop the client
                    return; // Return
                }

                if (bytesRead > 0) // Bytes are available
                {
                    byte[] dataBuffer = new byte[bytesRead]; // Define the data buffer
                    Array.Copy(readObject.buffer, dataBuffer, bytesRead); // Copy the bytes read to the data buffer

                    if (receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.Both || receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.DataRecv)
                    {
                        augmentations.ForEach((aug) => dataBuffer = aug.OnBeforeReceiveBytes(dataBuffer)); // Let the augmentations modify the data
                        augmentations.ForEach((aug) => aug.OnAfterReceiveBytes(dataBuffer)); // Let the augmentations inspect the data
                        DataReceivedMethod(dataBuffer); // Invoke the data received event
                    }

                    if (receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.Both || receiveCallbackMode == Utils.NetworkIO.ReadEventCodes.LineRecv) // Invoke the line received event
                    {
                        string subResult = RecvEncoder.GetString(dataBuffer, 0, bytesRead); // Encode the bytes to string
                        readObject.lineRecvResult.Append(subResult); // Appnend the data to the string buffer
                        if (subResult.EndsWith(ReadNewLine)) // Check if the buffer ends with the new line
                        {
                            string res = readObject.lineRecvResult.ToString(); // Get the resulting string
                            augmentations.ForEach((aug) => res = aug.OnBeforeReceiveString(res, RecvEncoder)); // Let the augmentations modify the data
                            augmentations.ForEach((aug) => aug.OnAfterReceiveString(res, RecvEncoder)); // Let the augmentations inspect the data
                            LineReceivedMethod(res); // Invoke the new line event
                            readObject.lineRecvResult.Clear(); // Clear the string buffer
                        }
                    }
                }

                Array.Clear(readObject.buffer, 0, RecvSize); // Clear the receive buffer

                try
                {
                    sslStream.BeginRead(readObject.buffer, 0, RecvSize, new AsyncCallback(ReceiveCallbackSSL), readObject); // Re-Read from the stream
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
                augmentations.ForEach((aug) => dataBuffer = aug.OnBeforeReceiveBytes(dataBuffer)); // Let the augmentations modify the data
                augmentations.ForEach((aug) => aug.OnAfterReceiveBytes(dataBuffer)); // Let the augmentations inspect the data
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
                    byte[] buffer = new byte[RecvSize]; // Define the receive buffer
                    int bytesRead = sslStream.Read(buffer, 0, RecvSize); // Read bytes from the stream
                    string subResult = RecvEncoder.GetString(buffer, 0, bytesRead); // Convert the bytes read to string
                    result.Append(subResult); // Append the string to the string buffer
                    if (subResult.EndsWith(ReadNewLine)) break; // Stop if a line has ended
                }

                string res = result.ToString(); // Get the resulting string

                augmentations.ForEach((aug) => res = aug.OnBeforeReceiveString(res, RecvEncoder)); // Let the augmentations modify the data
                augmentations.ForEach((aug) => aug.OnAfterReceiveString(res, RecvEncoder)); // Let the augmentations inspect the data

                return res; // Return the current new line
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
                augmentations.ForEach((aug) =>
                {
                    Tuple<byte[], int, int> result = aug.OnBeforeSendBytes(buffer, offset, length); // Get the result
                    // Unpack the tuple
                    buffer = result.Item1;
                    offset = result.Item2;
                    length = result.Item3;
                }); // Let the augmentations modify the data
                augmentations.ForEach((aug) => aug.OnAfterSendBytes(buffer, offset, length)); // Let the augmentations inspect the data
                sslStream.Write(buffer, offset, length); // Write the bytes to the stream
            }

            /// <summary>
            /// Write a new line to the stream
            /// </summary>
            /// <param name="data">The line to write out</param>
            public new void WriteLine(string data)
            {
                augmentations.ForEach((aug) => data = aug.OnBeforeReceiveString(data, SendEncoder)); // Let the augmentations modify the data
                augmentations.ForEach((aug) => aug.OnAfterReceiveString(data, SendEncoder)); // Let the augmentations inspect the data
                byte[] buffer = SendEncoder.GetBytes(data + WriteNewLine); // Convert the line and the terminator to a byte array
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
                /// <summary>
                /// Receive only byte type data
                /// </summary>
                DataRecv,
                /// <summary>
                /// Receive only string new lines
                /// </summary>
                LineRecv,
                /// <summary>
                /// Receive byte type and string new line data
                /// </summary>
                Both,
                /// <summary>
                /// Don't receive data actively
                /// </summary>
                Stopped
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

            /// <summary>
            /// A random value generated first when the generator is called, and incremented later
            /// </summary>
            public static int randomIncrementor = 0;
            /// <summary>
            /// The process id of the executing assembly loaded when the generator is called
            /// </summary>
            public static int currentPID = 0;

            /// <summary>
            /// Generate a runtime unique ID
            /// </summary>
            /// <returns>A unique ID to the current runtime</returns>
            public static string GenerateID()
            {
                TimeSpan epochTime = DateTime.UtcNow - new DateTime(1970, 1, 1); // Calculate the epoch time
                int milliSeconds = (int)epochTime.TotalMilliseconds; // Get the ms of the epoch time
                if (randomIncrementor == 0) // If the random incrementor isn't initialized
                {
                    System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider(); // Get the rng service
                    byte[] data = new byte[32]; // Create buffer for the result
                    rng.GetBytes(data); // Get the rng values
                    foreach (byte random in data) // Loop through the bytes
                    {
                        randomIncrementor += random; // Append the bytes to the incrementor
                    }
                }

                if (currentPID == 0) // If the PPID isn't initialized
                {
                    currentPID = System.Diagnostics.Process.GetCurrentProcess().Id; // Get the current PID
                }

                int randomInteger = randomIncrementor + milliSeconds + currentPID; // Add the random values together
                string randomValue = randomInteger.ToString("X"); // Convert them to a hex string
                randomIncrementor++; // Increment the incrementor
                return randomValue; // Return the random value
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

            /// <summary>
            /// Create an endpoint to connect/bind to
            /// </summary>
            /// <param name="destination">The hostname or address of the endpoint</param>
            /// <param name="portNumber">The port number of the endpoint</param>
            /// <returns>A valid endpoint generated from the specified data</returns>
            public static IPEndPoint CreateEndPoint(string destination, int portNumber)
            {
                if (!IsPortValid(portNumber)) throw new ArgumentException("Port number outside of the valid port number range");
                Tuple<bool, IPAddress> ip = IsValidDestination(destination);
                if (!ip.Item1) throw new ArgumentException("The specified destination is invalid!");
                return new IPEndPoint(ip.Item2, portNumber);
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
#if Logging_verbose
                    Console.WriteLine("Hostname resolvation failed, due to the following exception:");
                    Console.WriteLine(ex);
#endif
                    return new Tuple<bool, IPAddress>(false, null); // Return false and null for the IP Address
                }
            }
        }
    }
}
