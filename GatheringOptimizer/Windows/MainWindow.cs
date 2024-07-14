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
    private enum SortColumn {
        SORT_GP = 0,
        SORT_MIN,
        SORT_AVG,
        SORT_MAX,
    };

    public MainWindow(Plugin plugin) : base(
        "Gathering Optimizer", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.plugin = plugin;

        currentGP = plugin.Configuration.MaxGP;
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

        if (autoCurrentGP)
        {
            int newMaxGP = ((int?)Plugin.ClientState.LocalPlayer?.CurrentGp) ?? currentGP;
            if ((Plugin.ClientState.LocalPlayer != null) && (newMaxGP!= currentGP))
            {
                changed = true;
                currentGP = newMaxGP;
            }
            ImGui.Text($"{currentGP} Max GP");
        }
        else
        {
            ImGui.SetNextItemWidth(100);
            changed |= ImGui.InputInt("Max GP", ref currentGP);
        }
        ImGui.SameLine();
        ImGui.SetCursorPosX(170);
        if (ImGui.Checkbox("Auto Max GP", ref autoCurrentGP))
        {
            changed = true;
            if (!autoCurrentGP)
            {
                currentGP = plugin.Configuration.MaxGP;
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

        if (changed || results == null)
        {
            results = Optimizer.GenerateResults(GetParameters());
            SortResults();
        }

        int newSortColumn = (int)sortColumn;
        bool sortChanged = false;
        sortChanged |= ImGui.RadioButton("Min", ref newSortColumn, (int)SortColumn.SORT_MIN);
        ImGui.SameLine();
        sortChanged |= ImGui.RadioButton("Avg", ref newSortColumn, (int)SortColumn.SORT_AVG);
        ImGui.SameLine();
        sortChanged |= ImGui.RadioButton("Max", ref newSortColumn, (int)SortColumn.SORT_MAX);
        if (sortChanged)
        {
            sortColumn = (SortColumn)newSortColumn;
            SortResults();
        }
        ImGui.SameLine();
        ImGui.Checkbox("Debug View", ref debugView);
        ImGui.Spacing();

        if (results != null)
        {
            if (debugView)
            {
                DrawResultsDebugView(results);
            }
            else if (results.Any())
            {
                DrawTopResult(results.First());
            }
        }
    }

    private GatheringParameters GetParameters()
    {
        return new GatheringParameters(currentGP, attempts, gatheringChance / 100.0, gathererBoonChance / 100.0, attemptItems, bountifulYieldItems);
    }

    private static bool IsBotanist()
    {
        return Plugin.ClientState.LocalPlayer?.ClassJob.Id == 17;
    }

    private static void DrawTopResult(GatheringResult result)
    {
        if (ImGui.BeginChild("Result"))
        {
            ImGui.Columns(2);
            ImGui.Text($"GP:  {result.GP,4:N0}");
            ImGui.Text($"Min: {result.Min,5:N2}"); ImGui.SameLine();
            ImGui.Text($"Avg: {result.Avg,5:N2}"); ImGui.SameLine();
            ImGui.Text($"Max: {result.Max,5:N2}");
            ImGui.NextColumn();
            ImGui.Text("Actions");
            foreach (var action in result.ActionsList.Actions)
            {
                if (IsBotanist()) {
                    ImGui.Text($" {action.Name_BOTANIST}");
                } else
                {
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
                ImGui.Text($"{result.GP,4:N0}");
                ImGui.TableNextColumn();
                ImGui.Text($"{result.Min,5:N2}");
                ImGui.TableNextColumn();
                ImGui.Text($"{result.Avg,5:N2}");
                ImGui.TableNextColumn();
                ImGui.Text($"{result.Max,5:N2}");
                ImGui.TableNextColumn();
                ImGui.Text("");
                foreach (var action in result.ActionsList.Actions)
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

    private void SortResults()
    {
        if (results == null)
        {
            return;
        }
        switch (sortColumn)
        {
            case SortColumn.SORT_GP:
                {
                    results = results.OrderByDescending(x => (x.GP, x.Avg));
                    break;
                }

            case SortColumn.SORT_MIN:
                {
                    results = results.OrderByDescending(x => (x.Min, -x.GP));
                    break;
                }

            case SortColumn.SORT_AVG:
                {
                    results = results.OrderByDescending(x => (x.Avg, -x.GP));
                    break;
                }

            case SortColumn.SORT_MAX:
                {
                    results = results.OrderByDescending(x => (x.Max, -x.GP));
                    break;
                }
        }
    }

    private readonly Plugin plugin;

    private bool autoCurrentGP = true;
    private int currentGP;
    private int attempts = 6;
    private int attemptItems = 1;
    private int gatheringChance = 100;
    private int gathererBoonChance = 30;
    private int bountifulYieldItems = 2;

    private IEnumerable<GatheringResult>? results = null;
    private SortColumn sortColumn = SortColumn.SORT_AVG;
    private bool debugView = false;
}
