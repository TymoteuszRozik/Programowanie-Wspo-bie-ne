using ProgramowanieWspolbiezne.BilliardCore.Abstractions;

namespace ProgramowanieWspolbiezne.BilliardLogic.Abstractions;

public interface IBallService
{
    IEnumerable<IBall> GetBalls();
    void CreateBalls(int count, double tableWidth, double tableHeight);
    void StartMovement();
    void StopMovement();
    void SetBoundaries(double width, double height);
}