using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using GatheringOptimizer.Algorithm.Gathering;

namespace GatheringOptimizer.Algorithm;

internal static class Optimizer
{
    public static GatheringResult GenerateBestResult(GatheringParameters parameters, Func<GatheringResult, GatheringResult, bool> comparer, int currentGP)
    {
        var initialState = new GatheringState(parameters, currentGP);
        var initialResult = new GatheringResult(0, 0.0, 0, [], initialState);

        return RecursiveGenerateBestResult(parameters, comparer, initialResult, 0);
    }

    private static readonly ImmutableArray<IGatheringAction> NON_GATHER_ACTIONS = new IGatheringAction[] {
        IncreaseGatheringChanceAction.Instance,
        IncreaseGatheringChanceIIAction.Instance,
        IncreaseGatheringChanceIIIAction.Instance,
        IncreaseNextAttemptGatheringChanceAction.Instance,
        IncreaseBoonChanceIAction.Instance,
        IncreaseBoonChanceIIAction.Instance,
        IncreaseBoonItemsAction.Instance,
        IncreaseAttemptItemsAction.Instance,
        IncreaseAttemptItemsIIAction.Instance,
        IncreaseAttemptsAction.Instance,
        IncreaseNextAttemptItemsAction.Instance,
    }.OrderBy((x) => x.GP).ToImmutableArray();

    private static GatheringResult RecursiveGenerateBestResult(GatheringParameters parameters, Func<GatheringResult, GatheringResult, bool>  comparer, GatheringResult partialResult, int startAction)
    {
        if (partialResult.State.Integrity == 0)
        {
            return partialResult;
        }

        var bestResult = partialResult;
        for (int i = startAction; i < NON_GATHER_ACTIONS.Length; i++)
        {
            var action = NON_GATHER_ACTIONS[i];
            if (action.GP > partialResult.State.CurrentGP)
            {
                break;
            }
            if (action.CanExecute(partialResult.State))
            {
                var newResult = RecursiveGenerateBestResult(parameters, comparer, partialResult.ExecuteAction(action), i);
                Debug.Assert(newResult.State.Integrity == 0);
                if (comparer(newResult,  bestResult))
                {
                    bestResult = newResult;
                }
            }
        }

        var gatherAction = GatherAction.Instance;
        if (gatherAction.CanExecute(bestResult.State))
        {
            var newResult = RecursiveGenerateBestResult(parameters, comparer, bestResult.ExecuteAction(gatherAction), 0);
            if (comparer(newResult, bestResult))
            {
                bestResult = newResult;
            }
        }

        Debug.Assert(bestResult.State.Integrity == 0);
        return bestResult;
    }
}