using System.Net.Sockets;
using System.Net;
using System.Text;
using SMUtils;

namespace NietGrappigNetwerkDing
{
    internal class Program
    {
        static string tekst;
        static string naam;
        static string recipientIP;

        static void Main(string[] args)
        {
            var menu = new SMUtils.Menu();

            menu.AddOption('1', "Stuur een bericht", SendMessage);
            menu.AddOption('2', "Vang een bericht", ReceiveMessage);
            menu.AddOption('3', "Voeg wat toe aan de lijn", AddToMessageLine);
            menu.AddOption('4', "Verander bericht", ChangeMessage);
            menu.AddOption('5', "Verander IP", ChangeIP);
            menu.AddOption('6', "Verander Naam", ChangeName);
            menu.Start();
        }


        static void SendMessage()
        {
            try
            {
                //Console.WriteLine("Geef de IP waarnaar je iets wil sturen");
                // Define the recipient's IP address and port
                //string recipientIP = Console.ReadLine(); // Change to the recipient's IP address
                int port = 13000; // Change to the recipient's port

                // Create a TCP/IP socket
                using (Socket senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    Console.WriteLine("Connecteren...");
                    // Connect to the recipient
                    senderSocket.Connect(IPAddress.Parse(recipientIP), port);
                    Console.WriteLine("Geconnecteerd!");
                    // Prompt for the message to send
                    //Console.Write("Enter a message: ");
                    //string message = Console.ReadLine();

                    string message = naam + ": " + tekst;

                    // Convert the message to bytes
                    byte[] messageBytes = Encoding.ASCII.GetBytes(message);

                    // Send the message to the recipient
                    senderSocket.Send(messageBytes);
                    Console.WriteLine("Message send!");

                    // Close the socket
                    senderSocket.Shutdown(SocketShutdown.Both);
                    senderSocket.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        static void ReceiveMessage()
        {
            try
            {
                int port = 13000; // Change to the listening port

                // Create a TCP/IP socket
                using (Socket receiverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    // Bind the socket to the listening IP address and port
                    receiverSocket.Bind(new IPEndPoint(IPAddress.Any, port));

                    // Start listening for incoming connections
                    receiverSocket.Listen(1);

                    Console.WriteLine("Waiting for a connection...");

                    // Accept the incoming connection
                    Socket clientSocket = receiverSocket.Accept();

                    Console.WriteLine("Connection accepted.");

                    // Receive the message from the sender
                    byte[] buffer = new byte[1024];
                    int bytesRead = clientSocket.Receive(buffer);
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    Console.WriteLine("Received message:" + Environment.NewLine + message);

                    // Close the client socket
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        static void AddToMessageLine()
        {
            try
            {
                //Console.WriteLine("Geef de IP waarnaar je iets wil sturen");
                // Define the recipient's IP address and port
                //string recipientIP = Console.ReadLine(); // Change to the recipient's IP address
                int port = 13000;

                // Create a TCP/IP socket
                using (Socket receiverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    // Bind the socket to the listening IP address and port
                    receiverSocket.Bind(new IPEndPoint(IPAddress.Any, port));

                    // Start listening for incoming connections
                    receiverSocket.Listen(1);

                    Console.WriteLine("Waiting for a connection...");

                    // Accept the incoming connection
                    Socket clientSocket = receiverSocket.Accept();

                    Console.WriteLine("Connection accepted.");

                    // Receive the message from the sender
                    byte[] buffer = new byte[1024];
                    int bytesRead = clientSocket.Receive(buffer);
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    //Console.WriteLine("Voeg wat toe aan het bericht");
                    //message += Console.ReadLine();

                    message += Environment.NewLine + naam + ": " + tekst;

                    using (Socket senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        Console.WriteLine("Connecteren...");
                        // Connect to the recipient
                        senderSocket.Connect(IPAddress.Parse(recipientIP), port);
                        Console.WriteLine("Geconnecteerd!");
                        // Convert the message to bytes
                        byte[] messageBytes = Encoding.ASCII.GetBytes(message);

                        // Send the message to the recipient
                        senderSocket.Send(messageBytes);
                        Console.WriteLine("Message send!");

                        // Close the socket
                        senderSocket.Shutdown(SocketShutdown.Both);
                        senderSocket.Close();
                    }

                    // Close the client socket
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        static void ChangeMessage()
        {
            Console.WriteLine("Geef bericht:");
            tekst = Console.ReadLine();
        }
        static void ChangeIP()
        {
            Console.WriteLine("Geef IP:");
            recipientIP = Console.ReadLine();
        }
        static void ChangeName()
        {
            Console.WriteLine("Geef naam:");
            naam = Console.ReadLine();
        }

    }
}