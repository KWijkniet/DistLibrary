using System;
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
                    Console.WriteLine("Connected");

                    //Receive client_id from client
                    Message message = ReceiveMessage(newsocket);
                    if (message.Type == MessageType.Hello)
                    {
                        //Request next message from client
                        SendMessage(newsocket, MessageType.Welcome, "");

                        //Receive next message from client
                        message = ReceiveMessage(newsocket);
                        if(message.Type == MessageType.BookInquiry)
                        {
                            //Connect to book helper
                            Socket bookHelperSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            IPEndPoint iPEndPointHelper = new IPEndPoint(IPAddress.Parse(settings.BookHelperIPAddress), settings.BookHelperPortNumber);
                            bookHelperSocket.Connect(iPEndPointHelper);

                            //Request book from helper
                            SendMessage(bookHelperSocket, MessageType.BookInquiry, message.Content);

                            //Receive book from helper
                            message = ReceiveMessage(bookHelperSocket);
                            if (message.Type == MessageType.BookInquiryReply)
                            {
                                //Send book to client
                                SendMessage(newsocket, MessageType.BookInquiryReply, message.Content);
                            }

                            //Close book helper
                            bookHelperSocket.Close();
                        }
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

        private void SendMessage(Socket socket, MessageType type, string text)
        {
            //send request
            Message message = new Message();
            message.Type = type;
            message.Content = text;
            string messageString = JsonSerializer.Serialize(message);

            byte[] msg = Encoding.ASCII.GetBytes(messageString);
            socket.Send(msg);
            Console.WriteLine("Send: " + text);
        }

        private Message ReceiveMessage(Socket socket)
        {
            //receive response
            byte[] incomingmsg = new byte[1000];
            int response = socket.Receive(incomingmsg);
            string responseJson = Encoding.ASCII.GetString(incomingmsg, 0, response);

            Message message = JsonSerializer.Deserialize<Message>(responseJson);
            Console.WriteLine("Received: " + message.Content);
            return message;
        }
    }
}