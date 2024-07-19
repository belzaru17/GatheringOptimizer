using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.UI;
using GatheringOptimizer.Algorithm;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

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
        Plugin.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, "Gathering", GatheringAddonHandler);
        Plugin.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "Gathering", GatheringAddonHandler);
    }

    public void Dispose()
    {
        Plugin.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, "Gathering", GatheringAddonHandler);
        Plugin.AddonLifecycle.UnregisterListener(AddonEvent.PostRefresh, "Gathering", GatheringAddonHandler);
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
        changed |= ImGui.InputInt("Integrity", ref attempts);
        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Gathering Chance", ref gatheringChance);
        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Gatherer's Boon Chance", ref gatherersBoonChance);
        ImGui.Spacing();
        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Yield (#items)", ref attemptItems);
        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Bountiful Yield (# items)", ref bountifulYieldItems);
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
        return new GatheringParameters(currentGP, attempts, gatheringChance / 100.0, gatherersBoonChance / 100.0, attemptItems, bountifulYieldItems);
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

    private unsafe void GatheringAddonHandler(AddonEvent type, AddonArgs args)
    {
        if (type == AddonEvent.PostSetup)
        {
            opened = true;
            return;
        }
        if (!opened)
        {
            return;
        }
        try
        {
            var addon = (AddonGathering*)args.Addon;
            if (addon == null)
            {
                return;
            }
            opened = false;
            attempts = addon->IntegrityTotal->NodeText.ToInteger();

            int? newGatheringChance = null;
            int? newGatherersBoonChance = null ;
            bool emptyGatherersBoonChance = false;
            int? newItemsPerAttempt = null;

            AtkComponentCheckBox*[] gatherComponents = {
                addon->GatheredItemComponentCheckBox1,
                addon->GatheredItemComponentCheckBox2,
                addon->GatheredItemComponentCheckBox3,
                addon->GatheredItemComponentCheckBox4,
                addon->GatheredItemComponentCheckBox5,
                addon->GatheredItemComponentCheckBox6,
                addon->GatheredItemComponentCheckBox7,
                addon->GatheredItemComponentCheckBox8,
            };
            foreach (var component in gatherComponents)
            {
                GetGatheringChance(component, ref newGatheringChance);
                GetGatherersBoonChance(component, ref newGatherersBoonChance, ref emptyGatherersBoonChance);
            }

            if (newGatheringChance.HasValue)
            {
                gatheringChance = newGatheringChance.Value;
            }
            if (newGatherersBoonChance.HasValue)
            {
                gatherersBoonChance = newGatherersBoonChance.Value;
            }
            else if (emptyGatherersBoonChance)
            {
                gatherersBoonChance = 0;
            }
            if (newItemsPerAttempt.HasValue)
            {
                attemptItems = newItemsPerAttempt.Value;
            }
            bestResult = null;
            IsOpen = true;
        }
        catch (Exception e)
        {
            Plugin.Log.Warning(e, "Getting gathering data failed");
        }
    }

    private unsafe void GetGatheringChance(AtkComponentCheckBox* component, ref int? newGatheringChance)
    {
        var node = component->GetTextNodeById(10);
        if (node != null)
        {
            var textNode = node->GetAsAtkTextNode();
            if (textNode != null)
            {
                if (!(textNode->NodeText.ToString() == "" || textNode->NodeText.ToString() == "-"))
                {
                    var thisGatheringChance = textNode->NodeText.ToInteger();
                    if (!newGatheringChance.HasValue || (thisGatheringChance < newGatheringChance.Value))
                    {
                        newGatheringChance = thisGatheringChance;
                    }
                }
            }
        }
    }

    private unsafe void GetGatherersBoonChance(AtkComponentCheckBox* component, ref int? newGatherersBoonChance, ref bool emptyGatherersBoonChance)
    {
        var node = component->GetTextNodeById(16);
        if (node != null)
        {
            var textNode = node->GetAsAtkTextNode();
            if (textNode != null)
            {
                if (textNode->NodeText.ToString() == "-")
                {
                    emptyGatherersBoonChance = true;
                }
                else if (textNode->NodeText.ToString() != "")
                {
                    var thisGatherersBoonChance = textNode->NodeText.ToInteger();
                    if (!newGatherersBoonChance.HasValue || (thisGatherersBoonChance < newGatherersBoonChance.Value))
                    {
                        newGatherersBoonChance = thisGatherersBoonChance;
                    }
                }
            }
        }
    }

    private readonly Plugin plugin;

    private int maxGP;
    private int currentGP;
    private int attempts = 6;
    private int attemptItems = 1;
    private int gatheringChance = 100;
    private int gatherersBoonChance = 30;
    private int bountifulYieldItems = 2;

    private bool opened = false;

    private GatheringResult? bestResult = null;
    private BestResultSelector bestResultSelector = BestResultSelector.BEST_AVG;
}
