using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using ProgramowanieWspolbiezne.BilliardCore.Abstractions;
using ProgramowanieWspolbiezne.BilliardCore.Models;
using ProgramowanieWspolbiezne.BilliardLogic.Abstractions;

namespace ProgramowanieWspolbiezne.BilliardLogic.Services;

public class BallService : IBallService
{
    private readonly IBallRepository _ballRepository;
    private double _tableWidth;
    private double _tableHeight;
    private const double Restitution = 1;
    private CancellationTokenSource _cts;
    private readonly ConcurrentBag<IBall> _updatedBalls = new();

    public BallService(IBallRepository ballRepository)
    {
        _ballRepository = ballRepository;
        _gameTimer = new Timer(GameTick, null, Timeout.Infinite, Timeout.Infinite);
    }

    private Timer _gameTimer;

    public void StartMovement()
    {
        StopMovement();

        _cts = new CancellationTokenSource();

        _gameTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(32));
    }

    public void StopMovement()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        _gameTimer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    private void GameTick(object state)
    {
        if (_cts?.IsCancellationRequested ?? true)
            return;

        try
        {
            _updatedBalls.Clear();
            _ballRepository.UpdateAllPositions();

            var balls = _ballRepository.GetBalls().ToList();
            HandleWallCollisions(balls);
            HandleBallCollisions(balls);
        }
        catch (Exception ex)
        {
            StopMovement();
        }
    }

    public void Dispose()
    {
        StopMovement();
        _gameTimer?.Dispose();
    }


    private void HandleWallCollisions(IList<IBall> balls)
    {
        foreach (var ball in balls)
        {
            bool updated = false;
            double newX = ball.X;
            double newY = ball.Y;

            if (ball.X < 0)
            {
                newX = 0;
                ball.VelocityX = -ball.VelocityX * Restitution;
                updated = true;
            }
            else if (ball.X + ball.Diameter > _tableWidth)
            {
                newX = _tableWidth - ball.Diameter;
                ball.VelocityX = -ball.VelocityX * Restitution;
                updated = true;
            }

            if (ball.Y < 0)
            {
                newY = 0;
                ball.VelocityY = -ball.VelocityY * Restitution;
                updated = true;
            }
            else if (ball.Y + ball.Diameter > _tableHeight)
            {
                newY = _tableHeight - ball.Diameter;
                ball.VelocityY = -ball.VelocityY * Restitution;
                updated = true;
            }

            if (updated)
            {
                ball.Move(newX, newY);
                _updatedBalls.Add(ball);
            }
        }
    }

    public IEnumerable<IBall> GetBalls() => _ballRepository.GetBalls();

    public void CreateBalls(int count, double tableWidth, double tableHeight)
    {
        _tableWidth = tableWidth;
        _tableHeight = tableHeight;

        _ballRepository.Clear();

        var random = new Random();
        for (int i = 0; i < count; i++)
        {
            var diameter = 20 + random.NextDouble() * 20;
            var x = random.NextDouble() * (tableWidth - diameter);
            var y = random.NextDouble() * (tableHeight - diameter);
            var mass = diameter;
            var color = $"#{random.Next(0x1000000):X6}";

            var ball = new Ball(x, y, diameter, mass, color)
            {
                VelocityX = (random.NextDouble() - 0.5) * 4,
                VelocityY = (random.NextDouble() - 0.5) * 4
            };

            _ballRepository.AddBall(ball);
        }
    }

    public void SetBoundaries(double width, double height)
    {
        _tableWidth = width;
        _tableHeight = height;
    }

    private void HandleBallCollisions(IList<IBall> balls)
    {
        for (int i = 0; i < balls.Count; i++)
        {
            for (int j = i + 1; j < balls.Count; j++)
            {
                var ball1 = balls[i];
                var ball2 = balls[j];

                double ball1Radius = ball1.Diameter / 2;
                double ball2Radius = ball2.Diameter / 2;
                double dx = (ball1.X + ball1Radius) - (ball2.X + ball2Radius);
                double dy = (ball1.Y + ball1Radius) - (ball2.Y + ball2Radius);
                double distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance < ball1Radius + ball2Radius)
                {
                    double nx = dx / distance;
                    double ny = dy / distance;

                    double relativeVelocityX = ball1.VelocityX - ball2.VelocityX;
                    double relativeVelocityY = ball1.VelocityY - ball2.VelocityY;

                    double impulse = (2.0 * (relativeVelocityX * nx + relativeVelocityY * ny)) /
                                    (1.0 / ball1.Mass + 1.0 / ball2.Mass);

                    ball1.VelocityX -= impulse * nx / ball1.Mass;
                    ball1.VelocityY -= impulse * ny / ball1.Mass;
                    ball2.VelocityX += impulse * nx / ball2.Mass;
                    ball2.VelocityY += impulse * ny / ball2.Mass;

                    double overlap = (ball1Radius + ball2Radius) - distance;
                    ball1.Move(ball1.X + nx * overlap * 0.5, ball1.Y + ny * overlap * 0.5);
                    ball2.Move(ball2.X - nx * overlap * 0.5, ball2.Y - ny * overlap * 0.5);

                    _updatedBalls.Add(ball1);
                    _updatedBalls.Add(ball2);
                }
            }
        }
    }
}