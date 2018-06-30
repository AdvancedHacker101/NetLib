#define Validation_hard // Enable Hard, strict error cheking, may throw error even when the program will probably work fine
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
using System.Security.Cryptography;
using System.IO;

namespace NetLib
{
    /// <summary>
    /// Class for sotring custom event handlers
    /// </summary>
    public class EventHandlers
    {
        /// <summary>
        /// Bytes read from network event handler
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The data read from the network</param>
        public delegate void BytesReadEventHandler(object sender, EventArguments.BytesReadEventArgs e);
        /// <summary>
        /// String data read from the network event handler
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The message data</param>
        public delegate void StringReadEventHandler(object sender, EventArguments.StringReadEventArgs e);
        /// <summary>
        /// String read from multi socket server event handler
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The client and message data</param>
        public delegate void StringReadMultiEventHandler(object sender, EventArguments.StringReadMultiEventArgs e);
        /// <summary>
        /// Chat message send failure event handler
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The message the client failed to send</param>
        public delegate void ChatMessageFailEventHandler(object sender, EventArguments.ChatFailEventArgs e);
        /// <summary>
        /// Selected username taken event handler
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The username that's taken</param>
        public delegate void UsernameTakenEventHandler(object sender, EventArguments.UsernameTakenEventArgs e);
        /// <summary>
        /// Chat message received event handler
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The message and the name of it's sender</param>
        public delegate void ChatMessageReceivedEventHandler(object sender, EventArguments.ChatMessageReceivedEventArgs e);

        /// <summary>
        /// Client stopped event handler
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        public delegate void ClientStoppedEventHandler(object sender, EventArgs e);

        /// <summary>
        /// Bytes read from a multi socket server
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The client ID which sent the message and the message</param>
        public delegate void BytesReadMultiEventHandler(object sender, EventArguments.BytesReadMultiEventArgs e);
    }

    /// <summary>
    /// Class for storing custom event arguments
    /// </summary>
    public class EventArguments
    {
        /// <summary>
        /// Event arguments for bytes data received
        /// </summary>
        public class BytesReadEventArgs : EventArgs
        {
            /// <summary>
            /// Create a new bytes received event argument
            /// </summary>
            /// <param name="byteData">The data the socket received</param>
            public BytesReadEventArgs(byte[] byteData)
            {
                Bytes = byteData; // Set the data
            }

            /// <summary>
            /// The data the socket read from the stream
            /// </summary>
            public byte[] Bytes { get; private set; }
        }

        /// <summary>
        /// Event arguments for string data received
        /// </summary>
        public class StringReadEventArgs : EventArgs
        {
            /// <summary>
            /// Create a new line received event arguments
            /// </summary>
            /// <param name="stringData">The new line received from the server</param>
            public StringReadEventArgs(string stringData)
            {
                StringMessage = stringData; // Set the message
            }

            /// <summary>
            /// The new line the socket read from the stream
            /// </summary>
            public string StringMessage { get; private set; }
        }

        /// <summary>
        /// Event arguments for a multi socket server receiving string data
        /// </summary>
        public class BytesReadMultiEventArgs : EventArgs
        {
            /// <summary>
            /// Create a new multi string read event arguments
            /// </summary>
            /// <param name="clientID">The ID of the client which sent the message</param>
            /// <param name="byteData">The byte data the client sent</param>
            public BytesReadMultiEventArgs(string clientID, byte[] byteData)
            {
                ClientID = clientID;
                Bytes = byteData;
            }

            /// <summary>
            /// The message the client sent
            /// </summary>
            public byte[] Bytes { get; private set; }
            /// <summary>
            /// The ID of the client which sent the message
            /// </summary>
            public string ClientID { get; private set; }
        }

        /// <summary>
        /// Event arguments for the chat augmentation, when it fails at something
        /// </summary>
        public class ChatFailEventArgs : EventArgs
        {
            /// <summary>
            /// Create a new chat fail event arguments
            /// </summary>
            /// <param name="msg">The message the augmentation failed to send</param>
            public ChatFailEventArgs(string msg)
            {
                OriginalMessage = msg;
            }

            /// <summary>
            /// The message the client couldn't send
            /// </summary>
            public string OriginalMessage { get; private set; }
        }

        /// <summary>
        /// Event arguments for the chat augmentation, the username selected by the user if already taken
        /// </summary>
        public class UsernameTakenEventArgs : EventArgs
        {
            /// <summary>
            /// Create a new username taken event arguments
            /// </summary>
            /// <param name="user">The username that's already taken</param>
            public UsernameTakenEventArgs(string user)
            {
                Username = user;
            }

            /// <summary>
            /// The username the user wanted, but is already taken
            /// </summary>
            public string Username { get; private set; }
        }

        /// <summary>
        /// Message receive event arguments
        /// </summary>
        public class ChatMessageReceivedEventArgs : EventArgs
        {
            /// <summary>
            /// Create a new event argument object
            /// </summary>
            /// <param name="msg">The message the client sent</param>
            /// <param name="user">The name of the user who sent the message</param>
            public ChatMessageReceivedEventArgs(string msg, string user)
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
        /// Event arguments for a multi socket server receiving string data
        /// </summary>
        public class StringReadMultiEventArgs : EventArgs
        {
            /// <summary>
            /// Create a new multi string read event arguments
            /// </summary>
            /// <param name="clientID">The ID of the client which sent the message</param>
            /// <param name="stringMessage">The message the client sent</param>
            public StringReadMultiEventArgs(string clientID, string stringMessage)
            {
                ClientID = clientID;
                StringMessage = stringMessage;
            }

            /// <summary>
            /// The message the client sent
            /// </summary>
            public string StringMessage { get; private set; }
            /// <summary>
            /// The ID of the client which sent the message
            /// </summary>
            public string ClientID { get; private set; }
        }
    }

    /// <summary>
    /// A set of extension methods for simple tasks
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Remove the last characters of a string
        /// </summary>
        /// <param name="value">The string to work with</param>
        /// <param name="numberOfCharacters">The number of characters to remove from the end</param>
        /// <returns>The resulting string</returns>
        public static string RemoveLast(this string value, int numberOfCharacters)
        {
            return value.Substring(0, value.Length - numberOfCharacters); // Cut the end of the string
        }

        /// <summary>
        /// Combine two string only dictionaries
        /// </summary>
        /// <param name="target">The dictionary to copy the values to</param>
        /// <param name="source">The dictionary to copy the values from</param>
        public static void Merge(this Dictionary<string, string> target, Dictionary<string, string> source)
        {
            foreach (KeyValuePair<string, string> kvp in source) // Go through the key value pairs in the source dictionary
            {
                target.Add(kvp.Key, kvp.Value); // Add them to the target dictionary
            }
        }

        /// <summary>
        /// Convert a string response to a response object
        /// </summary>
        /// <param name="data">The string to convert</param>
        /// <returns>A response object</returns>
        public static Utils.FTP.Response ToResponse(this string data)
        {
            int sCode = data.Substring(0, 3).ToInt(); // Get the status code of the response
            string msg = data.Substring(4); // Get the message of the response
            msg = msg.RemoveLast(2); // Remove the new line from the message
            return new Utils.FTP.Response() // Construct the response
            {
                statusCode = (Utils.FTP.StatusCode)sCode, // Set the status code
                message = msg // Set the message
            };
        }

        /// <summary>
        /// Convert a string short month name to an integer month value
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <returns>The integer value of the month</returns>
        public static int ToMonth(this string value)
        {
            string v = value; // Get another refrence
            v = v.ToLower(); // Convert to lower
                             // Return based on the value
            if (v == "jan") return 1;
            else if (v == "feb") return 2;
            else if (v == "mar") return 3;
            else if (v == "apr") return 4;
            else if (v == "may") return 5;
            else if (v == "jun") return 6;
            else if (v == "jul") return 7;
            else if (v == "aug") return 8;
            else if (v == "sep") return 9;
            else if (v == "oct") return 10;
            else if (v == "nov") return 11;
            else return 12;
        }

        /// <summary>
        /// Convert a string value to an integer value
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <returns>The integer converted from the string</returns>
        public static int ToInt(this string value)
        {
            return int.Parse(value); // Return the integer value
        }

        /// <summary>
        /// Convert the string value to a long value
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <returns>The long converted from the string</returns>
        public static long ToLong(this string value)
        {
            return long.Parse(value); // Return the long value
        }
    }

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
            public virtual (byte[] data, int offset, int length) OnBeforeSendBytes(byte[] data, int offset, int length)
            {
                if (!disableConsole) Console.WriteLine("Before sending bytes");
                return (data, offset, length);
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
        /// Basic network client functionallity
        /// </summary>
        public interface INetworkClient
        {
            /// <summary>
            /// Get the underlying network stream of the client socket
            /// </summary>
            /// <returns></returns>
            NetworkStream GetRawStream();
            /// <summary>
            /// Close the network stream, but leave the client open
            /// </summary>
            void CloseStream();
            /// <summary>
            /// How many ms to wait before cancelling a read operation
            /// </summary>
            int ReadTimeout { get; set; }
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
            void AddEventDataReceived(EventHandlers.BytesReadEventHandler callback);
            /// <summary>
            /// Add an event listener for receiving lines
            /// </summary>
            /// <param name="callback">The function to call when lines are received</param>
            void AddEventLineReceived(EventHandlers.StringReadEventHandler callback);
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
            void AddEventDataReceived(EventHandlers.BytesReadEventHandler callback, string clientid);
            /// <summary>
            /// Add an event listener for receiving lines
            /// </summary>
            /// <param name="callback">The function to call when lines are received</param>
            /// <param name="clientid">The ID of the client to add the event to</param>
            void AddEventLineReceived(EventHandlers.StringReadEventHandler callback, string clientid);
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
            public event EventHandlers.StringReadEventHandler OnDataRead;
            /// <summary>
            /// Event listener for when a new message on a multi socket server is available
            /// </summary>
            public event EventHandlers.StringReadMultiEventHandler OnDataMultiRead;
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

                read.AddEventDataReceived((sender, ea) => // Check for new data
                {
                    byte[] data = ea.Bytes;
                    string msg = e.GetString(data); // Get the string message
                    fullString += msg; // Append it to the blob
                    for (int i = 0; i < 10; i++) // Spin 10 times, for larger network loads to clear
                    {
                        string result = ParseMessage(ref fullString, ref realLength); // Try to get the next message
                        if (result != null) OnDataRead?.Invoke(this, new EventArguments.StringReadEventArgs(result)); // Signal the message
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
                mRead.AddEventDataReceived((s, ea) => // Check for new data
                {
                    byte[] data = ea.Bytes;
                    string msg = e.GetString(data); // Get the string message
                    fullString += msg; // Append it to the blob
                    for (int i = 0; i < 10; i++) // Spin 10 times, for larger network loads to clear
                    {
                        string result = ParseMessage(ref fullString, ref realLength); // Try to get the next message
                        if (result != null) OnDataMultiRead?.Invoke(this, new EventArguments.StringReadMultiEventArgs(result, clientID)); // Signal the message
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
            private List<(string ID, string Name)> users = new List<(string, string)>();
            /// <summary>
            /// Event listener for when failed to send a direct message
            /// </summary>
            public event EventHandlers.ChatMessageFailEventHandler DirectMessageFailed;
            /// <summary>
            /// Event listener for when failed to broadcast a message
            /// </summary>
            public event EventHandlers.ChatMessageFailEventHandler BroadcastFailed;
            /// <summary>
            /// Event listener for when the selected username is already taken
            /// </summary>
            public event EventHandlers.UsernameTakenEventHandler UsernameSelectionFailed;
            /// <summary>
            /// Event listener for when a new message is received
            /// </summary>
            public event EventHandlers.ChatMessageReceivedEventHandler MessageReceived;
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
                var (ID, clientName) = users.Find((user) => clientID == user.ID); // Search for the client
                return ID != null && clientName != null; // Return the result
            }

            /// <summary>
            /// Get the username of a client
            /// </summary>
            /// <param name="cID">The ID of the client to get the username of</param>
            /// <returns>The username of the client</returns>
            private string GetUsername(string cID)
            {
                var (clientID, clientName) = users.Find((user) => cID == user.ID); // Search for the client
                if (clientID == null || clientName == null) return null; // Check if the resul's null
                return clientName; // Return the name of the client
            }

            /// <summary>
            /// Check if a user already exists with the specified username
            /// </summary>
            /// <param name="username">The username to check</param>
            /// <returns>True if the username isn't available, otherwise false</returns>
            private bool UsernameTaken(string username)
            {
                foreach (var (clientID, clientName) in users) // Loop through the registered users
                {
                    if (clientName == username) return true; // Return true if the username's found
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
                var (clientID, clientName) = users.Find((u) => user == u.Name); // Find the target user
                if (clientID == null || clientName == null) return false; // Return false if user not found
                server.SendData(clientID, $"{sender}{messageHeaderSeparator}{message}"); // Send the message to the user
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
                List<(string, string)> toRemove = new List<(string, string)>(); // Define a list of client to remove

                foreach (ValueTuple<string, string> clientData in users) // Loop through the users
                {
                    var (clientID, clientName) = clientData;
                    if (clientName == sender) continue; // Don't send the message if the user is the sender
                    try
                    {
                        server.SendData(clientID, $"{sender}{messageHeaderSeparator}{message}"); // Send the message to the current user
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
                    bi.OnDataMultiRead += (eventSender, ea) => // Event: client sent data
                    {
                        string clientID = ea.ClientID;
                        string message = ea.StringMessage;
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
                            users.Add((id, name)); // Add the user to the user list
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
                clientBI.OnDataRead += (sender, ea) => // Event: server sent data
                {
                    string message = ea.StringMessage;
                    const string dmFailed = "dmfailed" + messageHeaderSeparator; // Direct messgae failed header
                    const string broadcastFailed = "broadcastfailed" + messageHeaderSeparator; // Broadcast failed header
                    const string usernameFailed = "userfailed" + messageHeaderSeparator; // Username set failed header
                    if (message.StartsWith(dmFailed)) // Check if a direct message failed
                    {
                        DirectMessageFailed?.Invoke(this, new EventArguments.ChatFailEventArgs(message.Split(new string[] { messageHeaderSeparator }, StringSplitOptions.None)[1])); // Fire the event
                    }
                    else if (message.StartsWith(broadcastFailed)) // Check if a broadcast failed
                    {
                        BroadcastFailed?.Invoke(this, new EventArguments.ChatFailEventArgs(message.Split(new string[] { messageHeaderSeparator }, StringSplitOptions.None)[1])); // Fire the event
                    }
                    else if (message.StartsWith(usernameFailed)) // Check if the failed to set the username
                    {
                        UsernameSelectionFailed?.Invoke(this, new EventArguments.UsernameTakenEventArgs(message.Split(new string[] { messageHeaderSeparator }, StringSplitOptions.None)[1])); // Fire the event
                    }
                    else // Successful
                    {
                        string[] msgData = message.Split(new string[] { messageHeaderSeparator }, StringSplitOptions.None); // Get the contents of the message
                        string userName = msgData[0]; // Get the username
                        string msg = msgData[1]; // Get the message
                        EventArguments.ChatMessageReceivedEventArgs e = new EventArguments.ChatMessageReceivedEventArgs(msg, userName); // Create the event data
                        MessageReceived?.Invoke(this, e); // Invoke the event
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

        /// <summary>
        /// Base64 encodes outgoing data, decodes incoming data
        /// </summary>
        public class Base64 : Augmentation
        {
            /// <summary>
            /// Create a new base64 stream encoder
            /// </summary>
            public Base64()
            {
                disableConsole = true;
            }

            /// <summary>
            /// Base64 encodes string data
            /// </summary>
            /// <param name="data">The string message</param>
            /// <param name="encoding">The encoding of the message</param>
            /// <returns>The encoded message</returns>
            public override string OnBeforeSendString(string data, Encoding encoding)
            {
                return Convert.ToBase64String(encoding.GetBytes(data));
            }

            /// <summary>
            /// Base64 Decodes string data
            /// </summary>
            /// <param name="data">The string message</param>
            /// <param name="encoding">The encoding of the message</param>
            /// <returns>The base64 decoded string</returns>
            public override string OnBeforeReceiveString(string data, Encoding encoding)
            {
                return encoding.GetString(Convert.FromBase64String(data));
            }
        }

        /// <summary>
        /// Compress byte data before sending, decompress after receiving
        /// </summary>
        public class Compression : Augmentation
        {
            /// <summary>
            /// The compression to use
            /// </summary>
            private readonly Type mode;

            /// <summary>
            /// Defined the type of the compression
            /// </summary>
            public enum Type
            {
                /// <summary>
                /// Compress with deflate
                /// </summary>
                Deflate,
                /// <summary>
                /// Compress with GZIP
                /// </summary>
                GZIP
            }

            /// <summary>
            /// How many bytes were saved from sending/receiving by using compression
            /// </summary>
            public int BytesSaved { get; private set; }

            /// <summary>
            /// Create a new compression object
            /// </summary>
            /// <param name="compressionType">The compression to use</param>
            public Compression(Type compressionType)
            {
                disableConsole = true;
                mode = compressionType;
            }

            /// <summary>
            /// Test the compression locally
            /// </summary>
            /// <param name="data">The data to compress</param>
            /// <returns>Hopefully the same array as given</returns>
            public byte[] Test(byte[] data)
            {
                if (mode == Type.GZIP) return DecodeGzip(EncodeGzip(data)); // Test gzip
                else return DecodeDeflate(EncodeDeflate(data)); // Test deflate
            }

            /// <summary>
            /// Compress data before sending
            /// </summary>
            /// <param name="data">The array to compress</param>
            /// <param name="offset">The offset to start compressing from</param>
            /// <param name="length">The number of bytes to compress</param>
            /// <returns>The compressed byte array, offset, and it's length</returns>
            public override (byte[] data, int offset, int length) OnBeforeSendBytes(byte[] data, int offset, int length)
            {
                try
                {
                    byte[] toEncode = new byte[length]; // Define array to compress
                    Array.ConstrainedCopy(data, offset, toEncode, 0, length); // Copy data from the source array
                    byte[] encoded = null; // Define array to return

                    if (mode == Type.Deflate) // Use deflate
                    {
                        encoded = EncodeDeflate(toEncode); // Compress array
                    }
                    else // use gzip
                    {
                        encoded = EncodeGzip(toEncode); // Compress array
                    }

                    BytesSaved += length - encoded.Length; // Calculate the saved bytes

                    return (encoded, 0, encoded.Length); // Return the compressed array
                }
                catch (Exception ex) // Compression failed
                {
                    throw new Exception("Failed to compress data!", ex);
                }
            }

            /// <summary>
            /// Decompress data after reading
            /// </summary>
            /// <param name="data">The data to decompress</param>
            /// <returns>The decompressed array</returns>
            public override byte[] OnBeforeReceiveBytes(byte[] data)
            {
                try
                {
                    byte[] decoded = null; // Define array to return

                    if (mode == Type.Deflate) // Use deflate
                    {
                        decoded = DecodeDeflate(data); // Decompress array
                    }
                    else // use gzip
                    {
                        decoded = DecodeGzip(data); // Decompress array
                    }

                    BytesSaved += decoded.Length - data.Length;

                    return decoded; // Return the compressed array
                }
                catch (Exception ex) // Decompression failed
                {
                    throw new Exception("Failed to compress data!", ex);
                }
            }

            // gzip and deflate compression from:
            // https://github.com/AdvancedHacker101/C-Sharp-Proxy-Server/blob/master/proxyServer/Form1.cs#L5473

            /// <summary>
            /// Compress a byte array with deflate
            /// </summary>
            /// <param name="plainData">The data to compress</param>
            /// <returns>The compressed array of bytes</returns>
            private byte[] EncodeDeflate(byte[] plainData)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (System.IO.Compression.DeflateStream deflate = new System.IO.Compression.DeflateStream(ms, System.IO.Compression.CompressionMode.Compress, true))
                    {
                        deflate.Write(plainData, 0, plainData.Length);
                    }
                    return ms.ToArray();
                }
            }

            /// <summary>
            /// Decompress a deflate compressed array
            /// </summary>
            /// <param name="deflateData">The data to decompress</param>
            /// <returns>The decompressed data</returns>
            private byte[] DecodeDeflate(byte[] deflateData)
            {
                byte[] bytes = new byte[4096];
                byte[] decoded;

                using (System.IO.Compression.DeflateStream stream = new System.IO.Compression.DeflateStream(new MemoryStream(deflateData), System.IO.Compression.CompressionMode.Decompress))
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(bytes, 0, 4096);
                            if (count > 0)
                            {
                                memory.Write(bytes, 0, count);
                            }
                        }
                        while (count > 0);
                        decoded = memory.ToArray();
                    }
                }

                return decoded;
            }

            /// <summary>
            /// Compress a byte array with GZIP
            /// </summary>
            /// <param name="gzipData">The array to compress with GZIP</param>
            /// <returns>The GZIP compressed array</returns>
            private byte[] EncodeGzip(byte[] gzipData)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (System.IO.Compression.GZipStream gzip = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true))
                    {
                        gzip.Write(gzipData, 0, gzipData.Length);
                    }
                    return ms.ToArray();
                }
            }

            /// <summary>
            /// Decompress a byte array with GZIP
            /// </summary>
            /// <param name="gzipData">The array to decompress with GZIP</param>
            /// <returns>The GZIP decompressed array</returns>
            private byte[] DecodeGzip(byte[] gzipData)
            {
                byte[] bytes = new byte[4096];
                byte[] decoded;

                using (System.IO.Compression.GZipStream stream = new System.IO.Compression.GZipStream(new MemoryStream(gzipData), System.IO.Compression.CompressionMode.Decompress))
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(bytes, 0, 4096);
                            if (count > 0)
                            {
                                memory.Write(bytes, 0, count);
                            }
                        }
                        while (count > 0);
                        decoded = memory.ToArray();
                    }
                }

                return decoded;
            }
        }

        /// <summary>
        /// Encrypt decrypt data with custom functions
        /// </summary>
        public class Crypt : Augmentation
        {
            /// <summary>
            /// Collection of common crypto functions
            /// </summary>
            public class FunctionBuilder
            {
                /// <summary>
                /// Create general AES crypto functions
                /// </summary>
                /// <param name="pw">The password to encrypt with (client/server has to match)</param>
                /// <returns>The encrypt and decrypt function to use with the crypt augmentation</returns>
                public static (CryptoFunction encrypt, CryptoFunction decrypt) GetAES(string pw)
                {
                    byte[] salt = GetRandomBytes(16);

                    CryptoFunction enc = ((data) =>
                    {
                        byte[] finalResult;
                        // Utilizes helper function to generate random 16 byte salt using RNG

                        // Convert plain text to bytes

                        // create new password derived bytes using password/salt
                        using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(pw, salt))
                        {
                            using (Aes aes = Aes.Create())
                            {
                                // Generate key and iv from password/salt and pass to aes
                                aes.Key = pdb.GetBytes(aes.KeySize / 8);
                                aes.IV = pdb.GetBytes(aes.BlockSize / 8);

                                // Open a new memory stream to write the encrypted data to
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    // Create a crypto stream to perform encryption
                                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                                    {
                                        // write encrypted bytes to memory
                                        cs.Write(data, 0, data.Length);
                                    }
                                    // get the cipher bytes from memory
                                    byte[] cipherBytes = ms.ToArray();
                                    // create a new byte array to hold salt + cipher
                                    byte[] saltedCipherBytes = new byte[salt.Length + cipherBytes.Length];
                                    // copy salt + cipher to new array
                                    Array.Copy(salt, 0, saltedCipherBytes, 0, salt.Length);
                                    Array.Copy(cipherBytes, 0, saltedCipherBytes, salt.Length, cipherBytes.Length);
                                    // convert cipher array to base 64 string
                                    finalResult = saltedCipherBytes;
                                }
                                aes.Clear();
                            }
                        }
                        return finalResult;
                    });

                    CryptoFunction dec = ((data) =>
                    {
                        byte[] result = new byte[2048];
                        byte[] _salt = new byte[16];
                        byte[] toDecrypt = new byte[data.Length - 16];
                        Array.Copy(data, 0, _salt, 0, 16);
                        Array.Copy(data, 16, toDecrypt, 0, data.Length - 16);

                        using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(pw, _salt))
                        {
                            using (Aes aes = Aes.Create())
                            {
                                // Generate key and iv from password/salt and pass to aes
                                aes.Key = pdb.GetBytes(aes.KeySize / 8);
                                aes.IV = pdb.GetBytes(aes.BlockSize / 8);

                                // Open a new memory stream to write the encrypted data to
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    // Create a crypto stream to perform decryption
                                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                                    {
                                        // write decrypted data to memory
                                        cs.Write(toDecrypt, 0, toDecrypt.Length);
                                    }
                                    // convert decrypted array to plain text string
                                    result = ms.ToArray();
                                }
                                aes.Clear();
                            }
                        }
                        return result;
                    });

                    return (enc, dec);
                }

                /// <summary>
                /// Create general 3DES crypto functions
                /// </summary>
                /// <param name="pw">The password to encrypt with (client/server has to match)</param>
                /// <returns>The encrypt and decrypt function to use with the crypt augmentation</returns>
                public static (CryptoFunction encrypt, CryptoFunction decrypt) Get3DES(string pw)
                {
                    byte[] salt = GetRandomBytes(16);

                    CryptoFunction enc = ((data) =>
                    {
                        byte[] finalResult;
                        // Utilizes helper function to generate random 16 byte salt using RNG

                        // Convert plain text to bytes

                        // create new password derived bytes using password/salt
                        using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(pw, salt))
                        {
                            using (TripleDES tripleDES = TripleDES.Create())
                            {
                                // Generate key and iv from password/salt and pass to aes
                                tripleDES.Key = pdb.GetBytes(tripleDES.KeySize / 8);
                                tripleDES.IV = pdb.GetBytes(tripleDES.BlockSize / 8);

                                // Open a new memory stream to write the encrypted data to
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    // Create a crypto stream to perform encryption
                                    using (CryptoStream cs = new CryptoStream(ms, tripleDES.CreateEncryptor(), CryptoStreamMode.Write))
                                    {
                                        // write encrypted bytes to memory
                                        cs.Write(data, 0, data.Length);
                                    }
                                    // get the cipher bytes from memory
                                    byte[] cipherBytes = ms.ToArray();
                                    // create a new byte array to hold salt + cipher
                                    byte[] saltedCipherBytes = new byte[salt.Length + cipherBytes.Length];
                                    // copy salt + cipher to new array
                                    Array.Copy(salt, 0, saltedCipherBytes, 0, salt.Length);
                                    Array.Copy(cipherBytes, 0, saltedCipherBytes, salt.Length, cipherBytes.Length);
                                    // convert cipher array to base 64 string
                                    finalResult = saltedCipherBytes;
                                }
                                tripleDES.Clear();
                            }
                        }
                        return finalResult;
                    });

                    CryptoFunction dec = ((data) =>
                    {
                        byte[] result = new byte[2048];
                        byte[] _salt = new byte[16];
                        byte[] toDecrypt = new byte[data.Length - 16];
                        Array.Copy(data, 0, _salt, 0, 16);
                        Array.Copy(data, 16, toDecrypt, 0, data.Length - 16);

                        using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(pw, _salt))
                        {
                            using (TripleDES tripleDES = TripleDES.Create())
                            {
                                // Generate key and iv from password/salt and pass to aes
                                tripleDES.Key = pdb.GetBytes(tripleDES.KeySize / 8);
                                tripleDES.IV = pdb.GetBytes(tripleDES.BlockSize / 8);

                                // Open a new memory stream to write the encrypted data to
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    // Create a crypto stream to perform decryption
                                    using (CryptoStream cs = new CryptoStream(ms, tripleDES.CreateDecryptor(), CryptoStreamMode.Write))
                                    {
                                        // write decrypted data to memory
                                        cs.Write(toDecrypt, 0, toDecrypt.Length);
                                    }
                                    // convert decrypted array to plain text string
                                    result = ms.ToArray();
                                }
                                tripleDES.Clear();
                            }
                        }
                        return result;
                    });

                    return (enc, dec);
                }

                /// <summary>
                /// Demo only, this method doesn't provide security, only put in for demo purposes of custom encryption development
                /// </summary>
                /// <param name="xorKey">The byte to XOR the other bytes with</param>
                /// <returns>The encrypt and decrypt function to use with the crypt augmentation</returns>
                public static (CryptoFunction encrypt, CryptoFunction decrypt) GetXOR(int xorKey)
                {
                    CryptoFunction enc = ((data) =>
                    {
                        for (int i = 0; i < data.Length; i++)
                        {
                            int current = data[i];
                            data[i] = (byte)(current ^ xorKey);
                        }

                        return data;
                    });

                    return (enc, enc);
                }

                /// <summary>
                /// Generate random byte arrays
                /// </summary>
                /// <param name="vLength">The length of the random byte array</param>
                /// <returns>The random byte array</returns>
                private static byte[] GetRandomBytes(int vLength)
                {
                    byte[] data = new byte[vLength];
                    using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(data);
                    }
                    return data;
                }
            }

            /// <summary>
            /// Encrypt/Decrypt data
            /// </summary>
            /// <param name="data">The data to encrypt/decrypt</param>
            /// <returns>The encrypted/decrypted data</returns>
            public delegate byte[] CryptoFunction(byte[] data);
            /// <summary>
            /// Function to encrypt data with
            /// </summary>
            private readonly CryptoFunction enc;
            /// <summary>
            /// Function to decrypt data with
            /// </summary>
            private readonly CryptoFunction dec;

            /// <summary>
            /// Create a new crypt augmentation
            /// </summary>
            /// <param name="encrypt">The function to encrypt data with</param>
            /// <param name="decrypt">The function to decrypt data with</param>
            public Crypt(CryptoFunction encrypt, CryptoFunction decrypt)
            {
                disableConsole = true;
                // Set the crypt functions
                enc = encrypt;
                dec = decrypt;
            }

            /// <summary>
            /// Encrypt and outgoing byte data
            /// </summary>
            /// <param name="data">The data to encrypt</param>
            /// <param name="offset">The offset to start encrypting from</param>
            /// <param name="length">The number of bytes to encrypt</param>
            /// <returns>The encrypted data</returns>
            public override (byte[] data, int offset, int length) OnBeforeSendBytes(byte[] data, int offset, int length)
            {
                byte[] encrypt = new byte[length]; // Define the data to encrypt
                Array.ConstrainedCopy(data, offset, encrypt, 0, length); // Get the data to encrypt
                byte[] crypted = enc(encrypt); // Encrypt the data
                return (crypted, 0, crypted.Length); // Return the encrypted data
            }

            /// <summary>
            /// Decrypt incoming byte data
            /// </summary>
            /// <param name="data">The data to decrypt</param>
            /// <returns>The decrypted data</returns>
            public override byte[] OnBeforeReceiveBytes(byte[] data)
            {
                return dec(data); // Decrypt the data and return it
            }

        }
    }

    namespace Servers
    {
        /// <summary>
        /// Single Client Tcp Server
        /// </summary>
        public class SingleTcpServer : INetworkReader, INetworkWriter, INetworkSocket, INetworkServer, IAugmentable, IDisposable
        {
            /// <summary>
            /// The socket of the server
            /// </summary>
            private Socket serverSocket;
            /// <summary>
            /// The connected client's socket
            /// </summary>
            private Clients.TcpClient client;
            /// <summary>
            /// The endpoint to run the server on
            /// </summary>
            private IPEndPoint serverEndPoint;
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
            private bool serverOffline = true;
            /// <summary>
            /// Indicates if the server should restart after the current client closes
            /// </summary>
            private bool restartReading;
            /// <summary>
            /// Event listener for client disconnection
            /// </summary>
            private event Action ClientDisconnected;
            /// <summary>
            /// Event listener for client connection
            /// </summary>
            private event Action ClientConnected;
            /// <summary>
            /// List of installed augmentations
            /// </summary>
            private List<Augmentation> augmentations = new List<Augmentation>();
            /// <summary>
            /// Indicates if the object is disposed
            /// </summary>
            private bool disposed = false;

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
                    client.AddEventClientStopped((s, ea) => { if (restartReading) Start(); });
                    client.AddEventClientStopped((s, ea) => ClientDisconnected?.Invoke());
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
            }

            /// <summary>
            /// Close the client forcefully
            /// </summary>
            private void ForceCloseClient()
            {
                if (client == null) return; // Check if the client is null
                client.ForceStop(); // Close the client
            }

            /// <summary>
            /// Close the server
            /// </summary>
            private void ServerClose()
            {
                serverSocket.Close(); // Close the server socket
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
            public void AddEventDataReceived(EventHandlers.BytesReadEventHandler callback)
            {
                client.AddEventDataReceived(callback); // Use the wrapped client to listen for bytes
            }

            /// <summary>
            /// Add event listener for receiving new lines
            /// </summary>
            /// <param name="callback">The function to call when new lines are read from the stream</param>
            public void AddEventLineReceived(EventHandlers.StringReadEventHandler callback)
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
                WaitForClient(); // Wait for a connection
                client.WriteLine(data); // Write the new line to the stream
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

            /// <summary>
            /// Dispose the object and release all resources
            /// </summary>
            public void Dispose()
            {
                Dispose(true); // Dispose object
                GC.SuppressFinalize(this); // Suppress the finalizer call
            }

            /// <summary>
            /// Dispose the object
            /// </summary>
            /// <param name="disposing">True if called from dispose, false if called from finalizer</param>
            protected virtual void Dispose(bool disposing)
            {
                if (disposed) return; // Don't dispose twice

                if (disposing) // Check if we need to dispose managed resources
                {
                    try
                    {
                        if (!serverOffline)
                        {
                            ForceStop(); // Stop and dispose the server and the client 
                        }
                    }
                    finally
                    {
                        if (client != null) client.Dispose(); // Dispose the client
                        client = null; // Release client reference
                        if (serverSocket != null) serverSocket.Dispose(); // Dispose server socket
                        serverSocket = null; // Release server socket reference
                    }
                }

                disposed = true; // Object disposed
            }

            /// <summary>
            /// Finalizer for tcp server
            /// </summary>
            ~SingleTcpServer()
            {
                Dispose(false); // Dispose object
            }
        }

        /// <summary>
        /// Single Client SSL Server
        /// </summary>
        public class SingleSSLServer : INetworkReader, INetworkWriter, INetworkSocket, INetworkServer, IAugmentable, IDisposable
        {
            /// <summary>
            /// The server socket
            /// </summary>
            private Socket serverSocket;
            /// <summary>
            /// The wrapped client socket
            /// </summary>
            private Clients.SSLClient client;
            /// <summary>
            /// The endpoint to run the server on
            /// </summary>
            private IPEndPoint serverEndPoint;
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
            private bool serverOffline = true;
            /// <summary>
            /// Event for client connection notification
            /// </summary>
            private event Action ClientConnected;
            /// <summary>
            /// Indicates if the server should restart after it's client closes
            /// </summary>
            private bool restartReading;
            /// <summary>
            /// The SSL Parameters of the server
            /// </summary>
            private Utils.SSL.ServerSSLData sslParams;
            /// <summary>
            /// Event listener when the client disconnects
            /// </summary>
            private event Action ClientDisconnected;
            /// <summary>
            /// A list to keep installed augmentations in
            /// </summary>
            private List<Augmentation> augmentations = new List<Augmentation>();
            /// <summary>
            /// Indicates if the object is disposed
            /// </summary>
            private bool disposed = false;

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
            }

            /// <summary>
            /// Close the wrapped client forcefully
            /// </summary>
            private void ForceCloseClient()
            {
                if (client == null) return; // Check if the client's null
                client.ForceStop(); // Stop the client
            }

            /// <summary>
            /// Stop the server
            /// </summary>
            private void ServerClose()
            {
                serverSocket.Close(); // Close the server socket
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
            public void AddEventDataReceived(EventHandlers.BytesReadEventHandler callback)
            {
                client.AddEventDataReceived(callback); // Listen for bytes, using the wrapped client
            }

            /// <summary>
            /// Add event listener for reading new lines from the stream
            /// </summary>
            /// <param name="callback">The function to call when new line are read</param>
            public void AddEventLineReceived(EventHandlers.StringReadEventHandler callback)
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
                WaitForClient(); // Wait for a connection
                client.WriteLine(data); // Write the new line to the stream
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

            /// <summary>
            /// Dispose the object
            /// </summary>
            public void Dispose()
            {
                Dispose(true); // Dispose the object
                GC.SuppressFinalize(this); // Don't call finalizer
            }

            /// <summary>
            /// Dispose the object
            /// </summary>
            /// <param name="disposing">True if called from dispose, false if called from finalizer</param>
            protected virtual void Dispose(bool disposing)
            {
                if (disposed) return;

                if (disposing)
                {
                    try
                    {
                        if (!serverOffline) ForceStop();
                    }
                    finally
                    {
                        // Dispose client
                        client.Dispose();
                        client = null;
                        // Dispose server socket
                        serverSocket?.Dispose();
                        serverSocket = null;
                    }
                }

                disposed = true;
            }

            /// <summary>
            /// Object finalizer
            /// </summary>
            ~SingleSSLServer()
            {
                Dispose(false);
            }
        }

        /// <summary>
        /// Multi socket Tcp Server
        /// </summary>
        public class MultiTcpServer : INetworkMultiReader, INetworkMultiWriter, INetworkSocket, INetworkMultiServer, IAugmentable, IDisposable
        {
            /// <summary>
            /// The socket of the server
            /// </summary>
            private Socket serverSocket;
            /// <summary>
            /// The connected client's socket
            /// </summary>
            private List<(string ID, Clients.TcpClient Client)> clients = new List<(string, Clients.TcpClient)>();
            /// <summary>
            /// The endpoint to run the server on
            /// </summary>
            private IPEndPoint serverEndPoint;
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
            private bool serverOffline = true;
            /// <summary>
            /// Event listener for when a new client is connected
            /// </summary>
            private event Action<string> ClientConnected;
            /// <summary>
            /// Event listener for when a client is disconnected
            /// </summary>
            private event Action<string> ClientDisconnected;
            /// <summary>
            /// A list of installed augmentations
            /// </summary>
            private List<Augmentation> augmentations = new List<Augmentation>();
            /// <summary>
            /// Indicates if the object is diposed
            /// </summary>
            private bool disposed = false;

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
                        // Add the client to the list
                        clients.Add((clientID, client));
                        client.AddEventClientStopped((s, ea) => ClientDisconnected?.Invoke(clientID));
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

                foreach (var (clientID, client) in clients) // Go through the clients
                {
                    localClients.Add(clientID); // Add the ID of the clients
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
                var (ID, client) = clients.Find((data) => data.ID == clientID); // Find the matching client
                if (ID == null || client == null) // If not found
                {
#if Validation_hard // Not so important check
                    throw new ArgumentException("The specified clientID doesn't exist");
#else
                    return null;
#endif
                }

                return client; // Return the client instance
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
            }

            /// <summary>
            /// Close the client forcefully
            /// <param name="client">The tcp client to close the connection with</param>
            /// </summary>
            private void ForceCloseClient(Clients.TcpClient client)
            {
                if (client == null) return; // Check if the client is null
                client.ForceStop(); // Close the client
            }

            /// <summary>
            /// Close the server
            /// </summary>
            private void ServerClose()
            {
                serverSocket.Close(); // Close the server socket
            }

            /// <summary>
            /// Close the server gracefully
            /// </summary>
            public void GracefulStop()
            {
                augmentations.ForEach((aug) => aug.OnStop()); // Signal the stop event to the augmentations
                if (serverOffline) return; // Check if the server's stopped
                foreach (var (ID, client) in clients) // Go through the connected clients
                {
                    GracefulCloseClient(client); // Close the client
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
                foreach (var (ID, client) in clients) // Go through the connected clients
                {
                    ForceCloseClient(client); // Force close the client
                }
                ServerClose(); // Close the server
                serverOffline = true; // The server's stopped
            }

            /// <summary>
            /// Add event listener for receiving bytes
            /// </summary>
            /// <param name="callback">The function to call when bytes are read from the stream</param>
            /// <param name="clientID">The ID of the client to read from</param>
            public void AddEventDataReceived(EventHandlers.BytesReadEventHandler callback, string clientID)
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
            public void AddEventLineReceived(EventHandlers.StringReadEventHandler callback, string clientID)
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
                client.WriteLine(data); // Write the new line to the stream
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

                foreach (var (ID, client) in clients) // Go through the connected clients
                {
                    client.InstallAugmentation(augmentation); // Install the augmentation to the current client
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

                foreach (var (ID, client) in clients) // Go through the connected clients
                {
                    client.UninstallAugmentation(augmentation); // Uninstall the augmentation from the current client
                }
            }

            /// <summary>
            /// Dispose the object
            /// </summary>
            public void Dispose()
            {
                Dispose(true); // Dispose the object
                GC.SuppressFinalize(this); // Don't call finalizer
            }

            /// <summary>
            /// Dispose the object
            /// </summary>
            /// <param name="disposing">True if called from dispose, false if called from finalizer</param>
            protected virtual void Dispose(bool disposing)
            {
                if (disposed) return;

                if (disposing)
                {
                    try
                    {
                        if (!serverOffline) ForceStop();
                    }
                    finally
                    {
                        // Dispose clients
                        foreach (var (_, client) in clients)
                        {
                            client?.Dispose();
                        }

                        // Dispose server socket
                        serverSocket?.Dispose();
                        serverSocket = null;

                        // Dispose the client list
                        clients?.Clear();
                        clients = null;
                    }
                }

                disposed = true;
            }

            /// <summary>
            /// Object finalizer
            /// </summary>
            ~MultiTcpServer()
            {
                Dispose(false);
            }
        }

        /// <summary>
        /// Multi socket SSL Server
        /// </summary>
        public class MultiSSLServer : INetworkMultiReader, INetworkMultiWriter, INetworkSocket, INetworkMultiServer, IAugmentable, IDisposable
        {
            /// <summary>
            /// The socket of the server
            /// </summary>
            private Socket serverSocket;
            /// <summary>
            /// The connected client's socket
            /// </summary>
            private List<(string ID, Clients.SSLClient Client)> clients = new List<(string, Clients.SSLClient)>();
            /// <summary>
            /// The endpoint to run the server on
            /// </summary>
            private IPEndPoint serverEndPoint;
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
            private bool serverOffline = true;
            /// <summary>
            /// Event listener for when a new client is connected
            /// </summary>
            private event Action<string> ClientConnected;
            /// <summary>
            /// Event listener for when a client is disconnected
            /// </summary>
            private event Action<string> ClientDisconnected;
            /// <summary>
            /// A list of installed augmentations
            /// </summary>
            private List<Augmentation> augmentations = new List<Augmentation>();
            /// <summary>
            /// The SSL Parameters of the server
            /// </summary>
            private Utils.SSL.ServerSSLData sslParams;
            /// <summary>
            /// Indicates if the object is disposed
            /// </summary>
            private bool disposed = false;

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
                        // Add the client to the list
                        clients.Add((clientID, client));
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

                foreach (var (ID, client) in clients) // Go through the clients
                {
                    localClients.Add(ID); // Add the ID of the clients
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
                var (ID, client) = clients.Find((data) => data.ID == clientID); // Find the matching client
                if (ID == null || client == null) // If not found
                {
#if Validation_hard // Not so important check
                    throw new ArgumentException("The specified clientID doesn't exist");
#else
                    return null;
#endif
                }

                return client; // Return the client instance
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
            }

            /// <summary>
            /// Close the client forcefully
            /// <param name="client">The ssl client to close the connection with</param>
            /// </summary>
            private void ForceCloseClient(Clients.SSLClient client)
            {
                if (client == null) return; // Check if the client is null
                client.ForceStop(); // Close the client
            }

            /// <summary>
            /// Close the server
            /// </summary>
            private void ServerClose()
            {
                serverSocket.Close(); // Close the server socket
            }

            /// <summary>
            /// Close the server gracefully
            /// </summary>
            public void GracefulStop()
            {
                augmentations.ForEach((aug) => aug.OnStop()); // Signal the stop event to the augmentations
                if (serverOffline) return; // Check if the server's stopped
                foreach (var (ID, client) in clients) // Go through the connected clients
                {
                    GracefulCloseClient(client); // Close the client
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
                foreach (var (ID, client) in clients) // Go through the connected clients
                {
                    ForceCloseClient(client); // Force close the client
                }
                ServerClose(); // Close the server
                serverOffline = true; // The server's stopped
            }

            /// <summary>
            /// Add event listener for receiving bytes
            /// </summary>
            /// <param name="callback">The function to call when bytes are read from the stream</param>
            /// <param name="clientID">The ID of the client to read from</param>
            public void AddEventDataReceived(EventHandlers.BytesReadEventHandler callback, string clientID)
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
            public void AddEventLineReceived(EventHandlers.StringReadEventHandler callback, string clientID)
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
                client.WriteLine(data); // Write the new line to the stream
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

                foreach (var (ID, client) in clients) // Go through the connected clients
                {
                    client.InstallAugmentation(augmentation); // Install the augmentation to the current client
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

                foreach (var (ID, client) in clients) // Go through the connected clients
                {
                    client.UninstallAugmentation(augmentation); // Uninstall the augmentation from the current client
                }
            }

            /// <summary>
            /// Dispose the object
            /// </summary>
            public void Dispose()
            {
                Dispose(true); // Dispose the object
                GC.SuppressFinalize(this); // Don't call finalizer
            }

            /// <summary>
            /// Dispose the object
            /// </summary>
            /// <param name="disposing">True if called from dispose, false if called from finalizer</param>
            protected virtual void Dispose(bool disposing)
            {
                if (disposed) return;

                if (disposing)
                {
                    try
                    {
                        if (!serverOffline) ForceStop();
                    }
                    finally
                    {
                        // Dispose clients
                        foreach (var (_, client) in clients)
                        {
                            client?.Dispose();
                        }

                        // Dispose server socket
                        serverSocket?.Dispose();
                        serverSocket = null;

                        // Dispose client list
                        clients?.Clear();
                        clients = null;
                    }
                }

                disposed = true;
            }

            /// <summary>
            /// Object finalizer
            /// </summary>
            ~MultiSSLServer()
            {
                Dispose(false);
            }
        }
    }

    namespace Clients
    {
        /// <summary>
        /// Basic TCP Client
        /// </summary>
        public class TcpClient : INetworkReader, INetworkWriter, INetworkSocket, IAugmentable, INetworkClient, IDisposable
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
            /// How many ms to wait before cancelling read operation
            /// </summary>
            public int ReadTimeout
            {
                get
                {
                    return _readTimeout; // Return the timeout value
                }
                set
                {
                    _readTimeout = value; // Set the timeout value
                    if (client != null) client.ReceiveTimeout = _readTimeout; // Live-set the value if the client is alrady started
                }
            }
            /// <summary>
            /// Backing field for the read timeout property
            /// </summary>
            private int _readTimeout;
            /// <summary>
            /// Event for receiving byte data
            /// </summary>
            protected event EventHandlers.BytesReadEventHandler DataReceived;
            /// <summary>
            /// Event for receiving new lines
            /// </summary>
            protected event EventHandlers.StringReadEventHandler LineReceived;
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
            protected event EventHandlers.ClientStoppedEventHandler ClientStopped;
            /// <summary>
            /// A list of installed augmentations
            /// </summary>
            protected List<Augmentation> augmentations = new List<Augmentation>();
            /// <summary>
            /// Indicates if a server socket created this client
            /// </summary>
            protected bool fromServer = false;
            /// <summary>
            /// Indicates if the object is disposed
            /// </summary>
            private bool disposed = false;

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
                ReadTimeout = -1; // Set the read timeout
            }

            /// <summary>
            /// Init the client
            /// </summary>
            /// <param name="address">The IP Address or hostname to connect to</param>
            /// <param name="portNumber">The port to connect to</param>
            public TcpClient(string address, int portNumber)
            {
                ClientInit(); // Init the client
                var (isValid, ipAddress) = Utils.IPv4.IsValidDestination(address); // Check if the address is valid, and resolve dns if needed
#if Validation_hard // Not important check, socket connection will throw anyways if any of the data is wrong
                if (!isValid) throw new ArgumentException($"The following destination is not valid: {address}"); // Invalid address or hostname not resolved
                if (!Utils.IPv4.IsPortValid(portNumber)) throw new ArgumentException($"The following port number is not valid {portNumber}"); // Invalid port number
#endif
                clientEndPoint = new IPEndPoint(ipAddress, portNumber); // Set the endpoint to connect to
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
            /// Get the underlying network stream of the client
            /// </summary>
            /// <returns>The network stream of the client socket</returns>
            public NetworkStream GetRawStream()
            {
                if (client == null || !client.Connected) return null; // Check if the client isn't started/connected
                return new NetworkStream(client); // Return the network stream of the socket
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
                client.ReceiveTimeout = ReadTimeout; // Set the read timeout
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
                int bytesRead = 0; // Define read bytes count
                try
                {
                    bytesRead = client.Receive(buffer, 0, maxSize, SocketFlags.None); // Read bytes from the stream
                }
                catch (IOException) // Timeout happened
                {
                    return null; // Return empty result
                }
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
                    int bytesRead = 0; // Define read bytes length
                    try
                    {
                        bytesRead = client.Receive(buffer, 0, RecvSize, SocketFlags.None); // Read bytes from the stream
                    }
                    catch (IOException) // Timeout happened
                    {
                        return null; // Return empty result
                    }
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
                catch (IOException ioex) // Timeout happened
                {
#if Logging_verbose
                    Console.WriteLine("Suppressing read error, caused by timeout");
                    Console.WriteLine(ioex);
                    return;
#endif
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
                        DataReceived?.Invoke(this, new EventArguments.BytesReadEventArgs(dataBuffer)); // Invoke the data received event
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
                            LineReceived?.Invoke(this, new EventArguments.StringReadEventArgs(res)); // Invoke the callback
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
            public void AddEventDataReceived(EventHandlers.BytesReadEventHandler callback)
            {
                DataReceived += callback; // Add the function to the event
                StartCallbackRead(Utils.NetworkIO.ReadEventCodes.DataRecv); // Start the callback
            }

            /// <summary>
            /// Add an event listener for receiving new lines from the stream
            /// </summary>
            /// <param name="callback">The function to call when new lines are available</param>
            public void AddEventLineReceived(EventHandlers.StringReadEventHandler callback)
            {
                LineReceived += callback; // Add the function to the event
                StartCallbackRead(Utils.NetworkIO.ReadEventCodes.LineRecv); // Enable the callback
            }

            /// <summary>
            /// Add an event listener for when the client is stopped
            /// </summary>
            /// <param name="callback">The function to call when the client is disconnected</param>
            public void AddEventClientStopped(EventHandlers.ClientStoppedEventHandler callback)
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
                clientOffline = true; // The client isn't running
                ClientStopped?.Invoke(this, new EventArgs()); // Invoke the client stopped event
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
                clientOffline = true; // The client isn't running
                ClientStopped?.Invoke(this, new EventArgs()); // Invoke the client stopped event
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
                    (buffer, offset, length) = aug.OnBeforeSendBytes(buffer, offset, length); // Let the augmentations modify the data
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
                client.Send(sendBuffer, 0, sendBuffer.Length, SocketFlags.None); // Send the bytes to the stream
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

            /// <summary>
            /// Close the stream of the socket, but leave the connection open
            /// </summary>
            public void CloseStream()
            {
                NetworkStream ns = new NetworkStream(client); // Get the network stream of the client
                ns.Close(); // Close the stream
            }

            /// <summary>
            /// Dispose the object
            /// </summary>
            public void Dispose()
            {
                Dispose(true); // Dispose the object
                GC.SuppressFinalize(this); // Don't call finalizer
            }

            /// <summary>
            /// Dispose the object
            /// </summary>
            /// <param name="disposing">True if called from dispose, false if called from finalizer</param>
            protected virtual void Dispose(bool disposing)
            {
                if (disposed) return; // Don't dispose twice

                if (disposing)
                {
                    try
                    {
                        if (!clientOffline)
                        {
                            ForceStop(); // Try stopping the client, then dispose it 
                        }
                    }
                    finally
                    {
                        if (client != null) client.Dispose(); // Dispose the client
                        client = null; // Release the client reference
                    }
                }

                disposed = true; // Object disposed
            }

            /// <summary>
            /// Finalize the object
            /// </summary>
            ~TcpClient()
            {
                Dispose(false); // Call dispose
            }
        }

        /// <summary>
        /// SSL Network Client
        /// </summary>
        public class SSLClient : INetworkSocket, INetworkReader, INetworkWriter, INetworkClient, IAugmentable, IDisposable
        {
            /// <summary>
            /// The endpoint to connect to
            /// </summary>
            private IPEndPoint clientEndPoint;
            /// <summary>
            /// The client socket
            /// </summary>
            private Socket client;
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
            /// How many ms to wait before cancelling a read operation
            /// </summary>
            public int ReadTimeout
            {
                get
                {
                    return _readTimeout; // Return the timeout value
                }
                set
                {
                    _readTimeout = value; // Set the timeout value
                    if (sslStream != null) sslStream.ReadTimeout = _readTimeout; // Live-set the timeout value if the client is already started
                }
            }
            /// <summary>
            /// Backing field for the ReadTimeout property
            /// </summary>
            private int _readTimeout;
            /// <summary>
            /// Event for receiving byte data
            /// </summary>
            private event EventHandlers.BytesReadEventHandler DataReceived;
            /// <summary>
            /// Event for receiving new lines
            /// </summary>
            private event EventHandlers.StringReadEventHandler LineReceived;
            /// <summary>
            /// Enabled receive events
            /// </summary>
            private Utils.NetworkIO.ReadEventCodes receiveCallbackMode;
            /// <summary>
            /// Indicates if the client's running
            /// </summary>
            private bool clientOffline = true;
            /// <summary>
            /// Event for client disconnect notifications
            /// </summary>
            private event Action ClientStopped;
            /// <summary>
            /// A list of installed augmentations
            /// </summary>
            private List<Augmentation> augmentations = new List<Augmentation>();
            /// <summary>
            /// Indicates if a server socket created this client
            /// </summary>
            private readonly bool fromServer = false;
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
            /// Indicates if the object is disposed
            /// </summary>
            private bool disposed = false;

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
            /// Init the tcp client
            /// </summary>
            private void TcpClientInit()
            {
                clientEndPoint = null; // Reset the endpoint
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a new client socket
                RecvSize = 1024; // Set the receive buffer size
                ReadNewLine = "\n"; // Init the read new line terminator
                WriteNewLine = "\n"; // Init the write new line terminator
                SendEncoder = Encoding.ASCII; // Init the send string encoding
                RecvEncoder = Encoding.ASCII; // Init the read string encoding
                receiveCallbackMode = Utils.NetworkIO.ReadEventCodes.Stopped; // Set the receive event mode
                ReadTimeout = -1; // Set the read timeout
            }

            /// <summary>
            /// Init the client
            /// </summary>
            /// <param name="address">The hostname or address of the server</param>
            /// <param name="portNumber">The port to connect to</param>
            /// <param name="certWarn">True to ignore certificate warnings, otherwise false</param>
            public SSLClient(string address, int portNumber, bool certWarn)
            {
                TcpClientInit(); // Init the client
                var (isValid, ipAddress) = Utils.IPv4.IsValidDestination(address); // Check if the address is valid, and resolve dns if needed
#if Validation_hard // Not important check, socket connection will throw anyways if any of the data is wrong
                if (!isValid) throw new ArgumentException($"The following destination is not valid: {address}"); // Invalid address or hostname not resolved
                if (!Utils.IPv4.IsPortValid(portNumber)) throw new ArgumentException($"The following port number is not valid {portNumber}"); // Invalid port number
#endif
                clientEndPoint = new IPEndPoint(ipAddress, portNumber); // Set the endpoint to connect to
                ClientInit(certWarn); // Init the client
                serverDestination = address; // Set the destination
            }

            /// <summary>
            /// Init the client
            /// </summary>
            /// <param name="ep">The endpoint to connect to</param>
            /// <param name="certWarn">True to ignore certificate warnings, otherwise false</param>
            public SSLClient(IPEndPoint ep, bool certWarn)
            {
                TcpClientInit();
                clientEndPoint = ep;
                ClientInit(certWarn); // Init the client
                serverDestination = ep.Address.ToString(); // Set the server destination
            }

            /// <summary>
            /// Init the client
            /// </summary>
            /// <param name="stream">The SSL Stream to wrap around</param>
            public SSLClient(System.Net.Security.SslStream stream)
            {
                TcpClientInit();
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
            public void Start()
            {
                if (IgnoreCertificateWarning) // Ignore certificate warnings
                {
                    //Define a new callback for validation
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback((sender, cert, chain, errors) =>
                    {
                        return true; // Return true, without any validation
                    });
                }
#if Validation_soft // Important check
                if (clientEndPoint == null) throw new NullReferenceException("The EndPoint to connect to is null"); // Check if the endpoint isn't set
#endif
                client.Connect(clientEndPoint); // Connect to the endpoint
                clientOffline = false; // The client is running now
                if (!fromServer) augmentations.ForEach((aug) => aug.OnStart()); // Signal the start event to augmentations if we're not created from a server class
                NetworkStream ns = new NetworkStream(client); // Get the network stream of the client
                sslStream = new System.Net.Security.SslStream(ns)
                {
                    ReadTimeout = ReadTimeout
                }; // Wrap the stream inside ssl stream
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
                catch (IOException ioex) // Timeout happened
                {
#if Logging_verbose
                    Console.WriteLine("Suppressing socket error, caused by timing out");
                    Console.WriteLine(ioex);
#endif
                    return;
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
                        DataReceived?.Invoke(this, new EventArguments.BytesReadEventArgs(dataBuffer)); // Invoke the data received event
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
                            LineReceived?.Invoke(this, new EventArguments.StringReadEventArgs(res)); // Invoke the new line event
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
                clientOffline = true; // The client isn't running
                ClientStopped?.Invoke(); // Invoke the client stopped event
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
            /// Add an event listener for when new bytes are available
            /// </summary>
            /// <param name="callback">The function to call when new bytes are available</param>
            public void AddEventDataReceived(EventHandlers.BytesReadEventHandler callback)
            {
                DataReceived += callback; // Add the function to the event
                StartCallbackRead(Utils.NetworkIO.ReadEventCodes.DataRecv); // Start the callback
            }

            /// <summary>
            /// Add and event listener for when new lines are available
            /// </summary>
            /// <param name="callback">The function to call when new lines are available</param>
            public void AddEventLineReceived(EventHandlers.StringReadEventHandler callback)
            {
                LineReceived += callback; // Add the function to the event
                StartCallbackRead(Utils.NetworkIO.ReadEventCodes.LineRecv); // Enable the callback
            }

            /// <summary>
            /// Directly read bytes from the stream
            /// </summary>
            /// <param name="maxSize">The maximum number of bytes to read</param>
            /// <returns>The array of bytes read from the stream</returns>
            public byte[] DirectRead(int maxSize)
            {
#if Validation_soft // Important check
                if (sslStream == null || !fromServer && (client == null || !client.Connected)) throw new InvalidOperationException("Cannot read from stream, when client is offline"); // Check if the base client is running
#endif
#if Validation_hard // Not so important check
                if (!sslStream.CanRead) throw new InvalidOperationException("Cannot read from stream, SSL client can't read"); // Check if the sslStream object can read from the stream
#endif
                byte[] buffer = new byte[maxSize]; // Define the receive buffer
                int bytesRead = 0; // Define read bytes count
                try
                {
                    bytesRead = sslStream.Read(buffer, 0, maxSize); // Read from the stream
                }
                catch (IOException) // Check for timeouts
                {
                    return null; // Return null in case of timeout
                }
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
            public string ReadLine()
            {
#if Validation_soft // Important check
                if (sslStream == null || !fromServer && (client == null || !client.Connected)) throw new InvalidOperationException("Cannot read from stream, when client is offline"); // Check if the base client is running
#endif
#if Validation_hard // Not so important check
                if (!sslStream.CanRead) throw new InvalidOperationException("Cannot read from stream, SSL client can't read"); // Check if the sslStream can read
#endif
                StringBuilder result = new StringBuilder(); // Define string buffer

                while (true) // Loop infinitely
                {
                    byte[] buffer = new byte[RecvSize]; // Define the receive buffer
                    int bytesRead = 0; // Define read bytes count
                    try
                    {
                        bytesRead = sslStream.Read(buffer, 0, RecvSize); // Read bytes from the stream
                    }
                    catch (IOException) // Timeout happened
                    {
                        return null; // Return empty result
                    }
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
            public async Task<byte[]> DirectReadAsync(int maxSize)
            {
                return await Task<byte[]>.Factory.StartNew(() => DirectRead(maxSize)).ConfigureAwait(false); // Wait for the task to complete
            }

            /// <summary>
            /// Read a new line from the stream async
            /// </summary>
            /// <returns>The task with the new line read from the stream</returns>
            public async Task<string> ReadLineAsync()
            {
                return await Task<string>.Factory.StartNew(() => ReadLine()).ConfigureAwait(false); // Wait for the task to complete
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
            /// <param name="buffer">The array of bytes to write</param>
            /// <param name="offset">The offset to begin writing from</param>
            /// <param name="length">The number of bytes to write</param>
            public void DirectWrite(byte[] buffer, int offset, int length)
            {
                augmentations.ForEach((aug) =>
                {
                    (buffer, offset, length) = aug.OnBeforeSendBytes(buffer, offset, length); // Let the augmentations modify the data
                }); // Let the augmentations modify the data
                augmentations.ForEach((aug) => aug.OnAfterSendBytes(buffer, offset, length)); // Let the augmentations inspect the data
                sslStream.Write(buffer, offset, length); // Write the bytes to the stream
            }

            /// <summary>
            /// Write a new line to the stream
            /// </summary>
            /// <param name="data">The line to write out</param>
            public void WriteLine(string data)
            {
                augmentations.ForEach((aug) => data = aug.OnBeforeReceiveString(data, SendEncoder)); // Let the augmentations modify the data
                augmentations.ForEach((aug) => aug.OnAfterReceiveString(data, SendEncoder)); // Let the augmentations inspect the data
                byte[] buffer = SendEncoder.GetBytes(data + WriteNewLine); // Convert the line and the terminator to a byte array
                sslStream.Write(buffer, 0, buffer.Length); // Write the bytes to the stream
            }

            /// <summary>
            /// Gracefully stop the client
            /// </summary>
            public void GracefulStop()
            {
                if (clientOffline) return; // Check if the client's running
                if (sslStream != null)
                {
                    sslStream.Close(); // Close the stream
                }
                if (!fromServer) augmentations.ForEach((aug) => aug.OnStop()); // Signal the stop event to the augmentations if we're not created from server
                if (clientOffline) return; // Return if the client isn't running
#if Validation_soft // Important check
                if (client == null) throw new InvalidOperationException("Cannot close an offline client"); // The client isn't started
#endif

                client.Shutdown(SocketShutdown.Both); // Shutdown read and write sockets
                client.Disconnect(false); // Disconnect from the server
                client.Close(); // Close the client
                clientOffline = true; // The client isn't running
                ClientStopped?.Invoke(); // Invoke the client stopped event
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

            /// <summary>
            /// Get the underlying network stream of the client socket
            /// </summary>
            /// <returns>The network stream of the client socket</returns>
            public NetworkStream GetRawStream()
            {
                if (client == null || !client.Connected) return null; // Check if the client is started
                return new NetworkStream(client); // Return the network stream
            }

            /// <summary>
            /// Close the stream of the client, but leave the connection open
            /// </summary>
            public void CloseStream()
            {
                sslStream.ShutdownAsync().Wait(); // Shutdown the ssl stream (graceful)
                sslStream.Close(); // Close the stream
                sslStream.Dispose(); // Release resources
                sslStream = null; // Relese reference
            }

            /// <summary>
            /// Dispose the object
            /// </summary>
            public void Dispose()
            {
                Dispose(true); // Dispose the object
                GC.SuppressFinalize(this); // Don't call finalizer
            }

            /// <summary>
            /// Dispose the object
            /// </summary>
            /// <param name="disposing">True if called from dispose, false if called from finalizer</param>
            protected virtual void Dispose(bool disposing)
            {
                if (disposed) return;

                if (disposing)
                {
                    try
                    {
                        if (!clientOffline)
                        {
                            ForceStop(); // Stop client, then dispose 
                        }
                    }
                    finally
                    {
                        if (sslStream != null) sslStream.Dispose(); // Dispose the ssl stream
                        sslStream = null; // Release ssl stream reference
                        if (client != null) client.Dispose(); // Dispose the client
                        client = null; // Release client reference
                    }
                }

                disposed = true;
            }

            /// <summary>
            /// Object finalizer
            /// </summary>
            ~SSLClient()
            {
                Dispose(false);
            }
        }

        /// <summary>
        /// Simple basic FTP client
        /// </summary>
        public class FTPClient
        {
            /// <summary>
            /// The socket used for writing data
            /// </summary>
            private INetworkWriter write;
            /// <summary>
            /// The socket used for reading data
            /// </summary>
            private INetworkReader read;
            /// <summary>
            /// The socket used for client control
            /// </summary>
            private INetworkSocket socket;
            /// <summary>
            /// Indicates if the connection is over SSL
            /// </summary>
            private readonly bool ssl = false;
            /// <summary>
            /// True to validate certificates, otherwise false
            /// </summary>
            private bool checkSSL = true;
            /// <summary>
            /// The default port of an FTP server
            /// </summary>
            const int ftpDefaultPort = 21;
            /// <summary>
            /// The default username of an FTP server
            /// </summary>
            const string ftpDefaultUsername = "anonymous";
            /// <summary>
            /// The message after the client connects
            /// </summary>
            public string ServerInformation { get; internal set; } = "";
            /// <summary>
            /// The socket of the data channel
            /// </summary>
            private INetworkSocket dataSocket;
            /// <summary>
            /// The client interface of the client
            /// </summary>
            private INetworkClient netClient;
            /// <summary>
            /// Control the outputting of debug messages
            /// </summary>
            public bool DebugMode { get; set; } = false;
            /// <summary>
            /// The hostname of the server to validate TLS with
            /// </summary>
            private string serverHostname = "";
            /// <summary>
            /// Check the latency of the ftp server
            /// </summary>
            private Utils.LatencyCounter lc;

            /// <summary>
            /// Create a new FTP client
            /// </summary>
            /// <param name="useSSL">Set it to true to use ssl when connection to the server</param>
            public FTPClient(bool useSSL = false)
            {
                ssl = useSSL; // Set the ssl flag
                lc = new Utils.LatencyCounter(); // Create a new latency counter
                Debug($"[OBJ]: Created, ssl usage set to: {ssl}");
            }

            /// <summary>
            /// Get the full latency information string
            /// </summary>
            /// <returns>The string data of the latency counter</returns>
            public string GetLatencyData()
            {
                return lc.ToString(); // Return the latency information
            }

            /// <summary>
            /// Get the average latency between the client and the server
            /// </summary>
            /// <returns>The average latency between the client and the server</returns>
            public long GetAverageLatency()
            {
                return lc.AverageLatency; // Return the average latency
            }

            /// <summary>
            /// Change if the server should check if a certificate is valid
            /// </summary>
            /// <param name="check">True to check certificates, otherwise false</param>
            public void CheckCertificateValidity(bool check)
            {
                checkSSL = check; // Set the validation flag
                Debug($"[OBJ]: SSL Certificate Validation set to: {ssl}");
                if (!checkSSL) // If we ignore certificate warnings
                {
                    Debug($"[OBJ]: WARNING this could be potentially dangerous!");
                    //Define a new callback for validation
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback((sender, cert, chain, errors) =>
                    {
                        return true; // Return true, without any validation
                    });
                }
            }

            /// <summary>
            /// Disconnect from the current ftp server
            /// </summary>
            public void Disconnect()
            {
                Debug($"[CTRL]: Initiating quit command");
                if (write == null) throw new InvalidOperationException("Can't disconnect from server, when you're not even connected to it");
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "QUIT", arguments = "" }); // Send the command to the server
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.ClosingControlConnection) throw new System.Security.SecurityException("Server failed to close control channel connection"); // Throw if unexpected status code
                Debug($"[CTRL]: Server accepted quit");
                netClient.CloseStream(); // Close the stream
                socket.ForceStop(); // Close the socket
                Debug($"[OBJ]: All sockets, stream are now closed");
            }

            /// <summary>
            /// Connect to a server, with the default credentials
            /// </summary>
            /// <param name="hostname">The domain name or IP of the server to connect to</param>
            public void Connect(string hostname)
            {
                Connect(hostname, ftpDefaultPort, ftpDefaultUsername, ""); // Connect to the server
            }

            /// <summary>
            /// Connect to a server with the default credentials on the specified port
            /// </summary>
            /// <param name="hostname">The domain name or IP of the server to connect to</param>
            /// <param name="port">The port to connect to on the server</param>
            public void Connect(string hostname, int port)
            {
                Connect(hostname, port, ftpDefaultUsername, ""); // Connect to the server
            }

            /// <summary>
            /// Connect to a server
            /// </summary>
            /// <param name="hostname">The domain name or IP of the server to connect to</param>
            /// <param name="port">The port of the server to connect to</param>
            /// <param name="username">The username to login to the FTP server with</param>
            /// <param name="password">The password to login to the FTP server with</param>
            public void Connect(string hostname, int port, string username, string password)
            {
                Debug($"[CON]: Connecting to {hostname}:{port}");
                serverHostname = hostname;
                TcpClient client = new TcpClient(Utils.IPv4.CreateEndPoint(hostname, port))
                {
                    // Set the new line endings
                    ReadNewLine = "\r\n",
                    WriteNewLine = "\r\n"
                }; // Create a new tcp client
                client.Start(); // Connect to the server
                // Set the sockets
                write = client as INetworkWriter;
                read = client as INetworkReader;
                socket = client as INetworkSocket;
                netClient = client as INetworkClient;
                Debug($"[CON]: Connection to server established");

                GetServer(); // Get the message of the server
                if (ssl) BeginTLS();
                Login(username, password); // Login to the server
                SetType(); // Set the transfer type
                Pwd(); // Get the current working directory
                Debug($"[CON]: Client init completed, ready for work");
            }

            /// <summary>
            /// Execute and ftp command on the remote server
            /// </summary>
            /// <param name="req">The command to execute</param>
            private int ExecuteCommand(Utils.FTP.Request req)
            {
                write.WriteLine(req.ToString()); // Send the command to the server
                Debug($"[CMD]: {req.ToString()}"); // Debug output the command
                return lc.PacketSent(); // Start the latency timer
            }

            /// <summary>
            /// Download and save a file from the ftp server
            /// </summary>
            /// <param name="remoteFile">The name of the remote file to download</param>
            /// <param name="localFile">The path to save the file to (directory)</param>
            public void DownloadFile(string remoteFile, string localFile)
            {
                Debug($"[DWL]: Starting file download");
                string newName = localFile + "\\" + remoteFile; // Get the new path of the local file
                byte[] data = DownloadFile(remoteFile); // Download the file
                File.WriteAllBytes(newName, data); // Write the file to disk
                Debug($"[DWL]: File contents written to disk");
            }

            /// <summary>
            /// Download a file from the ftp server
            /// </summary>
            /// <param name="remoteFile">The name of the remote file to download</param>
            /// <returns>The contents of the file</returns>
            public byte[] DownloadFile(string remoteFile)
            {
                if (ssl) return DownloadSSL(remoteFile); // Download over TLS
                else return Download(remoteFile); // Downlod over insecure connection
            }

            /// <summary>
            /// Download a file when using normal ftp connection
            /// </summary>
            /// <param name="remoteFile">The remote file to download</param>
            /// <returns>The contents of the remote file</returns>
            private byte[] Download(string remoteFile)
            {
                Debug("[DWL]: Starting download procedure");
                ValueTuple<string, int> dataPort = GetDataPort(); // Get the data channel to connect to
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "RETR", arguments = remoteFile }); // Send the command to the server
                var (_read, _, _client) = ConnectDataPort(dataPort, false); // Connect to the data channel
                _client.ReadTimeout = Math.Max(5 * (int)lc.AverageLatency, 100); // Set a basic read timeout to know when the data is fully received
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.DataChannelReady) throw new System.Security.SecurityException("Failed to transfer the file"); // Throw if unexpected status code
                byte[] result = new byte[0]; // Define the result
                while (true)
                {
                    byte[] data = _read.DirectRead(2048); // Read from the client
                    if (data == null || data.Length == 0) break; // Check if no more data, then break
                    int newLength = result.Length + data.Length; // Get the new length of the buffer
                    byte[] tmp = new byte[result.Length]; // Init a temporary save buffer
                    Array.Copy(result, tmp, result.Length); // Save the current buffer
                    result = new byte[newLength]; // Create a new buffer
                    Array.Copy(tmp, result, tmp.Length); // Copy the previous buffer
                    Array.ConstrainedCopy(data, 0, result, tmp.Length, data.Length); // Copy the received data
                }
                _client.CloseStream(); // Close the ssl stream
                dataSocket.ForceStop(); // Stop the ssl client
                Debug("[DWL]: File bytes read from server");
                resp = GetResponse(-1); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.CloseFtpDataSuccess) throw new System.Security.SecurityException("Failed to start file transfer"); // Throw if unexpected status code
                Debug("[DWL]: Download finished");
                return result;
            }

            /// <summary>
            /// Download a file when using SSL ftp connection
            /// </summary>
            /// <param name="remoteFile">The remote file to download</param>
            /// <returns>The contents of the remote file</returns>
            private byte[] DownloadSSL(string remoteFile)
            {
                Debug("[DWL]: Starting download procedure");
                ValueTuple<string, int> dataPort = GetDataPort(); // Get the data channel to connect to
                var (_, _, _client) = ConnectDataPort(dataPort, false); // Connect to the data channel
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "RETR", arguments = remoteFile }); // Send the command to the server
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.DataChannelReady) throw new System.Security.SecurityException("Failed to transfer the file"); // Throw if unexpected status code
                System.Net.Security.SslStream sslStream = new System.Net.Security.SslStream(_client.GetRawStream());
                sslStream.AuthenticateAsClient(serverHostname); // Authenticate to the server
                SSLClient client = new SSLClient(sslStream)
                {
                    ReadTimeout = Math.Max(10 * (int)lc.AverageLatency, 350) // Set a basic read timeout to know when the data is fully received
                }; // Wrap the ssl stream in the ssl client
                byte[] result = new byte[0]; // Define the result
                while (true)
                {
                    byte[] data = client.DirectRead(2048); // Read from the client
                    if (data == null) break; // Check if no more data, then break
                    int newLength = result.Length + data.Length; // Get the new length of the buffer
                    byte[] tmp = new byte[result.Length]; // Init a temporary save buffer
                    Array.Copy(result, tmp, result.Length); // Save the current buffer
                    result = new byte[newLength]; // Create a new buffer
                    Array.Copy(tmp, result, tmp.Length); // Copy the previous buffer
                    Array.ConstrainedCopy(data, 0, result, tmp.Length, data.Length); // Copy the received data
                }
                client.CloseStream(); // Close the ssl stream
                client.ForceStop(); // Stop the ssl client
                dataSocket.ForceStop(); // Stop the tcp client
                Debug("[DWL]: File bytes read from server");
                resp = GetResponse(-1); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.CloseFtpDataSuccess) throw new System.Security.SecurityException("Failed to start file transfer"); // Throw if unexpected status code
                Debug("[DWL]: Download finished");
                return result;
            }

            /// <summary>
            /// Upload a file to the ftp server
            /// </summary>
            /// <param name="filePath">The path of the file to upload</param>
            public void UploadFile(string filePath)
            {
                Debug("[UPL]: Initiating file upload");
                string name = (new FileInfo(filePath)).Name; // Get the name of the file
                byte[] content = File.ReadAllBytes(filePath); // Get the byte content of the file
                Debug("[UPL]: File loaded into memory");
                UploadFile(name, content); // Upload the file to the server
            }

            /// <summary>
            /// Upload a file to the ftp server
            /// </summary>
            /// <param name="fileName">The name of the file to be uploaded</param>
            /// <param name="fileBytes">The content of the file</param>
            public void UploadFile(string fileName, byte[] fileBytes)
            {
                if (ssl) UploadSSL(fileName, fileBytes); // Upload over TLS
                else Upload(fileName, fileBytes); // Upload over insecure connection
            }

            /// <summary>
            /// Upload a file when using normal ftp connection
            /// </summary>
            /// <param name="fileName">The name of the file to be uploaded</param>
            /// <param name="fileBytes">The content of the file</param>
            private void Upload(string fileName, byte[] fileBytes)
            {
                Debug("[UPL]: Starting upload procedure");
                ValueTuple<string, int> dataPort = GetDataPort(); // Get the data channel to connect to
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "STOR", arguments = fileName }); // Send the command to the server
                var (_, _write, _) = ConnectDataPort(dataPort, ssl); // Get the write socket of the data channel
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.DataChannelReady) throw new System.Security.SecurityException("Failed to start file transfer"); // Throw if unexpected status code
                _write.DirectWrite(fileBytes, 0, fileBytes.Length); // Write the file to the stream
                dataSocket.ForceStop(); // Stop the data channel socket
                Debug("[UPL]: File bytes sent to server");
                resp = GetResponse(-1); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.CloseFtpDataSuccess) throw new System.Security.SecurityException("Failed to transfer the file"); // Throw if unexpected status code
                Debug("[UPL]: Upload finished");
            }

            /// <summary>
            /// Upload a file when using SSL ftp
            /// </summary>
            /// <param name="fileName">The name of the file to be uploaded</param>
            /// <param name="fileBytes">The content of the file</param>
            private void UploadSSL(string fileName, byte[] fileBytes)
            {
                Debug("[UPL]: Starting upload procedure");
                ValueTuple<string, int> dataPort = GetDataPort(); // Get the data channel to connect to
                var (_, _, _client) = ConnectDataPort(dataPort, false); // Connect to the data channel
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "STOR", arguments = fileName }); // Send the command to the server
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.DataChannelReady) throw new System.Security.SecurityException("Failed to start file transfer"); // Throw if unexpected status code
                System.Net.Security.SslStream sslStream = new System.Net.Security.SslStream(_client.GetRawStream());
                sslStream.AuthenticateAsClient(serverHostname); // Authenticate to the server
                SSLClient client = new SSLClient(sslStream); // Wrap the ssl stream in the ssl client
                client.DirectWrite(fileBytes); // Write the file to the stream
                client.CloseStream(); // Close the ssl stream
                client.ForceStop(); // Stop the ssl client
                dataSocket.ForceStop(); // Stop the tcp client
                Debug("[UPL]: File bytes sent to server");
                resp = GetResponse(-1); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.CloseFtpDataSuccess) throw new System.Security.SecurityException("Failed to transfer the file"); // Throw if unexpected status code
                Debug("[UPL]: Upload finished");
            }

            /// <summary>
            /// Move a file from the current folder
            /// </summary>
            /// <param name="fileToMove">The file to move (with extension)</param>
            /// <param name="targetDirectory">The directory to move the file to (specify full path!)</param>
            public void MoveFile(string fileToMove, string targetDirectory)
            {
                Debug($"[MV]: Constructing special rename command");
                string t = targetDirectory;
                if (t[t.Length - 1] == '/') t += fileToMove; // Check if the targetDirectory has an ending slash
                else t += "/" + fileToMove; // Add the ending slash
                RenameFile(fileToMove, t); // Rename the file to move it
            }

            /// <summary>
            /// Rename a file in the current directory
            /// </summary>
            /// <param name="fileToRename">The name of the file to rename (with extension)</param>
            /// <param name="newName">The new name of the file (with extension)</param>
            public void RenameFile(string fileToRename, string newName)
            {
                Debug($"[REN]: Initiating rename");
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "RNFR", arguments = fileToRename }); // Send the command to the server
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.ActionPending) throw new System.Security.SecurityException("Failed to init rename function"); // Throw if unexpected status code
                Debug($"[REN]: File to rename, located, so far so good");
                tid = ExecuteCommand(new Utils.FTP.Request() { command = "RNTO", arguments = newName }); // Send the command to the server
                resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.ActionSuccess) throw new System.Security.SecurityException("Failed to rename the file"); // Throw if unexpected status code
                Debug($"[REN]: File renamed");
            }

            /// <summary>
            /// Remove a directory from the remote server
            /// </summary>
            /// <param name="directoryName">The name of the directory to remove</param>
            public void DeleteDirectory(string directoryName)
            {
                Debug($"[DDEL]: Initiating directory delete");
                string pwd = Pwd(); // Get the current working directory
                string fullPath = pwd + "/" + directoryName; // Get the full path of the specified directory
                Debug($"[DDEL]: Got full directory path to delete => {fullPath}");
                Dictionary<string, string> rmfList = BuildRemoveFileList(fullPath); // Build the list of files to remove from the directory
                Debug($"[DDEL]: File remove list constructed");
                RemoveFileList(rmfList); // Remove the files from the directory and the directory itself
                Debug($"[DDEL]: File remove list executed without errors");
            }

            /// <summary>
            /// Go to the parent directory
            /// </summary>
            public void CdUp()
            {
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "CDUP", arguments = "" }); // Send the command to the server
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.ActionSuccess) throw new System.Security.SecurityException("Failed to create new directory"); // Throw if unexpected status code
                Debug($"[UP1]: Changed the current directory to the parent directory");
            }

            /// <summary>
            /// Remove files based on the special list format
            /// </summary>
            /// <param name="list">The list of files to remove</param>
            private void RemoveFileList(Dictionary<string, string> list)
            {
                Debug($"[RFL]: Begin remove file list execution");
                foreach (KeyValuePair<string, string> kvp in list) // Go through the files
                {
                    string[] files = kvp.Value.Split('/'); // Get each file one by one
                    if (files.Length == 0) continue; // Check if there are files in this folder
                    Cwd(kvp.Key); // Go to the target folder
                    foreach (string file in files) // Go through the folder's files
                    {
                        if (file == "") continue; // Skip non existent files
                        DeleteFile(file); // Remove the file from the folder
                    }

                    CdUp(); // Go to the parent folder
                    RemoveDirectory(kvp.Key); // Remove the folder
                }
                Debug($"[RFL]: End of remove file list execution");
            }

            /// <summary>
            /// Create a file remove list from a parent directory
            /// </summary>
            /// <param name="parentDir">The full path of the parent directory</param>
            /// <returns>The file list to remove</returns>
            private Dictionary<string, string> BuildRemoveFileList(string parentDir)
            {
                Debug($"[RFL]: Building remove file list of {parentDir}");
                if (parentDir == "/") throw new AccessViolationException("You can't delete the ftp root folder!"); // Check if someone's nasty
                Dictionary<string, string> result = new Dictionary<string, string>(); // Define the result
                Utils.FTP.File[] subFiles = ListDirectory(parentDir); // Get the files in the parent directory
                string files = ""; // Define the files concat string

                foreach (Utils.FTP.File file in subFiles) // Loop through the files
                {
                    if (file.isDirectory) // Directory
                    {
                        Dictionary<string, string> subResult = BuildRemoveFileList(parentDir + "/" + file.fileName); // Get the directory's files
                        result.Merge(subResult); // Merge the result with the result dictionary
                    }
                    else files += file.fileName + "/"; // Append the file to the files list
                }

                if (files.Length > 0) files = files.RemoveLast(1); // Remove the ending slash
                result.Add(parentDir, files); // Add the files to the result
                Debug($"[RFL]: Remove file list for {parentDir} built");

                return result; // Return the result
            }

            /// <summary>
            /// Remove a directory from the ftp server
            /// </summary>
            /// <param name="directoryName">The name of the directory to remove</param>
            private void RemoveDirectory(string directoryName)
            {
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "RMD", arguments = directoryName }); // Send the command to the server
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.ActionSuccess) throw new System.Security.SecurityException("Failed to create new directory"); // Throw if unexpected status code
                Debug($"[RMD]: Directory {directoryName} removed");
            }

            /// <summary>
            /// Remove a file from the current folder
            /// </summary>
            /// <param name="fileName">The name of the file to remove</param>
            public void DeleteFile(string fileName)
            {
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "DELE", arguments = fileName }); // Send the command to the server
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.ActionSuccess) throw new System.Security.SecurityException("Failed to create new directory"); // Throw if unexpected status code
                Debug($"[FDEL]: File {fileName} deleted");
            }

            /// <summary>
            /// Create a new directory in the current folder
            /// </summary>
            /// <param name="directoryName">The name of the new directory</param>
            public void CreateNewDirectory(string directoryName)
            {
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "MKD", arguments = directoryName }); // Send the command to the server
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.PathnameCreated) throw new System.Security.SecurityException("Failed to create new directory"); // Throw if unexpected status code
                Debug($"[MKD]: Directory {directoryName} created");
            }

            /// <summary>
            /// Secure the data channel of a TLS session
            /// </summary>
            private void SetupSecureDataChannel()
            {
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "PBSZ", arguments = "0" }); // Send the command to the server
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.Success) throw new System.Security.SecurityException("Failed to set the protection buffer size"); // Throw if unexpected status code
                tid = ExecuteCommand(new Utils.FTP.Request() { command = "PROT", arguments = "P" }); // Send the command to the server
                resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.Success) throw new System.Security.SecurityException("Failed to set the protection level of the data channel"); // Throw if unexpected status code
                Debug("[TLS]: Data channel secured");
            }

            /// <summary>
            /// Output ftp debug messages
            /// </summary>
            /// <param name="msg">The message to print to stdout</param>
            private void Debug(string msg)
            {
                if (DebugMode) Console.WriteLine(msg); // if debugging enabled write out the message
            }

            /// <summary>
            /// Upgrade the current connection to TLS
            /// </summary>
            private void BeginTLS()
            {
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "AUTH", arguments = "TLS" }); // Send the command to the server
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.AuthContinue) throw new System.Security.SecurityException("Server failed to init the TLS connection"); // Throw if unexpected status code
                System.Net.Security.SslStream sslStream = new System.Net.Security.SslStream(netClient.GetRawStream()); // Create the ssl stream from the client
                sslStream.AuthenticateAsClient(serverHostname); // Authenticate to the server
                SSLClient secureClient = new SSLClient(sslStream)
                {
                    // Set the new line endings
                    ReadNewLine = "\r\n",
                    WriteNewLine = "\r\n"
                };
                // Set the sockets
                write = secureClient as INetworkWriter;
                read = secureClient as INetworkReader;
                socket = secureClient as INetworkSocket;
                netClient = secureClient as INetworkClient;
                Debug("[TLS]: Connection established");
                SetupSecureDataChannel(); // Setup the secure data channel
            }

            /// <summary>
            /// Connect to the data channel on the FTP server
            /// <param name="result">The result of the getDataPort function</param>
            /// <param name="useSSL">True to create and secure the socket automatically, otherwise false</param>
            /// </summary>
            private (INetworkReader _read, INetworkWriter _write, INetworkClient _client) ConnectDataPort((string ip, int port) result, bool useSSL)
            {
                Debug("[PASV Handler]: Initiating PASV connection");
                // Define the network sockets
                INetworkReader _read = null;
                INetworkWriter _write = null;
                INetworkClient _client = null;

                if (!useSSL)
                {
                    TcpClient ftpData = new TcpClient(Utils.IPv4.CreateEndPoint(result.ip, result.port)); // Create a new tcp client
                    ftpData.Start(); // Connect to the server
                    dataSocket = ftpData as INetworkSocket; // Get the data channel socket
                    _read = ftpData as INetworkReader; // Get the read socket from the client
                    _write = ftpData as INetworkWriter; // Get the write socket from the client
                    _client = ftpData as INetworkClient; // Get the client interface
                }
                else
                {
                    SSLClient ftpData = new SSLClient(Utils.IPv4.CreateEndPoint(result.ip, result.port), !checkSSL); // Create a new ssl client
                    ftpData.SetServerDestination(serverHostname);
                    ftpData.Start(); // Connect to the server
                    dataSocket = ftpData as INetworkSocket; // Get the data channel socket
                    _read = ftpData as INetworkReader; // Get the read socket from the client
                    _write = ftpData as INetworkWriter; // Get the write socket from the client
                    _client = ftpData as INetworkClient; // Get the client interface
                    Debug("TLS client connected to the data channel");
                }

                Debug("[PASV Handler]: PASV channel connected");

                return (_read, _write, _client); // Return the result
            }

            /// <summary>
            /// Get the IP and port to connect to on a passive connection
            /// </summary>
            /// <returns>The IP and port to connect to</returns>
            private (string ip, int port) GetDataPort()
            {
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "PASV", arguments = "" }); // Send the command to the server
                Utils.FTP.Response resp = GetResponse(tid); // Wait for response
                if (resp.statusCode != Utils.FTP.StatusCode.EnterPasvMode) throw new Exception("Failed to start entering to passive mode"); // Throw an exception if unexpected status code
                Debug("[PASV Handler]: Got PASV destination");
                return Utils.FTP.GetPasvDestination(resp.message); // Return the parsed data
            }

            /// <summary>
            /// Set the transfer type
            /// </summary>
            /// <param name="type">Set to "I" for binary mode and set to "A" for ASCII mode</param>
            private void SetType(string type = "I")
            {
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "TYPE", arguments = type }); // Send the command to the server
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.Success) throw new Exception("Failed to set the transfer type"); // Throw if unexpected status code
                Debug($"[TYP]: Transfer type changed to: {type}");
            }

            /// <summary>
            /// List a directory's children
            /// </summary>
            /// <param name="directory">The directory to list the children of (full path required, leave empty to list current directory)</param>
            /// <returns>A list of files inside the directory</returns>
            public Utils.FTP.File[] ListDirectory(string directory)
            {
                Debug($"[FL]: Begin list directory");
                if (directory != "") Cwd(directory);
                Debug($"[FL]: Switched to target directory");
                ValueTuple<string, int> dataPort = GetDataPort();
                byte[] pasvBuffer = new byte[0];
                if (!ssl)
                {
                    var (_read, _, _) = ConnectDataPort(dataPort, ssl);
                    _read.AddEventDataReceived((sender, ea) => // When new data received
                    {
                        byte[] data = ea.Bytes;
                        int newLength = pasvBuffer.Length + data.Length; // Get the new length of the buffer
                        byte[] tmp = new byte[pasvBuffer.Length]; // Init a temporary save buffer
                        Array.Copy(pasvBuffer, tmp, pasvBuffer.Length); // Save the current buffer
                        pasvBuffer = new byte[newLength]; // Create a new buffer
                        Array.Copy(tmp, pasvBuffer, tmp.Length); // Copy the previous buffer
                        Array.ConstrainedCopy(data, 0, pasvBuffer, tmp.Length, data.Length); // Copy the received data
                    });
                    Debug($"[FL]: Read setup without TLS: done");
                }

                byte[] _tmp = ListFiles(dataPort); // Send the file list command (blocking call until file listing finishes)
                if (!ssl) dataSocket.ForceStop(); // Kill the data channel
                else pasvBuffer = _tmp;
                string result = socket.RecvEncoder.GetString(pasvBuffer); // Get the string result of the data buffer
                Debug($"[FL]: File list transfered, parsing the results");
                return Utils.FTP.ParseFileList(result); // Return the file list
            }

            /// <summary>
            /// Send the file list command to the server
            /// </summary>
            private byte[] ListFiles((string ip, int port) dPort)
            {
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "LIST", arguments = "" }); // Send the command to the server
                byte[] pasvBuffer = new byte[0]; // Define the result buffer
                if (ssl)
                {
                    var (_read, _, _) = ConnectDataPort(dPort, ssl); // Connect to the data port
                    _read.AddEventDataReceived((sender, ea) => // When new data received
                    {
                        byte[] data = ea.Bytes;
                        int newLength = pasvBuffer.Length + data.Length; // Get the new length of the buffer
                        byte[] tmp = new byte[pasvBuffer.Length]; // Init a temporary save buffer
                        Array.Copy(pasvBuffer, tmp, pasvBuffer.Length); // Save the current buffer
                        pasvBuffer = new byte[newLength]; // Create a new buffer
                        Array.Copy(tmp, pasvBuffer, tmp.Length); // Copy the previous buffer
                        Array.ConstrainedCopy(data, 0, pasvBuffer, tmp.Length, data.Length); // Copy the received data
                        dataSocket.ForceStop();
                    });
                    Debug($"[FL]: Read setup with TLS: done");
                }

                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.DataChannelReady) throw new Exception("Failed to start the listing of the requested directory"); // Throw if unexpected status code
                tid = lc.PacketSent();
                resp = GetResponse(tid); // Get the next response
                if (resp.statusCode != Utils.FTP.StatusCode.CloseFtpDataSuccess) throw new Exception("Failed to get the file list of the directory"); // Throw if unexpected status code
                Debug($"[FL]: Transfer OK");
                return pasvBuffer; // Return the result
            }

            /// <summary>
            /// Get a response from the FTP server
            /// </summary>
            /// <param name="latencyTracker">The latency tracker returned from the execute command call</param>
            /// <returns>The current response from the FTP server</returns>
            private Utils.FTP.Response GetResponse(int latencyTracker)
            {
                string result = read.ReadLineAsync().Result; // Get the next line from the server
                Utils.FTP.Response r = result.ToResponse(); // Parse the response
                Debug($"[RES]: ({(int)r.statusCode}) {r.message}");
                if (r.statusCode == Utils.FTP.StatusCode.Timeout) // Check if we're timed out
                {
                    throw new TimeoutException("The server dropped the connection due to too much inactive time"); // Throw an exception if we're timed out
                }
                else if (r.statusCode == Utils.FTP.StatusCode.AccessFailed) // Check if the current action failed
                {
                    throw new InvalidOperationException("The current action failed to complete, probably unexistent file parameter or lack of permissions to complete the action"); // Throw an exception on failed actions
                }
                else if (r.statusCode == Utils.FTP.StatusCode.AccessDenied) // Check if the current action resulted in access denied
                {
                    throw new AccessViolationException("The current action resulted in denial of access, probably insufficient file permissions"); // Throw an exception on access denials
                }
                else if (r.statusCode == Utils.FTP.StatusCode.LoginFailure)
                {
                    throw new AccessViolationException($"General login failure, here's the message of the server: {r.message}"); // Throw an exception on access denials
                }
                if (latencyTracker != -1) lc.PacketReceived(latencyTracker);
                return r; // Return the response of the server
            }

            /// <summary>
            /// Get the message of the server after connecting
            /// </summary>
            private void GetServer()
            {
                Utils.FTP.Response r = GetResponse(-1); // Get the response of the server
                if (r.statusCode == Utils.FTP.StatusCode.ReadyNewUser) // Check th status code
                {
                    ServerInformation = r.message; // Set the message of the server
                    Debug($"[MOTD]: {ServerInformation}");
                }
            }

            /// <summary>
            /// Change the current working directory
            /// </summary>
            /// <param name="targetDirectory">The directory to change to</param>
            public void Cwd(string targetDirectory)
            {
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "CWD", arguments = targetDirectory }); // Send the command to the server
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.ActionSuccess) throw new Exception("Failed to change the working directory"); // Check if the status code is positive
                Debug($"[WD]: Changed to {targetDirectory}");
            }

            /// <summary>
            /// Get the current working directory
            /// </summary>
            /// <returns>The path of the current working directory</returns>
            public string Pwd()
            {
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "PWD", arguments = "" }); // Send the command to the server
                Utils.FTP.Response resp = GetResponse(tid); // Get the response
                if (resp.statusCode != Utils.FTP.StatusCode.PathnameCreated) throw new Exception("Failed to get the current working directory"); // Check if the status code is positive
                // Format and return the current path
                int start = resp.message.IndexOf('\"') + 1; // Get the start of the substring
                int end = resp.message.LastIndexOf('\"'); // Get the end of the substring
                string wdir = resp.message.Substring(start, resp.message.Length - start - (resp.message.Length - end)); // Get the ip and the port string from the response
                Debug($"[WD]: Currently: {wdir}");
                return wdir; // Return the current working directory
            }

            /// <summary>
            /// Login to the FTP server
            /// </summary>
            /// <param name="user">The username to login with</param>
            /// <param name="pass">The password to login with</param>
            /// <returns>True if logged in, otherwise false</returns>
            private bool Login(string user, string pass)
            {
#if Validation_hard
                if (socket == null) throw new InvalidOperationException("Tried to login without connecting to the server"); // Check if the socket is alive
#endif
                int tid = ExecuteCommand(new Utils.FTP.Request() { command = "USER", arguments = user }); // Send the command to the server
                Debug($"[AUTH]: Username sent to server");
                Utils.FTP.Response resp = GetResponse(tid); // Get the response from the server
                if (resp.statusCode == Utils.FTP.StatusCode.SendPassword) // Check if we can send the username
                {
                    Debug($"[AUTH]: Allowed to send password");
                    tid = ExecuteCommand(new Utils.FTP.Request() { command = "PASS", arguments = pass }); // Send the command to the server
                    Debug($"[AUTH]: Password sent to server");
                    resp = GetResponse(tid); // Get the response
                    return resp.statusCode == Utils.FTP.StatusCode.LoginSuccess; // Check if the login was successful
                }

                Debug($"[AUTH]: Couldn't send password after username");
                return false; // Failed to login
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
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider(); // Get the rng service
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
        /// Counts the latency of the connection
        /// </summary>
        public class LatencyCounter
        {
            /// <summary>
            /// A list which containes the trackers and it's associated timer objects
            /// </summary>
            private List<(int tracker, System.Diagnostics.Stopwatch timer)> counterList = new List<(int, System.Diagnostics.Stopwatch)>();
            /// <summary>
            /// An integer incrementer to track the packets
            /// </summary>
            private int tracker;
            /// <summary>
            /// The average time between sending a message and a message arriving
            /// </summary>
            public long AverageLatency { get; private set; }
            /// <summary>
            /// The minimum time between sending a message and a message arriving
            /// </summary>
            public long MinimumLatency { get; private set; }
            /// <summary>
            /// The maximum time between sending a message and a message arriving
            /// </summary>
            public long MaximumLatency { get; private set; }
            /// <summary>
            /// The number of latency checks
            /// </summary>
            public int NumberOfSamples { get; private set; }

            /// <summary>
            /// Get the latency information of the network in a string format
            /// </summary>
            /// <returns>The string value of the object</returns>
            public override string ToString()
            {
                return $"Average Latency: {AverageLatency} ms\r\nMinimum Latency: {MinimumLatency} ms\r\nMaximum latency: {MaximumLatency} ms\r\nNumber of packets: {NumberOfSamples}"; // Return the string format of the object
            }

            /// <summary>
            /// Create a new latency counter object
            /// </summary>
            public LatencyCounter()
            {
                // Init the variables
                AverageLatency = 0;
                MinimumLatency = 0;
                MaximumLatency = 0;
                tracker = 0;
            }

            /// <summary>
            /// Start the timer and get it's associated ID
            /// </summary>
            /// <returns>The ID of the started time, which you can track the packet with</returns>
            public int PacketSent()
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch(); // Create a new timer
                sw.Start(); // Start the timer
                int current = tracker; // Copy the tracker ID
                counterList.Add((current, sw)); // Add the timer with the tracker to the timer list
                tracker++; // Increment the tracker ID
                return current; // Return the current tracker ID
            }

            /// <summary>
            /// Stop the associated timer and calculate the results
            /// </summary>
            /// <param name="trackerID">The ID returned from the PacketSent call</param>
            public void PacketReceived(int trackerID)
            {
                var (_, timer) = counterList.Find((item) => item.tracker == trackerID); // Get the timer based on the tracker ID
                timer.Stop(); // Stop the timer
                NumberOfSamples++; // Increment the number of samples
                long total = AverageLatency + timer.ElapsedMilliseconds; // Get the total elapsed milliseconds
                AverageLatency = total / NumberOfSamples; // Calculate the average elapsed milliseconds
                if (MinimumLatency == 0 || MinimumLatency > timer.ElapsedMilliseconds) MinimumLatency = timer.ElapsedMilliseconds; // Calculate the min latency
                if (MaximumLatency == 0 || MaximumLatency < timer.ElapsedMilliseconds) MaximumLatency = timer.ElapsedMilliseconds; // Calculate the max latency
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
                if (!File.Exists(filePath) || (new FileInfo(filePath)).Length == 0) throw new ArgumentException($"The specified file is not valid: {filePath}"); // Check if the file exists and isn't empty
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
            public static (bool isValid, IPAddress ipAddress) IsValidDestination(string destination)
            {
                ValueTuple<bool, IPAddress> ipCheck = IsValidAddress(destination); // Check if the IP is valid
                if (ipCheck.Item1) return ipCheck; // If IP is valid return the IP and true
                else return Dns.IsValidHostname(destination); // Try DNS resolving
            }

            /// <summary>
            /// Check if and IP Address is valid
            /// </summary>
            /// <param name="ipAddress">The IP Address to check</param>
            /// <returns>A boolean indicating if the ip is valid and the IP Address</returns>
            public static (bool isValid, IPAddress ipAddress) IsValidAddress(string ipAddress)
            {
                return (IPAddress.TryParse(ipAddress, out IPAddress address), address); // Return the results of the check and the IP Address
            }

            /// <summary>
            /// Create an endpoint to connect/bind to
            /// </summary>
            /// <param name="destination">The hostname or address of the endpoint</param>
            /// <param name="portNumber">The port number of the endpoint</param>
            /// <returns>A valid endpoint generated from the specified data</returns>
            public static IPEndPoint CreateEndPoint(string destination, int portNumber)
            {
                if (!IsPortValid(portNumber)) throw new ArgumentException("Port number outside of the valid port number range"); // Check if the port number is valid
                var (isValid, ip) = IsValidDestination(destination); // Check if the destination is valid
                if (!isValid) throw new ArgumentException("The specified destination is invalid!"); // Destination invalid
                return new IPEndPoint(ip, portNumber); // Return IPEndPoint
            }
        }

        /// <summary>
        /// FTP Utils
        /// </summary>
        public class FTP
        {
            /// <summary>
            /// An FTP Control Request
            /// </summary>
            public struct Request
            {
                /// <summary>
                /// The command
                /// </summary>
                public string command;
                /// <summary>
                /// The command's arguments
                /// </summary>
                public string arguments;
                /// <summary>
                /// Convert the request to a string
                /// </summary>
                /// <returns>The string value of the request</returns>
                public override string ToString()
                {
                    return $"{command} {arguments}"; // Return the request
                }
            }

            /// <summary>
            /// An FTP Control Response
            /// </summary>
            public struct Response
            {
                /// <summary>
                /// The status code of the action
                /// </summary>
                public StatusCode statusCode;
                /// <summary>
                /// The message of the response
                /// </summary>
                public string message;
            }

            /// <summary>
            /// FTP File list information
            /// </summary>
            public struct File
            {
                /// <summary>
                /// The name of the file
                /// </summary>
                public string fileName;
                /// <summary>
                /// The size of the file in bytes
                /// </summary>
                public long fileSize;
                /// <summary>
                /// The permissions of the file
                /// </summary>
                public string filePermissions;
                /// <summary>
                /// True if the file is a directory
                /// </summary>
                public bool isDirectory;
                /// <summary>
                /// The number of children of a directory
                /// </summary>
                public int numberOfChildren;
                /// <summary>
                /// The user-id who owns the file
                /// </summary>
                public int uid;
                /// <summary>
                /// The group-id who owns the file
                /// </summary>
                public int gid;
                /// <summary>
                /// The time the file was last modified
                /// </summary>
                public DateTime lastModifiedTime;
                /// <summary>
                /// Represent the file's data in with a string value
                /// </summary>
                /// <returns>The string value of the file</returns>
                public override string ToString()
                {
                    return $"P: {filePermissions} NoC: {numberOfChildren} UID: {uid} GID: {gid} Size: {fileSize} Last Modification: {lastModifiedTime.ToShortDateString()} Name: {fileName} Directory: {isDirectory}"; // Return the appended result
                }
            }

            /// <summary>
            /// Response status codes from the server
            /// </summary>
            public enum StatusCode : int
            {
                /// <summary>
                /// The server is ready to serve a new user
                /// </summary>
                ReadyNewUser = 220,
                /// <summary>
                /// A control action is successful
                /// </summary>
                Success = 200,
                /// <summary>
                /// The server wants the client to send the password
                /// </summary>
                SendPassword = 331,
                /// <summary>
                /// The username and password were correct
                /// </summary>
                LoginSuccess = 230,
                /// <summary>
                /// You didn't do anything for a while an the server dropped the connection
                /// </summary>
                Timeout = 421,
                /// <summary>
                /// The server successfully created the pathname variable
                /// </summary>
                PathnameCreated = 257,
                /// <summary>
                /// A file transfer action succeeded
                /// </summary>
                ActionSuccess = 250,
                /// <summary>
                /// You're about to enter passivr mode
                /// </summary>
                EnterPasvMode = 227,
                /// <summary>
                /// You're about to receive data on the data channel
                /// </summary>
                DataChannelReady = 150,
                /// <summary>
                /// Closing the data channel with successful transfer
                /// </summary>
                CloseFtpDataSuccess = 226,
                /// <summary>
                /// Authentication accepted, continue with authenticating
                /// </summary>
                AuthContinue = 234,
                /// <summary>
                /// Error, data channel insecure data
                /// </summary>
                InsecureDataChannel = 522,
                /// <summary>
                /// Failed to complete an action because we failed to access a file
                /// </summary>
                AccessFailed = 550,
                /// <summary>
                /// A requested action is pending, needs more data
                /// </summary>
                ActionPending = 350,
                /// <summary>
                /// Error, general access denied
                /// </summary>
                AccessDenied = 533,
                /// <summary>
                /// Server closes the control channel
                /// </summary>
                ClosingControlConnection = 221,
                /// <summary>
                /// General login failure
                /// </summary>
                LoginFailure = 530
            }

            /// <summary>
            /// Get the ip and port to connect to from a string response
            /// </summary>
            /// <param name="response">The response to get the data from</param>
            /// <returns>The ip and the port to connect to</returns>
            public static (string ip, int port) GetPasvDestination(string response)
            {
                int start = response.IndexOf('(') + 1; // Get the start of the substring
                int end = response.IndexOf(')'); // Get the end of the substring
                string details = response.Substring(start, response.Length - start - (response.Length - end)); // Get the ip and the port string from the response
                string ip = ""; // Define IP
                int port = 0; // Define Port
                string[] dataParts = details.Split(','); // Split the data into numbers
                for (int i = 0; i < 4; i++) // Construct the IP Address
                {
                    ip += dataParts[i] + "."; // Append the part to the IP
                }
                ip = ip.RemoveLast(1); // Remove the trailing dot
                int p1 = dataParts[4].ToInt(); // Convert the port1 to int
                int p2 = dataParts[5].ToInt(); // Convert the port2 to int
                port = p1 * 256 + p2; // Calculate the destination port
                return (ip, port); // Return the connection data
            }

            /// <summary>
            /// Generate a file list based on the response from the LIST command
            /// </summary>
            /// <param name="result">The result of the LIST command</param>
            /// <returns>The parsed file list</returns>
            public static File[] ParseFileList(string result)
            {
                string[] files = result.Split(new string[] { "\r\n" }, StringSplitOptions.None); // Get the files one-by-one
                List<File> finalResult = new List<File>(); // Define the result list

                foreach (string file in files) // Loop through the files
                {
                    if (file == "") continue; // Skip empty files
                    string fileName = ""; // The name of the file
                    string permissions = ""; // File permissions
                    string numberOfChildren = ""; // Number of children
                    string uid = ""; // User id
                    string gid = ""; // Group id
                    string date = ""; // Last modify date
                    string size = ""; // Total size in bytes
                    bool pDone = false; // Indicates if the permissions are parsed
                    bool cDone = false; // Indicates if the number of children is parsed
                    bool uDone = false; // Indicates if the user id is parsed
                    bool gDone = false; // Indicates if the group id is parsed
                    bool sDone = false; // Indicates if the size of the file is parsed
                    bool dDone = false; // Indicates if the last modify date is parsed
                    int prevDateLength = 0; // The length of the date string in the previous loop
                    int dateParts = 0; // Indicates the number of date-time data parts parsed

                    for (int i = 0; i < file.Length; i++) // Go through the characters
                    {
                        char current = file[i]; // Get the current character
                        if (!pDone) // Permissions not parsed
                        {
                            if (char.IsWhiteSpace(current)) // Check if the current char is a whitespace
                            {
                                pDone = true; // Permissions parsed
                                continue; // Skip the whitespace
                            }
                            permissions += current; // Add the current char to the permission string
                        }

                        if (pDone && !cDone) // Permissions parsed, number of children not
                        {
                            if (char.IsWhiteSpace(current)) // Check if the current char is a whitespace
                            {
                                if (numberOfChildren.Length > 0) cDone = true; // Number of children parsed if data is present
                                continue; // Skip the whitespace
                            }
                            numberOfChildren += current; // Add the current char to the number of children string
                        }

                        if (cDone && !uDone) // Number of children parsed, user id not
                        {
                            if (char.IsWhiteSpace(current)) // Check if the current char is a whitespace
                            {
                                if (uid.Length > 0) uDone = true; // User id parsed if data is present
                                continue; // Skip the whitespace
                            }
                            uid += current; // Add the current char to the user id string
                        }

                        if (uDone && !gDone) // User id parsed, group id not
                        {
                            if (char.IsWhiteSpace(current)) // Check if the current char is a whitespace
                            {
                                if (gid.Length > 0) gDone = true; // Group id parsed if data is present
                                continue; // Skip the whitespace
                            }
                            gid += current; // Add the current char to the group id string
                        }

                        if (gDone && !sDone) // Group id parsed, file size not
                        {
                            if (char.IsWhiteSpace(current)) // Check if the current char is a whitespace
                            {
                                if (size.Length > 0) sDone = true; // file size parsed if data is present
                                continue; // Skip the whitespace
                            }
                            size += current; // Add the current char to the file size string
                        }

                        if (sDone && !dDone) // File size parsed, date not
                        {
                            if (char.IsWhiteSpace(current)) // Check if the current char is a whitespace
                            {
                                if (date.Length != prevDateLength) // Check if the string is modified after the last whitespace encounter
                                {
                                    dateParts++; // We have another date part
                                    prevDateLength = date.Length; // Set the length to the current length
                                }

                                if (dateParts == 3) dDone = true; // If we have 3 parts then we're done
                                continue; // Skip the whitespace
                            }
                            date += current; // Add the current char to the date string
                        }

                        if (dDone) // If the date if parsed
                        {
                            fileName += current; // Add the current char to the file string
                        }
                    }
                    File f = new File() // Construct the file object
                    {
                        fileName = fileName, // Set the file name
                        filePermissions = permissions, // Set the file permissions
                        fileSize = size.ToLong(), // Set the file size
                        uid = uid.ToInt(), // Set the user id
                        gid = gid.ToInt(), // Set the group id
                        isDirectory = permissions.StartsWith("d"), // Determine if file is a directory
                        numberOfChildren = numberOfChildren.ToInt(), // Set the number of children
                        lastModifiedTime = ParseFileModifyTime(date) // Set the last modified time
                    };
                    finalResult.Add(f); // Add the file to the list
                }

                return finalResult.ToArray(); // Retrun an array of files
            }

            /// <summary>
            /// Get the last modify time from an ftp LIST date
            /// </summary>
            /// <param name="timeStr">The date string to parse</param>
            /// <returns>A date time object from the available data</returns>
            public static DateTime ParseFileModifyTime(string timeStr)
            {
                string month = timeStr.Substring(0, 3); // Get the month
                string day = timeStr.Substring(3, 2); // Get the day
                string year = ""; // Define the year
                string hours = "00"; // Define the hours
                string minutes = "00"; // Define the minutes
                string rest = timeStr.Substring(5); // Get the rest of the string, without the month, and day
                if (rest.Contains(":")) // Check if the rest of the string is a time
                {
                    year = DateTime.Now.Year.ToString(); // Set the year to the current year
                    string[] time = rest.Split(':'); // Split the time string
                    hours = time[0]; // Set the hours
                    minutes = time[1]; // Set the minutes
                }
                else year = rest; // Set the year to the rest of the string

                return new DateTime(year.ToInt(), month.ToMonth(), day.ToInt(), hours.ToInt(), minutes.ToInt(), 0); // Return the datetime object
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
            public static (bool isValid, IPAddress ipAddress) IsValidHostname(string hostname)
            {
                try
                {
                    IPAddress[] result = System.Net.Dns.GetHostAddresses(hostname); // Resolve the hostname
                    return (true, result[0]); // Return the IP Address of the hostname and true
                }
                catch (ArgumentException ex) // The hostname isn't valid
                {
#if Logging_verbose
                    Console.WriteLine("Hostname resolvation failed, due to the following exception:");
                    Console.WriteLine(ex);
#endif
                    return (false, null); // Return false and null for the IP Address
                }
            }
        }
    }
}