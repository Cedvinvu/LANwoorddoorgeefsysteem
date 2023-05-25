using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using SMUtils;

namespace NietGrappigNetwerkDing
{
    internal class Program
    {
        static string tekst;
        static string naam;
        static string IPaanvanger;
        static int port = 13000;

        static void Main(string[] args)
        {
            var menu = new SMUtils.Menu();

            menu.AddOption('1', "Stuur een bericht", SendMessageOption);
            menu.AddOption('2', "Vang een bericht", ReceiveMessageOption);
            menu.AddOption('3', "Voeg wat toe aan de lijn", AddToMessageLine);
            menu.AddOption('4', "Verander bericht", ChangeMessage);
            menu.AddOption('5', "Verander IP", ChangeIP);
            menu.AddOption('6', "Verander Naam", ChangeName);
            menu.AddOption('7', "Test de connectie", TestConnection);
            menu.AddOption('8', "Zie IP", GetLocalIPAddress);
            menu.Start();
        }


        static void SendMessage(string message)
        {
            try
            {
                using (Socket senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) // Create a TCP/IP socket
                {
                    Console.WriteLine("Connecteren...");
                    senderSocket.Connect(IPAddress.Parse(IPaanvanger), port); // Connect to the recipient
                    Console.WriteLine("Geconnecteerd!");

                    if (message == null) message = naam + ": " + tekst;
                    else message += Environment.NewLine + naam + ": " + tekst;

                    byte[] messageBytes = Encoding.ASCII.GetBytes(message); // Convert the message to bytes

                    senderSocket.Send(messageBytes); // Send the message to the recipient
                    Console.WriteLine("Message send!");

                    senderSocket.Shutdown(SocketShutdown.Both); // Close the socket
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

        static string ReceiveMessage(bool showmessage)
        {
            try
            {
                using (Socket receiverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) // Create a TCP/IP socket
                {
                    receiverSocket.Bind(new IPEndPoint(IPAddress.Any, port)); // Bind the socket to the listening IP address and port

                    receiverSocket.Listen(1); // Start listening for incoming connections
                    Console.WriteLine("Waiting for a connection...");

                    Socket clientSocket = receiverSocket.Accept(); // Accept the incoming connection
                    Console.WriteLine("Connection accepted.");

                    byte[] buffer = new byte[1024]; // Receive the message from the sender
                    int bytesRead = clientSocket.Receive(buffer);
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    if (showmessage) Console.WriteLine("Received message:" + Environment.NewLine + message); // Display the message

                    clientSocket.Shutdown(SocketShutdown.Both); // Close the client socket
                    clientSocket.Close();
                    return message;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
            return null;
        }

        static void TestConnection()
        {
            try
            {
                using (Socket senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) // Create a TCP/IP socket
                {
                    Console.WriteLine("Connecteren...");
                    senderSocket.Connect(IPAddress.Parse(IPaanvanger), port); // Connect to the recipient
                    Console.WriteLine("U bent geconnecteerd!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }


        static void SendMessageOption()
        {
            SendMessage(null);
        }

        static void ReceiveMessageOption()
        {
            ReceiveMessage(true);
        }

        static void AddToMessageLine()
        {
            string message = ReceiveMessage(false);
            SendMessage(message);

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
            IPaanvanger = Console.ReadLine();
        }
        static void ChangeName()
        {
            Console.WriteLine("Geef naam:");
            naam = Console.ReadLine();
        }
        static void GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (ip.ToString().StartsWith("192.168."))
                    {
                        Console.WriteLine("uw IP: " + ip.ToString());
                    }
                }
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

    }
}