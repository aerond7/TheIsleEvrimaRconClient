﻿using System.Collections.Generic;

namespace TheIsleEvrimaRconClient.Internal
{
    internal static class CommandByteMap
    {
        internal static readonly Dictionary<string, byte> Map = new Dictionary<string, byte>
        {
            {"announce", 0x10},
            {"updateplayables", 0x15},
            {"ban", 0x20},
            {"kick", 0x30},
            {"playerlist", 0x40},
            {"save", 0x50},
            {"custom", 0x70}
        };
    }
}
