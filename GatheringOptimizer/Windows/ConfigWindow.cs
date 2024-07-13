using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace GatheringOptimizer.Windows;

public class ConfigWindow : Window, IDisposable
{
    public ConfigWindow(Plugin plugin) : base(
        "Gathering Optimizer Configuration",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.plugin = plugin;

        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Draw()
    {
        var saveConfig = false;

        int maxGP = plugin.Configuration.MaxGP;
        if (ImGui.InputInt("Max GP", ref maxGP))
        {
            plugin.Configuration.MaxGP = maxGP;
            saveConfig = true;
        }

        ImGui.Spacing();
        ImGui.Separator();
        string version = typeof(Plugin).Assembly.GetName().Version?.ToString(3) ?? "Unknown";
        ImGui.Text($"Version: {version}");

        if (saveConfig)
        {
            plugin.Configuration.Save();
        }
    }

    private readonly Plugin plugin;
}
