using ProgramowanieWspolbiezne.BilliardCore.Abstractions;

namespace ProgramowanieWspolbiezne.BilliardCore.Models;

public class Ball : IBall
{
    public double X { get; private set; }
    public double Y { get; private set; }
    public double Diameter { get; }
    public double Mass { get; }
    public string Color { get; }

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
}