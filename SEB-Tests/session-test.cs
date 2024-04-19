using NUnit.Framework;
using Moq;
using Npgsql;
using SEB.models;
using SEB.main.database;
using System;

[TestFixture]
public class SessionSQLTests
{
    private Mock<NpgsqlConnection> _connectionMock;
    private SessionSQL _sessionSQL;
    private string _connectionString = "YourConnectionString";

    [SetUp]
    public void Setup()
    {
        _connectionMock = new Mock<NpgsqlConnection>(_connectionString);
        _sessionSQL = new SessionSQL();
    }

    [Test]
    public void UserLogin_ValidCredentials_LoginSuccessful()
    {
        // Arrange
        var userDataCredentials = new UserCredentials
        {
            Username = "ValidUsername",
            Password = "ValidPassword"
        };

        // Act
        _sessionSQL.UserLogin(userDataCredentials, _connectionString);

        // Assert
        Assert.IsNotNull(userDataCredentials.Token);
    }

    [Test]
    public void UserLogin_InvalidCredentials_ThrowsException()
    {
        // Arrange
        var userDataCredentials = new UserCredentials
        {
            Username = "InvalidUsername",
            Password = "InvalidPassword"
        };

        // Act & Assert
        Assert.Throws<Exception>(() => _sessionSQL.UserLogin(userDataCredentials, _connectionString));
    }
}