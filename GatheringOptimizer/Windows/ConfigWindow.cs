using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using GatheringOptimizer.Algorithm.Collectables;
using System;
using System.Numerics;

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
            MinimumSize = new Vector2(425, 380),
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

        if (ImGui.BeginTable("Rotation Configurations", 4))
        {
            ImGui.TableSetupColumn("Rotation", ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableSetupColumn("Enabled", ImGuiTableColumnFlags.WidthFixed, 60);
            ImGui.TableSetupColumn("Min GP", ImGuiTableColumnFlags.WidthFixed, 80);
            ImGui.TableSetupColumn("No Extra GP", ImGuiTableColumnFlags.WidthFixed, 80);
            ImGui.TableHeadersRow();

            foreach (var rotation in CollectableRotations.Rotations)
            {
                bool changed = false;
                RotationConfiguration rotationConfig;
                if (!plugin.Configuration.RotationConfigurations.TryGetValue(rotation.Id, out rotationConfig!))
                {
                    rotationConfig = rotation.DefaultConfiguration();
                }
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text(rotation.Title);
                ImGui.TableNextColumn();
                if (rotation.MinGP == 0) ImGui.BeginDisabled();
                bool enabled = rotationConfig.Enabled;
                changed |= ImGui.Checkbox($"##Enabled_{rotation.Id}", ref enabled);
                ImGui.TableNextColumn();
                int minGP = rotationConfig.MinGP;
                ImGui.SetNextItemWidth(65);
                changed |= ImGui.InputInt($"##MinGP_{rotation.Id}", ref minGP);
                if (rotation.MinGP == 0) ImGui.EndDisabled();
                ImGui.TableNextColumn();
                bool noExtraGP = rotationConfig.NoExtraGP;
                changed |= ImGui.Checkbox($"##NoExtraGP_{rotation.Id}", ref noExtraGP);

                if (changed)
                {
                    rotationConfig.Enabled = enabled;
                    rotationConfig.NoExtraGP = noExtraGP;
                    rotationConfig.MinGP = minGP;
                    plugin.Configuration.RotationConfigurations[rotation.Id] = rotationConfig;
                    saveConfig = true;
                }
            }

            ImGui.EndTable();
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
