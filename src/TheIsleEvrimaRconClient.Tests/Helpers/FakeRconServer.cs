using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheIsleEvrimaRconClient.Tests.Helpers
{
    /// <summary>
    /// An in-process fake RCON TCP server for unit testing.
    /// Accepts one client, validates the password, then responds to command packets
    /// using the pre-configured response dictionary.
    /// </summary>
    internal sealed class FakeRconServer : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly string _password;
        private readonly Dictionary<byte, string> _responses = new Dictionary<byte, string>();

        /// <summary>Gets the port the server is listening on.</summary>
        public int Port => ((IPEndPoint)_listener.LocalEndpoint).Port;

        public FakeRconServer(string password)
        {
            _password = password;
            _listener = new TcpListener(IPAddress.Loopback, 0); // 0 = random available port
            _listener.Start();
        }

        /// <summary>Pre-configures the response sent back for a given command byte.</summary>
        public void SetResponse(byte commandByte, string response)
            => _responses[commandByte] = response;

        /// <summary>
        /// Accepts one client, processes its auth + command packets, and returns
        /// when the client disconnects or <paramref name="ct"/> is cancelled.
        /// </summary>
        public async Task AcceptAndHandleOneClientAsync(CancellationToken ct = default)
        {
            TcpClient client;
            try
            {
                client = await _listener.AcceptTcpClientAsync(ct);
            }
            catch (OperationCanceledException) { return; }
            catch (SocketException) { return; }

            using (client)
            {
                await HandleClientAsync(client, ct);
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken ct)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];
            int bytesRead;

            // --- Auth packet: \x01 + password + \x00 ---
            try
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ct);
            }
            catch { return; }

            string packet = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            string expected = $"\x01{_password}\x00";

            if (packet == expected)
            {
                await WriteAsync(stream, "Password Accepted", ct);
            }
            else
            {
                await WriteAsync(stream, "Wrong password", ct);
                return;
            }

            // --- Command packet loop: \x02 + commandByte + argument + \x00 ---
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                    if (bytesRead == 0) break;

                    if (buffer[0] == 0x02 && bytesRead >= 2)
                    {
                        byte cmdByte = buffer[1];
                        string response = _responses.TryGetValue(cmdByte, out string? r) ? r : string.Empty;
                        await WriteAsync(stream, response, ct);
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (IOException) { break; }
                catch (Exception) { break; }
            }
        }

        private static async Task WriteAsync(NetworkStream stream, string data, CancellationToken ct)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            await stream.WriteAsync(bytes, 0, bytes.Length, ct);
        }

        public void Dispose() => _listener.Stop();
    }
}

