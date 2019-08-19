using DarkRift.Server;
using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlanetGorgon_Server
{

    public class PlayerManager : Plugin
    {
        public override bool ThreadSafe => true;
        public override Version Version => new Version(1, 0, 0);
        Dictionary<IClient, Player> players = new Dictionary<IClient, Player>();

        public PlayerManager(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            ClientManager.ClientConnected += ClientConnected;
            ClientManager.ClientDisconnected += ClientManager_ClientDisconnected;
        }

        private void ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            try
            {
                // Create New Player Object
                Player player = new Player(new Vec3(0, 0, 0), new Vec3(0, 0, 0), e.Client.ID);

                // Send New Player To Clients
                Console.WriteLine("Sending player spawn message to " + players.Count + " clients.");
                using (Message message = Message.Create(Tags.SpawnPlayerTag, player))
                {
                    foreach (IClient client in ClientManager.GetAllClients())
                    {
                        if (client != e.Client)
                            client.SendMessage(message, SendMode.Reliable);
                    }
                }

                // Add player to collection
                Console.WriteLine("Player added to collection.");
                players.Add(e.Client, player);

                // Get All Players And Spawn Them On Our Client
                Console.WriteLine("Sending player data for " + players.Count + " players to clientID " + e.Client.ID + " (" + e.Client.GetRemoteEndPoint("TCP").Address + ")");
                foreach (IClient client in ClientManager.GetAllClients())
                {
                    Player p;
                    lock (players)
                        p = players[client];

                    using (Message message = Message.Create(Tags.SpawnPlayerTag, p))
                        e.Client.SendMessage(message, SendMode.Reliable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed adding character to player collection: " + ex.Message);
            }

            //Subscribe to when this client sends messages
            e.Client.MessageReceived += MovementMessageReceived;
        }

        void MovementMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage() as Message)
            {
                if (message.Tag == Tags.MovePlayerTag)
                {
                    // Read Player Coordinates From The Message
                    try
                    {
                        using (DarkRiftReader reader = message.GetReader())
                        {
                            // Get Player ID
                            Player player;
                            lock (players)
                                player = players[e.Client];

                            // Get Player Positions & Serialize it
                            Vec3 newPosition = reader.ReadSerializable<Vec3>();
                            Vec3 newRotation = reader.ReadSerializable<Vec3>();

                            lock (player)
                            {
                                //Update the player
                                player.Position = newPosition;
                                player.Rotation = newRotation;

                                //Serialize the whole player to the message so that we also include the ID
                                message.Serialize(player);
                            }

                            //Console.WriteLine("Reading Message" + Encoding.Default.GetString(message.GetReader().ReadBytes()));

                            //Send to everyone else
                            foreach (IClient sendTo in ClientManager.GetAllClients().Except(new IClient[] { e.Client }))
                                sendTo.SendMessage(message, SendMode.Reliable);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine("An error was logged when the packet was received: " + ex.Message);
                        Console.WriteLine(ex.InnerException);
                        Console.WriteLine(ex.Data);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
        }

        private void ClientManager_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            players.Remove(e.Client);

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(e.Client.ID);

                using (Message message = Message.Create(Tags.DespawnSplayer, writer))
                {
                    foreach (IClient client in ClientManager.GetAllClients())
                        client.SendMessage(message, SendMode.Reliable);
                }
            }
        }
    }
}