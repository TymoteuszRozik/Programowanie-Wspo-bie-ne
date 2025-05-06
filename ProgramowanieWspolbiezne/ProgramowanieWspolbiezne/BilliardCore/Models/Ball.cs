using ProgramowanieWspolbiezne.BilliardCore.Abstractions;

namespace ProgramowanieWspolbiezne.BilliardCore.Models;

public class Ball : IBall
{
    public double X { get; private set; }
    public double Y { get; private set; }
    public double Diameter { get; }
    public double Radius => Diameter / 2;
    public double Mass { get; }
    public string Color { get; }
    public double VelocityX { get; set; }
    public double VelocityY { get; set; }

    public Ball(double x, double y, double diameter, double mass, string color)
    {
        X = x;
        Y = y;
        Diameter = diameter;
        Mass = mass;
        Color = color;
    }

    public void Move(double newX, double newY)
    {
        X = newX;
        Y = newY;
    }

    public void ApplyForce(double forceX, double forceY)
    {
        VelocityX += forceX / Mass;
        VelocityY += forceY / Mass;
    }
}