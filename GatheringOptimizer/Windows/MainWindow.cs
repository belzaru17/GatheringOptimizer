using Dalamud.Interface.Textures;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using GatheringOptimizer.Algorithm;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GatheringOptimizer.Windows;

public class MainWindow : Window, IDisposable
{
    private enum BestResultSelector {
        BEST_MIN = 0,
        BEST_AVG,
        BEST_MAX,
    };

    public MainWindow(Plugin plugin) : base(
        "Gathering Optimizer", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.plugin = plugin;

        currentGP = maxGP = plugin.Configuration.MaxGP;
        SizeConstraints = new() {
            MinimumSize = new Vector2(450, 600),
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

        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Max GP", ref maxGP);
        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Current GP", ref currentGP);
        ImGui.SameLine();
        ImGui.SetCursorPosX(200);
        if (ImGui.Button("Auto"))
        {
            int newMaxGP = ((int?)Plugin.ClientState.LocalPlayer?.MaxGp) ?? maxGP;
            int newCurrentGP = ((int?)Plugin.ClientState.LocalPlayer?.CurrentGp) ?? currentGP;
            if ((Plugin.ClientState.LocalPlayer != null) && ((newMaxGP != maxGP) || (newCurrentGP != currentGP)))
            {
                changed = true;
                maxGP = newMaxGP;
                currentGP = newCurrentGP;
            }
        }

        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Attempts", ref attempts);
        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Attempt Items", ref attemptItems);
        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Gathering Chance", ref gatheringChance);
        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Gatherer's Boon Chance", ref gathererBoonChance);
        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Bountiful Yield items", ref bountifulYieldItems);
        ImGui.SetNextItemWidth(100);
        ImGui.Spacing();
        ImGui.Separator();

        int newBestResultSelector = (int)bestResultSelector;
        changed |= ImGui.RadioButton("Min", ref newBestResultSelector, (int)BestResultSelector.BEST_MIN);
        ImGui.SameLine();
        changed |= ImGui.RadioButton("Avg", ref newBestResultSelector, (int)BestResultSelector.BEST_AVG);
        ImGui.SameLine();
        changed |= ImGui.RadioButton("Max", ref newBestResultSelector, (int)BestResultSelector.BEST_MAX);
        if (changed || bestResult == null)
        {
            bestResultSelector = (BestResultSelector)newBestResultSelector;
            switch (bestResultSelector)
            {
                case BestResultSelector.BEST_MIN:
                    {
                        bestResult = Optimizer.GenerateBestResult(GetParameters(), GatheringResult.BetterMin, currentGP);
                        break;
                    }
                case BestResultSelector.BEST_AVG:
                    {
                        bestResult = Optimizer.GenerateBestResult(GetParameters(), GatheringResult.BetterAvg, currentGP);
                        break;
                    }
                case BestResultSelector.BEST_MAX:
                    {
                        bestResult = Optimizer.GenerateBestResult(GetParameters(), GatheringResult.BetterMax, currentGP);
                        break;
                    }
            }
        }

        ImGui.Spacing();
        DrawTopResult(bestResult);
    }

    private GatheringParameters GetParameters()
    {
        return new GatheringParameters(currentGP, attempts, gatheringChance / 100.0, gathererBoonChance / 100.0, attemptItems, bountifulYieldItems);
    }

    private static bool IsBotanist()
    {
        return Plugin.ClientState.LocalPlayer?.ClassJob.Id == 17;
    }

    private void DrawTopResult(GatheringResult? result)
    {
        if (result == null)
        {
            return;
        }
        if (ImGui.BeginChild("Result"))
        {
            ImGui.Columns(2);
            ImGui.Text($"GP:  {result.State.UsedGP,4:N0}");
            ImGui.Text($"Min: {result.Min,5:N2}"); ImGui.SameLine();
            ImGui.Text($"Avg: {result.Avg,5:N2}"); ImGui.SameLine();
            ImGui.Text($"Max: {result.Max,5:N2}");
            ImGui.NextColumn();
            ImGui.Text("Actions");
            foreach (var action in result.Actions)
            {
                if (IsBotanist()) {
                    var icon = Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(action.IconId_BOTANIST));

                    ImGui.Image(icon.GetWrapOrEmpty().ImGuiHandle, new Vector2(24, 24));
                    ImGui.SameLine();
                    ImGui.Text($" {action.Name_BOTANIST}");
                } else
                {
                    var icon = Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(action.IconId_MINER));

                    ImGui.Image(icon.GetWrapOrEmpty().ImGuiHandle, new Vector2(24, 24));
                    ImGui.SameLine();
                    ImGui.Text($" {action.Name_MINER}");
                }
            }
            ImGui.EndChild();
        }
    }

    private static void DrawResultsDebugView(IEnumerable<GatheringResult> results)
    {
        bool botanist = IsBotanist();
        if (ImGui.BeginTable("Results", 5, ImGuiTableFlags.ScrollY | ImGuiTableFlags.RowBg))
        {
            ImGui.TableSetupColumn("GP", ImGuiTableColumnFlags.WidthFixed, 30);
            ImGui.TableSetupColumn("Min", ImGuiTableColumnFlags.WidthFixed, 40);
            ImGui.TableSetupColumn("Avg", ImGuiTableColumnFlags.WidthFixed, 40);
            ImGui.TableSetupColumn("Max", ImGuiTableColumnFlags.WidthFixed, 40);
            ImGui.TableSetupColumn("Actions");
            ImGui.TableHeadersRow();

            foreach (var result in results)
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text($"{result.State.UsedGP,4:N0}");
                ImGui.TableNextColumn();
                ImGui.Text($"{result.Min,5:N2}");
                ImGui.TableNextColumn();
                ImGui.Text($"{result.Avg,5:N2}");
                ImGui.TableNextColumn();
                ImGui.Text($"{result.Max,5:N2}");
                ImGui.TableNextColumn();
                ImGui.Text("");
                foreach (var action in result.Actions)
                {
                    ImGui.SameLine();
                    if (botanist)
                    {
                        ImGui.Text($" {action.Name_BOTANIST}");
                    }
                    else
                    {
                        ImGui.Text($" {action.Name_MINER}");
                    }
                }
            }
            ImGui.EndTable();
        }
    }

    private readonly Plugin plugin;

    private int maxGP;
    private int currentGP;
    private int attempts = 6;
    private int attemptItems = 1;
    private int gatheringChance = 100;
    private int gathererBoonChance = 30;
    private int bountifulYieldItems = 2;

    private GatheringResult? bestResult = null;
    private BestResultSelector bestResultSelector = BestResultSelector.BEST_AVG;
}
