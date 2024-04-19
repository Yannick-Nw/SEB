using NUnit.Framework;
using SEB.models;
using System.Text.Json;

namespace SEB.models.tests
{
    [TestFixture]
    public class UserDataTests
    {
        [Test]
        public void TestReadMethod()
        {
            // Arrange
            var userData = new UserData();
            var jsonString = "{\"Name\":\"Test Name\",\"Bio\":\"Test Bio\",\"Image\":\"Test Image\"}";

            // Act
            userData.Read(jsonString);

            // Assert
            Assert.AreEqual("Test Name", userData.Name);
            Assert.AreEqual("Test Bio", userData.Bio);
            Assert.AreEqual("Test Image", userData.Image);
        }
    }
}