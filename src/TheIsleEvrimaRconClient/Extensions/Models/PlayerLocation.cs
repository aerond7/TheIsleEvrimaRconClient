namespace TheIsleEvrimaRconClient.Extensions.Models
{
    /// <summary>
    /// Represents a player's world-space coordinates as reported by the GetPlayerData command.
    /// </summary>
    public class PlayerLocation
    {
        /// <summary>X coordinate in world-space units.</summary>
        public double X { get; set; }

        /// <summary>Y coordinate in world-space units.</summary>
        public double Y { get; set; }

        /// <summary>Z coordinate (height) in world-space units.</summary>
        public double Z { get; set; }

        /// <inheritdoc/>
        public override string ToString() => $"X={X} Y={Y} Z={Z}";
    }
}

