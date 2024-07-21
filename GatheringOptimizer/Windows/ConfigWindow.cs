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
        bool autoOpenOnAnyGather = plugin.Configuration.AutoOpenOnAnyGather;
        if (ImGui.Checkbox("Auto Open on Gather Any Node", ref autoOpenOnAnyGather))
        {
            plugin.Configuration.AutoOpenOnAnyGather = autoOpenOnAnyGather;
            saveConfig = true;
        }
        if (autoOpenOnAnyGather) ImGui.BeginDisabled();
        bool autoOpenOnLegendaryGather = plugin.Configuration.AutoOpenOnLegendaryGather;
        if (ImGui.Checkbox("Auto Open of Gather Legendary Nodes", ref autoOpenOnLegendaryGather))
        {
            plugin.Configuration.AutoOpenOnLegendaryGather = autoOpenOnLegendaryGather;
            saveConfig = true;
        }
        bool autoOpenOnUnspoiledGather = plugin.Configuration.AutoOpenOnUnspoiledGather;
        if (ImGui.Checkbox("Auto Open of Gather Unspoiled Nodes", ref autoOpenOnUnspoiledGather))
        {
            plugin.Configuration.AutoOpenOnUnspoiledGather = autoOpenOnUnspoiledGather;
            saveConfig = true;
        }
        bool autoOpenOnEphemeralGather = plugin.Configuration.AutoOpenOnEphemeralGather;
        if (ImGui.Checkbox("Auto Open of Gather Ephemeeral Nodes", ref autoOpenOnEphemeralGather))
        {
            plugin.Configuration.AutoOpenOnEphemeralGather = autoOpenOnEphemeralGather;
            saveConfig = true;
        }
        if (autoOpenOnAnyGather) ImGui.EndDisabled();

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
