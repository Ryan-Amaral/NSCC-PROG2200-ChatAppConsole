using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace ChatLib
{

    public class Client : IMessageable
    {
        private TcpClient client; // the actual client in this wrapper class
        private NetworkStream stream; // the stream of data

        // const ip address and port to change easier later
        private const String ipAddress = "127.0.0.1";
        private const int port = 13000;

        // tells whether the client is connected
        private bool isConnected;
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }

            set
            {
                isConnected = value;
            }
        }

        private List<string> messages;
        public List<string> Messages
        {
            get
            {
                List<String> messagesCopy = new List<string>(messages); // copy messages
                messages.Clear(); // clear the messages
                return messagesCopy; // send the copy still with the messages
            }

        }

        public void Connect(String ipAddress = "127.0.0.1", int port = 13000)
        {
            try
            {
                client = new TcpClient(ipAddress, port);
                stream = client.GetStream();
                IsConnected = true;
            }
            catch (ArgumentNullException e)
            {
                IsConnected = false;
            }
            catch (SocketException e)
            {
                IsConnected = false;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Client()
        {
            messages = new List<string>(); // initialize messages list
        }

        /// <summary>
        /// Sends a message to the server, returns true on success.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool WriteMessage(string message)
        {
            try
            {
                //string dMessage = message + ";"; // add delimeter on to message
                // convert the message to bytes to send accross network
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);

                return true;
            }
            catch (ArgumentNullException e)
            {
                return false;
            }
            catch (SocketException e)
            {
                return false;
            }
            catch (IOException e)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the NetworkStream of the client.
        /// </summary>
        /// <returns></returns>
        public NetworkStream GetStream()
        {
            return stream;
        }

        /// <summary>
        /// Tells the client to check for new messages.
        /// </summary>
        public void CheckForMessages()
        {
            int numBytes; // the amount of bytes that the current method actually is
            Byte[] bytes = new Byte[256]; // create a 256 byte buffer to receive data
            try
            {
                // read message if there is one
                if (stream.DataAvailable)
                {
                    // get how many bytes long the message is
                    numBytes = stream.Read(bytes, 0, bytes.Length);
                    // split at delimeter ';'
                    string msg = System.Text.Encoding.ASCII.GetString(bytes, 0, numBytes);
                    // add to messages
                    messages.Add(msg);
                }
            }
            catch (ArgumentNullException e) { }
            catch (ArgumentOutOfRangeException e) { }
            catch (IOException e) { }
            catch (ObjectDisposedException e) { }
        }

        /// <summary>
        /// Disconnects the client from the server.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                WriteMessage("CLIENT QUIT THE APP!");
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e) { }
            catch (IOException e) {}
            catch (SocketException e) { }
        }
    }
}
