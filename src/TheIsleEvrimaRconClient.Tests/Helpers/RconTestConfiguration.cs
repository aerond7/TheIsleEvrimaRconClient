using System.IO;
using System.Text.Json;

namespace TheIsleEvrimaRconClient.Tests.Helpers
{
    /// <summary>
    /// Reads integration-test server details from testconfig.json.
    /// To run integration tests, fill in Host, Port, and Password in that file.
    /// </summary>
    internal class RconTestConfiguration
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 8888;
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Returns true when both Host and Password are non-empty,
        /// meaning integration tests should run against a real server.
        /// </summary>
        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(Password);

        public static RconTestConfiguration Load()
        {
            string path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "testconfig.json");
            if (!File.Exists(path))
                return new RconTestConfiguration();

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<RconTestConfiguration>(json,
                       new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new RconTestConfiguration();
        }
    }
}

