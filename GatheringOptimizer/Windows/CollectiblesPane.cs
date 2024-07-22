using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiNET;

namespace GatheringOptimizer.Windows;

internal class CollectiblesPane : IPane
{
    public string AddonName => "GatheringMasterpiece";
    public string Title => "Collectibles";

    public void DrawPane()
    {
        ImGui.Text("Coming soon!");
    }

    public void SetupFromAddon(AddonEvent type, AddonArgs args)
    {
    }

    public bool ShouldAutoOpen()
    {
        return true;
    }

    public bool UpdateFromAddon(AddonEvent type, AddonArgs args)
    {
        if (Plugin.ClientState.LocalPlayer != null)
        {
            uint currentGP = Plugin.ClientState.LocalPlayer.CurrentGp;
        }

        return true;
    }

    private unsafe void Debug()
    {
        var addon = (AddonGatheringMasterpiece*)Plugin.GameGui.GetAddonByName(AddonName);
        if (addon == null)
        {
            Plugin.Log.Information("addon is null");
            return;
        }
        Plugin.Log.Information($"Integrity: {addon->IntegrityLeftover->NodeText}");
        var text = AddonUtils.GetTextNode(addon->GetTextNodeById(47));
        Plugin.Log.Information($"Collectability: {text}");
    }
}