using System.Collections.Generic;
using Gameplay.UI.Console;

namespace BlasphemousMultiworld
{
    public class MultiworldCommand : ConsoleCommand
    {
        public override void Execute(string command, string[] parameters)
        {
            List<string> paramList;
            string subcommand = GetSubcommand(parameters, out paramList);
            if (command != null && command == "multiworld")
            {
                processMultiworld(subcommand, paramList);
            }
        }

        private void processMultiworld(string command, List<string> parameters)
        {
            if (command == null)
            {
                Console.Write("Command unknown, use multiworld help");
                return;
            }
            string fullCommand = "multiworld " + command;

            if (command == "help" && ValidateParams(fullCommand, 0, parameters))
            {
                Console.Write("Available MULTIWORLD commands:");
                Console.Write("multiworld status: Display connection status");
                Console.Write("multiworld connect SERVER NAME [PASSWORD]: Connect to SERVER with player name as NAME with optional PASSWORD");
                Console.Write("multiworld disconnect: Disconnect from current server");
                Console.Write("multiworld deathlink: Toggles deathlink on/off");
                Console.Write("multiworld players : List all players in this multiworld");
            }
            else if (command == "status" && ValidateParams(fullCommand, 0, parameters))
            {
                string server = Main.Multiworld.connection.getServer();
                if (server == "")
                    Console.Write("Not connected to any server");
                else
                    Console.Write($"Connected to {server}");
            }
            else if (command == "connect")
            {
                string password;
                if (parameters.Count == 2) { password = ""; }
                else if (parameters.Count == 3) { password = parameters[2]; }
                else
                {
                    Console.Write("The command 'connect' requires either 2 or 3 parameters.  You passed " + parameters.Count);
                    return;
                }

                Console.Write($"Attempting to connect to {parameters[0]} as {parameters[1]}...");
                string result = Main.Multiworld.tryConnect(parameters[0], parameters[1], password);
                Console.Write(result);
            }
            else if (command == "disconnect" && ValidateParams(fullCommand, 0, parameters))
            {
                if (Main.Multiworld.connection.connected)
                {
                    Main.Multiworld.connection.Disconnect();
                    Console.Write("Disconnected from server");
                }
                else
                    Console.Write("Not connected to any server!");
            }
            else if (command == "deathlink" && ValidateParams(fullCommand, 0, parameters))
            {
                if (Main.Multiworld.connection.connected)
                {
                    bool enabled = Main.Multiworld.toggleDeathLink();
                    Console.Write("Deathlink has been " + (enabled ? "enabled" : "disabled"));
                }
                else
                    Console.Write("Not connected to any server!");
            }
            else if (command == "players" && ValidateParams(fullCommand, 0, parameters))
            {
                string[] players = Main.Multiworld.connection.getPlayers();
                if (players.Length == 0)
                {
                    Console.Write("Not connected to any server!");
                    return;
                }

                string output = "Multiworld players: ";
                foreach (string player in players)
                    output += player + ", ";
                Console.Write(output.Substring(0, output.Length - 2));
            }
        }

        public override List<string> GetNames()
        {
            return new List<string> { "multiworld" };
        }

        public override bool ToLowerAll() { return false; }

        public override bool HasLowerParameters() { return false; }
    }
}
