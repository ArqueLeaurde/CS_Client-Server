class Program
{
    static void Main()
    {
        TcpChatClient client = new TcpChatClient("127.0.0.1", 5000);
        client.Connect();
        client.StartChat();
        Console.WriteLine("Appuyez sur une touche pour quitter...");
        Console.ReadKey();
    }
}
// On peut lancer plusieurs clients en console en même temps pour simuler plusieurs personnes sur un chat
// ip et port a modifier selon usage