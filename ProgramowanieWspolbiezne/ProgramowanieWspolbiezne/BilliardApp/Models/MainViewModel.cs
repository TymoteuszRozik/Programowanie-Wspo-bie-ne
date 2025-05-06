using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ProgramowanieWspolbiezne.BilliardCore.Abstractions;
using ProgramowanieWspolbiezne.BilliardLogic.Abstractions;

namespace ProgramowanieWspolbiezne.BilliardApp.Models;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IBallService ballService;
    private readonly ObservableCollection<BallModel> balls = new();
    private int ballCount = 5;
    private bool isRunning;

    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }

    public ObservableCollection<BallModel> Balls => balls;
    public int BallCount
    {
        get => ballCount;
        set
        {
            ballCount = value;
            OnPropertyChanged();
        }
    }

    public bool IsRunning
    {
        get => isRunning;
        private set
        {
            isRunning = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel(IBallService ballService)
    {
        this.ballService = ballService;
        this.ballService.BallMoved += OnBallMoved;
    
        StartCommand = new RelayCommand(Start);
        StopCommand = new RelayCommand(Stop);
    }

    private void OnBallMoved(object? sender, IBall ball)
    {
        var ballModel = Balls.FirstOrDefault(b => 
            Math.Abs(b.X - ball.X) < 0.1 && 
            Math.Abs(b.Y - ball.Y) < 0.1);
    
        ballModel?.Update();
    }

    private void Start()
    {
        Balls.Clear();
        ballService.CreateBalls(BallCount, 863, 400);
    
        foreach (var ball in ballService.GetBalls())
        {
            Balls.Add(new BallModel(ball, 1.0));
        }
    
        ballService.StartMovement();
        IsRunning = true;
    }

    private void Stop()
    {
        ballService.StopMovement();
        IsRunning = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}