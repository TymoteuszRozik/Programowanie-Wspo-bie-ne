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

    // Metody
    void Move(double newX, double newY);
    void ApplyForce(double forceX, double forceY);
    void UpdatePosition();
    public interface IBall
{
    // ... existing properties ...
    DateTime LastLoggedTime { get; }
    string GetLogEntry();
}
}