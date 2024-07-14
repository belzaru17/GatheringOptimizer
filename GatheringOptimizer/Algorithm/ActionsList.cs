using System;
using System.Collections.Immutable;
using System.Linq;

namespace GatheringOptimizer.Algorithm;

internal record ActionsList(ImmutableArray<IAction> Actions) : IAction
{
    public string Name_MINER => "ActionList";
    public string Name_BOTANIST => "ActionList";

    public int GP => Actions.Sum(i => i.GP);

    public double GatheringBonus => Actions.Sum(i => i.GatheringBonus);

    public double GatherersBoonBonus => Actions.Sum(i => i.GatherersBoonBonus);
    public int GatherersBoonExtraItems => Actions.Sum(i => i.GatherersBoonExtraItems);

    public int ExtraAttempts => Actions.Sum(i => i.ExtraAttempts);
    public int ExtraAttemptsProcs => Actions.Sum(i => i.ExtraAttemptsProcs);

    public int AttemptExtraItems => Actions.Sum(i => i.AttemptExtraItems);

    public int ExtraBountifulYieldCount => Actions.Sum(i => i.ExtraBountifulYieldCount);

    public GatheringResult Result(GatheringParameters p)
    {
        var adjAttempts = p.Attempts + ExtraAttempts;
        var adjGatheringChance = Math.Min(p.GatheringChance + GatheringBonus, 1.0);
        var adjGatherersBoonChance = Math.Min(p.GatherersBoonChance + GatherersBoonBonus, 1.0);
        var adjGatherersBoonItems = 1 + GatherersBoonExtraItems;
        var adjAttemptItems = p.AttemptItems + AttemptExtraItems;

        var minItems = (adjGatheringChance < 1.0) ? 0 : Calculate(p, adjAttempts, adjAttemptItems);
        var avgItems = adjGatheringChance * Calculate(p, adjAttempts + ExtraAttemptsProcs/2.0, adjAttemptItems + adjGatherersBoonChance * adjGatherersBoonItems);
        var maxItems = Calculate(p, adjAttempts + ExtraAttemptsProcs, adjAttemptItems + adjGatherersBoonItems);

        return new GatheringResult(GP, minItems, avgItems, maxItems, this);
    }

    private double Calculate(GatheringParameters p, double attempts, double attemptItems)
    {
        return attempts * attemptItems + Math.Min(ExtraBountifulYieldCount, attempts) * p.BountifulYieldItems;
    }
}