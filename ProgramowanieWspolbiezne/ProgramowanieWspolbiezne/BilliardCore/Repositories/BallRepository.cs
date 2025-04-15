using ProgramowanieWspolbiezne.BilliardCore.Abstractions;

namespace ProgramowanieWspolbiezne.BilliardCore.Repositories;

public class BallRepository : IBallRepository
{
    private readonly List<IBall> balls = new();

    public IEnumerable<IBall> GetBalls() => balls.AsReadOnly();
    public void AddBall(IBall ball) => balls.Add(ball);
    public void RemoveBall(IBall ball) => balls.Remove(ball);
    public void Clear() => balls.Clear();
}