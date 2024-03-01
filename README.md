# TheIsleEvrimaRconClient
RCON client for The Isle Evrima.

# Example usage
```csharp
var rcon = new EvrimaRconClient("127.0.0.1", 8888, "rcon_password", 5000);
var connected = await rcon.ConnectAsync();
if (connected)
{
     await rcon.SendCommandAsync(EvrimaRconCommand.Announce, "Hello World");
     await rcon.SendCommandAsync("announce", "Hello World 2");
     var response = await rcon.SendCommandAsync(EvrimaRconCommand.PlayerList);
}
```