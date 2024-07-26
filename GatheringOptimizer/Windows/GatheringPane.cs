using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Textures;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using GatheringOptimizer.Algorithm;
using ImGuiNET;
using System;
using System.Numerics;

namespace GatheringOptimizer.Windows;

internal class GatheringPane : IPane
{
    public string AddonName => "Gathering";
    public string Title => "Gathering";

    public GatheringPane(Plugin plugin)
    {
        this.plugin = plugin;
    }

    public void DrawPane()
    {
        unsafe
        {
            var addon = GetAddon();
            if (addon->RootNode != null)
            {
                ImGui.SetWindowPos(new Vector2(addon->X + addon->RootNode->Width * addon->Scale, addon->Y));
            }
        }


        bool changed = false;

        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("##CurrentGP", ref currentGP);
        ImGui.SameLine(); ImGui.Text("/"); ImGui.SameLine();
        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("GP##MaxGP", ref maxGP);
        if (currentGP > maxGP)
        {
            currentGP = maxGP;
        }

        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Integrity", ref integrity);
        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Gathering Chance", ref gatheringChance);
        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Gatherer's Boon Chance", ref gatherersBoonChance);
        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Yield (#items)", ref yield);
        ImGui.Spacing();
        ImGui.SetNextItemWidth(100);
        changed |= ImGui.InputInt("Bountiful Yield (# items) [manual]", ref bountifulYieldItems);
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

    public void SetupFromAddon(AddonEvent type, AddonArgs args)
    {
    }

    public unsafe bool UpdateFromAddon(AddonEvent type, AddonArgs args)
    {
        if (type == AddonEvent.PostUpdate) return false;

        if (Plugin.ClientState.LocalPlayer != null)
        {
            maxGP = ((int?)Plugin.ClientState.LocalPlayer?.MaxGp) ?? maxGP;
            currentGP = ((int?)Plugin.ClientState.LocalPlayer?.CurrentGp) ?? currentGP;
        }

        int? newGatheringChance = null;
        int? newGatherersBoonChance = null;
        bool emptyGatherersBoonChance = false;
        int newYield = 1;

        try
        {
            var addon = (AddonGathering*)args.Addon;
            if (addon == null) return false;

            if (addon->IntegrityTotal == null) return false;
            integrity = addon->IntegrityTotal->NodeText.ToInteger();

            for (int i = 0; i < addon->GatheredItemComponentCheckbox.Length; i++)
            {
                var component = addon->GatheredItemComponentCheckbox[i];
                AddonUtils.GetTextNode(component, 10, (value) =>
                {
                    if (!(value.ToString() == "" || value.ToString() == "-"))
                    {
                        var iValue = value.ToInteger();
                        if (!newGatheringChance.HasValue || (iValue < newGatheringChance.Value))
                        {
                            newGatheringChance = iValue;
                        }
                    }
                });
                AddonUtils.GetTextNode(component, 16, (value) =>
                {
                    if (value.ToString() == "-")
                    {
                        emptyGatherersBoonChance = true;
                    }
                    else if (value.ToString() != "")
                    {
                        var iValue = value.ToInteger();
                        if (!newGatherersBoonChance.HasValue || (iValue < newGatherersBoonChance.Value))
                        {
                            newGatherersBoonChance = iValue;
                        }
                    }
                });
                var thisYield = GetYieldItems(component);
                if (thisYield.HasValue)
                {
                    newYield = thisYield.Value;
                }
            }
        }
        catch (Exception e)
        {
            Plugin.Log.Warning(e, "Getting gathering data failed");
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
        yield = newYield;

        bestResult = null;
        return true;
    }

    public unsafe bool ShouldAutoOpen()
    {
        if (plugin.Configuration.AutoOpenOnAnyGather) return true;

        var targetSystem = TargetSystem.Instance();
        if (targetSystem != null)
        {
            var target = targetSystem->GetTargetObject();
            if (target != null && target->ObjectKind == ObjectKind.GatheringPoint)
            {
                var name = target->NameString;
                return ((plugin.Configuration.AutoOpenOnLegendaryGather && name.Contains("Legendary")) ||
                        (plugin.Configuration.AutoOpenOnUnspoiledGather && name.Contains("Unspoiled")) ||
                        (plugin.Configuration.AutoOpenOnEphemeralGather && name.Contains("Ephemeral")));
            }
        }

        return false;
    }

    public unsafe bool ShouldAutoClose()
    {
        var addon = GetAddon();
        return addon == null || !addon->IsVisible;
    }

    public void OnActionUsed(int actionId) { }

    public void OnActorControl(uint type) { }


    private unsafe AddonGathering* GetAddon()
    {
        return (AddonGathering*)Plugin.GameGui.GetAddonByName(AddonName);
    }

    private enum BestResultSelector
    {
        BEST_MIN = 0,
        BEST_AVG,
        BEST_MAX,
    };

    private GatheringParameters GetParameters()
    {
        return new GatheringParameters(currentGP, integrity, gatheringChance / 100.0, gatherersBoonChance / 100.0, yield, bountifulYieldItems);
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
                if (AddonUtils.IsBotanist())
                {
                    var icon = Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(action.IconId_BOTANIST));

                    ImGui.Image(icon.GetWrapOrEmpty().ImGuiHandle, new Vector2(24, 24));
                    ImGui.SameLine();
                    ImGui.Text($" {action.Name_BOTANIST}");
                }
                else
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

    private static unsafe int? GetYieldItems(AtkComponentCheckBox* component)
    {
        if (component == null) return null;

        var iconNode = component->UldManager.SearchNodeById(31);
        if (iconNode == null || !iconNode->IsVisible()) return null;

        var textNode = iconNode->GetComponent()->GetTextNodeById(7);
        if (textNode == null) return null;

        var tNode = textNode->GetAsAtkTextNode();
        if (tNode == null || tNode->NodeText.ToString() == "") return null;
        return tNode->NodeText.ToInteger();
    }

    private readonly Plugin plugin;

    private int maxGP = 800;
    private int currentGP = 800;
    private int integrity = 6;
    private int yield = 1;
    private int gatheringChance = 100;
    private int gatherersBoonChance = 30;
    private int bountifulYieldItems = 2;

    private GatheringResult? bestResult = null;
    private BestResultSelector bestResultSelector = BestResultSelector.BEST_AVG;
}
