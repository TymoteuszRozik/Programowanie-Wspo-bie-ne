namespace ProgramowanieWspolbiezne.BilliardCore.Abstractions;

public interface IBall
{
    double X { get; }
    double Y { get; }
    double Diameter { get; }
    double Mass { get; }
    string Color { get; }

    double VelocityX { get; set; }
    double VelocityY { get; set; }

    void Move(double newX, double newY);
    void ApplyForce(double forceX, double forceY);
    void UpdatePosition();
    public interface IBall
    DateTime LastLoggedTime { get; }
    string GetLogEntry();
}