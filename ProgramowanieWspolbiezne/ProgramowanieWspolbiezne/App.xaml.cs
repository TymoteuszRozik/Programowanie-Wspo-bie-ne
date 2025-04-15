using System.Configuration;
using System.Data;
using System.Windows;
using ProgramowanieWspolbiezne.BilliardApp.Models;
using ProgramowanieWspolbiezne.BilliardCore.Repositories;
using ProgramowanieWspolbiezne.BilliardLogic.Services;

namespace ProgramowanieWspolbiezne;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        var ballRepository = new BallRepository();
        var ballService = new BallService(ballRepository);
        var mainViewModel = new MainViewModel(ballService);
        
        var mainWindow = new MainWindow { DataContext = mainViewModel };
        mainWindow.Show();
    }
}