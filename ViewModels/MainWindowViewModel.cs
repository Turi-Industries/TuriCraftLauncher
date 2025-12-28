using System;
using System.Threading.Tasks;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TuriCraftLauncher.Services;

namespace TuriCraftLauncher.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly LauncherService _launcherService = new();

    private MLaunchOption? _launchOption;

    // Pseudo fixe pour le mode Offline
    private const string DefaultUsername = "Turi-Ip-Ip";

    [ObservableProperty]
    private string _statusText = "Prêt à jouer";

    [ObservableProperty]
    private bool _isBusy = false;

    [ObservableProperty]
    private double _progressValue = 0;
    
    [ObservableProperty]
    private bool _isIndeterminate = false;

    [RelayCommand]
    private async Task PlayAsync()
    {
        _launchOption = new MLaunchOption
        {
            MaximumRamMb = 6144,
            Session = MSession.CreateOfflineSession(DefaultUsername),
        };
        try
        {
            IsBusy = true;
            StatusText = "Initialisation...";
            ProgressValue = 0;
            IsIndeterminate = true;

            StatusText = "Vérification du modpack";
            await _launcherService.Installer();
            
            StatusText = "Lancement du jeu...";
            await _launcherService.Runner(_launchOption);
            
            StatusText = "Jeu lancé ! Bon jeu !";
            
            // Optionnel : Fermer le launcher après quelques secondes
            await Task.Delay(3000);
            // Environment.Exit(0); 
        }
        catch (Exception ex)
        {
            StatusText = $"Erreur : {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            IsIndeterminate = false;
            ProgressValue = 0;
        }
    }
}