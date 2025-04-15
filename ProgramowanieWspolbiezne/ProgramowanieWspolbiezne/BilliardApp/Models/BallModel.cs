using System.ComponentModel;
using System.Drawing;
using System.Windows.Media;
using ProgramowanieWspolbiezne.BilliardCore.Abstractions;

namespace ProgramowanieWspolbiezne.BilliardApp.Models;

public class BallModel : INotifyPropertyChanged
{
    private readonly IBall ball;
    private readonly double scale;

    public BallModel(IBall ball, double scale)
    {
        this.ball = ball;
        this.scale = scale;
    }

    public double X => ball.X * scale;
    public double Y => ball.Y * scale;
    public double Diameter => ball.Diameter * scale;
    public Brush Color => (Brush)new BrushConverter().ConvertFromString(ball.Color);

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Update()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(X)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Y)));
    }
}