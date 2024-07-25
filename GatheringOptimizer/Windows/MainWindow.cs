using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.InteropServices;

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

        try
        {
            _onActionUsedHook = Plugin.GameInteropProvider.HookFromSignature<OnActionUsedDelegate>(
                "40 ?? 56 57 41 ?? 41 ?? 41 ?? 48 ?? ?? ?? ?? ?? ?? ?? 48",
                OnActionUsed
            );
            _onActorControlHook = Plugin.GameInteropProvider.HookFromSignature<OnActorControlDelegate>(
                "E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64",
                OnActorControl
            );
        }
        catch (Exception e)
        {
            Plugin.Log.Error("Error hooking into actions: " + e.Message);
        }
    }

    public void Dispose()
    {
        _onActionUsedHook?.Disable();
        _onActorControlHook?.Disable();
        _onActionUsedHook?.Dispose();
        _onActorControlHook?.Dispose();
        foreach (var pane in panes)
        {
            Plugin.AddonLifecycle.UnregisterListener(AddonEvent.PostUpdate, pane.AddonName);
            Plugin.AddonLifecycle.UnregisterListener(AddonEvent.PostRefresh, pane.AddonName);
            Plugin.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, pane.AddonName);
        }
        GC.SuppressFinalize(this);
    }

    public override void OnOpen()
    {
        base.OnOpen();
        _onActionUsedHook?.Enable();
        _onActorControlHook?.Enable();
    }

    public override void OnClose()
    {
        base.OnClose();
        _onActionUsedHook?.Disable();
        _onActorControlHook?.Disable();
    }

    public override void Draw()
    {
        if (autoOpened && currentPane.ShouldAutoClose())
        {
            IsOpen = autoOpened = false;
            return;
        }

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
            IsOpen = autoOpened = true;
        }
    }

    private void OnActionUsed(uint sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail)
    {
        _onActionUsedHook?.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);

        IPlayerCharacter? player = Plugin.ClientState.LocalPlayer;
        if (player == null || sourceId != player.GameObjectId) { return; }

        int actionId = Marshal.ReadInt32(effectHeader, 0x8);
        if (actionId != 0)
        {
            currentPane.OnActionUsed(actionId);
        }

    }

    private void OnActorControl(uint entityId, uint type, uint buffID, uint direct, uint actionId, uint sourceId, uint arg4, uint arg5, ulong targetId, byte a10)
    {
        _onActorControlHook?.Original(entityId, type, buffID, direct, actionId, sourceId, arg4, arg5, targetId, a10);

        IPlayerCharacter? player = Plugin.ClientState.LocalPlayer;
        if (player == null || entityId != player.GameObjectId) { return; }

        currentPane.OnActorControl(type);
    }

    private readonly Plugin plugin;
    private readonly ImmutableArray<IPane> panes;

    private delegate void OnActionUsedDelegate(uint sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
    private Hook<OnActionUsedDelegate>? _onActionUsedHook;
    private delegate void OnActorControlDelegate(uint entityId, uint type, uint buffID, uint direct, uint actionId, uint sourceId, uint arg4, uint arg5, ulong targetId, byte a10);
    private Hook<OnActorControlDelegate>? _onActorControlHook;

    private IPane currentPane;
    private bool addonWindowJustOpened = false;
    private bool autoOpened = false;
}