using System;
using System.Threading;
using System.Threading.Tasks;
using TheIsleEvrimaRconClient.Extensions;
using TheIsleEvrimaRconClient.Tests.Helpers;
using Xunit;

namespace TheIsleEvrimaRconClient.Tests.Unit
{
    /// <summary>
    /// Tests for extension methods, focusing on <see cref="EvrimaRconClientExtensions.GetPlayerList"/>
    /// which contains non-trivial parsing logic.
    /// </summary>
    public class GetPlayerListExtensionTests : IDisposable
    {
        private const string Password = "testpassword";
        private readonly FakeRconServer _server;

        public GetPlayerListExtensionTests()
        {
            _server = new FakeRconServer(Password);
        }

        public void Dispose() => _server.Dispose();

        private EvrimaRconClientConfiguration Config()
            => new EvrimaRconClientConfiguration("127.0.0.1", _server.Port, Password);

        // ── GetPlayerList parsing ────────────────────────────────────────────────

        [Fact]
        public async Task GetPlayerList_WithMultiplePlayers_ReturnsAllPlayers()
        {
            // Response format: <header>\n<id1,id2>\n<name1,name2>
            _server.SetResponse(0x40, "PlayerList\nid1,id2\nAlice,Bob");

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            var players = await client.GetPlayerList();

            Assert.Equal(2, players.Count);
            Assert.Equal("id1", players[0].PlayerId);
            Assert.Equal("Alice", players[0].PlayerName);
            Assert.Equal("id2", players[1].PlayerId);
            Assert.Equal("Bob", players[1].PlayerName);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task GetPlayerList_WithSinglePlayer_ReturnsOnePlayer()
        {
            _server.SetResponse(0x40, "PlayerList\nplayer1\nJohnDoe");

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            var players = await client.GetPlayerList();

            Assert.Single(players);
            Assert.Equal("player1", players[0].PlayerId);
            Assert.Equal("JohnDoe", players[0].PlayerName);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task GetPlayerList_WithEmptyResponse_ReturnsEmptyList()
        {
            _server.SetResponse(0x40, string.Empty);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            var players = await client.GetPlayerList();

            Assert.Empty(players);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task GetPlayerList_WithHeaderOnlyResponse_ReturnsEmptyList()
        {
            _server.SetResponse(0x40, "PlayerList");

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            var players = await client.GetPlayerList();

            Assert.Empty(players);

            cts.Cancel();
            await serverTask;
        }

        // ── Other extension methods (smoke tests) ────────────────────────────────

        [Fact]
        public async Task Announce_DoesNotThrow()
        {
            _server.SetResponse(0x10, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            await client.Announce("Test message");  // must not throw

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task DirectMessage_FormatsArgumentCorrectly()
        {
            _server.SetResponse(0x11, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            // DirectMessage sends "playerId,message" as the argument
            string result = await client.SendCommandAsync(EvrimaRconCommand.DirectMessage, "pid123,Hello!");
            Assert.Equal("Direct message sent: pid123,Hello!", result);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task WipeCorpses_DoesNotThrow()
        {
            _server.SetResponse(0x13, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            await client.WipeCorpses();

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task SetAIDensity_ReturnsAIDensitySetMessage()
        {
            _server.SetResponse(0x92, string.Empty);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.AIDensity, "2.0");
            Assert.Equal("AI density set to: 2.0", result);

            cts.Cancel();
            await serverTask;
        }
    }
}

