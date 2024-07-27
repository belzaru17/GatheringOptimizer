using Dalamud.Configuration;
using Dalamud.Plugin;
using GatheringOptimizer.Algorithm.Collectables;
using System;
using System.Collections.Generic;

namespace GatheringOptimizer;

[Serializable]
public class RotationConfiguration
{
    public bool Enabled { get; set; } = true;
    public bool NoExtraGP { get; set; } = true;
    public int MinGP { get; set; } = 0;
}

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    public bool AutoOpenOnAnyGather { get; set; } = false;
    public bool AutoOpenOnLegendaryGather { get; set; } = true;
    public bool AutoOpenOnUnspoiledGather { get; set; } = true;
    public bool AutoOpenOnEphemeralGather { get; set; } = true;

    public Dictionary<int, RotationConfiguration> RotationConfigurations { get; set; } = DefaultRotationConfigurations();


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

    private static Dictionary<int, RotationConfiguration> DefaultRotationConfigurations()
    {
        Dictionary<int, RotationConfiguration> rotationConfigs = new();
        foreach (var rotation in CollectableRotations.Rotations)
        {
            rotationConfigs.Add(rotation.Id, rotation.DefaultConfiguration());
        }
        return rotationConfigs;
    }
}
