using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheIsleEvrimaRconClient.Extensions.Models;

namespace TheIsleEvrimaRconClient.Extensions
{
    public static class EvrimaRconClientExtensions
    {
        public static Task Announce(this EvrimaRconClient client, string message)
        {
            return client.SendCommandAsync(EvrimaRconCommand.Announce, message);
        }

        public static Task<string> UpdatePlayables(this EvrimaRconClient client)
        {
            return client.SendCommandAsync(EvrimaRconCommand.UpdatePlayables);
        }

        public static Task Ban(this EvrimaRconClient client, string eosId)
        {
            return client.SendCommandAsync(EvrimaRconCommand.Ban, eosId);
        }

        public static Task Kick(this EvrimaRconClient client, string eosId)
        {
            return client.SendCommandAsync(EvrimaRconCommand.Kick, eosId);
        }

        public static async Task<List<ServerPlayer>> GetPlayerList(this EvrimaRconClient client)
        {
            var result = new List<ServerPlayer>();

            var response = await client.SendCommandAsync(EvrimaRconCommand.PlayerList);
            var lines = response.Split('\n')
                                .Skip(1)
                                .ToArray();
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

        public static Task Save(this EvrimaRconClient client)
        {
            return client.SendCommandAsync(EvrimaRconCommand.Save);
        }
    }
}
