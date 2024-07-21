using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using ImGuiNET;

namespace GatheringOptimizer.Windows;

internal class CollectiblesPane : IPane
{
    public string AddonName => "GatheringMasterpiece";
    public string Title => "Collectibles";

    public void DrawPane()
    {
        ImGui.Text("Gathering collectible -- coming soon!");
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
        return true;
    }
}
