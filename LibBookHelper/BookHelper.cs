using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using LibData;

namespace BookHelper
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
        private Setting settings;
        private IPAddress ipAddress;


        public SequentialHelper()
        {
            //todo: implement the body. Add extra fields and methods to the class if needed
            try
            {
                //reading the json file
                string configContent = File.ReadAllText(configFile);
                this.settings = JsonSerializer.Deserialize<Setting>(configContent);
                this.ipAddress = IPAddress.Parse(settings.BookHelperIPAddress);
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
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, settings.BookHelperPortNumber);
            socket.Bind(iPEndPoint);
            socket.Listen(settings.ServerListeningQueue);
            Socket newsocket = socket.Accept();
            //het wacht op een connect atm 
            //nu krijg een book naam en die moet ik gaan zoeken tussne json file
            
            
            
        }
    }
}
