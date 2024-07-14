using Dalamud.Interface.Windowing;
using GatheringOptimizer.Algorithm;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

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

        if (autoGP)
        {
            int newMaxGP = ((int?)Plugin.ClientState.LocalPlayer?.MaxGp) ?? maxGP;
            int newCurrentGP = ((int?)Plugin.ClientState.LocalPlayer?.CurrentGp) ?? currentGP;
            if ((Plugin.ClientState.LocalPlayer != null) && ((newMaxGP !=  maxGP) || (newCurrentGP != currentGP)))
            {
                changed = true;
                maxGP = newMaxGP;
                currentGP = newCurrentGP;
            }
            ImGui.Text($"{maxGP} Max GP");
            ImGui.Text($"{currentGP} Max GP");
        }
        else
        {
            ImGui.SetNextItemWidth(100);
            changed |= ImGui.InputInt("Max GP", ref maxGP);
            ImGui.SetNextItemWidth(100);
            changed |= ImGui.InputInt("Current GP", ref currentGP);
        }
        ImGui.SameLine();
        ImGui.SetCursorPosX(200);
        if (ImGui.Checkbox("Auto GP", ref autoGP))
        {
            changed = true;
            if (!autoGP)
            {
                currentGP = maxGP = plugin.Configuration.MaxGP;
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

        if (changed)
        {
            results = null;
            calculatePending = true;
        }
        if (!(calculateThread?.IsAlive ?? true))
        {
            calculateThread = null;
        }
        if (calculatePending && results != null)
        {
            results = null;
        }
        if (results == null && calculateThread == null)
        {
            calculatePending = false;
            calculateThread = new Thread(() =>
            {
                results = SortResults(Optimizer.GenerateResults(GetParameters(), currentGP));
            });
            calculateThread.Start();
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
            if (results != null)
            {
                results = SortResults(results);
            }
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
        else
        {
            ImGui.Text("Calculating...");
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
            ImGui.Text($"GP:  {result.State.UsedGP,4:N0}");
            ImGui.Text($"Min: {result.Min,5:N2}"); ImGui.SameLine();
            ImGui.Text($"Avg: {result.Avg,5:N2}"); ImGui.SameLine();
            ImGui.Text($"Max: {result.Max,5:N2}");
            ImGui.NextColumn();
            ImGui.Text("Actions");
            foreach (var action in result.Actions)
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

    private IEnumerable<GatheringResult> SortResults(IEnumerable<GatheringResult> results)
    {
        switch (sortColumn)
        {
            case SortColumn.SORT_GP:
                {
                    return results.OrderByDescending(x => (x.State.UsedGP, x.Avg));
                }

            case SortColumn.SORT_MIN:
                {
                    return results.OrderByDescending(x => (x.Min, -x.State.UsedGP));
                }

            case SortColumn.SORT_AVG:
                {
                    return results.OrderByDescending(x => (x.Avg, -x.State.UsedGP));
                }

            case SortColumn.SORT_MAX:
                {
                    return results.OrderByDescending(x => (x.Max, -x.State.UsedGP));
                }
        }
        return results;
    }

    private readonly Plugin plugin;

    private bool autoGP = true;
    private int maxGP;
    private int currentGP;
    private int attempts = 6;
    private int attemptItems = 1;
    private int gatheringChance = 100;
    private int gathererBoonChance = 30;
    private int bountifulYieldItems = 2;

    private IEnumerable<GatheringResult>? results = null;
    private SortColumn sortColumn = SortColumn.SORT_AVG;
    private bool debugView = false;

    private bool calculatePending = true;
    private Thread? calculateThread;
}
