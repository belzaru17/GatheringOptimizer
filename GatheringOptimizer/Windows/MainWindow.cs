using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Immutable;
using System.Numerics;

namespace GatheringOptimizer.Windows;

public class MainWindow : Window, IDisposable
{
    public MainWindow(Plugin plugin) : base(
        "Gathering Optimizer", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.plugin = plugin;

        var gatheringPane = new GatheringPane(plugin);
        var collectiblesPane = new CollectablesPane();
        currentPane = gatheringPane;
        panes = [gatheringPane, collectiblesPane];

        SizeConstraints = new()
        {
            MinimumSize = new Vector2(450, 600),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };

        foreach (var pane in panes)
        {
            Plugin.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, pane.AddonName, (eveneType, eventArgs) => AddonSetupHandler(eveneType, eventArgs, pane));
            Plugin.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, pane.AddonName, AddonUpdateHandler);
            Plugin.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, pane.AddonName, AddonUpdateHandler);
        }
    }

    public void Dispose()
    {
        foreach (var pane in panes)
        {
            Plugin.AddonLifecycle.UnregisterListener(AddonEvent.PostUpdate, pane.AddonName);
            Plugin.AddonLifecycle.UnregisterListener(AddonEvent.PostRefresh, pane.AddonName);
            Plugin.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, pane.AddonName);
        }
        GC.SuppressFinalize(this);
    }

    public override void Draw()
    {
        foreach (var pane in panes)
        {

            if (pane == currentPane) ImGui.BeginDisabled();
            if (ImGui.Button(pane.Title))
            {
                currentPane = pane;
            }
            if (pane == currentPane) ImGui.EndDisabled();
            ImGui.SameLine();
        }
        ImGui.Spacing();
        ImGui.Separator();
        currentPane.DrawPane();
    }

    private void AddonSetupHandler(AddonEvent type, AddonArgs args, IPane pane)
    {
        pane.SetupFromAddon(type, args);
        currentPane = pane;
        addonWindowJustOpened = true;
    }

    private void AddonUpdateHandler(AddonEvent type, AddonArgs args)
    {
        if (!addonWindowJustOpened)
        {
            return;
        }

        if (!currentPane.UpdateFromAddon(type, args)) return;

        addonWindowJustOpened = false;
        if (!IsOpen && currentPane.ShouldAutoOpen())
        {
            IsOpen = true;
        }
    }

    private readonly Plugin plugin;
    private readonly ImmutableArray<IPane> panes;

    private IPane currentPane;
    private bool addonWindowJustOpened = false;
}