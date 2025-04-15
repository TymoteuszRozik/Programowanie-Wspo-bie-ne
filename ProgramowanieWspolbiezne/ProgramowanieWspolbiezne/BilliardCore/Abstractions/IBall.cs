﻿namespace ProgramowanieWspolbiezne.BilliardCore.Abstractions;

public interface IBall
{
    double X { get; }
    double Y { get; }
    double Diameter { get; }
    double Mass { get; }
    string Color { get; }
}
