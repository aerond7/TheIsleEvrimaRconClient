# TheIsleEvrimaRconClient

RCON client for The Isle Evrima, aligned with [RCON spec v0.17.54](https://github.com/aerond7/TheIsleEvrimaRconClient).

Use `EvrimaRconClientConfiguration` to configure host, port, password, and timeout.
The client is constructed from that configuration object.

## NuGet

https://www.nuget.org/packages/TheIsleEvrimaRconClient

> **Note:** Extension methods and models (previously a separate `TheIsleEvrimaRconClient.Extensions`
> package) are now bundled in the main package under the `TheIsleEvrimaRconClient.Extensions`
> namespace. No additional package is required.

---

## Basic usage

```csharp
using TheIsleEvrimaRconClient;

var config = new EvrimaRconClientConfiguration(IPAddress.Parse("127.0.0.1"), 8888, "rcon_password");
// or via object initializer
var config = new EvrimaRconClientConfiguration
{
    Host     = IPAddress.Parse("127.0.0.1"),
    Port     = 8888,
    Password = "rcon_password"
};

using var rcon = new EvrimaRconClient(config);
bool connected = await rcon.ConnectAsync();

if (connected)
{
    // Raw command via enum
    await rcon.SendCommandAsync(EvrimaRconCommand.Announce, "Hello World");
    // Raw command via string
    await rcon.SendCommandAsync("announce", "Hello World");
    // Raw command via combined string
    await rcon.SendCommandAsync("announce Hello World");
}
```

---

## Available commands (`EvrimaRconCommand`)

All commands are defined in the `EvrimaRconCommand` enum and map directly to RCON spec v0.17.54.

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

---

## Extensions (`TheIsleEvrimaRconClient.Extensions`)

A strongly-typed wrapper over `EvrimaRconClient` that provides a clean method-per-command API.
Three methods return parsed, typed objects instead of raw strings:

- `GetPlayerList()` → `List<ServerPlayer>`
- `GetPlayerData()` → `List<PlayerData>`
- `GetServerDetails()` → `ServerDetails`

```csharp
using TheIsleEvrimaRconClient;
using TheIsleEvrimaRconClient.Extensions;
using TheIsleEvrimaRconClient.Extensions.Models;

using var rcon = new EvrimaRconClient(config);
await rcon.ConnectAsync();

// ── Fire-and-forget ──────────────────────────────────────────────────────────
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

// ── Typed responses ──────────────────────────────────────────────────────────
List<ServerPlayer> players = await rcon.GetPlayerList();
foreach (var p in players)
    Console.WriteLine($"{p.PlayerName} ({p.PlayerId})");

List<PlayerData> playerData = await rcon.GetPlayerData();
foreach (var p in playerData)
    Console.WriteLine($"{p.Name} — {p.Class} growth:{p.Growth:P0} hp:{p.Health:P0} @ {p.Location}");

ServerDetails details = await rcon.GetServerDetails();
Console.WriteLine($"{details.Name} on {details.Map} — {details.CurrentPlayers}/{details.MaxPlayers} players");
```

**All methods are asynchronous and return `Task` or `Task<T>`.**

| Method | Returns | Description |
| --- | --- | --- |
| `Announce(message)` | `void` | Announces a message to all players. |
| `DirectMessage(playerId, message)` | `void` | Sends a direct message to a specific player. |
| `Ban(playerId)` | `void` | Bans a player by player id (EOS or Steam). |
| `Kick(playerId)` | `void` | Kicks a player by player id (EOS or Steam). |
| `Save()` | `void` | Saves all game data. |
| `WipeCorpses()` | `void` | Wipes all corpses on the server. |
| `ToggleWhitelist()` | `void` | Toggles the server whitelist on/off. |
| `AddWhitelistIds(playerIds)` | `void` | Adds player id(s) to the whitelist (comma-delimited). |
| `RemoveWhitelistIds(playerIds)` | `void` | Removes player id(s) from the whitelist (comma-delimited). |
| `ToggleGlobalChat()` | `void` | Toggles global chat on/off. |
| `ToggleHumans()` | `void` | Toggles humans on/off. |
| `ToggleAI()` | `void` | Toggles AI spawns on/off. |
| `DisableAIClasses(aiClasses)` | `void` | Updates the allowable AI spawn list (comma-delimited). |
| `SetAIDensity(density)` | `void` | Adjusts the AI spawn density. |
| `UpdatePlayables()` | `string` | Sends the UpdatePlayables command; returns the server response. |
| `UpdatePlayables(playables)` | `string` | Sets playable classes from a comma-delimited string; returns the server response. |
| `GetPlayerList()` | `List<ServerPlayer>` | Returns a parsed list of online players. |
| `GetPlayerData()` | `List<PlayerData>` | Returns player stats parsed into a typed list, one entry per online player. |
| `GetServerDetails()` | `ServerDetails` | Returns server configuration parsed into a typed object. |

---

## Models (`TheIsleEvrimaRconClient.Extensions.Models`)

### `ServerPlayer`

Represents an online player as returned by `GetPlayerList()`.

| Property | Type | Description |
| --- | --- | --- |
| `PlayerId` | `string` | The player's id (EOS or Steam). |
| `PlayerName` | `string` | The player's username. |

---

### `PlayerData`

Represents the full stats for a single online player as returned by `GetPlayerData()`.

| Property | Type | Description |
| --- | --- | --- |
| `Name` | `string` | The player's in-game name. |
| `PlayerId` | `string` | The player's platform id (EOS or Steam). |
| `Gender` | `string` | The player's gender (e.g. `Male`, `Female`). |
| `Location` | `PlayerLocation` | Current world-space coordinates. |
| `Class` | `string` | The player's current dinosaur class (e.g. `Tyrannosaurus`). |
| `Growth` | `float` | Growth factor (0.00 – 1.00). |
| `Health` | `float` | Current health as a fraction (0.00 – 1.00). |
| `Stamina` | `float` | Current stamina as a fraction (0.00 – 1.00). |
| `Hunger` | `float` | Current hunger as a fraction (0.00 – 1.00). |
| `Thirst` | `float` | Current thirst as a fraction (0.00 – 1.00). |
| `MutationSlots` | `Dictionary<int, string>` | Mutation slots (slot index → mutation name or `"None"`). |
| `ParentMutationSlots` | `Dictionary<int, string>` | Parent mutation slots. |
| `ElderMutationSlotsA` | `Dictionary<int, string>` | Elder mutation slots A. |
| `ElderMutationSlotsB` | `Dictionary<int, string>` | Elder mutation slots B. |
| `PrimeElder` | `bool` | Whether the player is a Prime Elder. |

### `PlayerLocation`

| Property | Type | Description |
| --- | --- | --- |
| `X` | `double` | X coordinate. |
| `Y` | `double` | Y coordinate. |
| `Z` | `double` | Z coordinate (height). |

---

### `ServerDetails`

Represents the server configuration as returned by `GetServerDetails()`.

| Property | Type | RCON key | Description |
| --- | --- | --- | --- |
| `Name` | `string` | `ServerName` | The server's display name. |
| `Password` | `string` | `ServerPassword` | The RCON / admin password (as reported by the server). |
| `Map` | `string` | `ServerMap` | The active map name (e.g. `Gateway`). |
| `MaxPlayers` | `int` | `ServerMaxPlayers` | Maximum number of player slots. |
| `CurrentPlayers` | `int` | `ServerCurrentPlayers` | Number of players currently connected. |
| `MutationsEnabled` | `bool` | `bEnableMutations` | Whether dinosaur mutations are enabled. |
| `HumansEnabled` | `bool` | `bEnableHumans` | Whether humans are enabled. |
| `PasswordProtected` | `bool` | `bServerPassword` | Whether a join password is required. |
| `QueueEnabled` | `bool` | `bQueueEnabled` | Whether the player queue is enabled. |
| `WhitelistEnabled` | `bool` | `bServerWhitelist` | Whether the server whitelist is active. |
| `SpawnAIEnabled` | `bool` | `bSpawnAI` | Whether AI spawning is enabled. |
| `AllowRecordingReplay` | `bool` | `bAllowRecordingReplay` | Whether recording / replay is allowed. |
| `RegionSpawningEnabled` | `bool` | `bUseRegionSpawning` | Whether region-based spawning is used. |
| `RegionSpawnCooldownEnabled` | `bool` | `bUseRegionSpawnCooldown` | Whether the region spawn cooldown is active. |
| `RegionSpawnCooldownTimeSeconds` | `int` | `RegionSpawnCooldownTimeSeconds` | Region spawn cooldown duration in seconds. |
| `DayLengthMinutes` | `int` | `ServerDayLengthMinutes` | In-game day length in minutes. |
| `NightLengthMinutes` | `int` | `ServerNightLengthMinutes` | In-game night length in minutes. |
| `GlobalChatEnabled` | `bool` | `bEnableGlobalChat` | Whether the global chat channel is enabled. |

