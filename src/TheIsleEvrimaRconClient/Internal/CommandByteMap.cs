using System.Collections.Generic;

namespace TheIsleEvrimaRconClient.Internal
{
    internal static class CommandByteMap
    {
        internal static readonly Dictionary<string, byte> Map = new Dictionary<string, byte>
        {
            {"announce", 0x10},
            {"directmessage", 0x11},
            {"serverdetails", 0x12},
            {"wipecorpses", 0x13},
            {"updateplayables", 0x15},
            {"ban", 0x20},
            {"kick", 0x30},
            {"playerlist", 0x40},
            {"save", 0x50},
            {"getplayerdata", 0x77},
            {"togglewhitelist", 0x81},
            {"addwhitelistid", 0x82},
            {"removewhitelistid", 0x83},
            {"toggleglobalchat", 0x84},
            {"togglehumans", 0x86},
            {"toggleai", 0x90},
            {"disableaiclasses", 0x91},
            {"aidensity", 0x92}
        };
    }
}
