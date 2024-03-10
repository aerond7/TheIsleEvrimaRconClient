using System;
using System.Net;

namespace TheIsleEvrimaRconClient
{
    public class EvrimaRconClientConfiguration
    {
        public IPAddress Host { get; set; } = IPAddress.Parse("127.0.0.1");
        public int Port { get; set; } = 8888;
        public string Password { get; set; } = string.Empty;
        public int Timeout { get; set; } = 5000;

        public EvrimaRconClientConfiguration()
        { }

        public EvrimaRconClientConfiguration(IPAddress host, int port, string password)
        {
            this.Host = host;
            this.Port = port;
            this.Password = password;
        }

        public EvrimaRconClientConfiguration(string host, int port, string password)
        {
            InitializeConfiguration(host, port, password, timeout: null);
        }

        public EvrimaRconClientConfiguration(IPAddress host, int port, string password, int timeout)
        {
            this.Host = host;
            this.Port = port;
            this.Password = password;
            this.Timeout = timeout;
        }

        public EvrimaRconClientConfiguration(string host, int port, string password, int timeout)
        {
            InitializeConfiguration(host, port, password, timeout);
        }

        private void InitializeConfiguration(string host, int port, string password, int? timeout)
        {
            if (!IPAddress.TryParse(host, out IPAddress ipAddress))
            {
                throw new ArgumentException("The host is not a valid IP address");
            }

            this.Host = ipAddress;
            this.Port = port;
            this.Password = password;
            if (timeout.HasValue)
            {
                this.Timeout = timeout.Value;
            }
        }
    }
}
