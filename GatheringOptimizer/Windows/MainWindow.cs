using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using ImGuiNET;
using System;
using System.Collections.Immutable;
using System.Numerics;
using static FFXIVClientStructs.FFXIV.Client.Game.Character.ActionEffectHandler;

namespace GatheringOptimizer.Windows;

public class MainWindow : Window, IDisposable
{
    public unsafe MainWindow(Plugin plugin) : base(
        "Gathering Optimizer", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        this.plugin = plugin;
        settingsIcon = Plugin.GetTexture("settings_FILL1_wght400_GRAD0_opsz48.png");

        var gatheringPane = new GatheringPane(plugin);
        var collectiblesPane = new CollectablesPane(plugin);
        currentPane = gatheringPane;
        panes = [gatheringPane, collectiblesPane];

        SizeConstraints = new()
        {
            MinimumSize = new Vector2(450, 674),
            MaximumSize = new Vector2(450, 674),
        };

        foreach (var pane in panes)
        {
            Plugin.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, pane.AddonName, (eveneType, eventArgs) => AddonSetupHandler(eveneType, eventArgs, pane));
            Plugin.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, pane.AddonName, AddonUpdateHandler);
            Plugin.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, pane.AddonName, AddonUpdateHandler);
        }

        try
        {
            _onActionUsedHook = Plugin.GameInteropProvider.HookFromAddress<ActionEffectHandler.Delegates.Receive>(
                ActionEffectHandler.MemberFunctionPointers.Receive,
                OnActionUsed
            );
            if (_onActionUsedHook == null)
            {
                Plugin.Log.Error("Failed to hook into actions used");
            }
            _onActorControlHook = Plugin.GameInteropProvider.HookFromSignature<OnActorControlDelegate>(
                "E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64",
                OnActorControl
            );
            if (_onActorControlHook == null)
            {
                Plugin.Log.Error("Failed to hook into actor control");
            }
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
        ImGui.SetCursorPosX(ImGui.GetWindowSize().X - 30);
        if (ImGui.ImageButton(settingsIcon.GetWrapOrEmpty().ImGuiHandle, new(17, 17)))
        {
            plugin.OpenConfigUI();
        }

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

    private unsafe void OnActionUsed(uint actorId, Character* casterPtr, Vector3* targetPos, Header* header, TargetEffects* effects, GameObjectId* targetEntityIds)
    {
        _onActionUsedHook?.Original(actorId, casterPtr, targetPos, header, effects, targetEntityIds);

        IPlayerCharacter? player = Plugin.ClientState.LocalPlayer;
        if (player == null || actorId != player.GameObjectId) { return; }

        uint actionId = header->ActionId;
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
    private readonly ISharedImmediateTexture settingsIcon;
    private readonly ImmutableArray<IPane> panes;

    private Hook<ActionEffectHandler.Delegates.Receive>? _onActionUsedHook;
    private delegate void OnActorControlDelegate(uint entityId, uint type, uint buffID, uint direct, uint actionId, uint sourceId, uint arg4, uint arg5, ulong targetId, byte a10);
    private Hook<OnActorControlDelegate>? _onActorControlHook;

    private IPane currentPane;
    private bool addonWindowJustOpened = false;
    private bool autoOpened = false;
}