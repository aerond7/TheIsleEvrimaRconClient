# TheIsleEvrimaRconClient
RCON client for The Isle Evrima, aligned with [RCON spec v0.17.54](https://github.com/aerond7/TheIsleEvrimaRconClient).

Use `EvrimaRconClientConfiguration` to configure host, port, password, and timeout; the client is constructed from that configuration object.

# NuGet
https://www.nuget.org/packages/TheIsleEvrimaRconClient

https://www.nuget.org/packages/TheIsleEvrimaRconClient.Extensions

# Example usage
```csharp
using TheIsleEvrimaRconClient;

// Construct the client from configuration
var rcon = new EvrimaRconClient(new EvrimaRconClientConfiguration(IPAddress.Parse("127.0.0.1"), 8888, "rcon_password"));
// or using object initializer
var rcon = new EvrimaRconClient(new EvrimaRconClientConfiguration
{
    Host = IPAddress.Parse("127.0.0.1"),
    Port = 8888,
    Password = "rcon_password"
});

// Connecting and executing commands
var connected = await rcon.ConnectAsync();
if (connected)
{
    // Using the enum
    await rcon.SendCommandAsync(EvrimaRconCommand.Announce, "Hello World");
    // Using a string command name
    await rcon.SendCommandAsync("announce", "Hello World");
    // Using a full command string (command name + argument)
    await rcon.SendCommandAsync("announce Hello World");

    var playerList = await rcon.SendCommandAsync(EvrimaRconCommand.PlayerList);
}
```

# Available Commands (`EvrimaRconCommand`)
All commands are defined in the `EvrimaRconCommand` enum and map directly to the RCON spec v0.17.54.

| Command | Description |
| --- | --- |
| `Announce` | Announces a message displayed to all players. |
| `DirectMessage` | Sends a direct message to a specific player. |
| `ServerDetails` | Retrieves all current server settings. |
| `WipeCorpses` | Wipes all corpses on the server. |
| `UpdatePlayables` | Modifies the playable classes. |
| `Ban` | Bans a player by player id (EOS or Steam). |
| `Kick` | Kicks a player by player id (EOS or Steam). |
| `PlayerList` | Returns a list of all online players. |
| `Save` | Saves all game data. |
| `GetPlayerData` | Retrieves info about each player (location, character stats, etc.). |
| `ToggleWhitelist` | Turns the server whitelist on/off. |
| `AddWhitelistId` | Adds player id(s) to the server whitelist. |
| `RemoveWhitelistId` | Removes player id(s) from the server whitelist. |
| `ToggleGlobalChat` | Turns global chat on/off. |
| `ToggleHumans` | Turns humans on/off. |
| `ToggleAI` | Turns AI spawns on/off. |
| `DisableAIClasses` | Updates the allowable AI spawn list. |
| `AIDensity` | Adjusts the AI spawn density. |

# TheIsleEvrimaRconClient.Extensions
A simple extension method wrapper over `EvrimaRconClient` that provides a strongly-typed, easy-to-use interface for all RCON commands.

```csharp
using TheIsleEvrimaRconClient;
using TheIsleEvrimaRconClient.Extensions;

var rcon = new EvrimaRconClient(new EvrimaRconClientConfiguration(IPAddress.Parse("127.0.0.1"), 8888, "rcon_password"));
await rcon.ConnectAsync();

await rcon.Announce("Hello World!");
await rcon.DirectMessage("eos_player_id", "Hello!");
await rcon.Ban("eos_player_id");
await rcon.Kick("eos_player_id");
await rcon.Save();
await rcon.WipeCorpses();
await rcon.ToggleWhitelist();
await rcon.AddWhitelistIds("id1,id2");
await rcon.RemoveWhitelistIds("id1");
await rcon.ToggleGlobalChat();
await rcon.ToggleHumans();
await rcon.ToggleAI();
await rcon.DisableAIClasses("Troodon,Herrerasaurus");
await rcon.SetAIDensity("0.5");
await rcon.UpdatePlayables("Deinonychus,Herrerasaurus");

var serverDetails = await rcon.GetServerDetails();
var playerData = await rcon.GetPlayerData();
var players = await rcon.GetPlayerList(); // returns List<ServerPlayer>
```

**All methods are asynchronous and return `Task` or `Task<T>`.**

| Method | Returns | Description |
| --- | --- | --- |
| `Announce(message)` | void | Announces a message to all players. |
| `DirectMessage(playerId, message)` | void | Sends a direct message to a specific player. |
| `GetServerDetails()` | string | Returns all current server settings. |
| `WipeCorpses()` | void | Wipes all corpses on the server. |
| `UpdatePlayables()` | string | Sends the updateplayables command. Returns the server response. |
| `UpdatePlayables(playables)` | string | Sets playable classes from a comma-delimited string. Returns the server response. |
| `Ban(playerId)` | void | Bans a player by player id (EOS or Steam). |
| `Kick(playerId)` | void | Kicks a player by player id (EOS or Steam). |
| `GetPlayerList()` | `List<ServerPlayer>` | Returns a parsed list of online players. |
| `Save()` | void | Saves all game data. |
| `GetPlayerData()` | string | Returns info about each player (location, character stats, etc.). |
| `ToggleWhitelist()` | void | Toggles the server whitelist on/off. |
| `AddWhitelistIds(playerIds)` | void | Adds player id(s) to the whitelist (comma-delimited). |
| `RemoveWhitelistIds(playerIds)` | void | Removes player id(s) from the whitelist (comma-delimited). |
| `ToggleGlobalChat()` | void | Toggles global chat on/off. |
| `ToggleHumans()` | void | Toggles humans on/off. |
| `ToggleAI()` | void | Toggles AI spawns on/off. |
| `DisableAIClasses(aiClasses)` | void | Updates the allowable AI spawn list (comma-delimited). |
| `SetAIDensity(density)` | void | Adjusts the AI spawn density. |

# TheIsleEvrimaRconClient.Extensions.Models

## ServerPlayer
Represents an online player on the server.

| Property | Type | Description |
| --- | --- | --- |
| `PlayerId` | string | The player's id (EOS or Steam). |
| `PlayerName` | string | The player's username. |

