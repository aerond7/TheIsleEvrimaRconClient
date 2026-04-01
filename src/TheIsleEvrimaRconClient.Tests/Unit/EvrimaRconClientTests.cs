using System;
using System.Threading;
using System.Threading.Tasks;
using TheIsleEvrimaRconClient.Tests.Helpers;
using Xunit;

namespace TheIsleEvrimaRconClient.Tests.Unit
{
    /// <summary>
    /// Unit tests for <see cref="EvrimaRconClient"/> using a <see cref="FakeRconServer"/>
    /// so no real server is required.
    /// </summary>
    public class EvrimaRconClientTests : IDisposable
    {
        private const string Password = "testpassword";
        private readonly FakeRconServer _server;

        public EvrimaRconClientTests()
        {
            _server = new FakeRconServer(Password);
        }

        public void Dispose() => _server.Dispose();

        private EvrimaRconClientConfiguration Config(string? password = null)
            => new EvrimaRconClientConfiguration("127.0.0.1", _server.Port, password ?? Password);

        // ── IsConnected ─────────────────────────────────────────────────────────

        [Fact]
        public void IsConnected_BeforeConnecting_ReturnsFalse()
        {
            using var client = new EvrimaRconClient(Config());
            Assert.False(client.IsConnected);
        }

        // ── ConnectAsync ────────────────────────────────────────────────────────

        [Fact]
        public async Task ConnectAsync_WithCorrectPassword_ReturnsTrueAndIsConnected()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);

            using var client = new EvrimaRconClient(Config());
            bool result = await client.ConnectAsync();

            Assert.True(result);
            Assert.True(client.IsConnected);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task ConnectAsync_WithWrongPassword_ReturnsFalse()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);

            using var client = new EvrimaRconClient(Config("wrongpassword"));
            bool result = await client.ConnectAsync();

            Assert.False(result);

            await serverTask;
        }

        [Fact]
        public async Task ConnectAsync_WhenNoServerListening_ReturnsFalse()
        {
            // Spin up a temporary listener just to get a free port, then stop it
            int freePort;
            using (var tmp = new FakeRconServer("any"))
                freePort = tmp.Port;

            var config = new EvrimaRconClientConfiguration("127.0.0.1", freePort, "pass", timeout: 500);
            using var client = new EvrimaRconClient(config);
            bool result = await client.ConnectAsync();

            Assert.False(result);
        }

        // ── SendCommandAsync – pre-connection guard ──────────────────────────────

        [Fact]
        public async Task SendCommandAsync_WhenNotConnected_ThrowsInvalidOperationException()
        {
            using var client = new EvrimaRconClient(Config());
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => client.SendCommandAsync(EvrimaRconCommand.PlayerList));
        }

        // ── SendCommandAsync(string) – string parsing ────────────────────────────

        [Fact]
        public async Task SendCommandAsync_EmptyString_ReturnsInvalidCommandFormat()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(string.Empty);
            Assert.Equal("Invalid command format", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_WhitespaceString_ReturnsInvalidCommandFormat()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync("   ");
            Assert.Equal("Invalid command format", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_UnknownCommand_ReturnsUnknownCommandMessage()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync("fakecommand");
            Assert.Equal("Unknown command: fakecommand", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_StringWithSpace_SplitsIntoCommandAndArgument()
        {
            _server.SetResponse(0x20, string.Empty); // ban = 0x20
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync("ban player123");
            Assert.Equal("Banned: player123", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_CommandCaseInsensitive_Works()
        {
            _server.SetResponse(0x50, string.Empty); // save = 0x50
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync("SAVE");
            Assert.Equal("Server saved", result);

            cts.Cancel();
            await serverTask;
        }

        // ── GetFriendlyCommandResponse coverage ──────────────────────────────────

        [Fact]
        public async Task SendCommandAsync_Announce_ReturnsAnnouncedMessage()
        {
            _server.SetResponse(0x10, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.Announce, "Hello World");
            Assert.Equal("Announced: Hello World", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_DirectMessage_ReturnsDirectMessageSentMessage()
        {
            _server.SetResponse(0x11, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.DirectMessage, "pid,Hi");
            Assert.Equal("Direct message sent: pid,Hi", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_WipeCorpses_ReturnsCorpsesWipedMessage()
        {
            _server.SetResponse(0x13, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.WipeCorpses);
            Assert.Equal("Corpses wiped", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_Ban_ReturnsBannedMessage()
        {
            _server.SetResponse(0x20, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.Ban, "player-abc");
            Assert.Equal("Banned: player-abc", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_Kick_ReturnsKickedMessage()
        {
            _server.SetResponse(0x30, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.Kick, "player-xyz");
            Assert.Equal("Kicked: player-xyz", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_Save_ReturnsServerSavedMessage()
        {
            _server.SetResponse(0x50, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.Save);
            Assert.Equal("Server saved", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_ToggleWhitelist_ReturnsWhitelistToggledMessage()
        {
            _server.SetResponse(0x81, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.ToggleWhitelist);
            Assert.Equal("Whitelist toggled", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_AddWhitelistId_ReturnsAddedToWhitelistMessage()
        {
            _server.SetResponse(0x82, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.AddWhitelistId, "pid-001");
            Assert.Equal("Added to whitelist: pid-001", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_RemoveWhitelistId_ReturnsRemovedFromWhitelistMessage()
        {
            _server.SetResponse(0x83, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.RemoveWhitelistId, "pid-001");
            Assert.Equal("Removed from whitelist: pid-001", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_ToggleGlobalChat_ReturnsGlobalChatToggledMessage()
        {
            _server.SetResponse(0x84, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.ToggleGlobalChat);
            Assert.Equal("Global chat toggled", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_ToggleHumans_ReturnsHumansToggledMessage()
        {
            _server.SetResponse(0x86, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.ToggleHumans);
            Assert.Equal("Humans toggled", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_ToggleAI_ReturnsAIToggledMessage()
        {
            _server.SetResponse(0x90, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.ToggleAI);
            Assert.Equal("AI toggled", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_DisableAIClasses_ReturnsAIClassesUpdatedMessage()
        {
            _server.SetResponse(0x91, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.DisableAIClasses, "ClassA,ClassB");
            Assert.Equal("AI classes updated: ClassA,ClassB", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SendCommandAsync_AIDensity_ReturnsAIDensitySetMessage()
        {
            _server.SetResponse(0x92, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.AIDensity, "1.5");
            Assert.Equal("AI density set to: 1.5", result);

            cts.Cancel();
            await serverTask;
        }

        // ── ServerDetails / PlayerList / GetPlayerData pass raw response through ─

        [Fact]
        public async Task SendCommandAsync_ServerDetails_ReturnsRawServerResponse()
        {
            string raw = "ServerInfo=Test;Players=10";
            _server.SetResponse(0x12, raw);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.ServerDetails);
            Assert.Equal(raw, result);

            cts.Cancel();
            await serverTask;
        }

        // ── Dispose ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Dispose_AfterConnect_ClientIsNoLongerConnected()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);

            var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();
            Assert.True(client.IsConnected);

            client.Dispose();
            Assert.False(client.IsConnected);

            cts.Cancel();
            await serverTask;
        }
    }
}

