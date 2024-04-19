using System.Text;
using System.Net;
using System.Net.Sockets;
using SEB.httpServer.request;


namespace SEB.httpServer.server
{
    public class HttpServer
    {
        private readonly int port;
        private readonly IPAddress host = IPAddress.Loopback;
        private TcpListener _listener;
        private string connection;

        bool listening;

        public HttpServer(int clientPort)
        {
            port = clientPort;
            listening = true;
        }

        public void Start(string connectionString)
        {
            this.connection = connectionString;
            _listener = new TcpListener(host, port);
            _listener.Start();
            //?
            Console.WriteLine("Server started...");

            byte[] buffer = new byte[1024];

            while (listening)
            {
                Console.WriteLine("Waiting for incoming connections...");

                TcpClient client = _listener.AcceptTcpClient();
                new Thread(() => HandleClient(client, buffer)).Start();
            }
        }

        public void HandleClient(TcpClient client, byte[] buffer)
        {
            Console.WriteLine("Accepted new client connection...");

            using var reader = new StreamReader(client.GetStream());
            using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };

            string? requestToHandle;
            StringBuilder req = new StringBuilder();

            Console.WriteLine("Handling request...");

            RequestHandler requestHandler = new RequestHandler(client, reader, writer, req.ToString(), connection);
            requestHandler.HandleRequest();

            client.Close();

            Console.WriteLine("Client disconnected");
        }

        public void Stop()
        {
            _listener.Stop();

            Console.WriteLine("Server stopped");
        }
    }
}