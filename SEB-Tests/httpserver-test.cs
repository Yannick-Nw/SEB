using NUnit.Framework;
using System.Net.Sockets;
using System.Threading;
using SEB.httpServer.server;

namespace SEB.httpServer.tests
{
    [TestFixture]
    public class HttpServerTests
    {
        private HttpServer _httpServer;
        private Thread _serverThread;

        [SetUp]
        public void Setup()
        {
            _httpServer = new HttpServer(10001);
            _serverThread = new Thread(() => _httpServer.Start("Host=localhost;Database=sebdb;Username=user;Password=seb-password"));
            _serverThread.Start();
        }

        [Test]
        public void TestServerStartAndStop()
        {
            // Allow the server to start
            Thread.Sleep(1000);

            // Check if the server is listening
            using (TcpClient client = new TcpClient("localhost", 10001))
            {
                Assert.IsTrue(client.Connected);
            }
            
            // Stop the server
            _httpServer.Stop();

            // Allow the server to stop
            Thread.Sleep(1000);

            // Check if the server has stopped
            Assert.Throws<SocketException>(() =>
            {
                TcpClient client = new TcpClient("localhost", 10001);
            });
        }

        [TearDown]
        public void TearDown()
        {
            _httpServer.Stop();
            _serverThread.Join();
        }
    }
}