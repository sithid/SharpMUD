using SharpMUD.Shared.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class MudServer {
    private TcpListener listener;
    private List<ClientHandler> clients = new List<ClientHandler>();

    public Room StartRoom { get; set; } = new Room("Start", "The default start room for all players.");

    public MudServer(int port) {
        listener = new TcpListener(IPAddress.Any, port);
    }

    /// <summary>
    /// Start the MUD server.
    /// </summary>
    public void Start() {
        listener.Start();

        Console.WriteLine("MUD server started on port " + listener.LocalEndpoint);

        while (true) {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected: " + client.Client.RemoteEndPoint);
            
            ClientHandler clientHandler = new ClientHandler(client, this);
            clients.Add(clientHandler);

            Thread clientThread = new Thread(clientHandler.Run);
            clientThread.Start();
        }
    }

    public void RemoveClient(ClientHandler clientHandler) {
        clients.Remove(clientHandler);
        Console.WriteLine("Client disconnected: " + clientHandler.Client.Client.RemoteEndPoint);
    }

    public void BroadcastMessage(string message, ClientHandler sender) {
        foreach (ClientHandler client in clients) {
            if (client != sender) {
                client.SendToPlayer(message);
            }
        }
    }
}

public class ClientHandler {
    private TcpClient client;
    public TcpClient Client { get { return client; } }
    
    private MudServer server;
    public MudServer Server { get { return server; } }

    private Player player;
    public Player Player { get { return player; } }

    private NetworkStream stream;

    public ClientHandler( TcpClient client, MudServer server) {
        this.client = client;
        this.server = server;
        stream = client.GetStream();
    }

    public void Run() {
        try {
            SendToPlayer("Welcome to the MUD!\r\nWhat is your name?");
            string name = ReadLine();
            
            player = new Player(name, server.StartRoom);

            SendToPlayer($"Hello, {player.Name}!\r\n");
            SendToPlayer($"Current Room: {player.CurrentRoom.Name}");
            SendToPlayer(player.CurrentRoom.Description + "\n\r");

            while (true) {
                string input = ReadLine();
                ProcessCommand(input);
            }
        }
        catch (Exception ex) {
            Console.WriteLine("Error handling client: " + ex.Message);
        }
        finally {
            stream.Close();
            client.Close();
            server.RemoveClient(this);
        }
    }

    public void SendToPlayer(string message) {
        byte[] buffer = Encoding.ASCII.GetBytes(message + "\r\n");
        stream.Write(buffer, 0, buffer.Length);
    }

    private string ReadLine() {
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        return Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
    }

    private void ProcessCommand(string input) {
        string[] parts = input.Split(' ');
        string command = parts[0].ToLower();

        if (command == "look") {
            SendToPlayer(player.CurrentRoom.Description);
            foreach (Item item in player.CurrentRoom.Items) {
                SendToPlayer($"There is a {item.Name} here.");
            }
        }
        else if (command == "go") {
            if (parts.Length > 1) {
                string direction = parts[1].ToLower();
                if (player.CurrentRoom.Exits.ContainsKey(direction)) {
                    player.CurrentRoom = player.CurrentRoom.Exits[direction];
                    SendToPlayer($"You go {direction}.\r\n{player.CurrentRoom.Description}");
                }
                else {
                    SendToPlayer("You can't go that way.");
                }
            }
            else {
                SendToPlayer("Go where?");
            }
        }
        else {
            SendToPlayer("Invalid command.");
        }
    }
}