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
        private List<UserData> users = new List<UserData>();

        public SequentialHelper()
        {
            //todo: implement the body. Add extra fields and methods to the class if needed
            try
            {
                //reading the json file
                string configContent = File.ReadAllText(configFile);
                this.settings = JsonSerializer.Deserialize<Setting>(configContent);
                this.ipAddress = IPAddress.Parse(settings.UserHelperIPAddress);

                string userContent = File.ReadAllText(authorFile);
                this.users = JsonSerializer.Deserialize<List<UserData>>(userContent);
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

                while (true)
                {
                    //Connect
                    Socket newsocket = socket.Accept();

                    // het opnemen van informatie dat dr binnenkrijgt en uitprinten
                    while (true)
                    {
                        Message message = ReceiveMessage(newsocket);
                        if (message.Type == MessageType.UserInquiry)
                        {
                            UserData user = FindUserById(message.Content);
                            if (user == null)
                            {
                                SendMessage(newsocket, MessageType.NotFound, "");
                            }
                            else
                            {
                                SendMessage(newsocket, MessageType.UserInquiryReply, JsonSerializer.Serialize(user));
                            }
                        }
                        else if (message.Type == MessageType.EndCommunication)
                        {
                            break;
                        }
                    }
                    newsocket.Close();
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nAn error occured: " + e.Message + "\n" + e.StackTrace, ConsoleColor.Red);
            }

            socket.Close();
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
        }

        private Message ReceiveMessage(Socket socket)
        {
            //receive response
            byte[] incomingmsg = new byte[1000];
            int response = socket.Receive(incomingmsg);
            string responseJson = Encoding.ASCII.GetString(incomingmsg, 0, response);

            Message message = JsonSerializer.Deserialize<Message>(responseJson);
            return message;
        }

        private UserData FindUserById(string id)
        {
            foreach (UserData user in users)
            {
                if (user.User_id == id)
                {
                    return user;
                }
            }
            return null;
        }
    }
}
