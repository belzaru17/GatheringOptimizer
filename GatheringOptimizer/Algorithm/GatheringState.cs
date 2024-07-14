using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace GatheringOptimizer.Algorithm;

internal class GatheringState
{
    public readonly GatheringParameters Parameters;

    public readonly int CurrentGP;

    public readonly int Integrity;

    public readonly int UsedGP;

    public readonly ImmutableHashSet<IBuff> Buffs;


    public double GatheringChance => Math.Min(Parameters.BaseGatheringChance + Buffs.Sum(i => i.GatheringBonus), 1.0);

    public double GatherersBoonChance => Math.Min(Parameters.BaseGatherersBoonChance + Buffs.Sum(i => i.GatherersBoonBonus), 1.0);
    public int GatherersBoonExtraItems => Buffs.Sum(i => i.GatherersBoonExtraItems);

    public int AttemptItems => Parameters.BaseAttemptItems  + Buffs.Sum(i => i.AttemptExtraItems) + (Buffs.Any((i) => i.BountifulYield) ? Parameters.BountifulYieldItems : 0);

    public int MinItems => (GatheringChance < 1) ? 0 : AttemptItems;
    public double AvgItems => GatheringChance * (AttemptItems + GatherersBoonChance * (1 + GatherersBoonExtraItems));
    public int MaxItems => AttemptItems + 1 + GatherersBoonExtraItems;


    public GatheringState(GatheringParameters parameters) : this(parameters, parameters.MaxGP)
    {
    }

    public GatheringState(GatheringParameters parameters, int currentGP) : this(parameters, currentGP, parameters.MaxIntegrity, 0, [])
    {
    }

    public GatheringState AddBuff(IBuff buff, int gpCost)
    {
        Debug.Assert(CurrentGP >= gpCost);
        Debug.Assert(!Buffs.Contains(buff), "Buff already present: " + buff.DebugName);

        var newBuffs = Buffs.ToHashSet();
        newBuffs.Add(buff);
        return new GatheringState(Parameters, CurrentGP - gpCost, Integrity, UsedGP + gpCost, newBuffs);
    }

    public GatheringState AddIntegrity(int gpCost, IBuff? removeBuff = null)
    {
        Debug.Assert(CurrentGP >= gpCost);
        Debug.Assert(Integrity < Parameters.MaxIntegrity);
        Debug.Assert(removeBuff == null || Buffs.Contains(removeBuff));

        var newBuffs = Buffs.ToHashSet();
        if (removeBuff != null)
        {
            newBuffs.Remove(removeBuff);
        }
        return new GatheringState(Parameters, CurrentGP - gpCost, Integrity + 1, UsedGP + gpCost, newBuffs);
    }

    public ActionResult Gather()
    {
        Debug.Assert(Integrity > 0);

        var newBuffs = Buffs.ToHashSet();
        newBuffs.RemoveWhere((x) => x.Ephemeral);
        var newParameters = Parameters;

        var avgItems = AvgItems;
        var maxItems = MaxItems;

        if (newBuffs.Contains(ExtraAttemptProcBuff.Instance))
        {
            newBuffs.Remove(ExtraAttemptProcBuff.Instance);
            var tmpState = new GatheringState(Parameters, CurrentGP, 1, UsedGP, newBuffs);
            avgItems += tmpState.AvgItems / 2;  // 50% chance
            maxItems += tmpState.MaxItems;
        }

        return new ActionResult(MinItems, avgItems, maxItems, new GatheringState(newParameters, Math.Min(CurrentGP + GP_RECOVERY_PER_GATHER, Parameters.MaxGP), Integrity - 1, UsedGP,  newBuffs));
    }

    private static readonly int GP_RECOVERY_PER_GATHER = 6;

    private GatheringState(GatheringParameters parameters, int currentGP,  int integrity, int usedGP, HashSet<IBuff> buffs)
    {
        Parameters = parameters;
        CurrentGP = currentGP;
        Integrity = integrity;
        UsedGP = usedGP;
        Buffs = [.. buffs];
    }
}