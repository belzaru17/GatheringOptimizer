using Dalamud.Interface.Windowing;
using GatheringOptimizer.Algorithm;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GatheringOptimizer.Windows;

public class MainWindow : Window, IDisposable
{
    public MainWindow(Plugin plugin) : base(
        "Gathering Optimizer", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        maxGP = plugin.Configuration.MaxGP;
        SizeConstraints = new() {
            MinimumSize = new Vector2(1000, 400),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Draw()
    {
        bool changed = false;
        changed |= ImGui.InputInt("Max GP", ref maxGP);
        changed |= ImGui.InputInt("Attempts", ref attempts);
        changed |= ImGui.InputInt("Attempt Items", ref attemptItems);
        changed |= ImGui.InputInt("Gathering Chance", ref gatheringChance);
        changed |= ImGui.InputInt("Gatherer's Boon Chance", ref gathererBoonChance);
        changed |= ImGui.InputInt("Bountiful Yield items", ref bountifulYieldItems);

        if (changed)
        {
            result = null;
        }
        if (ImGui.Button("Min"))
        {
            result = Optimizer.GenerateResults(GetParameters()).MaxBy(x => x.Min);
        }
        ImGui.SameLine();
        if (ImGui.Button("Avg"))
        {
            result = Optimizer.GenerateResults(GetParameters()).MaxBy(x => x.Avg);
        }
        ImGui.SameLine();
        if (ImGui.Button("Max"))
        {
            result = Optimizer.GenerateResults(GetParameters()).MaxBy(x => x.Max);
        }
        if (result != null)
        {
            ImGui.Text($"GP: {result.GP,4:N0} Min: {result.Min,5:N2} Avg: {result.Avg} Max: {result.Avg,5:N2}");
            foreach (var action in result.ActionsList.Actions)
            {
                ImGui.Text(action.Name);
            }
        }
    }

    private GatheringParameters GetParameters()
    {
        return new GatheringParameters(maxGP, attempts, gatheringChance / 100.0, gathererBoonChance / 100.0, attemptItems, bountifulYieldItems);
    }


    private int maxGP;
    private int attempts = 4;
    private int attemptItems = 1;
    private int gatheringChance = 100;
    private int gathererBoonChance = 0;
    private int bountifulYieldItems = 1;

    private GatheringResult? result = null;
}
