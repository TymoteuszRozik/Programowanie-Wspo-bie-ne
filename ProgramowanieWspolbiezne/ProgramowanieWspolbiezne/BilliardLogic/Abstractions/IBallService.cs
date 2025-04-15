using ProgramowanieWspolbiezne.BilliardCore.Abstractions;

namespace ProgramowanieWspolbiezne.BilliardLogic.Abstractions;

public interface IBallService
{
    event EventHandler<IBall>? BallMoved;
    IEnumerable<IBall> GetBalls();
    void CreateBalls(int count, double tableWidth, double tableHeight);
    void StartMovement();
    void StopMovement();
}