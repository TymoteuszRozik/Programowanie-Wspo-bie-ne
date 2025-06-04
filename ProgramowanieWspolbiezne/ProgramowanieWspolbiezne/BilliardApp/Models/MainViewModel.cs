using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ProgramowanieWspolbiezne.BilliardCore.Abstractions;
using ProgramowanieWspolbiezne.BilliardLogic.Abstractions;
using System.Windows.Threading;

namespace ProgramowanieWspolbiezne.BilliardApp.Models;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IBallService _ballService;
    private readonly ObservableCollection<BallModel> _balls = new();
    private readonly Dictionary<IBall, BallModel> _ballMap = new();
    private readonly DispatcherTimer _renderTimer;
    private int _ballCount = 5;
    private bool _isRunning;
    private double _tableWidth = 863;
    private double _tableHeight = 400;

    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand ToggleCommand { get; }

    public ObservableCollection<BallModel> Balls => _balls;

    public int BallCount
    {
        get => _ballCount;
        set => SetField(ref _ballCount, value);
    }

    public bool IsRunning
    {
        get => _isRunning;
        private set => SetField(ref _isRunning, value);
    }

    public MainViewModel(IBallService ballService)
    {
        _ballService = ballService;

        // Initialize commands with automatic re-evaluation
        StartCommand = new RelayCommand(Start, () => !IsRunning);
        StopCommand = new RelayCommand(Stop, () => IsRunning);
        ToggleCommand = new RelayCommand(Toggle);

        _renderTimer = new DispatcherTimer(DispatcherPriority.Render)
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        _renderTimer.Tick += OnRenderFrame;
    }
    private void Start()
    {
        _balls.Clear();
        _ballMap.Clear();

        _ballService.CreateBalls(BallCount, _tableWidth, _tableHeight);

        var newBalls = _ballService.GetBalls()
            .Select(ball =>
            {
                var model = new BallModel(ball, 1.0);
                _ballMap[ball] = model;
                return model;
            }).ToList();

        foreach (var ball in newBalls)
        {
            _balls.Add(ball);
        }

        _ballService.StartMovement();
        _renderTimer.Start();
        IsRunning = true;
        CommandManager.InvalidateRequerySuggested();
    }

    private void Stop()
    {
        _ballService.StopMovement();
        _renderTimer.Stop();
        IsRunning = false;
        CommandManager.InvalidateRequerySuggested();
    }


    private void OnRenderFrame(object? sender, EventArgs e)
    {
        // Update all ball positions on UI thread
        foreach (var ballModel in _balls)
        {
            ballModel.Update();
        }
    }

    private void Toggle()
    {
        if (IsRunning) Stop();
        else Start();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}