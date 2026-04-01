namespace TheIsleEvrimaRconClient.Extensions.Models
{
    /// <summary>
    /// Represents a player on the server, as returned by the PlayerList command.
    /// </summary>
    public class ServerPlayer
    {
        /// <summary>
        /// The player's id (EOS or Steam).
        /// </summary>
        public string PlayerId { get; set; }

        /// <summary>
        /// The player's name.
        /// </summary>
        public string PlayerName { get; set; }
    }
}

