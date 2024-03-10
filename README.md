# TheIsleEvrimaRconClient
RCON client for The Isle Evrima.

# NuGet
https://www.nuget.org/packages/TheIsleEvrimaRconClient

https://www.nuget.org/packages/TheIsleEvrimaRconClient.Extensions

# Example usage
```csharp
using TheIsleEvrimaRconClient;

// Option 1
var rcon = new EvrimaRconClient(new EvrimaRconClientConfiguration(IPAddress.Parse("127.0.0.1"), 8888, "rcon_password"));
// or
var rcon = new EvrimaRconClient(new EvrimaRconClientConfiguration
{
    Host = IPAddress.Parse("127.0.0.1"),
    Port = 8888,
    Password = "rcon_password"
});
// you may also pass a string as the host in the 1st option, but do note that behind the scenes it still parses the string into an instace of IPAddress, so any invalid IP will throw an invalid argument exception

// Option 2 (may become obsolete in the future)
var rcon = new EvrimaRconClient("127.0.0.1", 8888, "rcon_password");

// Connecting and executing commands
var connected = await rcon.ConnectAsync();
if (connected)
{
     await rcon.SendCommandAsync(EvrimaRconCommand.Announce, "Hello World");
     await rcon.SendCommandAsync("announce", "Hello World 2");
     var response = await rcon.SendCommandAsync(EvrimaRconCommand.PlayerList);
}
```

# TheIsleEvrimaRconClient.Extensions
This library is a simple extensions wrapper over the `EvrimaRconClient` which creates an easy to use interface to execute commands via the RCON.

**All methods are asynchronous and return Task.**

Currently implemented extension methods:
| Method | Returns | Description |
| --- | --- | --- |
| Announce | void | Sends the "announce" command to the server with a message that is displayed to all players. |
| UpdatePlayables | string | Sends the "updateplayables" command to the server. Returns the response from the server. |
| Ban | void | Sends the "ban" command to the server along with an EOS ID of a player that gets banned. |
| Kick | void | Sends the "kick" command to the server along with an EOS ID of a player that gets kicked. |
| GetPlayerList | List of [ServerPlayer](#serverplayer) | Sends the "playerlist" command to the server and automatically parses the response into a list of [ServerPlayer](#serverplayer) objects. |
| Save | void | Sends the "save" command to the server. |

# TheIsleEvrimaRconClient.Extensions.Models

## ServerPlayer
Object that represents a player on a server.

Properties:
| Name | Data type | Description |
| --- | --- | --- |
| EosId | string | The EOS ID of the player. |
| PlayerName | string | The username of the player. |