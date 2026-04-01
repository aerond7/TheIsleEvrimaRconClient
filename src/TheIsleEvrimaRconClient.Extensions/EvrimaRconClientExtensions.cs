using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheIsleEvrimaRconClient.Extensions.Models;

namespace TheIsleEvrimaRconClient.Extensions
{
    public static class EvrimaRconClientExtensions
    {
        /// <summary>
        /// Announces a message on the server, displayed to all players.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message">The message to announce</param>
        public static Task Announce(this EvrimaRconClient client, string message)
        {
            return client.SendCommandAsync(EvrimaRconCommand.Announce, message);
        }

        /// <summary>
        /// Sends a direct message (announcement) to a specific player.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="playerId">The player id (EOS or Steam)</param>
        /// <param name="message">The message to send</param>
        public static Task DirectMessage(this EvrimaRconClient client, string playerId, string message)
        {
            return client.SendCommandAsync(EvrimaRconCommand.DirectMessage, $"{playerId},{message}");
        }

        /// <summary>
        /// Retrieves all the current server settings.
        /// </summary>
        /// <param name="client"></param>
        /// <returns>Server details as a string</returns>
        public static Task<string> GetServerDetails(this EvrimaRconClient client)
        {
            return client.SendCommandAsync(EvrimaRconCommand.ServerDetails);
        }

        /// <summary>
        /// Wipes all corpses on the server.
        /// </summary>
        /// <param name="client"></param>
        public static Task WipeCorpses(this EvrimaRconClient client)
        {
            return client.SendCommandAsync(EvrimaRconCommand.WipeCorpses);
        }

        /// <summary>
        /// Sends the UpdatePlayables command to the server.
        /// </summary>
        /// <param name="client"></param>
        /// <returns>Response from the server</returns>
        public static Task<string> UpdatePlayables(this EvrimaRconClient client)
        {
            return client.SendCommandAsync(EvrimaRconCommand.UpdatePlayables);
        }

        /// <summary>
        /// Modifies the playable classes on the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="playables">Comma-delimited string of playable classes to set</param>
        /// <returns>Response from the server</returns>
        public static Task<string> UpdatePlayables(this EvrimaRconClient client, string playables)
        {
            return client.SendCommandAsync(EvrimaRconCommand.UpdatePlayables, playables);
        }

        /// <summary>
        /// Bans a player on the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="playerId">The player id (EOS or Steam)</param>
        public static Task Ban(this EvrimaRconClient client, string playerId)
        {
            return client.SendCommandAsync(EvrimaRconCommand.Ban, playerId);
        }

        /// <summary>
        /// Kicks a player on the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="playerId">The player id (EOS or Steam)</param>
        public static Task Kick(this EvrimaRconClient client, string playerId)
        {
            return client.SendCommandAsync(EvrimaRconCommand.Kick, playerId);
        }

        /// <summary>
        /// Gets the player list of online players on the server.
        /// </summary>
        /// <param name="client"></param>
        /// <returns>Collection of online players</returns>
        public static async Task<List<ServerPlayer>> GetPlayerList(this EvrimaRconClient client)
        {
            var result = new List<ServerPlayer>();

            var response = await client.SendCommandAsync(EvrimaRconCommand.PlayerList);
            var lines = response.Split('\n')
                                .Skip(1)
                                .ToArray();

            if (lines.Length != 2)
            {
                return result;
            }

            var playerIds = lines[0].Split(',')
                                    .Where(id => !string.IsNullOrEmpty(id))
                                    .ToArray();
            var names = lines[1].Split(',')
                                .Where(name => !string.IsNullOrEmpty(name))
                                .ToArray();

            for (int i = 0; i < playerIds.Length; i++)
            {
                result.Add(new ServerPlayer
                {
                    PlayerId = playerIds[i],
                    PlayerName = names[i]
                });
            }

            return result;
        }

        /// <summary>
        /// Saves all game data on the server.
        /// </summary>
        /// <param name="client"></param>
        public static Task Save(this EvrimaRconClient client)
        {
            return client.SendCommandAsync(EvrimaRconCommand.Save);
        }

        /// <summary>
        /// Retrieves info about each player like location, character stats, etc.
        /// </summary>
        /// <param name="client"></param>
        /// <returns>Player data as a string</returns>
        public static Task<string> GetPlayerData(this EvrimaRconClient client)
        {
            return client.SendCommandAsync(EvrimaRconCommand.GetPlayerData);
        }

        /// <summary>
        /// Turns the server whitelist on or off.
        /// </summary>
        /// <param name="client"></param>
        public static Task ToggleWhitelist(this EvrimaRconClient client)
        {
            return client.SendCommandAsync(EvrimaRconCommand.ToggleWhitelist);
        }

        /// <summary>
        /// Adds player id(s) to the server whitelist.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="playerIds">Comma-delimited string of player ids to add</param>
        public static Task AddWhitelistIds(this EvrimaRconClient client, string playerIds)
        {
            return client.SendCommandAsync(EvrimaRconCommand.AddWhitelistId, playerIds);
        }

        /// <summary>
        /// Removes player id(s) from the server whitelist.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="playerIds">Comma-delimited string of player ids to remove</param>
        public static Task RemoveWhitelistIds(this EvrimaRconClient client, string playerIds)
        {
            return client.SendCommandAsync(EvrimaRconCommand.RemoveWhitelistId, playerIds);
        }

        /// <summary>
        /// Turns global chat on or off.
        /// </summary>
        /// <param name="client"></param>
        public static Task ToggleGlobalChat(this EvrimaRconClient client)
        {
            return client.SendCommandAsync(EvrimaRconCommand.ToggleGlobalChat);
        }

        /// <summary>
        /// Turns humans on or off.
        /// </summary>
        /// <param name="client"></param>
        public static Task ToggleHumans(this EvrimaRconClient client)
        {
            return client.SendCommandAsync(EvrimaRconCommand.ToggleHumans);
        }

        /// <summary>
        /// Turns AI spawns on or off.
        /// </summary>
        /// <param name="client"></param>
        public static Task ToggleAI(this EvrimaRconClient client)
        {
            return client.SendCommandAsync(EvrimaRconCommand.ToggleAI);
        }

        /// <summary>
        /// Updates the allowable AI spawn list (disables specified classes).
        /// </summary>
        /// <param name="client"></param>
        /// <param name="aiClasses">Comma-delimited string of AI classes to disable</param>
        public static Task DisableAIClasses(this EvrimaRconClient client, string aiClasses)
        {
            return client.SendCommandAsync(EvrimaRconCommand.DisableAIClasses, aiClasses);
        }

        /// <summary>
        /// Adjusts the AI spawn density.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="density">The AI density value</param>
        public static Task SetAIDensity(this EvrimaRconClient client, string density)
        {
            return client.SendCommandAsync(EvrimaRconCommand.AIDensity, density);
        }
    }
}
