using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System;
using TheIsleEvrimaRconClient.Internal;
using System.Net;

namespace TheIsleEvrimaRconClient
{
    /// <summary>
    /// RCON client for The Isle Evrima.
    /// </summary>
    public class EvrimaRconClient : IDisposable
    {
        /// <summary>
        /// Indicates whether the client is connected or not.
        /// </summary>
        public bool IsConnected => _client != null && _stream != null && _client.Connected;

        private readonly EvrimaRconClientConfiguration config;

        private TcpClient _client;
        private NetworkStream _stream;
        private bool _isAuthorized = false;

        /// <summary>
        /// Initialize the client using a configuration object. This is the recommended way.
        /// </summary>
        /// <param name="configuration">RCON connection configuration</param>
        public EvrimaRconClient(EvrimaRconClientConfiguration configuration)
        {
            this.config = configuration;
        }

        /// <summary>
        /// Initialize the client. This method is not yet obsolete, but it may be deprecated in the future.
        /// </summary>
        /// <param name="host">IP address of the RCON server</param>
        /// <param name="port">Port of the RCON server</param>
        /// <param name="password">Password to the RCON server</param>
        public EvrimaRconClient(IPAddress host,
            int port,
            string password)
        {
            this.config = new EvrimaRconClientConfiguration(host, port, password);
        }

        /// <summary>
        /// Initialize the client. This method is not yet obsolete, but it may be deprecated in the future.
        /// </summary>
        /// <param name="host">IP address of the RCON server</param>
        /// <param name="port">Port of the RCON server</param>
        /// <param name="password">Password to the RCON server</param>
        /// <param name="timeout">Connection timeout in milliseconds</param>
        public EvrimaRconClient(IPAddress host,
            int port,
            string password,
            int timeout)
        {
            this.config = new EvrimaRconClientConfiguration(host, port, password, timeout);
        }

        /// <summary>
        /// Initialize the client. This method is not yet obsolete, but it may be deprecated in the future.
        /// </summary>
        /// <param name="host">IP address of the RCON server</param>
        /// <param name="port">Port of the RCON server</param>
        /// <param name="password">Password to the RCON server</param>
        public EvrimaRconClient(string host,
            int port,
            string password)
        {
            this.config = new EvrimaRconClientConfiguration(host, port, password);
        }

        /// <summary>
        /// Initialize the client. This method is not yet obsolete, but it may be deprecated in the future.
        /// </summary>
        /// <param name="host">IP address of the RCON server</param>
        /// <param name="port">Port of the RCON server</param>
        /// <param name="password">Password to the RCON server</param>
        /// <param name="timeout">Connection timeout in milliseconds</param>
        public EvrimaRconClient(string host,
            int port,
            string password,
            int timeout)
        {
            this.config = new EvrimaRconClientConfiguration(host, port, password, timeout);
        }

        /// <summary>
        /// Connects the client to the RCON server, must be called before executing any commands.
        /// </summary>
        /// <returns>True if connection succeeded, False if connection failed</returns>
        public async Task<bool> ConnectAsync()
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(config.Host, config.Port);
                _stream = _client.GetStream();
                _stream.ReadTimeout = config.Timeout;
                return await AuthorizeAsync();
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> AuthorizeAsync()
        {
            if (!_isAuthorized)
            {
                await SendPacketAsync($"\x01{config.Password}\x00");
                var response = await ReadPacketAsync();
                if (!response.Contains("Password Accepted"))
                {
                    return false;
                }
                _isAuthorized = true;
                await ReconnectAsync();
                return true;
            }
            return false;
        }

        private void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
        }

        private async Task ReconnectAsync()
        {
            Disconnect();
            await ConnectAsync();
        }

        private async Task SendPacketAsync(string data)
        {
            if (_stream == null)
            {
                throw new InvalidOperationException("Network stream is null, the client may have disconnected");
            }

            byte[] buffer = Encoding.ASCII.GetBytes(data);
            await _stream.WriteAsync(buffer, 0, buffer.Length);
        }

        private async Task<string> ReadPacketAsync()
        {
            if (_stream == null)
            {
                throw new InvalidOperationException("Network stream is null, the client may have disconnected");
            }

            byte[] buffer = new byte[4096];
            int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }

        /// <summary>
        /// Sends a command to the RCON server with the specified argument.
        /// </summary>
        /// <param name="commandName">The command name</param>
        /// <param name="commandArgument">The command argument</param>
        /// <returns>Response from the server</returns>
        /// <exception cref="InvalidOperationException">Thrown if the client is not connected</exception>
        public async Task<string> SendCommandAsync(string commandName, string commandArgument = "")
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("The client must be connected before sending commands");
            }

            var formattedCommand = commandName.ToLower().Trim();

            if (!CommandByteMap.Map.TryGetValue(formattedCommand, out byte commandByte))
            {
                return $"Unknown command: {formattedCommand}";
            }

            string commandPacket = $"\x02{Convert.ToChar(commandByte)}{commandArgument}\x00";
            await SendPacketAsync(commandPacket);
            string response = await ReadPacketAsync();

            return GetFriendlyCommandResponse(formattedCommand, response, commandArgument);
        }

        /// <summary>
        /// Sends a pre-defined command to the RCON server with the specified argument.
        /// </summary>
        /// <param name="command">Command to send</param>
        /// <param name="commandArgument">Command argument</param>
        /// <returns>Response from the server</returns>
        /// <exception cref="InvalidOperationException">Thrown if the client is not connected</exception>
        public Task<string> SendCommandAsync(EvrimaRconCommand command, string commandArgument = "")
        {
            return SendCommandAsync(command.ToString(), commandArgument);
        }

        /// <summary>
        /// Sends a command to the RCON server.
        /// </summary>
        /// <param name="command">The full command to send</param>
        /// <returns>Response from the server</returns>
        /// <exception cref="InvalidOperationException">Thrown if the client is not connected</exception>
        public Task<string> SendCommandAsync(string command)
        {
            var split = command.Split(' ');
            if (split.Length <= 0)
            {
                return Task.FromResult("Invalid command format");
            }

            return SendCommandAsync(split[0], split.Length > 1 ? split[1] : "");
        }

        private string GetFriendlyCommandResponse(string commandName, string response, string input)
        {
            switch (commandName)
            {
                case "announce":
                    return $"Announced: {input}";
                case "updateplayables":
                    return response;
                case "ban":
                    return $"Banned: {input}";
                case "kick":
                    return $"Kicked: {input}";
                case "playerlist":
                    return response;
                case "save":
                    return "Server saved";
                case "custom":
                    return "Command executed";
                default:
                    return "Unknown command";
            }
        }

        /// <summary>
        /// Disconnects and disposes the client
        /// </summary>
        public void Dispose()
        {
            Disconnect();
            _stream?.Dispose();
            _client?.Dispose();
        }
    }
}
