using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheIsleEvrimaRconClient.Extensions.Models;

namespace TheIsleEvrimaRconClient.Extensions
{
    public static class EvrimaRconClientExtensions
    {
        /// <summary>
        /// Announces a message on the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message">The message to announce</param>
        public static Task Announce(this EvrimaRconClient client, string message)
        {
            return client.SendCommandAsync(EvrimaRconCommand.Announce, message);
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
        /// Bans a player on the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="eosId">The EOS ID of the player</param>
        public static Task Ban(this EvrimaRconClient client, string eosId)
        {
            return client.SendCommandAsync(EvrimaRconCommand.Ban, eosId);
        }

        /// <summary>
        /// Kicks a player on the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="eosId">The EOS ID of the player</param>
        public static Task Kick(this EvrimaRconClient client, string eosId)
        {
            return client.SendCommandAsync(EvrimaRconCommand.Kick, eosId);
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

            var eosIds = lines[0].Split(',')
                                 .Where(eos => !string.IsNullOrEmpty(eos))
                                 .ToArray();
            var names = lines[1].Split(',')
                                .Where(name => !string.IsNullOrEmpty(name))
                                .ToArray();

            for (int i = 0; i < eosIds.Length; i++)
            {
                result.Add(new ServerPlayer
                {
                    EosId = eosIds[i],
                    PlayerName = names[i]
                });
            }

            return result;
        }

        /// <summary>
        /// Saves the server.
        /// </summary>
        /// <param name="client"></param>
        public static Task Save(this EvrimaRconClient client)
        {
            return client.SendCommandAsync(EvrimaRconCommand.Save);
        }
    }
}
