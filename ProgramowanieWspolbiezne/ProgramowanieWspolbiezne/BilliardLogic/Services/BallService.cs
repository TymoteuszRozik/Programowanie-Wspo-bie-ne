using System;
using ProgramowanieWspolbiezne.BilliardCore.Abstractions;
using ProgramowanieWspolbiezne.BilliardCore.Models;
using ProgramowanieWspolbiezne.BilliardLogic.Abstractions;

namespace ProgramowanieWspolbiezne.BilliardLogic.Services;

public class BallService : IBallService
{
    private readonly Random random = new();
    private readonly IBallRepository ballRepository;
    private readonly System.Timers.Timer timer;
    private double tableWidth;
    private double tableHeight;
    private const double Friction = 0.0; // Brak tarcia
    private const double Restitution = 1.0; // Odbicie doskonale sprężyste

    public event EventHandler<IBall>? BallMoved;

    public BallService(IBallRepository ballRepository)
    {
        this.ballRepository = ballRepository;
        timer = new System.Timers.Timer(16); // ~60 FPS
        timer.Elapsed += async (s, e) => await MoveBallsAsync();
        timer.AutoReset = true;
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
            var mass = diameter;
            var color = $"#{random.Next(0x1000000):X6}";

            var ball = new Ball(x, y, diameter, mass, color);

            ball.VelocityX = (random.NextDouble() - 0.5) * 5;
            ball.VelocityY = (random.NextDouble() - 0.5) * 5;

            ballRepository.AddBall(ball);
        }
    }

    public void StartMovement() => timer.Start();
    public void StopMovement() => timer.Stop();

    private async Task MoveBallsAsync()
    {
        var balls = ballRepository.GetBalls().ToList();

        foreach (var ball in balls.Cast<Ball>())
        {
            var newX = ball.X + ball.VelocityX;
            var newY = ball.Y + ball.VelocityY;

            HandleWallCollision(ball, ref newX, ref newY);

            ball.Move(newX, newY);
        }

        HandleBallCollisions(balls);

        foreach (var ball in balls)
        {
            BallMoved?.Invoke(this, ball);
        }
    }

    private void HandleWallCollision(Ball ball, ref double newX, ref double newY)
    {
        if (newX < 0)
        {
            newX = 0;
            ball.VelocityX = -ball.VelocityX * Restitution;
        }
        else if (newX + ball.Diameter > tableWidth)
        {
            newX = tableWidth - ball.Diameter;
            ball.VelocityX = -ball.VelocityX * Restitution;
        }

        if (newY < 0)
        {
            newY = 0;
            ball.VelocityY = -ball.VelocityY * Restitution;
        }
        else if (newY + ball.Diameter > tableHeight)
        {
            newY = tableHeight - ball.Diameter;
            ball.VelocityY = -ball.VelocityY * Restitution;
        }
    }

    private void HandleBallCollisions(IEnumerable<IBall> balls)
    {
        var ballList = balls.ToList();

        for (int i = 0; i < ballList.Count; i++)
        {
            for (int j = i + 1; j < ballList.Count; j++)
            {
                var ball1 = (Ball)ballList[i];
                var ball2 = (Ball)ballList[j];

                double dx = (ball1.X + ball1.Radius) - (ball2.X + ball2.Radius);
                double dy = (ball1.Y + ball1.Radius) - (ball2.Y + ball2.Radius);
                double distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance < ball1.Radius + ball2.Radius)
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

                    double overlap = (ball1.Radius + ball2.Radius) - distance;
                    ball1.Move(ball1.X + nx * overlap * 0.5, ball1.Y + ny * overlap * 0.5);
                    ball2.Move(ball2.X - nx * overlap * 0.5, ball2.Y - ny * overlap * 0.5);
                }
            }
        }
    }
}