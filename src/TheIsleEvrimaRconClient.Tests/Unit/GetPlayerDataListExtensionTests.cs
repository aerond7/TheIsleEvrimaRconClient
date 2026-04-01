using System;
using System.Threading;
using System.Threading.Tasks;
using TheIsleEvrimaRconClient.Extensions;
using TheIsleEvrimaRconClient.Tests.Helpers;
using Xunit;

namespace TheIsleEvrimaRconClient.Tests.Unit
{
    /// <summary>
    /// Tests for <see cref="EvrimaRconClientExtensions.GetPlayerData"/>,
    /// which parses the raw GetPlayerData RCON response into typed <see cref="TheIsleEvrimaRconClient.Extensions.Models.PlayerData"/> objects.
    /// </summary>
    public class GetPlayerDataListExtensionTests : IDisposable
    {
        private const string Password = "testpassword";
        private readonly FakeRconServer _server;

        public GetPlayerDataListExtensionTests()
        {
            _server = new FakeRconServer(Password);
        }

        public void Dispose() => _server.Dispose();

        private EvrimaRconClientConfiguration Config()
            => new EvrimaRconClientConfiguration("127.0.0.1", _server.Port, Password);

        // ── Full single-player response ──────────────────────────────────────────

        [Fact]
        public async Task GetPlayerDataList_WithSinglePlayer_ParsesAllFields()
        {
            const string response =
                "[2026.04.01-00.00.00] PlayerData\n" +
                "Name: TestPlayer, PlayerID: 76561198000000001, Gender: Male, " +
                "Location: X=1234.500 Y=-5678.250 Z=300.000, Class: Tyrannosaurus, " +
                "Growth: 1.00, Health: 1.00, Stamina: 1.00, Hunger: 0.98, Thirst: 0.98, " +
                "MutationSlots: [1=None,2=None,3=None,4=None], " +
                "ParentMutationSlots: [1=None,2=None,3=None,4=None], " +
                "ElderMutationSlotsA: [1=None,2=None,3=None,4=None], " +
                "ElderMutationSlotsB: [1=None,2=None,3=None,4=None], PrimeElder: false\n" +
                "PlayerDataEnd";

            _server.SetResponse(0x77, response);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            var players = await client.GetPlayerData();

            Assert.Single(players);
            var p = players[0];

            // Identity
            Assert.Equal("TestPlayer", p.Name);
            Assert.Equal("76561198000000001", p.PlayerId);
            Assert.Equal("Male", p.Gender);

            // Class & growth
            Assert.Equal("Tyrannosaurus", p.Class);
            Assert.Equal(1.00f, p.Growth);

            // Stats
            Assert.Equal(1.00f, p.Health);
            Assert.Equal(1.00f, p.Stamina);
            Assert.Equal(0.98f, p.Hunger);
            Assert.Equal(0.98f, p.Thirst);

            // Location
            Assert.Equal(1234.5,   p.Location.X, 3);
            Assert.Equal(-5678.25, p.Location.Y, 3);
            Assert.Equal(300.0,    p.Location.Z, 3);

            // Mutation slots
            Assert.Equal(4, p.MutationSlots.Count);
            Assert.Equal("None", p.MutationSlots[1]);
            Assert.Equal("None", p.MutationSlots[4]);

            Assert.Equal(4, p.ParentMutationSlots.Count);
            Assert.Equal(4, p.ElderMutationSlotsA.Count);
            Assert.Equal(4, p.ElderMutationSlotsB.Count);

            // PrimeElder
            Assert.False(p.PrimeElder);

            cts.Cancel();
            await serverTask;
        }

        // ── Multiple players ─────────────────────────────────────────────────────

        [Fact]
        public async Task GetPlayerDataList_WithMultiplePlayers_ReturnsAllPlayers()
        {
            const string response =
                "[2026.04.01-14.50.30] PlayerData\n" +
                "Name: Alice, PlayerID: 111, Gender: Female, " +
                "Location: X=0 Y=0 Z=0, Class: Triceratops, " +
                "Growth: 0.50, Health: 0.80, Stamina: 1.00, Hunger: 0.75, Thirst: 0.60, " +
                "MutationSlots: [1=None,2=None,3=None,4=None], " +
                "ParentMutationSlots: [1=None,2=None,3=None,4=None], " +
                "ElderMutationSlotsA: [1=None,2=None,3=None,4=None], " +
                "ElderMutationSlotsB: [1=None,2=None,3=None,4=None], PrimeElder: false\n" +
                "Name: Bob, PlayerID: 222, Gender: Male, " +
                "Location: X=100 Y=200 Z=300, Class: Pachycephalosaurus, " +
                "Growth: 1.00, Health: 0.50, Stamina: 0.90, Hunger: 0.85, Thirst: 0.95, " +
                "MutationSlots: [1=None,2=None,3=None,4=None], " +
                "ParentMutationSlots: [1=None,2=None,3=None,4=None], " +
                "ElderMutationSlotsA: [1=None,2=None,3=None,4=None], " +
                "ElderMutationSlotsB: [1=None,2=None,3=None,4=None], PrimeElder: true\n" +
                "PlayerDataEnd";

            _server.SetResponse(0x77, response);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            var players = await client.GetPlayerData();

            Assert.Equal(2, players.Count);

            Assert.Equal("Alice",         players[0].Name);
            Assert.Equal("111",           players[0].PlayerId);
            Assert.Equal("Female",        players[0].Gender);
            Assert.Equal("Triceratops",   players[0].Class);
            Assert.Equal(0.50f,           players[0].Growth);
            Assert.False(players[0].PrimeElder);

            Assert.Equal("Bob",                  players[1].Name);
            Assert.Equal("222",                  players[1].PlayerId);
            Assert.Equal("Pachycephalosaurus",   players[1].Class);
            Assert.Equal(100d,                   players[1].Location.X, 3);
            Assert.True(players[1].PrimeElder);

            cts.Cancel();
            await serverTask;
        }

        // ── Edge cases ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetPlayerDataList_WithEmptyResponse_ReturnsEmptyList()
        {
            _server.SetResponse(0x77, string.Empty);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            var players = await client.GetPlayerData();

            Assert.Empty(players);

            cts.Cancel();
            await serverTask;
        }

        [Fact]
        public async Task GetPlayerDataList_WithHeaderAndSentinelOnly_ReturnsEmptyList()
        {
            _server.SetResponse(0x77, "[2026.04.01-14.50.30] PlayerData\nPlayerDataEnd");

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            var players = await client.GetPlayerData();

            Assert.Empty(players);

            cts.Cancel();
            await serverTask;
        }

        // ── Unit-level parsing (no network) ──────────────────────────────────────

        [Fact]
        public void ParsePlayerDataResponse_PrimeElder_ParsedAsTrueWhenValueIsTrue()
        {
            const string response =
                "[2026.04.01-00.00.00] PlayerData\n" +
                "Name: TestElder, PlayerID: 00000000000000001, Gender: Male, " +
                "Location: X=0 Y=0 Z=0, Class: Rex, " +
                "Growth: 1.00, Health: 1.00, Stamina: 1.00, Hunger: 1.00, Thirst: 1.00, " +
                "MutationSlots: [1=None,2=None,3=None,4=None], " +
                "ParentMutationSlots: [1=None,2=None,3=None,4=None], " +
                "ElderMutationSlotsA: [1=None,2=None,3=None,4=None], " +
                "ElderMutationSlotsB: [1=None,2=None,3=None,4=None], PrimeElder: true\n" +
                "PlayerDataEnd";

            var players = EvrimaRconClientExtensions.ParsePlayerDataResponse(response);

            Assert.Single(players);
            Assert.True(players[0].PrimeElder);
        }

        [Fact]
        public void ParsePlayerDataResponse_MutationSlotsWithValues_ParsedCorrectly()
        {
            const string response =
                "Name: Mutant, PlayerID: 1, Gender: Male, " +
                "Location: X=0 Y=0 Z=0, Class: Carnotaurus, " +
                "Growth: 1.00, Health: 1.00, Stamina: 1.00, Hunger: 1.00, Thirst: 1.00, " +
                "MutationSlots: [1=Speedy,2=Bulky,3=None,4=None], " +
                "ParentMutationSlots: [1=None,2=None,3=None,4=None], " +
                "ElderMutationSlotsA: [1=None,2=None,3=None,4=None], " +
                "ElderMutationSlotsB: [1=None,2=None,3=None,4=None], PrimeElder: false";

            var players = EvrimaRconClientExtensions.ParsePlayerDataResponse(response);

            Assert.Single(players);
            Assert.Equal("Speedy", players[0].MutationSlots[1]);
            Assert.Equal("Bulky",  players[0].MutationSlots[2]);
            Assert.Equal("None",   players[0].MutationSlots[3]);
        }
    }
}

