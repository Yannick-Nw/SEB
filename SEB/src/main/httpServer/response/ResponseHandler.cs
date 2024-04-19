using System.Text;

namespace SEB.httpServer.response;

public class ResponseHandler
{
    public void SendPlaintextResponse(StreamWriter writer, string content, int code)
    {
        string response = $"HTTP/1.1 {code} OK\r\n" +
                          $"Content-Type: text/plain\r\n" +
                          $"Content-Length: {Encoding.UTF8.GetByteCount(content)}\r\n" +
                          "\r\n" +
                          $"{content}";

        writer.Write(response);
        writer.Flush();
    }

    public void SendJsonResponse(StreamWriter writer, string content, int code)
    {
        string response = $"HTTP/1.1 {code} OK\r\n" +
                          $"Content-Type: application/json\r\n" +
                          $"Content-Length: {Encoding.UTF8.GetByteCount(content)}\r\n" +
                          "\r\n" +
                          $"{content}";

        writer.Write(response);
    }

    public void SendErrorResponse(StreamWriter writer, string content, int code)
    {
        string response = $"HTTP/1.1 {code} ERR\r\n" +
                          $"Content-Type: text/plain\r\n" +
                          $"Content-Length: {Encoding.UTF8.GetByteCount(content)}\r\n" +
                          "\r\n" +
                          $"{content}";

        writer.Write(response);
    }
}