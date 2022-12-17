﻿using System;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Enums;
using BlasphemousRandomizer.Config;
using Newtonsoft.Json.Linq;
using BlasphemousMultiworld.Structures;

namespace BlasphemousMultiworld
{
    public class Connection
    {
        private ArchipelagoSession session;
        public bool connected { get; private set; }

        public bool Connect(string server, string player, string password)
        {
            // Create login
            LoginResult result;

            // Try connection
            try
            {
                session = ArchipelagoSessionFactory.CreateSession(server);
                session.Items.ItemReceived += recieveItem;
                session.Socket.SocketClosed += disconnected;
                result = session.TryConnectAndLogin("Blasphemous", player, ItemsHandlingFlags.RemoteItems, new Version(0, 3, 6), null, null, password == "" ? null : password);
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            // Connection failed
            if (!result.Successful)
            {
                connected = false;
                LoginFailure failure = result as LoginFailure;
                string errorMessage = "Multiworld connection failed:\n";
                foreach (string error in failure.Errors)
                {
                    errorMessage += error + "\n";
                }
                Main.Randomizer.Log(errorMessage.Substring(0, errorMessage.Length - 1);
                return false;
            }

            // Connection successful
            connected = true;
            LoginSuccessful login = result as LoginSuccessful;
            Main.Randomizer.Log("Multiworld connection successful");

            // Retrieve new locations
            ArchipelagoLocation[] locations = ((JArray)login.SlotData["locations"]).ToObject<ArchipelagoLocation[]>();
            MainConfig config = ((JObject)login.SlotData["cfg"]).ToObject<MainConfig>();
            Main.Multiworld.onConnect(player, locations, config);
            return true;
        }

        public void Disconnect()
        {
            if (connected)
            {
                session.Socket.Disconnect();
                connected = false;
                session = null;
                Main.Randomizer.Log("Disconnecting from multiworld server");
            }
        }

        // Returns a list of player names, or if unconnected then an empty list
        public string[] getPlayers()
        {
            if (connected)
            {
                string[] players = new string[session.Players.AllPlayers.Count];
                for (int i = 0; i < session.Players.AllPlayers.Count; i++)
                {
                    players[i] = session.Players.AllPlayers[i].Name;
                }
                return players;
            }
            return new string[0];
        }

        public string getServer()
        {
            if (connected)
            {
                return session.Socket.Uri.ToString();
            }
            return "";
        }

        // Sends a new location check to the server
        public void sendLocation(long apLocationId)
        {
            if (connected)
                session.Locations.CompleteLocationChecks(apLocationId);
        }

        // Recieves a new item from the server
        private void recieveItem(ReceivedItemsHelper helper)
        {
            Main.Multiworld.recieveItem(helper.PeekItemName());
            helper.DequeueItem();
        }

        // Got disconnected from server
        private void disconnected(string reason)
        {
            Main.Randomizer.LogDisplay("Disconnected from multiworld server!");
            connected = false;
            session = null;
        }
    }
}
