using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// Classe du serveur de chat
public class TcpChatServerAdvanced
{
    private readonly int port;
    private TcpListener listener;
    private List<ClientInfo> clients = new List<ClientInfo>();
    private int clientCounter = 0;
    private readonly object lockObj = new object();

    // Dictionnaire d'utilisateurs valides (login, mot de passe)
    private readonly Dictionary<string, string> validUsers = new Dictionary<string, string>
    {
        { "Arthur", "lm;k,jn8yAuctdy" },
        { "admin", "pass" },
        { "chocoblast", "ch0c0b74St" }
    };

    public TcpChatServerAdvanced(int port)
    {
        this.port = port;
    }

    //Demarrage du serveur
    public void Start()
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"[Serveur] En écoute sur le port {port}...");

        //Boucle d'ecoude tcp
        while (true)
        {
            TcpClient tcpClient = listener.AcceptTcpClient();
            var clientInfo = new ClientInfo
            {
                TcpClient = tcpClient
            };

            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(clientInfo);
        }
    }

    //Gestion des clients
    private void HandleClient(object obj)
    {
        ClientInfo client = (ClientInfo)obj;
        NetworkStream stream = client.TcpClient.GetStream();
        byte[] buffer = new byte[1024];

        try
        {
            //Auth
            string username = null;
            string password = null;

            SendToClient(stream, "Nom d'utilisateur: ");
            username = ReadFromClient(stream, buffer);

            SendToClient(stream, "Mot de passe: ");
            password = ReadFromClient(stream, buffer);

            if (!validUsers.ContainsKey(username) || validUsers[username] != password)
            {
                SendToClient(stream, "Authentification échouée. Déconnexion...");
                stream.Close();
                client.TcpClient.Close();
                Console.WriteLine($"[Serveur] Échec d'authentification pour {username ?? "?"}");
                return;
            }

            client.Name = username;
            lock (lockObj)
            {
                clients.Add(client);
            }

            Console.WriteLine($"[Serveur] {username} connecté.");
            Broadcast($"[Serveur] {username} a rejoint le chat.", client);


            //broadcast des messages du serveur
            while (true)
            {
                int readBytes = stream.Read(buffer, 0, buffer.Length);
                if (readBytes == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, readBytes);
                string fullMessage = $"{username}: {message}";

                Console.WriteLine(fullMessage);

                if (message.ToLower() == "exit")
                    break;

                Broadcast(fullMessage, client);
            }
        }
        catch
        {
            Console.WriteLine($"[Serveur] {client.Name} déconnecté avec une erreur.");
        }
        finally
        {
            lock (lockObj)
            {
                clients.Remove(client);
            }

            try
            {
                stream.Close();
                client.TcpClient.Close();
            }
            catch { }

            Console.WriteLine($"[Serveur] {client.Name} déconnecté.");
            Broadcast($"[Serveur] {client.Name} a quitté le chat.", null);
        }
    }
    //fonction de broadcast des messages a l'ensemble des clients
    private void Broadcast(string message, ClientInfo sender)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        lock (lockObj)
        {
            foreach (ClientInfo client in clients)
            {
                if (client != sender)
                {
                    try
                    {
                        NetworkStream stream = client.TcpClient.GetStream();
                        stream.Write(data, 0, data.Length);
                    }
                    catch
                    {
                        // Ignorer les erreurs
                    }
                }
            }
        }
    }

    //fonction d'envoi des messages aux clients
    private void SendToClient(NetworkStream stream, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    //fonction de reception des messages du client
    private string ReadFromClient(NetworkStream stream, byte[] buffer)
    {
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
    }
}

//Infos client
public class ClientInfo
{
    public string Name { get; set; }
    public TcpClient TcpClient { get; set; }
}
