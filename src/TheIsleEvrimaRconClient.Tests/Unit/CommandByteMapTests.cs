using TheIsleEvrimaRconClient.Internal;
using Xunit;

namespace TheIsleEvrimaRconClient.Tests.Unit
{
    public class CommandByteMapTests
    {
        [Fact]
        public void Map_ContainsAllEighteenCommands()
        {
            Assert.Equal(18, CommandByteMap.Map.Count);
        }

        [Theory]
        [InlineData("announce",        0x10)]
        [InlineData("directmessage",   0x11)]
        [InlineData("serverdetails",   0x12)]
        [InlineData("wipecorpses",     0x13)]
        [InlineData("updateplayables", 0x15)]
        [InlineData("ban",             0x20)]
        [InlineData("kick",            0x30)]
        [InlineData("playerlist",      0x40)]
        [InlineData("save",            0x50)]
        [InlineData("getplayerdata",   0x77)]
        [InlineData("togglewhitelist", 0x81)]
        [InlineData("addwhitelistid",  0x82)]
        [InlineData("removewhitelistid", 0x83)]
        [InlineData("toggleglobalchat",  0x84)]
        [InlineData("togglehumans",    0x86)]
        [InlineData("toggleai",        0x90)]
        [InlineData("disableaiclasses", 0x91)]
        [InlineData("aidensity",       0x92)]
        public void Map_HasCorrectByteForEachCommand(string command, byte expectedByte)
        {
            Assert.True(CommandByteMap.Map.TryGetValue(command, out byte actual),
                $"Command '{command}' was not found in the map");
            Assert.Equal(expectedByte, actual);
        }

        [Theory]
        [InlineData("unknowncommand")]
        [InlineData("Announce")]        // map keys are lowercase
        [InlineData("PLAYERLIST")]
        [InlineData("")]
        public void Map_DoesNotContainUnknownOrMixedCaseKeys(string key)
        {
            Assert.False(CommandByteMap.Map.ContainsKey(key));
        }
    }
}

