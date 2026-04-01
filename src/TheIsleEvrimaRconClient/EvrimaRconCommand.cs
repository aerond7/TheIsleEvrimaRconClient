namespace TheIsleEvrimaRconClient
{
    /// <summary>
    /// Available RCON commands as defined in the RCON spec v0.17.54.
    /// </summary>
    public enum EvrimaRconCommand
    {
        /// <summary>Announces a message on the server displayed to all players.</summary>
        Announce,
        /// <summary>Sends a direct message to a specific player.</summary>
        DirectMessage,
        /// <summary>Retrieves all the current server settings.</summary>
        ServerDetails,
        /// <summary>Wipes all corpses on the server.</summary>
        WipeCorpses,
        /// <summary>Modifies the playable classes.</summary>
        UpdatePlayables,
        /// <summary>Bans a player by player id (EOS or Steam).</summary>
        Ban,
        /// <summary>Kicks a player by player id (EOS or Steam).</summary>
        Kick,
        /// <summary>Returns a list of all players.</summary>
        PlayerList,
        /// <summary>Saves all game data.</summary>
        Save,
        /// <summary>Retrieves info about each player like location, character stats etc.</summary>
        GetPlayerData,
        /// <summary>Turns the server whitelist on/off.</summary>
        ToggleWhitelist,
        /// <summary>Adds player id(s) to the server whitelist.</summary>
        AddWhitelistId,
        /// <summary>Removes player id(s) from the server whitelist.</summary>
        RemoveWhitelistId,
        /// <summary>Turns global chat on/off.</summary>
        ToggleGlobalChat,
        /// <summary>Turns humans on/off.</summary>
        ToggleHumans,
        /// <summary>Turns AI spawns on/off.</summary>
        ToggleAI,
        /// <summary>Updates the allowable AI spawn list.</summary>
        DisableAIClasses,
        /// <summary>Adjusts the AI spawn density.</summary>
        AIDensity
    }
}
