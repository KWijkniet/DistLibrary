using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using LibData;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace LibClient
{
    // Note: Do not change this class 
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

    // Note: Do not change this class 
    public class Output
    {
        public string Client_id { get; set; } // the id of the client that requests the book
        public string BookName { get; set; } // the name of the book to be reqyested
        public string Status { get; set; } // final status received from the server
        public string BorrowerName { get; set; } // the name of the borrower in case the status is borrowed, otherwise null
        public string BorrowerEmail { get; set; } // the email of the borrower in case the status is borrowed, otherwise null
    }

    // Note: Complete the implementation of this class. You can adjust the structure of this class.
    public class SimpleClient
    {
        // some of the fields are defined. 
        public Output result;
        public Socket clientSocket;
        public IPEndPoint serverEndPoint;
        public IPAddress ipAddress;
        public Setting settings;
        public string client_id;
        private string bookName;
        // all the required settings are provided in this file
        //public string configFile = @"../ClientServerConfig.json";
        public string configFile = @"../../../../ClientServerConfig.json"; // for debugging

        // todo: add extra fields here in case needed 

        /// <summary>
        /// Initializes the client based on the given parameters and seeting file.
        /// </summary>
        /// <param name="id">id of the clients provided by the simulator</param>
        /// <param name="bookName">name of the book to be requested from the server, provided by the simulator</param>
        public SimpleClient(int id, string bookName)
        {
            //todo: extend the body if needed.
            this.bookName = bookName;
            this.client_id = "Client " + id.ToString();
            this.result = new Output();
            result.BookName = bookName;
            result.Client_id = this.client_id;

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

        /// <summary>
        /// Establishes the connection with the server and requests the book according to the specified protocol.
        /// Note: The signature of this method must not change.
        /// </summary>
        /// <returns>The result of the request</returns>
        public Output start()
        {
            // todo: implement the body to communicate with the server and requests the book. Return the result as an Output object.
            // Adding extra methods to the class is permitted. The signature of this method must not change.
            Console.WriteLine("Starting client: " + client_id + "\nBook: " + bookName);

            //voorbereiding voor TCP connectie
            serverEndPoint = new IPEndPoint(ipAddress, settings.ServerPortNumber);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // het proberen om TCP connectie te maken
            clientSocket.Connect(serverEndPoint);

            //send request
            SendMessage(MessageType.Hello, client_id);
            Message message = ReceiveMessage();
            if (message.Type == MessageType.Welcome)
            {
                SendMessage(MessageType.BookInquiry, bookName.Length > 0 ? bookName : "TERMINATE");

                message = ReceiveMessage();
                if (message.Type == MessageType.BookInquiryReply)
                {
                    BookData book = JsonSerializer.Deserialize<BookData>(message.Content);
                    if(book != null)
                    {
                        result.Status = book.Status;
                    }
                }
            }

            //End of client
            clientSocket.Close();
            Console.WriteLine("Quiting client: " + client_id);
            return result;
        }

        private void SendMessage(MessageType type, string text)
        {
            //send request
            Message message = new Message();
            message.Type = type;
            message.Content = text;
            string messageString = JsonSerializer.Serialize(message);

            byte[] msg = Encoding.ASCII.GetBytes(messageString);
            clientSocket.Send(msg);

            Console.WriteLine("Send: " + text);
        }

        private Message ReceiveMessage()
        {
            //receive response
            byte[] incomingmsg = new byte[1000];
            int response = clientSocket.Receive(incomingmsg);
            string responseJson = Encoding.ASCII.GetString(incomingmsg, 0, response);

            Message message = JsonSerializer.Deserialize<Message>(responseJson);
            Console.WriteLine("Received: " + message.Content);
            return message;
        }
    }
}
