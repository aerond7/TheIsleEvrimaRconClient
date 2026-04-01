using System;
using System.Net;
using Xunit;

namespace TheIsleEvrimaRconClient.Tests.Unit
{
    public class EvrimaRconClientConfigurationTests
    {
        [Fact]
        public void DefaultConstructor_SetsExpectedDefaults()
        {
            var config = new EvrimaRconClientConfiguration();

            Assert.Equal(IPAddress.Parse("127.0.0.1"), config.Host);
            Assert.Equal(8888, config.Port);
            Assert.Equal(string.Empty, config.Password);
            Assert.Equal(5000, config.Timeout);
        }

        [Fact]
        public void Constructor_WithIPAddress_SetsProperties()
        {
            var ip = IPAddress.Parse("192.168.1.1");
            var config = new EvrimaRconClientConfiguration(ip, 9999, "secret");

            Assert.Equal(ip, config.Host);
            Assert.Equal(9999, config.Port);
            Assert.Equal("secret", config.Password);
            Assert.Equal(5000, config.Timeout); // default unchanged
        }

        [Fact]
        public void Constructor_WithIPAddressAndTimeout_SetsAllProperties()
        {
            var ip = IPAddress.Parse("10.0.0.1");
            var config = new EvrimaRconClientConfiguration(ip, 7777, "pass", 3000);

            Assert.Equal(ip, config.Host);
            Assert.Equal(7777, config.Port);
            Assert.Equal("pass", config.Password);
            Assert.Equal(3000, config.Timeout);
        }

        [Fact]
        public void Constructor_WithValidHostString_ParsesIPCorrectly()
        {
            var config = new EvrimaRconClientConfiguration("192.168.0.1", 8888, "pass");

            Assert.Equal(IPAddress.Parse("192.168.0.1"), config.Host);
            Assert.Equal(8888, config.Port);
            Assert.Equal("pass", config.Password);
            Assert.Equal(5000, config.Timeout); // default unchanged
        }

        [Fact]
        public void Constructor_WithValidHostStringAndTimeout_SetsAllProperties()
        {
            var config = new EvrimaRconClientConfiguration("10.0.0.5", 8888, "pass", 2000);

            Assert.Equal(IPAddress.Parse("10.0.0.5"), config.Host);
            Assert.Equal(2000, config.Timeout);
        }

        [Theory]
        [InlineData("not-an-ip")]
        [InlineData("256.0.0.1")]
        [InlineData("hostname")]
        [InlineData("localhost")]
        [InlineData("")]
        public void Constructor_WithInvalidHostString_ThrowsArgumentException(string invalidHost)
        {
            Assert.Throws<ArgumentException>(() =>
                new EvrimaRconClientConfiguration(invalidHost, 8888, "pass"));
        }

        [Theory]
        [InlineData("not-an-ip")]
        [InlineData("256.1.1.1")]
        public void Constructor_WithInvalidHostStringAndTimeout_ThrowsArgumentException(string invalidHost)
        {
            Assert.Throws<ArgumentException>(() =>
                new EvrimaRconClientConfiguration(invalidHost, 8888, "pass", 5000));
        }

        [Fact]
        public void Properties_CanBeSetDirectly()
        {
            var config = new EvrimaRconClientConfiguration
            {
                Host = IPAddress.Parse("172.16.0.1"),
                Port = 1234,
                Password = "mypassword",
                Timeout = 10000
            };

            Assert.Equal(IPAddress.Parse("172.16.0.1"), config.Host);
            Assert.Equal(1234, config.Port);
            Assert.Equal("mypassword", config.Password);
            Assert.Equal(10000, config.Timeout);
        }
    }
}

