using ProgramowanieWspolbiezne.BilliardCore.Abstractions;
using System;
using System.IO;
using System.Threading;
using System.Text;
using System.Diagnostics;

namespace ProgramowanieWspolbiezne.BilliardCore.Models;

public class Ball : IBall
{
    private readonly object _positionLock = new object();
    private readonly object _velocityLock = new object();
    private readonly string _logFilePath;
    private DateTime _lastUpdateTime;

    public double X { get; private set; }
    public double Y { get; private set; }
    public double Diameter { get; }
    public double Radius => Diameter / 2;
    public double Mass { get; }
    public string Color { get; }

    private double _velocityX;
    private double _velocityY;

    public double VelocityX
    {
        get { lock (_velocityLock) return _velocityX; }
        set { lock (_velocityLock) _velocityX = value; }
    }

    public double VelocityY
    {
        get { lock (_velocityLock) return _velocityY; }
        set { lock (_velocityLock) _velocityY = value; }
    }

    public Ball(double x, double y, double diameter, double mass, string color)
    {
        X = x;
        Y = y;
        Diameter = diameter;
        Mass = mass;
        Color = color;
        _lastUpdateTime = DateTime.Now;
        _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"ball_{Guid.NewGuid()}.log");
    }

    public void Move(double newX, double newY)
    {
        lock (_positionLock)
        {
            X = newX;
            Y = newY;
        }
        LogState();
    }

    public void ApplyForce(double forceX, double forceY)
    {
        lock (_velocityLock)
        {
            _velocityX += forceX / Mass;
            _velocityY += forceY / Mass;
        }
        LogState();
    }

    public void UpdatePosition()
    {
        var currentTime = DateTime.Now;
        double deltaTime = (currentTime - _lastUpdateTime).TotalSeconds;
        _lastUpdateTime = currentTime;


        lock (_positionLock)
            lock (_velocityLock)
            {
                double scaledVelocityX = _velocityX * 100;
                double scaledVelocityY = _velocityY * 10;

                Move(
                    X + scaledVelocityX * deltaTime,
                    Y + scaledVelocityY * deltaTime
                );

                _velocityX *= Math.Pow(1, deltaTime);
                _velocityY *= Math.Pow(1, deltaTime);
            }
    }

    private void LogState()
    {
        try
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    var logEntry = $"{DateTime.Now:O}|{X}|{Y}|{VelocityX}|{VelocityY}\n";
                    File.AppendAllText(_logFilePath, logEntry, Encoding.ASCII);
                }
                catch (IOException)
                {
                    Debug.WriteLine("Logging delayed due to IO congestion");
                }
            });
        }
        catch
        {
        }
    }
}