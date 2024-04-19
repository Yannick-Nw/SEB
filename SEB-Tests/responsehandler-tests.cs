using NUnit.Framework;
using System.IO;
using System.Text;
using SEB.httpServer.response;

[TestFixture]
public class ResponseHandlerTests
{
    private ResponseHandler _responseHandler;
    private StringWriter _stringWriter;
    private StreamWriter _streamWriter;

    [SetUp]
    public void SetUp()
    {
        _responseHandler = new ResponseHandler();
        _stringWriter = new StreamWriter();
        _streamWriter = new StreamWriter(_stringWriter);
    }

    [Test]
    public void SendPlaintextResponse_WritesExpectedResponse()
    {
        string content = "Hello, World!";
        int code = 200;

        _responseHandler.SendPlaintextResponse(_streamWriter, content, code);

        string expectedResponse = $"HTTP/1.1 {code} OK\r\n" +
                                  $"Content-Type: text/plain\r\n" +
                                  $"Content-Length: {Encoding.UTF8.GetByteCount(content)}\r\n" +
                                  "\r\n" +
                                  $"{content}";

        Assert.AreEqual(expectedResponse, _stringWriter.ToString());
    }

    [TearDown]
    public void TearDown()
    {
        _streamWriter.Close();
        _stringWriter.Close();
    }
}