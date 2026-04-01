using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheIsleEvrimaRconClient.Extensions.Models;

namespace TheIsleEvrimaRconClient.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="EvrimaRconClient"/> that provide a typed,
    /// command-oriented API on top of the raw <see cref="EvrimaRconClient.SendCommandAsync"/> calls.
    /// </summary>
    public static class EvrimaRconClientExtensions
    {
        // ── Simple fire-and-forget / raw-string commands ─────────────────────────

        /// <summary>Announces a message on the server, displayed to all players.</summary>
        /// <param name="client"></param>
        /// <param name="message">The message to announce</param>
        public static Task Announce(this EvrimaRconClient client, string message)
            => client.SendCommandAsync(EvrimaRconCommand.Announce, message);

        /// <summary>Sends a direct message to a specific player.</summary>
        /// <param name="client"></param>
        /// <param name="player">The player to message (EOS ID, Steam ID, or player name)</param>
        /// <param name="message">The message to send</param>
        public static Task DirectMessage(this EvrimaRconClient client, string player, string message)
            => client.SendCommandAsync(EvrimaRconCommand.DirectMessage, $"{player},{message}");

        /// <summary>Wipes all corpses on the server.</summary>
        public static Task WipeCorpses(this EvrimaRconClient client)
            => client.SendCommandAsync(EvrimaRconCommand.WipeCorpses);

        /// <summary>
        /// Sets a playable class to enabled or disabled.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="className">The class name</param>
        /// <param name="enabled"><c>true</c> to enable the class, <c>false</c> to disable it</param>
        /// <returns>The raw server response</returns>
        public static Task<string> UpdatePlayables(this EvrimaRconClient client, string className, bool enabled)
            => client.SendCommandAsync(EvrimaRconCommand.UpdatePlayables,
                $"{className}:{(enabled ? "enabled" : "disabled")}");

        /// <summary>
        /// Sets multiple playable classes at once, each with an enabled or disabled state.
        /// Builds an argument string in the format <c>class:enabled,class:disabled,...</c>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="playables">
        /// A dictionary mapping class names to their desired state
        /// (<c>true</c> = enabled, <c>false</c> = disabled).
        /// </param>
        /// <returns>The raw server response</returns>
        public static Task<string> UpdatePlayables(this EvrimaRconClient client,
            IDictionary<string, bool> playables)
        {
            var argument = string.Join(",",
                playables.Select(kv => $"{kv.Key}:{(kv.Value ? "enabled" : "disabled")}"));
            return client.SendCommandAsync(EvrimaRconCommand.UpdatePlayables, argument);
        }

        /// <summary>
        /// Sets playable classes using a raw argument string in the format <c>class:enabled/disabled</c>.
        /// Multiple entries can be comma-separated, e.g. <c>"Deinonychus:enabled,Herrerasaurus:disabled"</c>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="argument">Raw argument string</param>
        /// <returns>The raw server response</returns>
        public static Task<string> UpdatePlayables(this EvrimaRconClient client, string argument)
            => client.SendCommandAsync(EvrimaRconCommand.UpdatePlayables, argument);

        /// <summary>Bans a player from the server.</summary>
        /// <param name="client"></param>
        /// <param name="player">The player to ban (EOS ID, Steam ID, or player name)</param>
        /// <param name="reason">The reason for the ban</param>
        public static Task Ban(this EvrimaRconClient client, string player, string reason)
            => client.SendCommandAsync(EvrimaRconCommand.Ban, $"{player},{reason}");

        /// <summary>Kicks a player from the server.</summary>
        /// <param name="client"></param>
        /// <param name="player">The player to kick (EOS ID, Steam ID, or player name)</param>
        /// <param name="reason">The reason for the kick</param>
        public static Task Kick(this EvrimaRconClient client, string player, string reason)
            => client.SendCommandAsync(EvrimaRconCommand.Kick, $"{player},{reason}");

        /// <summary>Saves all game data on the server.</summary>
        /// <param name="client"></param>
        /// <param name="backupName">Optional backup name for the save</param>
        public static Task Save(this EvrimaRconClient client, string backupName = null)
            => client.SendCommandAsync(EvrimaRconCommand.Save, backupName ?? string.Empty);

        /// <summary>Enables or disables the server whitelist.</summary>
        /// <param name="client"></param>
        /// <param name="enabled"><c>true</c> to enable the whitelist, <c>false</c> to disable it</param>
        public static Task ToggleWhitelist(this EvrimaRconClient client, bool enabled)
            => client.SendCommandAsync(EvrimaRconCommand.ToggleWhitelist, enabled ? "1" : "0");

        /// <summary>Adds a player ID to the server whitelist.</summary>
        /// <param name="client"></param>
        /// <param name="playerIds">Comma-delimited string of player IDs to add</param>
        public static Task AddWhitelistIds(this EvrimaRconClient client, string playerIds)
            => client.SendCommandAsync(EvrimaRconCommand.AddWhitelistId, playerIds);

        /// <summary>Removes a player ID from the server whitelist.</summary>
        /// <param name="client"></param>
        /// <param name="playerIds">Comma-delimited string of player IDs to remove</param>
        public static Task RemoveWhitelistIds(this EvrimaRconClient client, string playerIds)
            => client.SendCommandAsync(EvrimaRconCommand.RemoveWhitelistId, playerIds);

        /// <summary>Enables or disables global chat.</summary>
        /// <param name="client"></param>
        /// <param name="enabled"><c>true</c> to enable global chat, <c>false</c> to disable it</param>
        public static Task ToggleGlobalChat(this EvrimaRconClient client, bool enabled)
            => client.SendCommandAsync(EvrimaRconCommand.ToggleGlobalChat, enabled ? "1" : "0");

        /// <summary>Enables or disables humans on the server.</summary>
        /// <param name="client"></param>
        /// <param name="enabled"><c>true</c> to enable humans, <c>false</c> to disable them</param>
        public static Task ToggleHumans(this EvrimaRconClient client, bool enabled)
            => client.SendCommandAsync(EvrimaRconCommand.ToggleHumans, enabled ? "1" : "0");

        /// <summary>Enables or disables AI spawns on the server.</summary>
        /// <param name="client"></param>
        /// <param name="enabled"><c>true</c> to enable AI, <c>false</c> to disable it</param>
        public static Task ToggleAI(this EvrimaRconClient client, bool enabled)
            => client.SendCommandAsync(EvrimaRconCommand.ToggleAI, enabled ? "1" : "0");

        /// <summary>Updates the AI spawn list by specifying classes to disable.</summary>
        /// <param name="client"></param>
        /// <param name="aiClasses">Comma-delimited list of AI classes to disable</param>
        public static Task DisableAIClasses(this EvrimaRconClient client, string aiClasses)
            => client.SendCommandAsync(EvrimaRconCommand.DisableAIClasses, aiClasses);

        /// <summary>Sets the AI spawn density (0.0 – 1.0).</summary>
        /// <param name="client"></param>
        /// <param name="density">Density value between 0.0 and 1.0</param>
        public static Task SetAIDensity(this EvrimaRconClient client, float density)
            => client.SendCommandAsync(EvrimaRconCommand.AIDensity,
                density.ToString("0.0##", CultureInfo.InvariantCulture));

        /// <summary>Sets the AI spawn density (0.0 – 1.0).</summary>
        /// <param name="client"></param>
        /// <param name="density">Density value as a string, e.g. <c>"0.5"</c></param>
        public static Task SetAIDensity(this EvrimaRconClient client, string density)
            => client.SendCommandAsync(EvrimaRconCommand.AIDensity, density);

        // ── Parsed / typed commands ──────────────────────────────────────────────

        /// <summary>
        /// Gets the list of online players on the server.
        /// </summary>
        /// <returns>Collection of online players</returns>
        public static async Task<List<ServerPlayer>> GetPlayerList(this EvrimaRconClient client)
        {
            var result = new List<ServerPlayer>();

            var response = await client.SendCommandAsync(EvrimaRconCommand.PlayerList);
            var lines = response.Split('\n')
                                .Skip(1)
                                .ToArray();

            if (lines.Length != 2)
                return result;

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
        /// Retrieves and parses player data into a typed list of <see cref="PlayerData"/> objects.
        /// Each entry corresponds to one online player and contains stats such as location,
        /// class, health, hunger, mutation slots, and more.
        /// </summary>
        /// <returns>
        /// A list of <see cref="PlayerData"/>, one per online player.
        /// Returns an empty list when no players are online or the response cannot be parsed.
        /// </returns>
        public static async Task<List<PlayerData>> GetPlayerData(this EvrimaRconClient client)
        {
            var response = await client.SendCommandAsync(EvrimaRconCommand.GetPlayerData);
            return ParsePlayerDataResponse(response);
        }

        // ── PlayerData parsing helpers ───────────────────────────────────────────

        internal static List<PlayerData> ParsePlayerDataResponse(string response)
        {
            var result = new List<PlayerData>();
            if (string.IsNullOrWhiteSpace(response))
                return result;

            foreach (var rawLine in response.Split('\n'))
            {
                var line = rawLine.Trim('\r', '\n', ' ');

                // Skip header line (e.g. "[2026.04.01-14.50.30] PlayerData"), sentinel lines,
                // and blank lines.
                if (string.IsNullOrWhiteSpace(line) ||
                    line.StartsWith("[") ||
                    line == "PlayerData" ||
                    line == "PlayerDataEnd")
                    continue;

                var player = TryParsePlayerDataLine(line);
                if (player != null)
                    result.Add(player);
            }

            return result;
        }

        private static PlayerData TryParsePlayerDataLine(string line)
        {
            try
            {
                // Build a key→value dictionary, splitting on ", " while respecting [...] brackets.
                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var field in SplitPlayerDataLine(line))
                {
                    int sep = field.IndexOf(": ", StringComparison.Ordinal);
                    if (sep < 0) continue;
                    dict[field.Substring(0, sep).Trim()] = field.Substring(sep + 2).Trim();
                }

                return new PlayerData
                {
                    Name               = GetField(dict, "Name"),
                    PlayerId           = GetField(dict, "PlayerID"),
                    Gender             = GetField(dict, "Gender"),
                    Location           = ParseLocation(GetField(dict, "Location")),
                    Class              = GetField(dict, "Class"),
                    Growth             = ParseFloat(GetField(dict, "Growth")),
                    Health             = ParseFloat(GetField(dict, "Health")),
                    Stamina            = ParseFloat(GetField(dict, "Stamina")),
                    Hunger             = ParseFloat(GetField(dict, "Hunger")),
                    Thirst             = ParseFloat(GetField(dict, "Thirst")),
                    MutationSlots      = ParseSlots(GetField(dict, "MutationSlots")),
                    ParentMutationSlots  = ParseSlots(GetField(dict, "ParentMutationSlots")),
                    ElderMutationSlotsA  = ParseSlots(GetField(dict, "ElderMutationSlotsA")),
                    ElderMutationSlotsB  = ParseSlots(GetField(dict, "ElderMutationSlotsB")),
                    PrimeElder         = string.Equals(GetField(dict, "PrimeElder"), "true",
                                             StringComparison.OrdinalIgnoreCase)
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Splits a player data line on commas that are NOT inside square brackets.
        /// This correctly handles MutationSlots values such as [1=None,2=None,3=None,4=None].
        /// </summary>
        private static IEnumerable<string> SplitPlayerDataLine(string line)
        {
            int depth = 0;
            int start = 0;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if      (c == '[') depth++;
                else if (c == ']') depth--;
                else if (c == ',' && depth == 0)
                {
                    yield return line.Substring(start, i - start).Trim();
                    start = i + 1;
                }
            }
            if (start < line.Length)
                yield return line.Substring(start).Trim();
        }

        /// <summary>Parses "X=1.0 Y=2.0 Z=3.0" into a <see cref="PlayerLocation"/>.</summary>
        private static PlayerLocation ParseLocation(string locationStr)
        {
            var loc = new PlayerLocation();
            if (string.IsNullOrWhiteSpace(locationStr)) return loc;

            foreach (var part in locationStr.Split(' '))
            {
                int eq = part.IndexOf('=');
                if (eq < 0) continue;
                string axis = part.Substring(0, eq);
                double val  = ParseDouble(part.Substring(eq + 1));
                switch (axis)
                {
                    case "X": loc.X = val; break;
                    case "Y": loc.Y = val; break;
                    case "Z": loc.Z = val; break;
                }
            }
            return loc;
        }

        /// <summary>Parses "[1=None,2=None,...]" into a slot-index → value dictionary.</summary>
        private static Dictionary<int, string> ParseSlots(string slotsStr)
        {
            var result = new Dictionary<int, string>();
            if (string.IsNullOrWhiteSpace(slotsStr)) return result;

            string inner = slotsStr.Trim('[', ']');
            if (string.IsNullOrWhiteSpace(inner)) return result;

            foreach (var entry in inner.Split(','))
            {
                int eq = entry.IndexOf('=');
                if (eq < 0) continue;
                if (int.TryParse(entry.Substring(0, eq), out int slot))
                    result[slot] = entry.Substring(eq + 1);
            }
            return result;
        }

        private static string GetField(Dictionary<string, string> dict, string key)
            => dict.TryGetValue(key, out string value) ? value : string.Empty;

        private static float ParseFloat(string value)
        {
            return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result)
                ? result : 0f;
        }

        private static double ParseDouble(string value)
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result)
                ? result : 0d;
        }

        private static int ParseInt(string value)
            => int.TryParse(value, out int result) ? result : 0;

        private static bool ParseBool(string value)
            => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

        // ── GetServerDetails ───────────────────────────────────────────────

        /// <summary>
        /// Retrieves and parses the server details into a typed <see cref="ServerDetails"/> object.
        /// </summary>
        /// <returns>
        /// A <see cref="ServerDetails"/> instance populated from the server response.
        /// Returns a default (empty) instance when the response cannot be parsed.
        /// </returns>
        public static async Task<ServerDetails> GetServerDetails(this EvrimaRconClient client)
        {
            var response = await client.SendCommandAsync(EvrimaRconCommand.ServerDetails);
            return ParseServerDetailsResponse(response);
        }

        // ── ServerDetails parsing helpers ────────────────────────────────────────

        internal static ServerDetails ParseServerDetailsResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return new ServerDetails();

            foreach (var rawLine in response.Split('\n'))
            {
                // Strip the "[timestamp] ServerDetails" prefix.
                // The server may emit the data on the same line as the header
                // (e.g. "[2026.04.01-15.25.40] ServerDetailsServerName: ...")
                // or on a separate line; the regex handles both cases.
                string line = Regex.Replace(
                    rawLine.Trim('\r', '\n', ' '),
                    @"^\[.*?\]\s*ServerDetails\s*",
                    string.Empty,
                    RegexOptions.IgnoreCase);

                if (string.IsNullOrWhiteSpace(line)) continue;

                var parsed = TryParseServerDetailsLine(line);
                if (parsed != null) return parsed;
            }

            return new ServerDetails();
        }

        private static ServerDetails TryParseServerDetailsLine(string line)
        {
            try
            {
                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                // Split on ", " that is immediately followed by an identifier then ": ".
                // This avoids false splits inside values like server names that may
                // themselves contain commas (e.g. "My Server, The Best").
                var fields = Regex.Split(line, @",\s+(?=[A-Za-z]\w*\s*:\s)");
                foreach (var field in fields)
                {
                    int sep = field.IndexOf(": ", StringComparison.Ordinal);
                    if (sep < 0) continue;
                    dict[field.Substring(0, sep).Trim()] = field.Substring(sep + 2).Trim();
                }

                if (dict.Count == 0) return null;

                return new ServerDetails
                {
                    Name                         = GetField(dict, "ServerName"),
                    Password                     = GetField(dict, "ServerPassword"),
                    Map                          = GetField(dict, "ServerMap"),
                    MaxPlayers                   = ParseInt(GetField(dict, "ServerMaxPlayers")),
                    CurrentPlayers               = ParseInt(GetField(dict, "ServerCurrentPlayers")),
                    MutationsEnabled             = ParseBool(GetField(dict, "bEnableMutations")),
                    HumansEnabled                = ParseBool(GetField(dict, "bEnableHumans")),
                    PasswordProtected            = ParseBool(GetField(dict, "bServerPassword")),
                    QueueEnabled                 = ParseBool(GetField(dict, "bQueueEnabled")),
                    WhitelistEnabled             = ParseBool(GetField(dict, "bServerWhitelist")),
                    SpawnAIEnabled               = ParseBool(GetField(dict, "bSpawnAI")),
                    AllowRecordingReplay         = ParseBool(GetField(dict, "bAllowRecordingReplay")),
                    RegionSpawningEnabled        = ParseBool(GetField(dict, "bUseRegionSpawning")),
                    RegionSpawnCooldownEnabled   = ParseBool(GetField(dict, "bUseRegionSpawnCooldown")),
                    RegionSpawnCooldownTimeSeconds = ParseInt(GetField(dict, "RegionSpawnCooldownTimeSeconds")),
                    DayLengthMinutes             = ParseInt(GetField(dict, "ServerDayLengthMinutes")),
                    NightLengthMinutes           = ParseInt(GetField(dict, "ServerNightLengthMinutes")),
                    GlobalChatEnabled            = ParseBool(GetField(dict, "bEnableGlobalChat"))
                };
            }
            catch
            {
                return null;
            }
        }
    }
}

