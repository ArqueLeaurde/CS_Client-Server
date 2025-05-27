using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TcpChatClient
{
    private readonly string serverIP;
    private readonly int port;
    private TcpClient client;
    private NetworkStream stream;

    public TcpChatClient(string serverIP, int port)
    {
        this.serverIP = serverIP;
        this.port = port;
    }

    public void Connect()
    {
        client = new TcpClient(serverIP, port);
        stream = client.GetStream();

        // Saisie login
        Console.Write("Nom d'utilisateur: ");
        string username = Console.ReadLine();
        SendMessage(username);

        Console.Write("Mot de passe: ");
        string password = Console.ReadLine();
        SendMessage(password);

        // Lire la réponse du serveur
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        if (response.ToLower().Contains("échouée"))
        {
            Console.WriteLine(response);
            Disconnect();
            Environment.Exit(0);
        }

        Console.WriteLine("[Client] Connecté au serveur.");
    }

    public void StartChat()
    {
        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start();

        while (true)
        {
            Console.Write("Vous :  ");
            string message = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(message))
                continue;

            SendMessage(message);

            if (message.ToLower() == "exit")
                break;
        }

        Disconnect();
    }

    private void SendMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    private void ReceiveMessages()
    {
        try
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                int currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, currentLineCursor);

                Console.WriteLine(message);
                Console.Write("Vous :  ");
            }
        }
        catch
        {
            Console.WriteLine("[Client] Déconnecté du serveur.");
        }
    }

    private void Disconnect()
    {
        stream.Close();
        client.Close();
        Console.WriteLine("[Client] Déconnecté.");
    }
}
