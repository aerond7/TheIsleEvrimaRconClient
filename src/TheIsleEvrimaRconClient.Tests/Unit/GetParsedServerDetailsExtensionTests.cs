using System;
using System.Threading;
using System.Threading.Tasks;
using TheIsleEvrimaRconClient.Extensions;
using TheIsleEvrimaRconClient.Tests.Helpers;
using Xunit;

namespace TheIsleEvrimaRconClient.Tests.Unit
{
    /// <summary>
    /// Tests for <see cref="EvrimaRconClientExtensions.GetServerDetails"/>,
    /// which parses the raw ServerDetails RCON response into a typed
    /// <see cref="TheIsleEvrimaRconClient.Extensions.Models.ServerDetails"/> object.
    /// </summary>
    public class GetParsedServerDetailsExtensionTests : IDisposable
    {
        private const string Password = "testpassword";
        private readonly FakeRconServer _server;

        public GetParsedServerDetailsExtensionTests()
        {
            _server = new FakeRconServer(Password);
        }

        public void Dispose() => _server.Dispose();

        private EvrimaRconClientConfiguration Config()
            => new EvrimaRconClientConfiguration("127.0.0.1", _server.Port, Password);

        // ── Inline format (header + data on the same line) ───────────────────────
        // This is the format the server emits in practice.

        [Fact]
        public async Task GetParsedServerDetails_InlineFormat_ParsesAllFields()
        {
            // The server concatenates the "[timestamp] ServerDetails" header directly
            // with the data — no newline separator between them.
            const string response =
                "[2026.04.01-00.00.00] ServerDetails" +
                "ServerName: Test Server, ServerPassword: testpwd, ServerMap: Gateway, " +
                "ServerMaxPlayers: 100, ServerCurrentPlayers: 5, " +
                "bEnableMutations: true, bEnableHumans: true, bServerPassword: false, " +
                "bQueueEnabled: false, bServerWhitelist: false, bSpawnAI: true, " +
                "bAllowRecordingReplay: true, bUseRegionSpawning: false, " +
                "bUseRegionSpawnCooldown: false, RegionSpawnCooldownTimeSeconds: 30, " +
                "ServerDayLengthMinutes: 90, ServerNightLengthMinutes: 20, " +
                "bEnableGlobalChat: true";

            _server.SetResponse(0x12, response);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            var details = await client.GetServerDetails();

            // String fields
            Assert.Equal("Test Server", details.Name);
            Assert.Equal("testpwd",     details.Password);
            Assert.Equal("Gateway",     details.Map);

            // Integer fields
            Assert.Equal(100, details.MaxPlayers);
            Assert.Equal(5,   details.CurrentPlayers);
            Assert.Equal(30,  details.RegionSpawnCooldownTimeSeconds);
            Assert.Equal(90,  details.DayLengthMinutes);
            Assert.Equal(20,  details.NightLengthMinutes);

            // Boolean fields – enabled flags
            Assert.True(details.MutationsEnabled);
            Assert.True(details.HumansEnabled);
            Assert.True(details.SpawnAIEnabled);
            Assert.True(details.AllowRecordingReplay);
            Assert.True(details.GlobalChatEnabled);

            // Boolean fields – disabled flags
            Assert.False(details.PasswordProtected);
            Assert.False(details.QueueEnabled);
            Assert.False(details.WhitelistEnabled);
            Assert.False(details.RegionSpawningEnabled);
            Assert.False(details.RegionSpawnCooldownEnabled);

            cts.Cancel();
            await serverTask;
        }

        // ── Newline-separated format (header on its own line) ────────────────────

        [Fact]
        public async Task GetParsedServerDetails_NewlineFormat_ParsesAllFields()
        {
            const string response =
                "[2026.04.01-00.00.00] ServerDetails\n" +
                "ServerName: Test Server, ServerPassword: testpwd, ServerMap: Gateway, " +
                "ServerMaxPlayers: 50, ServerCurrentPlayers: 0, " +
                "bEnableMutations: false, bEnableHumans: false, bServerPassword: true, " +
                "bQueueEnabled: true, bServerWhitelist: true, bSpawnAI: false, " +
                "bAllowRecordingReplay: false, bUseRegionSpawning: true, " +
                "bUseRegionSpawnCooldown: true, RegionSpawnCooldownTimeSeconds: 60, " +
                "ServerDayLengthMinutes: 45, ServerNightLengthMinutes: 10, " +
                "bEnableGlobalChat: false";

            _server.SetResponse(0x12, response);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            var details = await client.GetServerDetails();

            Assert.Equal("Test Server", details.Name);
            Assert.Equal(50, details.MaxPlayers);
            Assert.Equal(0,  details.CurrentPlayers);

            // Inverted booleans compared to the previous test
            Assert.False(details.MutationsEnabled);
            Assert.False(details.HumansEnabled);
            Assert.True(details.PasswordProtected);
            Assert.True(details.QueueEnabled);
            Assert.True(details.WhitelistEnabled);
            Assert.False(details.SpawnAIEnabled);
            Assert.True(details.RegionSpawningEnabled);
            Assert.True(details.RegionSpawnCooldownEnabled);
            Assert.Equal(60, details.RegionSpawnCooldownTimeSeconds);
            Assert.Equal(45, details.DayLengthMinutes);
            Assert.Equal(10, details.NightLengthMinutes);
            Assert.False(details.GlobalChatEnabled);

            cts.Cancel();
            await serverTask;
        }

        // ── Edge cases ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetParsedServerDetails_WithEmptyResponse_ReturnsDefaultInstance()
        {
            _server.SetResponse(0x12, string.Empty);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var serverTask = _server.AcceptAndHandleOneClientAsync(cts.Token);
            using var client = new EvrimaRconClient(Config());
            await client.ConnectAsync();

            var details = await client.GetServerDetails();

            Assert.NotNull(details);
            Assert.Equal(string.Empty, details.Name);
            Assert.Equal(0, details.MaxPlayers);

            cts.Cancel();
            await serverTask;
        }

        // ── Unit-level parsing (no network) ──────────────────────────────────────

        [Fact]
        public void ParseServerDetailsResponse_ServerNameWithSpecialChars_ParsedCorrectly()
        {
            // Server name contains "|" and spaces; ensure they are not mangled.
            const string response =
                "[2026.04.01-00.00.00] ServerDetails" +
                "ServerName: Test | Server 42, ServerPassword: pw, ServerMap: Island, " +
                "ServerMaxPlayers: 10, ServerCurrentPlayers: 1, " +
                "bEnableMutations: true, bEnableHumans: true, bServerPassword: false, " +
                "bQueueEnabled: false, bServerWhitelist: false, bSpawnAI: true, " +
                "bAllowRecordingReplay: true, bUseRegionSpawning: false, " +
                "bUseRegionSpawnCooldown: false, RegionSpawnCooldownTimeSeconds: 0, " +
                "ServerDayLengthMinutes: 60, ServerNightLengthMinutes: 15, " +
                "bEnableGlobalChat: true";

            var details = EvrimaRconClientExtensions.ParseServerDetailsResponse(response);

            Assert.Equal("Test | Server 42", details.Name);
            Assert.Equal("Island",           details.Map);
            Assert.Equal(10,                 details.MaxPlayers);
        }

        [Fact]
        public void ParseServerDetailsResponse_HeaderOnlyLine_ReturnsDefaultInstance()
        {
            const string response = "[2026.04.01-00.00.00] ServerDetails";

            var details = EvrimaRconClientExtensions.ParseServerDetailsResponse(response);

            Assert.NotNull(details);
            Assert.Equal(string.Empty, details.Name);
        }
    }
}

