using System.Text;
using SEB.main.httpServer.request.handlers;

[TestFixture]
public class DeleteRequestHandlerTests
{
    private DeleteRequestHandler _handler;
    private MemoryStream _memoryStream;
    private StreamWriter writer;
    private string _connectionString = "Host=localhost;Database=sebdb;Username=user;Password=seb-password";

    [SetUp]
    public void Setup()
    {
        _handler = new DeleteRequestHandler();
        _memoryStream = new MemoryStream();
        writer = new StreamWriter(_memoryStream);
    }

    [TearDown]
    public void Teardown()
    {
        writer.Close();
        _memoryStream.Close();
    }

/*
    [Test]
    public void DeleteRequest_WithAdminToken_ShouldDeleteTables()
    {
        // Arrange
        string data = "admin-sebToken";
        string route = "/delete";

        // Act
        _handler.DeleteRequest(writer, data, route, _connectionString);

        // Assert
    }
    */

    [Test]
    public void DeleteRequest_WithWrongToken_ShouldReturnErrorResponse()
    {
        // Arrange
        string data = "wrongToken";
        string route = "/delete";

        // Act
        _handler.DeleteRequest(writer, data, route, _connectionString);
        writer.Flush();

        string result = Encoding.UTF8.GetString(_memoryStream.ToArray());

        // Assert
        Assert.IsTrue(result.Contains("Wrong user"));
    }

    [Test]
    public void DeleteRequest_WithWrongRoute_ShouldReturnErrorResponse()
    {
        // Arrange
        string data = "admin-sebToken";
        string route = "/wrongRoute";

        // Act
        _handler.DeleteRequest(writer, data, route, _connectionString);
        writer.Flush();

        string result = Encoding.UTF8.GetString(_memoryStream.ToArray());

        // Assert
        Assert.IsTrue(result.Contains("Route does not exist"));
    }
}