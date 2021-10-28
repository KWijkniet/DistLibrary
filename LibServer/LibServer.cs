using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
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
            IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, settings.ServerPortNumber);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(ipEndpoint);
            socket.Listen(3);
            Console.WriteLine("\n Waiting for clients...");
            Socket newSocket = socket.Accept();

            while (true)
            {
                int b = newSocket.Receive(buffer);
                data = Encoding.ASCII.GetString(buffer, 0, b);

                if (data == "Closed")
                {
                    newSocket.Close();
                    Console.WriteLine("Closing the socket...");
                    break;
                }

                Console.WriteLine("" + data);
                data = null;
                newSocket.Send(msg);
            }
            socket.Close();
        }
    }

}



