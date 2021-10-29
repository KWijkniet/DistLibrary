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
            byte[] incomingmsgCLIENT = new byte[1000];
            string data =null;
            while (true)
            {
                //todo: implement the body. Add extra fields and methods to the class if it is needed
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, settings.ServerPortNumber);
                socket.Bind(iPEndPoint);
                socket.Listen(settings.ServerListeningQueue);
                Console.WriteLine("waiting lah");
                Socket newsocket = socket.Accept();

                while (true)
                {
                    int b = newsocket.Receive(incomingmsgCLIENT);
                    data = Encoding.ASCII.GetString(incomingmsgCLIENT, 0, b);
                    Console.WriteLine(data);
                }
                socket.Close();
            }
        }
    }

}



