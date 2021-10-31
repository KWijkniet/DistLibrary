using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using LibData;
using System.Text;

namespace UserHelper
{
    // Note: Do not change this class.
    public class Setting
    {
        public int ServerPortNumber { get; set; }
        public int BookHelperPortNumber { get; set; }
        public int UserHelperPortNumber { get; set; }
        public string ServerIPAddress { get; set; }
        public string BookHelperIPAddress { get; set; }
        public string UserHelperIPAddress { get; set; }
        public int ServerListeningQueue { get; set; }
    }

    // Note: Complete the implementation of this class. You can adjust the structure of this class.
    public class SequentialHelper
    {
        public string configFile = @"../../../../ClientServerConfig.json"; // for debugging
        public string authorFile = @"../../../Users.json"; // for debugging
        private Setting settings;
        private IPAddress ipAddress;
        private List<UserData> authors = new List<UserData>();

        public SequentialHelper()
        {
            //todo: implement the body. Add extra fields and methods to the class if needed

            try
            {
                //reading the json file
                string configContent = File.ReadAllText(configFile);
                this.settings = JsonSerializer.Deserialize<Setting>(configContent);
                this.ipAddress = IPAddress.Parse(settings.UserHelperIPAddress);

                string authorContent = File.ReadAllText(authorFile);
                this.authors = JsonSerializer.Deserialize<List<UserData>>(authorContent);
            }
            catch (Exception e)
            {
                //gives out error when unable to read json file
                Console.Out.WriteLine("[Client Exception] {0}", e.Message);
            }
        }

        public void start()
        {
            //todo: implement the body. Add extra fields and methods to the class if needed

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, settings.UserHelperPortNumber);

            try
            {
                //het wachten op een client voor een connectie
                socket.Bind(iPEndPoint);
                socket.Listen(settings.ServerListeningQueue);
                Console.WriteLine("\n Connection started. Awaiting clients...");

                // het opnemen van informatie dat de server binnne krijgt en uitprinten
                while (true)
                {
                    //Connect
                    Socket newsocket = socket.Accept();

                    //Handle incoming client request
                    string clientRequest = ClientRequestHandler(newsocket);
                    Console.WriteLine("Client connected with message: " + clientRequest);

                    //Find the correct book
                    UserData author = FindAuthorByName(clientRequest);

                    //Handle response to client
                    string response = JsonSerializer.Serialize(author);
                    ClientResponseHandler(newsocket, response);
                    Console.WriteLine("returned response: " + response + "\n");

                    //Check if connection should be terminated
                    if (clientRequest.Length <= 0 || clientRequest == "TERMINATE")
                    {
                        break;
                    }
                }

                Console.WriteLine("\n Closing connection...");
                socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nAn error occured: " + e.Message + "\n" + e.StackTrace, ConsoleColor.Red);
            }
        }

        //Handle incoming request from the client
        private string ClientRequestHandler(Socket socket)
        {
            byte[] incomingmsgCLIENT = new byte[1000];
            int b = socket.Receive(incomingmsgCLIENT);
            return Encoding.ASCII.GetString(incomingmsgCLIENT, 0, b);
        }

        //Handle outgoing response to the client
        private void ClientResponseHandler(Socket socket, string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message);
            socket.Send(msg);
        }

        private UserData FindAuthorByName(string name)
        {
            foreach (UserData author in authors)
            {
                if (author.Name == name)
                {
                    return author;
                }
            }
            return null;
        }
    }
}
