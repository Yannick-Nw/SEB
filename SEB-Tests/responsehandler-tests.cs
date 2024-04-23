using NUnit.Framework;
using System.IO;
using System.Text;
using SEB.httpServer.response;

[TestFixture]
public class ResponseHandlerTests
{
    private ResponseHandler _responseHandler;
    private MemoryStream _memoryStream;
    private StreamWriter _streamWriter;

    [SetUp]
    public void SetUp()
    {
        _responseHandler = new ResponseHandler();
        _memoryStream = new MemoryStream();
        _streamWriter = new StreamWriter(_memoryStream);
    }

    [Test]
    public void SendPlaintextResponse_WritesExpectedResponse()
    {
        string content = "Hello, World!";
        int code = 200;

        _responseHandler.SendPlaintextResponse(_streamWriter, content, code);
        _streamWriter.Flush();

        string expectedResponse = $"HTTP/1.1 {code} OK\r\n" +
                                  $"Content-Type: text/plain\r\n" +
                                  $"Content-Length: {Encoding.UTF8.GetByteCount(content)}\r\n" +
                                  "\r\n" +
                                  $"{content}";

        string actualResponse = Encoding.UTF8.GetString(_memoryStream.ToArray());

        Assert.AreEqual(expectedResponse, actualResponse);
    }
    
    /*
    [Test]
    public void SendJsonResponse_WritesExpectedResponse()
    {
        string content = "{\"message\":\"Hello, World!\"}";
        int code = 200;

        _responseHandler.SendJsonResponse(_streamWriter, content, code);
        _streamWriter.Flush();

        string expectedResponse = $"HTTP/1.1 {code} OK\r\n" +
                                  $"Content-Type: application/json\r\n" +
                                  $"Content-Length: {Encoding.UTF8.GetByteCount(content)}\r\n" +
                                  "\r\n" +
                                  $"{content}";

        string actualResponse = Encoding.UTF8.GetString(_memoryStream.ToArray());

        Assert.AreEqual(expectedResponse, actualResponse);
    }

    [Test]
    public void SendErrorResponse_WritesExpectedResponse()
    {
        string content = "Error occurred";
        int code = 404;

        _responseHandler.SendErrorResponse(_streamWriter, content, code);
        _streamWriter.Flush();

        string expectedResponse = $"HTTP/1.1 {code} ERR\r\n" +
                                  $"Content-Type: text/plain\r\n" +
                                  $"Content-Length: {Encoding.UTF8.GetByteCount(content)}\r\n" +
                                  "\r\n" +
                                  $"{content}";

        string actualResponse = Encoding.UTF8.GetString(_memoryStream.ToArray());

        Assert.AreEqual(expectedResponse, actualResponse);
    }
    */


    [TearDown]
    public void TearDown()
    {
        _streamWriter.Close();
        _memoryStream.Close();
    }
}