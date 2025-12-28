using System;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Installer.NeoForge;
using CmlLib.Core.Installer.NeoForge.Installers;
using CmlLib.Core.ProcessBuilder;

namespace TuriCraftLauncher.Services;

public class LauncherService
{
    // Config
    const string MinecraftVersion = "1.21.1";
    const string NeoForgeVersion = "21.1.217";
    
    readonly MinecraftPath _minecraftPath;
    readonly MinecraftLauncher _launcher;
    readonly NeoForgeInstaller _modLoader;

    public LauncherService()
    {
        var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _minecraftPath = new MinecraftPath(Path.Combine(folderPath, "TuriCraft"));
        
        _launcher =  new MinecraftLauncher(new MinecraftPath(Path.Combine(folderPath, "TuriCraft")));
        _modLoader = new NeoForgeInstaller(_launcher);
        
        Console.WriteLine($"Minecraft Path: {_minecraftPath}");
        Console.WriteLine($"Minecraft Version: {MinecraftVersion}");
    }

    public async Task Installer()
    {
        await _modLoader.Install(MinecraftVersion, NeoForgeVersion, new NeoForgeInstallOptions());

        await _launcher.InstallAsync(MinecraftVersion);
    }

    public async Task Runner(MLaunchOption launchOption)
    {
        var process = await _launcher.BuildProcessAsync(MinecraftVersion, launchOption);
        
        // 1. On force la redirection pour empêcher l'affichage par défaut
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        // 2. On s'abonne avec une fonction vide.
        // C'est CRUCIAL : cela "vide" le buffer au fur et à mesure pour éviter que le jeu ne freeze,
        // mais comme le code est vide, rien ne s'affiche dans votre console.
        process.OutputDataReceived += (s, e) => { /* On ignore les logs */ };
        process.ErrorDataReceived += (s, e) => { /* On ignore les erreurs */ };

        process.Start();

        // 3. On démarre la lecture (qui va tourner en fond et jeter les données.)
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }

}