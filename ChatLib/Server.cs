using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace ChatLib
{
    public class Server : IMessageable
    {
        TcpListener server;
        TcpClient client; // the server needs the client object
        NetworkStream stream;

        // const ip address and port to change easier later
        private const String ipAddress = "127.0.0.1";
        private const int port = 13000;

        // tells if server is started
        private bool isStarted;
        public bool IsStarted
        {
            get
            {
                return isStarted;
            }

            set
            {
                isStarted = value;
            }
        }

        // tells whether the client is connected
        private bool hasConnection;
        public bool HasConnection
        {
            get
            {
                return hasConnection;
            }

            set
            {
                hasConnection = value;
            }
        }

        private List<string> messages;
        public List<string> Messages
        {
            get
            {
                // copy message
                List<String> messagesCopy = new List<string>();
                foreach (String message in messages)
                {
                    messagesCopy.Add(message);
                }
                messages.Clear(); // clear the messages

                return messagesCopy; // send the copy still with the messages
            }
        }

        public Server()
        {
            messages = new List<string>();
        }

        /// <summary>
        /// Starts the server and gets the stream from the client.
        /// </summary>
        /// <param name="client"></param>
        public void Start()
        {
            try {
                server = new TcpListener(IPAddress.Parse(ipAddress), port);
                server.Start();
                IsStarted = true;
            }
            catch (SocketException e)
            {
                IsStarted = false;
            }
        }

        /// <summary>
        /// Tells the server to keep listening for a client.
        /// </summary>
        public void AwaitConnection()
        {
            try {
                client = server.AcceptTcpClient();
                stream = client.GetStream();
                HasConnection = true;
            }
            catch (InvalidOperationException e)
            {

            }
            catch (SocketException e)
            {

            }
        }

        /// <summary>
        /// Returns the NetworkStream of the server.
        /// </summary>
        /// <returns></returns>
        public NetworkStream GetStream()
        {
            return stream;
        }

        /// <summary>
        /// Sends a message to the client, returns true on success.
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
        /// Tells the server to check for new messages.
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
                    messages.Add(msg);
                }
            }
            catch (ArgumentNullException e){}
            catch (ArgumentOutOfRangeException e) { }
            catch (IOException e) { }
            catch (ObjectDisposedException e) { }
        }

        /// <summary>
        /// Ends the server.
        /// </summary>
        public void Disconnect()
        {
            try {
                WriteMessage("SERVER HAS ENDED!");
                stream.Close();
                server.Stop();
            }
            catch (ArgumentNullException e) { }
            catch (IOException e) { }
            catch (SocketException e) { }
        }
    }
}
