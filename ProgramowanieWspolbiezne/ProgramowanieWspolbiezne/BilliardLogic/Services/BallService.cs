using ProgramowanieWspolbiezne.BilliardCore.Abstractions;
using ProgramowanieWspolbiezne.BilliardCore.Models;
using ProgramowanieWspolbiezne.BilliardLogic.Abstractions;

namespace ProgramowanieWspolbiezne.BilliardLogic.Services;

public class BallService : IBallService
{
    private readonly IBallRepository ballRepository;
    private readonly Random random = new();
    private readonly System.Timers.Timer timer;
    private double tableWidth;
    private double tableHeight;

    public event EventHandler<IBall>? BallMoved;

    public BallService(IBallRepository ballRepository)
    {
        this.ballRepository = ballRepository;
        timer = new System.Timers.Timer(16); // ~60 FPS
        timer.Elapsed += (s, e) => MoveBalls();
        timer.AutoReset = true; // Make sure this is set!
    }

    public IEnumerable<IBall> GetBalls() => ballRepository.GetBalls();

    public void CreateBalls(int count, double tableWidth, double tableHeight)
    {
        this.tableWidth = tableWidth;
        this.tableHeight = tableHeight;
        
        ballRepository.Clear();
        
        for (int i = 0; i < count; i++)
        {
            var diameter = 20 + random.NextDouble() * 20;
            var x = random.NextDouble() * (tableWidth - diameter);
            var y = random.NextDouble() * (tableHeight - diameter);
            var mass = diameter; // Simplified - mass proportional to size
            var color = $"#{random.Next(0x1000000):X6}";
            
            ballRepository.AddBall(new Ball(x, y, diameter, mass, color));
        }
    }

    public void StartMovement() => timer.Start();
    public void StopMovement() => timer.Stop();

    private void MoveBalls()
    {
        foreach (var ball in ballRepository.GetBalls().Cast<Ball>())
        {
            // Simple random movement keeping ball within bounds
            var newX = ball.X + (random.NextDouble() * 4 - 2);
            var newY = ball.Y + (random.NextDouble() * 4 - 2);
        
            // Boundary checks (considering ball diameter)
            newX = Math.Max(0, Math.Min(tableWidth - ball.Diameter, newX));
            newY = Math.Max(0, Math.Min(tableHeight - ball.Diameter, newY));
        
            ball.Move(newX, newY);
            BallMoved?.Invoke(this, ball);
        }
    }
}