﻿using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using LibData;


namespace LibServer
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
    public class SequentialServer
    {
        // all the required settings are provided in this file
        //public string configFile = @"../ClientServerConfig.json";
        public string configFile = @"../../../../ClientServerConfig.json"; // for debugging

        private Setting settings;
        private IPAddress ipAddress;

        public SequentialServer()
        {
            //todo: implement the body. Add extra fields and methods to the class if it is needed
            
            // read JSON directly from a file
            try
            {
                string configContent = File.ReadAllText(configFile);
                this.settings = JsonSerializer.Deserialize<Setting>(configContent);
                this.ipAddress = IPAddress.Parse(settings.ServerIPAddress);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("[Client Exception] {0}", e.Message);
            }
        }

        public void start()
        {
            //todo: implement the body. Add extra fields and methods to the class if it is needed

            //voorbereiding connectie
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, settings.ServerPortNumber);

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

                    //hier komt connectie naar bookserver
                    string bookHelper = BookHelperHandler(newsocket, clientRequest);
                    //hier komt connectie naar userserver
                    string userHelper = UserHelperHandler(newsocket, clientRequest);

                    //Handle response to client
                    string response = "Request received";
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

        //Handle outgoing request to the bookHelper
        private string BookHelperHandler(Socket socket, string bookName)
        {
            return "";
        }

        //Handle outgoing request to the userHelper
        private string UserHelperHandler(Socket socket, string bookName)
        {
            return "";
        }
    }

}



