using System.Threading.Tasks;
using TheIsleEvrimaRconClient.Extensions;
using TheIsleEvrimaRconClient.Tests.Helpers;
using Xunit;

namespace TheIsleEvrimaRconClient.Tests.Integration
{
    /// <summary>
    /// Integration tests that run against a real Isle Evrima RCON server.
    ///
    /// To enable these tests, fill in your server details in
    /// TheIsleEvrimaRconClient.Tests/testconfig.json:
    ///
    ///   {
    ///     "Host":     "your-server-ip",
    ///     "Port":     8888,
    ///     "Password": "your-rcon-password"
    ///   }
    ///
    /// When Host and Password are empty the tests are automatically skipped.
    /// </summary>
    public class RconIntegrationTests
    {
        private readonly RconTestConfiguration _cfg;

        public RconIntegrationTests()
        {
            _cfg = RconTestConfiguration.Load();
        }

        private EvrimaRconClientConfiguration ServerConfig()
            => new EvrimaRconClientConfiguration(_cfg.Host, _cfg.Port, _cfg.Password);

        // ── Connection ───────────────────────────────────────────────────────────

        [SkippableFact]
        public async Task ConnectAsync_WithRealServer_ReturnsTrueAndIsConnected()
        {
            Skip.If(!_cfg.IsConfigured, "Fill in testconfig.json to enable integration tests.");

            using var client = new EvrimaRconClient(ServerConfig());
            bool connected = await client.ConnectAsync();

            Assert.True(connected, "Expected ConnectAsync to succeed.");
            Assert.True(client.IsConnected);
        }

        // ── Player queries ───────────────────────────────────────────────────────

        [SkippableFact]
        public async Task GetPlayerList_WithRealServer_ReturnsNonNullList()
        {
            Skip.If(!_cfg.IsConfigured, "Fill in testconfig.json to enable integration tests.");

            using var client = new EvrimaRconClient(ServerConfig());
            await client.ConnectAsync();

            var players = await client.GetPlayerList();

            Assert.NotNull(players);
        }

        [SkippableFact]
        public async Task GetPlayerData_WithRealServer_ReturnsNonNullList()
        {
            Skip.If(!_cfg.IsConfigured, "Fill in testconfig.json to enable integration tests.");

            using var client = new EvrimaRconClient(ServerConfig());
            await client.ConnectAsync();

            var players = await client.GetPlayerData();

            // The list may be empty when no players are online, but must never be null.
            Assert.NotNull(players);
        }

        [SkippableFact]
        public async Task GetServerDetails_WithRealServer_ReturnsPopulatedObject()
        {
            Skip.If(!_cfg.IsConfigured, "Fill in testconfig.json to enable integration tests.");

            using var client = new EvrimaRconClient(ServerConfig());
            await client.ConnectAsync();

            var details = await client.GetServerDetails();

            Assert.NotNull(details);
            Assert.False(string.IsNullOrWhiteSpace(details.Name),
                "Expected ServerDetails.Name to be populated.");
            Assert.False(string.IsNullOrWhiteSpace(details.Map),
                "Expected ServerDetails.Map to be populated.");
            Assert.True(details.MaxPlayers > 0,
                "Expected ServerDetails.MaxPlayers to be greater than zero.");
        }

        // ── Commands ─────────────────────────────────────────────────────────────

        [SkippableFact]
        public async Task Save_WithRealServer_ReturnsServerSavedMessage()
        {
            Skip.If(!_cfg.IsConfigured, "Fill in testconfig.json to enable integration tests.");

            using var client = new EvrimaRconClient(ServerConfig());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.Save);

            Assert.Equal("Server saved", result);
        }

        [SkippableFact]
        public async Task Announce_WithRealServer_Succeeds()
        {
            Skip.If(!_cfg.IsConfigured, "Fill in testconfig.json to enable integration tests.");

            using var client = new EvrimaRconClient(ServerConfig());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(
                EvrimaRconCommand.Announce, "[Integration Test] Hello from unit tests!");

            Assert.Equal("Announced: [Integration Test] Hello from unit tests!", result);
        }

        [SkippableFact]
        public async Task WipeCorpses_WithRealServer_Succeeds()
        {
            Skip.If(!_cfg.IsConfigured, "Fill in testconfig.json to enable integration tests.");

            using var client = new EvrimaRconClient(ServerConfig());
            await client.ConnectAsync();

            string result = await client.SendCommandAsync(EvrimaRconCommand.WipeCorpses);

            Assert.Equal("Corpses wiped", result);
        }
    }
}

