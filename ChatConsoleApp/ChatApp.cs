using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatLib;

namespace ChatConsoleApp
{
    class ChatApp
    {
        static void Main(string[] args)
        {
            ChatApp chap = new ChatApp();

            // no args make client
            if (args.Length == 0)
            {
                Client client = new Client();
                chap.ClientLoop(client);
            }
            // one arg makes server if says '-server'
            else if(args.Length == 1)
            {
                // check for '-server' in here to avoid null reference
                // allow some tollerance for typing of server
                if (args[0].Equals("-server") || args[0].Equals("server") 
                    || args[0].Equals("-Server") || args[0].Equals("Server"))
                {
                    Server server = new Server();
                    chap.ServerLoop(server);
                }
                // not server so end app
                else
                {
                    Console.WriteLine("To create a server instance, use the option '-server'.");
                }
            }
            // else stop immediately
            else
            {
                Console.WriteLine("There can only be zero or one argument.");
            }
        }

        /// <summary>
        /// Loop through the things that the client does.
        /// </summary>
        void ClientLoop(Client client)
        {
            // try to connect
            Console.WriteLine("Trying to connect to server on port 13000...");
            client.Connect(); //connect client to server

            // if connect go through loop
            if (client.IsConnected)
            {
                Console.WriteLine("Connected to server!");
                Console.WriteLine();

                RunChat(client);
                
            }
            // else end program
            else {
                Console.WriteLine("No server found. Ending.");
            }
        }

        /// <summary>
        /// Loop through the things that the server has to do
        /// </summary>
        void ServerLoop(Server server)
        {
            server.Start(); // start the server
            if (server.IsStarted == true)
            {
                Console.WriteLine("Server started on port 13000.");
                Console.WriteLine("Waiting for client connection...");
                server.AwaitConnection(); // await connection from client
                Console.WriteLine("Client connected!");
                Console.WriteLine();

                RunChat(server);
            }
        }


        void RunChat(IMessageable messenger)
        {
            string input; // the input to use in input mode
            List<string> messages;// store messages from client
            while (true)
            {
                // show any newly recieved messages from client
                messenger.CheckForMessages();
                messages = messenger.Messages;
                if (messages.Count > 0)
                {
                    foreach (string message in messages)
                    {
                        Console.WriteLine(message);
                    }
                }

                // keep looping until I is pressed
                // learned from: http://stackoverflow.com/questions/5891538/listen-for-key-press-in-net-console-app
                do
                {
                    // keep in this loop while there is no key pressed
                    while (!Console.KeyAvailable)
                    {
                        // update the text from client if there is any new
                        messenger.CheckForMessages();
                        messages = messenger.Messages;
                        if (messages.Count > 0)
                        {
                            foreach (string message in messages)
                            {
                                Console.WriteLine(message);
                            }
                        }
                    }
                }
                // stop when I is pressed
                while (Console.ReadKey(true).Key != ConsoleKey.I);

                // I was pressed now await user input
                Console.Write(">> ");
                input = Console.ReadLine();

                // quit if input = 'quit'
                if (input.Equals("quit"))
                {
                    // do stuff to quit
                    messenger.Disconnect();
                    return;
                }
                // send message if input is not blank
                else if (!input.Equals(""))
                {
                    messenger.WriteMessage(input);
                }
            }

        }
    }
}
