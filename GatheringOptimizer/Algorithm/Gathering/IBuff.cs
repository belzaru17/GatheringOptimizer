namespace GatheringOptimizer.Algorithm.Gathering;

internal interface IBuff
{
    string DebugName { get; }

    bool Ephemeral { get; }

    double GatheringBonus { get; }

    double GatherersBoonBonus { get; }
    int GatherersBoonExtraItems { get; }

    bool ExtraAttemptProc { get; }

    int ExtraYield { get; }
    bool BountifulYield { get; }
}
