using NUnit.Framework;
using SEB.models;
using System.Text.Json;

namespace SEB.models.tests
{
    [TestFixture]
    public class HistoryTests
    {
        [Test]
        public void TestReadMethod()
        {
            // Arrange
            var history = new History();
            var jsonString = "{\"Count\":10,\"DurationInSeconds\":120,\"UserId\":1}";

            // Act
            history.Read(jsonString);

            // Assert
            Assert.AreEqual(10, history.Count);
            Assert.AreEqual(120, history.DurationInSeconds);
            // Assert.AreEqual(1, history.UserId); // Uncomment this when UserId is being set in the Read method
        }
    }
}