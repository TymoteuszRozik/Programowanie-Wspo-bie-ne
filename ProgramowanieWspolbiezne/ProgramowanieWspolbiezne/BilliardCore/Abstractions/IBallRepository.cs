namespace ProgramowanieWspolbiezne.BilliardCore.Abstractions;

public interface IBallRepository
{
    IEnumerable<IBall> GetBalls();
    void AddBall(IBall ball);
    void RemoveBall(IBall ball);
    void Clear();
}
