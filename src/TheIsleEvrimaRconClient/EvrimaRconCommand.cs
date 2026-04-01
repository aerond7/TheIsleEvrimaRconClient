namespace TheIsleEvrimaRconClient
{
    /// <summary>
    /// Available RCON commands as defined in the RCON server specification.
    /// </summary>
    public enum EvrimaRconCommand
    {
        /// <summary>Announces a message on the server displayed to all players.</summary>
        Announce,
        /// <summary>
        /// Sends a direct message to a specific player.
        /// Argument format: <c>player,message</c> where <c>player</c> is an EOS ID, Steam ID, or player name.
        /// </summary>
        DirectMessage,
        /// <summary>Retrieves all the current server settings.</summary>
        ServerDetails,
        /// <summary>Wipes all corpses on the server.</summary>
        WipeCorpses,
        /// <summary>
        /// Sets a playable class to enabled or disabled.
        /// Argument format: <c>class:enabled</c> or <c>class:disabled</c>.
        /// </summary>
        UpdatePlayables,
        /// <summary>
        /// Bans a player from the server.
        /// Argument format: <c>player,reason</c> where <c>player</c> is an EOS ID, Steam ID, or player name.
        /// </summary>
        Ban,
        /// <summary>
        /// Kicks a player from the server.
        /// Argument format: <c>player,reason</c> where <c>player</c> is an EOS ID, Steam ID, or player name.
        /// </summary>
        Kick,
        /// <summary>Returns a list of all online players.</summary>
        PlayerList,
        /// <summary>Saves all game data. Optionally accepts a backup name as argument.</summary>
        Save,
        /// <summary>Retrieves detailed stats per player (location, character stats, etc.).</summary>
        GetPlayerData,
        /// <summary>
        /// Toggles the server whitelist.
        /// Argument: <c>0</c> to disable, <c>1</c> to enable.
        /// </summary>
        ToggleWhitelist,
        /// <summary>Adds a player ID to the server whitelist.</summary>
        AddWhitelistId,
        /// <summary>Removes a player ID from the server whitelist.</summary>
        RemoveWhitelistId,
        /// <summary>
        /// Toggles global chat.
        /// Argument: <c>0</c> to disable, <c>1</c> to enable.
        /// </summary>
        ToggleGlobalChat,
        /// <summary>
        /// Toggles humans on the server.
        /// Argument: <c>0</c> to disable, <c>1</c> to enable.
        /// </summary>
        ToggleHumans,
        /// <summary>
        /// Toggles AI spawns on the server.
        /// Argument: <c>0</c> to disable, <c>1</c> to enable.
        /// </summary>
        ToggleAI,
        /// <summary>Updates the AI spawn list. Argument format: <c>class</c> or <c>class,class,...</c>.</summary>
        DisableAIClasses,
        /// <summary>Sets the AI spawn density. Argument: a value between <c>0.0</c> and <c>1.0</c>.</summary>
        AIDensity
    }
}
