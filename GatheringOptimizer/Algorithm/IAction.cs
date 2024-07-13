namespace GatheringOptimizer.Algorithm;

internal interface IAction
{
    string Name { get; }

    int GP { get; }

    double GatheringBonus { get; }

    double GatherersBoonBonus { get; }
    int GatherersBoonExtraItems { get; }

    int ExtraAttempts { get; }
    int ExtraAttemptsProcs { get; }

    int AttemptExtraItems { get; }

    int ExtraBountifulYieldCount { get; }
}    