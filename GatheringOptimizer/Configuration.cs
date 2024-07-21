using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace GatheringOptimizer;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    public bool AutoOpenOnAnyGather { get; set; } = false;
    public bool AutoOpenOnLegendaryGather { get; set; } = true;
    public bool AutoOpenOnUnspoiledGather { get; set; } = true;
    public bool AutoOpenOnEphemeralGather { get; set; } = true;

    // the below exist just to make saving less cumbersome
    [NonSerialized]
    private IDalamudPluginInterface? pluginInterface;

    public void Initialize(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
    }

    public void Save()
    {
        this.pluginInterface!.SavePluginConfig(this);
    }
}
