using System.Collections.Generic;

namespace TheIsleEvrimaRconClient.Extensions.Models
{
    /// <summary>
    /// Represents the parsed data for a single player, as returned by the GetPlayerData command.
    /// </summary>
    public class PlayerData
    {
        /// <summary>The player's in-game name.</summary>
        public string Name { get; set; }

        /// <summary>The player's platform ID (EOS or Steam).</summary>
        public string PlayerId { get; set; }

        /// <summary>The player's gender (e.g. Male, Female).</summary>
        public string Gender { get; set; }

        /// <summary>The player's current world-space location.</summary>
        public PlayerLocation Location { get; set; }

        /// <summary>The player's current dinosaur class (e.g. Tyrannosaurus).</summary>
        public string Class { get; set; }

        /// <summary>Growth factor in the range 0.00–1.00.</summary>
        public float Growth { get; set; }

        /// <summary>Current health as a fraction in the range 0.00–1.00.</summary>
        public float Health { get; set; }

        /// <summary>Current stamina as a fraction in the range 0.00–1.00.</summary>
        public float Stamina { get; set; }

        /// <summary>Current hunger as a fraction in the range 0.00–1.00.</summary>
        public float Hunger { get; set; }

        /// <summary>Current thirst as a fraction in the range 0.00–1.00.</summary>
        public float Thirst { get; set; }

        /// <summary>Mutation slots (slot index → mutation name or "None").</summary>
        public Dictionary<int, string> MutationSlots { get; set; }

        /// <summary>Parent mutation slots (slot index → mutation name or "None").</summary>
        public Dictionary<int, string> ParentMutationSlots { get; set; }

        /// <summary>Elder mutation slots A (slot index → mutation name or "None").</summary>
        public Dictionary<int, string> ElderMutationSlotsA { get; set; }

        /// <summary>Elder mutation slots B (slot index → mutation name or "None").</summary>
        public Dictionary<int, string> ElderMutationSlotsB { get; set; }

        /// <summary>Whether the player is a Prime Elder.</summary>
        public bool PrimeElder { get; set; }
    }
}

