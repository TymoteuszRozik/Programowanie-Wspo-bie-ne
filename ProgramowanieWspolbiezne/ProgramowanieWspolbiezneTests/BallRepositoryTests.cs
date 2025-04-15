
using ProgramowanieWspolbiezne.BilliardCore.Models;
using ProgramowanieWspolbiezne.BilliardCore.Repositories;

namespace ProgramowanieWspolbiezneTests
{
    [TestClass]
    public class BallRepositoryTests
    {
        [TestMethod]
        public void AddBall_ShouldIncreaseCount()
        {
            // Arrange
            var repo = new BallRepository();
            var ball = new Ball(10, 10, 20, 20, "#FF0000");

            // Act
            repo.AddBall(ball);

            // Assert
            Assert.AreEqual(1, repo.GetBalls().Count());
        }
    }
}
