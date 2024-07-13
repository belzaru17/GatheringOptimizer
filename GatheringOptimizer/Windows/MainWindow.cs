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
            lastSortColumn = -1;
        }

        if (results != null)
        {
            if (ImGui.BeginTable("Results", 5, ImGuiTableFlags.ScrollY | ImGuiTableFlags.RowBg | ImGuiTableFlags.Sortable))
            {
                ImGui.TableSetupColumn("GP", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.PreferSortDescending, 30);
                ImGui.TableSetupColumn("Min", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.PreferSortDescending, 40);
                ImGui.TableSetupColumn("Avg", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.PreferSortDescending, 40);
                ImGui.TableSetupColumn("Max", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.PreferSortDescending, 40);
                ImGui.TableSetupColumn("Actions");
                ImGui.TableHeadersRow();

                var sortSpecs = ImGui.TableGetSortSpecs();
                if (sortSpecs.SpecsCount > 0)
                {
                    var sortOrder = sortSpecs.Specs;
                    if (sortOrder.ColumnIndex != lastSortColumn || sortOrder.SortDirection != lastSortDirection)
                    {
                        if (sortOrder.ColumnIndex == 0)
                        {
                            results = results.OrderByDescending(x => x.GP);
                        }
                        else if (sortOrder.ColumnIndex == 1)
                        {
                            results = results.OrderByDescending(x => x.Min);
                        }
                        else if (sortOrder.ColumnIndex == 2)
                        {
                            results = results.OrderByDescending(x => x.Avg);
                        }
                        else if (sortOrder.ColumnIndex == 3)
                        {
                            results = results.OrderByDescending(x => x.Max);
                        }
                        if (sortOrder.SortDirection == ImGuiSortDirection.Ascending)
                        {
                            results = results.Reverse();
                        }
                        lastSortColumn = sortOrder.ColumnIndex;
                        lastSortDirection = sortOrder.SortDirection;
                    }
                }

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
                    DrawResult(result);
                }
                ImGui.EndTable();
            }
        }
    }

    private GatheringParameters GetParameters()
    {
        return new GatheringParameters(maxGP, attempts, gatheringChance / 100.0, gathererBoonChance / 100.0, attemptItems, bountifulYieldItems);
    }

    private static void DrawResult(GatheringResult result)
    {
        ImGui.Text("");
        foreach (var action in result.ActionsList.Actions)
        {
            ImGui.SameLine();
            ImGui.Text($" {action.Name}");
        }

    }

    private int maxGP;
    private int attempts = 6;
    private int attemptItems = 1;
    private int gatheringChance = 100;
    private int gathererBoonChance = 26;
    private int bountifulYieldItems = 1;

    private IEnumerable<GatheringResult>? results = null;
    private short lastSortColumn = -1;
    private ImGuiSortDirection lastSortDirection = ImGuiSortDirection.Descending;
}
