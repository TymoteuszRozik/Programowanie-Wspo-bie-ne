


using ProgramowanieWspolbiezne.BilliardCore.Abstractions;
using ProgramowanieWspolbiezne.BilliardCore.Models;
using ProgramowanieWspolbiezne.BilliardCore.Repositories;
using ProgramowanieWspolbiezne.BilliardLogic.Services;

namespace ProgramowanieWspolbiezneTests
{
    [TestClass]
    public class BallServiceTests
    {
        private class TestBallRepository : IBallRepository
        {
            public List<IBall> Balls { get; } = new();
            public void AddBall(IBall ball) => Balls.Add(ball);
            public IEnumerable<IBall> GetBalls() => Balls;
            public void RemoveBall(IBall ball) => Balls.Remove(ball);
            public void Clear() => Balls.Clear();
        }

        [TestMethod]
        public void CreateBalls_ShouldAddCorrectNumberOfBalls()
        {
            var testRepo = new TestBallRepository();
            var service = new BallService(testRepo);

            service.CreateBalls(5, 800, 400);

            // Assert
            Assert.AreEqual(5, testRepo.Balls.Count);
        }
        [TestMethod]
        public void BallMovement_ShouldRaiseEvent()
        {
            // Arrange
            var testRepo = new TestBallRepository();
            var service = new BallService(testRepo);
            service.CreateBalls(1, 800, 400);

            bool eventRaised = false;
            service.BallMoved += (s, e) => eventRaised = true;

            // Act
            service.StartMovement();
            Thread.Sleep(100); // Wait a bit for timer to tick

            // Assert
            Assert.IsTrue(eventRaised);
        }
    }
}
