# TheIsleEvrimaRconClient

RCON client for The Isle Evrima.

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
    // Raw command via string name + argument
    await rcon.SendCommandAsync("announce", "Hello World");
    // Raw command via combined string
    await rcon.SendCommandAsync("announce Hello World");
}
```

---

## Available commands (`EvrimaRconCommand`)

All commands are defined in the `EvrimaRconCommand` enum.

| Command | Argument format | Description |
| --- | --- | --- |
| `Announce` | `message` | Announces a message displayed to all players. |
| `DirectMessage` | `player,message` | Sends a direct message to a player (EOS ID, Steam ID, or name). |
| `ServerDetails` | *(none)* | Retrieves all current server settings. |
| `WipeCorpses` | *(none)* | Wipes all corpses on the server. |
| `UpdatePlayables` | `class:enabled` or `class:disabled` | Sets a playable class to enabled or disabled. |
| `Ban` | `player,reason` | Bans a player (EOS ID, Steam ID, or name). |
| `Kick` | `player,reason` | Kicks a player (EOS ID, Steam ID, or name). |
| `PlayerList` | *(none)* | Returns a list of all online players. |
| `Save` | `backupName` *(optional)* | Saves all game data, with an optional backup name. |
| `GetPlayerData` | *(none)* | Retrieves detailed stats per player. |
| `ToggleWhitelist` | `0` or `1` | Disables or enables the server whitelist. |
| `AddWhitelistId` | `playerId` | Adds a player ID to the server whitelist. |
| `RemoveWhitelistId` | `playerId` | Removes a player ID from the server whitelist. |
| `ToggleGlobalChat` | `0` or `1` | Disables or enables global chat. |
| `ToggleHumans` | `0` or `1` | Disables or enables humans. |
| `ToggleAI` | `0` or `1` | Disables or enables AI spawns. |
| `DisableAIClasses` | `class[,class,...]` | Updates the AI spawn list. |
| `AIDensity` | `0.0`–`1.0` | Sets the AI spawn density. |

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

// player argument accepts EOS ID, Steam ID, or player name
await rcon.DirectMessage("PlayerName", "Hello!");
await rcon.Ban("PlayerName", "Cheating");
await rcon.Kick("eos_player_id", "AFK");

// Optional backup name
await rcon.Save();
await rcon.Save("backup_01");

await rcon.WipeCorpses();

// Toggle methods now require an explicit enabled/disabled state
await rcon.ToggleWhitelist(true);   // enable
await rcon.ToggleWhitelist(false);  // disable
await rcon.AddWhitelistIds("id1,id2");
await rcon.RemoveWhitelistIds("id1");
await rcon.ToggleGlobalChat(true);
await rcon.ToggleHumans(false);
await rcon.ToggleAI(true);

await rcon.DisableAIClasses("raptor,stego");

// AIDensity accepts float or string
await rcon.SetAIDensity(0.5f);
await rcon.SetAIDensity("0.5");

// UpdatePlayables: single class, multiple classes via dictionary, or raw string
await rcon.UpdatePlayables("raptor", true);           // raptor:enabled
await rcon.UpdatePlayables("raptor", false);        // raptor:disabled
await rcon.UpdatePlayables(new Dictionary<string, bool>    // raptor:enabled,stego:disabled,...
{
    { "raptor", true },
    { "stego", false },
});
await rcon.UpdatePlayables("raptor:enabled,stego:disabled"); // raw string

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
| `DirectMessage(player, message)` | `void` | Sends a direct message. `player` can be an EOS ID, Steam ID, or name. |
| `Ban(player, reason)` | `void` | Bans a player. `player` can be an EOS ID, Steam ID, or name. |
| `Kick(player, reason)` | `void` | Kicks a player. `player` can be an EOS ID, Steam ID, or name. |
| `Save()` | `void` | Saves all game data. |
| `Save(backupName)` | `void` | Saves all game data with the specified backup name. |
| `WipeCorpses()` | `void` | Wipes all corpses on the server. |
| `ToggleWhitelist(enabled)` | `void` | Enables (`true`) or disables (`false`) the server whitelist. |
| `AddWhitelistIds(playerIds)` | `void` | Adds player ID(s) to the whitelist (comma-delimited). |
| `RemoveWhitelistIds(playerIds)` | `void` | Removes player ID(s) from the whitelist (comma-delimited). |
| `ToggleGlobalChat(enabled)` | `void` | Enables (`true`) or disables (`false`) global chat. |
| `ToggleHumans(enabled)` | `void` | Enables (`true`) or disables (`false`) humans. |
| `ToggleAI(enabled)` | `void` | Enables (`true`) or disables (`false`) AI spawns. |
| `DisableAIClasses(aiClasses)` | `void` | Updates the AI spawn list (comma-delimited class names). |
| `SetAIDensity(density)` | `void` | Sets the AI spawn density. Accepts `float` (0.0–1.0) or `string`. |
| `UpdatePlayables(className, enabled)` | `string` | Sets a single playable class to enabled or disabled. |
| `UpdatePlayables(playables)` | `string` | Sets multiple playable classes at once via a `Dictionary<string, bool>`. |
| `UpdatePlayables(argument)` | `string` | Sends a raw `updateplayables` argument, e.g. `"Deinonychus:enabled,Rex:disabled"`. |
| `GetPlayerList()` | `List<ServerPlayer>` | Returns a parsed list of online players. |
| `GetPlayerData()` | `List<PlayerData>` | Returns player stats parsed into a typed list, one entry per online player. |
| `GetServerDetails()` | `ServerDetails` | Returns server configuration parsed into a typed object. |

---

## Models (`TheIsleEvrimaRconClient.Extensions.Models`)

### `ServerPlayer`

Represents an online player as returned by `GetPlayerList()`.

| Property | Type | Description |
| --- | --- | --- |
| `PlayerId` | `string` | The player's ID (EOS or Steam). |
| `PlayerName` | `string` | The player's username. |

---

### `PlayerData`

Represents the full stats for a single online player as returned by `GetPlayerData()`.

| Property | Type | Description |
| --- | --- | --- |
| `Name` | `string` | The player's in-game name. |
| `PlayerId` | `string` | The player's platform ID (EOS or Steam). |
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

