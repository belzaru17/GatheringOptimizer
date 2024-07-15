using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace GatheringOptimizer.Algorithm;

internal static class Optimizer
{
   // public static IComparer<double> BEST_AVG = (sxddfdfdfdd, xxcdfd) => -1;

    public static GatheringResult GenerateBestResult(GatheringParameters parameters, Func<GatheringResult, GatheringResult, bool> comparer, int currentGP)
    {
        var initialState = new GatheringState(parameters, currentGP);
        var initialResult = new GatheringResult(0, 0.0, 0, [], initialState);

        return RecursiveGenerateBestResult(parameters, comparer, initialResult, POSSIBLE_NON_GATHER_ACTIONS);
    }

    private static List<IAction> POSSIBLE_NON_GATHER_ACTIONS = new List<IAction>() {
        IncreaseGatheringChanceAction.Instance,
        IncreaseGatheringChanceIIAction.Instance,
        IncreaseGatheringChanceIIIAction.Instance,
        IncreaseBoonChanceIAction.Instance,
        IncreaseBoonChanceIIAction.Instance,
        IncreaseBoonItemsAction.Instance,
        IncreaseAttemptItemsAction.Instance,
        IncreaseAttemptItemsIIAction.Instance,
        IncreaseAttemptsAction.Instance,
        IncreaseNextAttemptItemsAction.Instance,
    }.OrderBy((x) => x.GP).ToList();

    private static GatheringResult RecursiveGenerateBestResult(GatheringParameters parameters, Func<GatheringResult, GatheringResult, bool>  comparer, GatheringResult partialResult, List<IAction> possibleActions)
    {
        if (partialResult.State.Integrity == 0)
        {
            return partialResult;
        }

        var bestResult = partialResult;
        for (int i = 0; i < possibleActions.Count; i++)
        {
            var action = possibleActions[i];
            if (action.GP > partialResult.State.CurrentGP)
            {
                break;
            }
            if (action.CanExecute(partialResult.State))
            {
                var newResult = RecursiveGenerateBestResult(parameters, comparer, partialResult.ExecuteAction(action), possibleActions.Slice(i, possibleActions.Count - i));
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
            var newResult = RecursiveGenerateBestResult(parameters, comparer, bestResult.ExecuteAction(gatherAction), POSSIBLE_NON_GATHER_ACTIONS);
            if (comparer(newResult, bestResult))
            {
                bestResult = newResult;
            }
        }

        Debug.Assert(bestResult.State.Integrity == 0);
        return bestResult;
    }
}