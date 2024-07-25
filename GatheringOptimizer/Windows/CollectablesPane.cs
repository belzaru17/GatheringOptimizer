﻿using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Textures;
using FFXIVClientStructs.FFXIV.Client.UI;
using GatheringOptimizer.Algorithm;
using ImGuiNET;
using System;
using System.Collections.Immutable;
using System.Numerics;

namespace GatheringOptimizer.Windows;

internal class CollectablesPane : IPane
{
    public string AddonName => "GatheringMasterpiece";
    public string Title => "Collectables";

    public unsafe void DrawPane()
    {
        var addon = GetAddon();
        var collecting = addon != null && addon->IsFullyLoaded();
        if (collecting) UpdateFromCurrentState(addon);

        ImGui.SetNextItemWidth(200);
        if (ImGui.BeginCombo("Rotation", rotations[currentRotation].Title))
        {
            for (int i = 0; i < rotations.Length; i++)
            {
                if (ImGui.Selectable(rotations[i].Title))
                {
                    Reset();
                }
            }
            ImGui.EndCombo();
        }
        ImGui.SameLine();
        ImGui.Text(collecting ? "Collecting" : "Simulating");
        ImGui.Spacing();
        ImGui.Separator();

        if (collecting) ImGui.BeginDisabled();
        ImGui.SetNextItemWidth(100);
        ImGui.InputInt("GP", ref currentGP);
        ImGui.SetNextItemWidth(100);
        ImGui.InputInt("##CurrentIntegrity", ref currentIntegrity);
        ImGui.SameLine(); ImGui.Text("/"); ImGui.SameLine();
        ImGui.SetNextItemWidth(100);
        ImGui.InputInt("Integrity", ref maxIntegrity);
        ImGui.SetNextItemWidth(100);
        ImGui.InputInt("Collectability", ref currentCollectability);

        string[] buffValues = ["None", "Collector's Standard", "Collector's High Standard"];
        int newCurrentBuff = (currentBuff == null) ? 0 : ((currentBuff == CollectableBuffs.CollectorsStandard) ? 1 : 2);
        ImGui.SetNextItemWidth(200);
        if (ImGui.Combo("Buffs", ref newCurrentBuff, buffValues, 3))
        {
            switch (newCurrentBuff)
            {
                case 1:
                    currentBuff = CollectableBuffs.CollectorsStandard; break;
                case 2:
                    currentBuff = CollectableBuffs.CollectorsHighStandard; break;
                default:
                    currentBuff = null; break;
            }
        }
        ImGui.SetNextItemWidth(100);
        ImGui.Checkbox("Eureka proc", ref eurekaBuff);
        ImGui.Spacing();
        ImGui.Separator();

        ISharedImmediateTexture icon;
        string actionName;
        if (collecting && advanceToNextStep && missingStateData)
        {
            icon = Plugin.TextureProvider.GetFromGameIcon(38);
            actionName = "Move your mouse off actions";
        }
        else
        {
            if (advanceToNextStep || nextAction == null)
            {
                currentStep = (nextAction == null) ? 0 : nextStep;
                advanceToNextStep = false;
            }
            (nextStep, nextAction) = rotations[currentRotation].NextAction(currentStep, currentGP, currentIntegrity, maxIntegrity, currentCollectability, currentBuff, eurekaBuff);
            icon = Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(AddonUtils.IsBotanist() ? nextAction.IconId_BOTANIST : nextAction.IconId_MINER));
            actionName = AddonUtils.IsBotanist() ? nextAction.Name_BOTANIST : nextAction.Name_MINER;
        }

        ImGui.SetNextItemWidth(100);
        ImGui.InputInt("Step", ref currentStep);
        ImGui.Spacing();
        if (collecting) ImGui.EndDisabled();

        var region = ImGui.GetWindowContentRegionMax();
        ImGui.SetCursorPos(new Vector2(region.X / 2 - 24, region.Y/2 - 24));
        if (collecting)
        {
            ImGui.Image(icon.GetWrapOrEmpty().ImGuiHandle, new Vector2(48, 48));
        }
        else if (ImGui.ImageButton(icon.GetWrapOrEmpty().ImGuiHandle, new Vector2(48, 48)))
        {
            if (nextAction != null)
            {
                currentGP -= nextAction.GP;
                if (nextAction.GP == 0)
                {
                    currentBuff = null;
                    currentIntegrity--;
                }
            }
            advanceToNextStep = true;
        }
        ImGui.Spacing();
        ImGui.SetCursorPosX((region.X - ImGui.CalcTextSize(actionName).X) / 2);
        ImGui.Text(actionName);
    }

    public void SetupFromAddon(AddonEvent type, AddonArgs args)
    {
    }

    public bool ShouldAutoOpen()
    {
        return true;
    }

    public unsafe bool UpdateFromAddon(AddonEvent type, AddonArgs args)
    {
        Reset();
        if (Plugin.ClientState.LocalPlayer != null)
        {
            bool leveling = Plugin.ClientState.LocalPlayer.Level < 100;
            currentGP = (int)Plugin.ClientState.LocalPlayer.CurrentGp;
            for (int i = rotations.Length - 1; i >= 0; i--)
            {
                var rotation = rotations[i];
                if (((leveling == rotation.Leveling) || rotation.MinGP == 0) && currentGP >= rotation.MinGP)
                {
                    currentRotation = i;
                    break;
                }
            }
        }

        UpdateFromCurrentState((AddonGatheringMasterpiece*)args.Addon);

        return true;
    }

    public void OnActionUsed(int actionId)
    {
        if (actionId == (AddonUtils.IsBotanist()? nextAction?.ActionId_BOTANIST : nextAction?.ActionId_MINER)) advanceToNextStep = true;
    }

    public void OnActorControl(uint type)
    {
        if (type == 308 /* Progress */ || type == 43 /* Collect */)
        {
            if ((AddonUtils.IsBotanist() ? nextAction?.ActionId_BOTANIST : nextAction?.ActionId_MINER) == 0) advanceToNextStep = true;
        }
    }

    private void Reset()
    {
        currentStep = 0;
        currentGP = 800;
        currentCollectability = 0;
        currentBuff = null;
        eurekaBuff = false;
        advanceToNextStep = false;
        nextAction = null;
        nextStep = 0;
        maxIntegrity = currentIntegrity = 4;
    }


    private unsafe AddonGatheringMasterpiece* GetAddon()
    {
        return (AddonGatheringMasterpiece*)Plugin.GameGui.GetAddonByName(AddonName);
    }

    private unsafe void UpdateFromCurrentState(AddonGatheringMasterpiece* addon)
    {
        bool newMissingStateData = false;
        if (Plugin.ClientState.LocalPlayer != null)
        {
            currentGP = (int)Plugin.ClientState.LocalPlayer.CurrentGp;

            var statusList = Plugin.ClientState.LocalPlayer.StatusList;
            eurekaBuff = false;
            CollectableBuff? buffFound = null;
            for (var i = 0; i < statusList.Length; i++)
            {
                var buff = statusList[i];
                if (buff?.StatusId == CollectableBuffs.CollectorsStandard.StatusId)
                {
                    buffFound = CollectableBuffs.CollectorsStandard;
                }
                else if (buff?.StatusId == CollectableBuffs.CollectorsHighStandard.StatusId)
                {
                    buffFound = CollectableBuffs.CollectorsHighStandard;
                }
                else if (buff?.StatusId == CollectableBuffs.Eureka.StatusId)
                {
                    eurekaBuff = true;
                }
            }
            currentBuff = buffFound;
        }
        else
        {
            newMissingStateData = true;
        }

        if (addon != null)
        {
            var newItegrity = addon->IntegrityLeftover->NodeText;
            if (newItegrity.ToString().Length > 0) currentIntegrity = newItegrity.ToInteger();
            var newMaxItegrity = addon->IntegrityTotal->NodeText;
            if (newMaxItegrity.ToString().Length > 0) maxIntegrity = newMaxItegrity.ToInteger();
            var integrityProgress = addon->GetImageNodeById(61);
            if (integrityProgress == null || !integrityProgress->IsVisible())
            {
                var newCollectability = AddonUtils.GetTextNode(addon->GetTextNodeById(47));
                if (newCollectability?.ToString().Length > 0 && !(newCollectability?.ToString().Contains("-") ?? false))
                {
                    currentCollectability = newCollectability?.ToInteger() ?? 0;
                }
                else
                {
                    newMissingStateData = true;
                }
            }
            else
            {
                newMissingStateData = true;
            }
        } else
        {
            newMissingStateData = true;
        }
        missingStateData = newMissingStateData;
    }

    static CollectablesPane() {
        rotations = [
            new Rotation_0GP(),
            new Rotation_400GP(), new Rotation_700GP(),
            new Rotation_600_800GP(), new Rotation_800_900GP(), new Rotation_1000GP(),
        ];
    }

    private static readonly ImmutableArray<ICollectableRotation> rotations;

    private int currentRotation = 0;
    private int currentStep = 0;

    private int currentGP = 800;
    private int maxIntegrity = 4;
    private int currentIntegrity = 4;
    private int currentCollectability = 0;
    private CollectableBuff? currentBuff = null;
    private bool eurekaBuff = false;

    private ICollectableAction? nextAction = null;
    private int nextStep;
    private bool advanceToNextStep = false;
    private bool missingStateData = false;
}