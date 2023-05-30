using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using SMUtils;
using System.Text.Json;

namespace NietGrappigNetwerkDing
{
    internal class Program
    {
        static SaveData data;
        static int port = 13000;

        static void Main(string[] args)
        {
            LoadData();
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

            data.Save();
        }


        static void SendMessage(string message)
        {
            try
            {
                using (Socket senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) // Create a TCP/IP socket
                {
                    Console.WriteLine("Connecteren...");
                    senderSocket.Connect(IPAddress.Parse(data.IPaanvanger), port); // Connect to the recipient
                    Console.WriteLine("Geconnecteerd!");

                    if (message == null) message = data.Naam + ": " + data.Bericht;
                    else message += Environment.NewLine + data.Naam + ": " + data.Bericht;

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
                    Console.WriteLine("Waiting for a connection...    (press any key to cancel)");

                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(); // Create a cancellation token source

                    // Start a thread to listen for cancellation by pressing Enter
                    Thread cancellationThread = new Thread(() =>
                    {
                        Console.ReadKey();
                        cancellationTokenSource.Cancel();
                    });
                    cancellationThread.Start();

                    var acceptTask = receiverSocket.AcceptAsync(); // Accept the connection

                    var completedTask = Task.WhenAny(acceptTask, Task.Delay(-1, cancellationTokenSource.Token)).Result; // Wait for either the cancellation or the accept operation to complete

                    // Check if the cancellation task completed
                    if (completedTask == acceptTask)
                    {
                        Socket clientSocket = acceptTask.GetAwaiter().GetResult(); // Accept the incoming connection
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return null;
        }

        static void TestConnection()
        {
            try
            {
                using (Socket senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) // Create a TCP/IP socket
                {
                    Console.WriteLine("Connecteren...");
                    senderSocket.Connect(IPAddress.Parse(data.IPaanvanger), port); // Connect to the recipient
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
            data.Bericht = Console.ReadLine();
        }
        static void ChangeIP()
        {
            Console.WriteLine("Geef IP:");
            data.IPaanvanger = Console.ReadLine();
        }
        static void ChangeName()
        {
            Console.WriteLine("Geef naam:");
            data.Naam = Console.ReadLine();
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

        static void LoadData()
        {
            //Maak de path
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "woorddoorgeefsysteem");
            var filePath = Path.Combine(path, "savedata");

            // kijk of het bestand bestaat
            if (File.Exists(filePath))
            {
                // lees alle tekst uit het bestand
                string content = File.ReadAllText(filePath);

                // probeer de inhoud in de library te plaatsen
                data = JsonSerializer.Deserialize<SaveData>(content);
            }
            else
            {
                data = new SaveData();
            }
            return;
        }

    }
}