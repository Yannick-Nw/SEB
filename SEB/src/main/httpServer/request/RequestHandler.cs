using System.Net.Sockets;
using SEB.main.httpServer.request.handlers;

namespace SEB.httpServer.request;

public class RequestHandler
{
    private TcpClient client;
    private StreamReader reader;
    private StreamWriter writer;
    private string req;
    private string connectionString;
    public HttpMethod Method { get; set; }

    public RequestHandler(TcpClient client, StreamReader reader, StreamWriter writer, string req,
        string connectionString)
    {
        this.client = client;
        this.reader = reader;
        this.writer = writer;
        this.req = req;
        this.connectionString = connectionString;
    }

    public void HandleRequest()
    {
        string? line = reader.ReadLine();
        Console.WriteLine(line);
        string[]? firstLineParts = line?.Split(' ');
        Method = (HttpMethod)Enum.Parse(typeof(HttpMethod), firstLineParts?[0] ?? "ERROR");

        char[] clientBuffer = new char[client.ReceiveBufferSize];
        int bytesRead = reader.Read(clientBuffer, 0, client.ReceiveBufferSize);
        string data = new string(clientBuffer, 0, bytesRead);


        switch (Method)
        {
            case HttpMethod.GET:
                GetRequestHandler gethandler = new GetRequestHandler();
                Console.WriteLine("Choose method: " + Method);
                gethandler.GetRequest(client, writer, data, firstLineParts[1], connectionString);
                break;
            case HttpMethod.POST:
                PostRequestHandler posthandler = new PostRequestHandler();
                Console.WriteLine("Choose method: " + Method);
                posthandler.PostRequest(writer, data, firstLineParts[1], connectionString);
                break;
            case HttpMethod.PUT:
                // PutRequestHandler putRequestHandler = new PutRequestHandler(client, reader, writer, req);
                break;
            case HttpMethod.DELETE:
                DeleteRequestHandler deleteRequestHandler = new DeleteRequestHandler();
                Console.WriteLine("Choose method: " + Method);
                deleteRequestHandler.DeleteRequest(writer, data, firstLineParts[1], connectionString);
                break;
            default:
                Console.WriteLine("Unsupported method: " + Method);
                break;
        }
    }
}