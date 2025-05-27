class Program
{
    static void Main()
    {
        TcpChatServerAdvanced server = new TcpChatServerAdvanced(5000);
        server.Start();
        Console.WriteLine("Appuyez sur une touche pour quitter...");
        Console.ReadKey();
    }
}
