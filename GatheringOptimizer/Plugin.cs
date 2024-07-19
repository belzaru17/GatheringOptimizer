using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using GatheringOptimizer.Windows;
using System;
using Dalamud.Plugin.Services;

namespace GatheringOptimizer;

public class Plugin : IDalamudPlugin
{
    public static string Name => "Gathering Optimizer";

    public Configuration Configuration { get; init; }
    public WindowSystem WindowSystem = new("GatheringOptimizer");

    private const string CommandName = "/go";

    [PluginService]
    public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

    [PluginService]
    public static ICommandManager CommandManager { get; private set; } = null!;

    [PluginService]
    public static IClientState ClientState { get; private set; } = null!;

    [PluginService]
    public static IDataManager Data { get; private set; } = null!;

    [PluginService]
    public static IPluginLog Log { get; private set; } = null!;

    [PluginService]
    public static ITextureProvider TextureProvider { get; private set; } = null!;

    [PluginService]
    public static IGameGui GameGui { get; private set; } = null!;

    [PluginService]
    public static IAddonLifecycle AddonLifecycle { get; private set; } = null!;

    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);

        this.ConfigWindow = new ConfigWindow(this);
        this.MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Optimize gathering nodes"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenMainUi += OpenMainUI;
        PluginInterface.UiBuilder.OpenConfigUi += OpenConfigUI;
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();
        CommandManager.RemoveHandler(CommandName);

        MainWindow.Dispose();
        ConfigWindow.Dispose();

        GC.SuppressFinalize(this);
    }

    private void OnCommand(string command, string args)
    {
        if (args == "config")
        {
            OpenConfigUI();
        }
        else
        {
            OpenMainUI();
        }
    }

    private void DrawUI()
    {

        WindowSystem.Draw();
    }

    public void OpenMainUI()
    {
        MainWindow.IsOpen = true;
    }

    public void OpenConfigUI()
    {
        ConfigWindow.IsOpen = true;
    }
}
