namespace TheIsleEvrimaRconClient.Extensions.Models
{
    /// <summary>
    /// Typed representation of the server details returned by the ServerDetails RCON command.
    /// </summary>
    public class ServerDetails
    {
        /// <summary>The server's display name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>The RCON / admin password (as reported by the server).</summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>The active map name (e.g. "Gateway").</summary>
        public string Map { get; set; } = string.Empty;

        /// <summary>Maximum number of player slots.</summary>
        public int MaxPlayers { get; set; }

        /// <summary>Number of players currently connected.</summary>
        public int CurrentPlayers { get; set; }

        /// <summary>Whether dinosaur mutations are enabled.</summary>
        public bool MutationsEnabled { get; set; }

        /// <summary>Whether humans are enabled on the server.</summary>
        public bool HumansEnabled { get; set; }

        /// <summary>Whether a join password is required (bServerPassword).</summary>
        public bool PasswordProtected { get; set; }

        /// <summary>Whether the player queue is enabled.</summary>
        public bool QueueEnabled { get; set; }

        /// <summary>Whether the server whitelist is active.</summary>
        public bool WhitelistEnabled { get; set; }

        /// <summary>Whether AI spawning is enabled.</summary>
        public bool SpawnAIEnabled { get; set; }

        /// <summary>Whether recording / replay is allowed.</summary>
        public bool AllowRecordingReplay { get; set; }

        /// <summary>Whether region-based spawning is used.</summary>
        public bool RegionSpawningEnabled { get; set; }

        /// <summary>Whether the region spawn cooldown is active.</summary>
        public bool RegionSpawnCooldownEnabled { get; set; }

        /// <summary>Region spawn cooldown duration in seconds.</summary>
        public int RegionSpawnCooldownTimeSeconds { get; set; }

        /// <summary>In-game day length in minutes.</summary>
        public int DayLengthMinutes { get; set; }

        /// <summary>In-game night length in minutes.</summary>
        public int NightLengthMinutes { get; set; }

        /// <summary>Whether the global chat channel is enabled.</summary>
        public bool GlobalChatEnabled { get; set; }
    }
}


