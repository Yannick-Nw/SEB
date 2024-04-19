using NUnit.Framework;
using System.Text.Json;
using SEB.models;

namespace SEB.tests
{
    public class UserCredentialsTests
    {
        private UserCredentials _userCredentials;

        [SetUp]
        public void Setup()
        {
            _userCredentials = new UserCredentials();
        }

        [Test]
        public void TestRead()
        {
            string req = "{\"Username\":\"testuser\",\"Password\":\"testpass\"}";
            _userCredentials.Read(req);

            Assert.AreEqual("testuser", _userCredentials.Username);
            Assert.AreEqual("testpass", _userCredentials.Password);
        }

        [Test]
        public void TestTokenSearch()
        {
            string req = "Authorization: Basic testtoken\n";
            _userCredentials.TokenSearch(req);

            Assert.AreEqual("testtoken", _userCredentials.Token);
        }
    }
}