using System.Collections.Generic;
using System.Collections.Immutable;

namespace GatheringOptimizer.Algorithm;

internal static class Optimizer
{
    public static ImmutableArray<GatheringResult> GenerateResults(GatheringParameters parameters, int currentGP)
    {
        var initialState = new GatheringState(parameters, currentGP);
        var initialResult = new GatheringResult(0, 0.0, 0, [], initialState);

        return [.. RecursiveGenerateActionLists(parameters, initialResult, POSSIBLE_BUFF_ACTIONS)];
    }

    private static List<IAction> POSSIBLE_BUFF_ACTIONS = [
        IncreaseGatheringChanceAction.Instance,
        IncreaseGatheringChanceIIAction.Instance,
        IncreaseGatheringChanceIIIAction.Instance,
        IncreaseBoonChanceIAction.Instance,
        IncreaseBoonChanceIIAction.Instance,
        IncreaseBoonItemsAction.Instance,
        IncreaseAttemptItemsAction.Instance,
        IncreaseAttemptItemsIIAction.Instance,
        IncreaseAttemptsAction.Instance,
        // IncreaseAttemptsProcAction.Instance,
        IncreaseNextAttemptItemsAction.Instance,
    ];

    private static List<GatheringResult> RecursiveGenerateActionLists(GatheringParameters parameters, GatheringResult partialResult, List<IAction> possibleActions)
    {
        if (partialResult.State.Integrity == 0)
        {
            return [partialResult];
        }

        var result = new List<GatheringResult>();
        for (int i = 0; i < possibleActions.Count; i++)
        {
            var action = possibleActions[i];
            if (action.CanExecute(partialResult.State))
            {
                result.AddRange(RecursiveGenerateActionLists(parameters, partialResult.ExecuteAction(action), possibleActions.Slice(i, possibleActions.Count - i)));
            }
        }
        var gatherAction = GatherAction.Instance;
        if (gatherAction.CanExecute(partialResult.State))
        {
            result.AddRange(RecursiveGenerateActionLists(parameters, partialResult.ExecuteAction(gatherAction), POSSIBLE_BUFF_ACTIONS));
        }
        return result;
    }
}