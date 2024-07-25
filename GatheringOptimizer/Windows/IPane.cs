using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using System;

namespace GatheringOptimizer.Windows;

internal interface IPane
{
    public string AddonName { get; }
    public string Title { get; }

    void DrawPane();
    void SetupFromAddon(AddonEvent type, AddonArgs args);
    bool UpdateFromAddon(AddonEvent type, AddonArgs args);
    bool ShouldAutoOpen();
    bool ShouldAutoClose();

    void OnActionUsed(int actionId);
    void OnActorControl(uint type);
}