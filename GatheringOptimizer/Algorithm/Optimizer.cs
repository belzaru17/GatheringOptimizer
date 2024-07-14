using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace GatheringOptimizer.Algorithm;

internal static class Optimizer
{
    public static ImmutableArray<GatheringResult> GenerateResults(GatheringParameters p)
    {
        return [.. GenerateActionLists(p.MaxGP).Select(x => x.Result(p)).Where(x => x.GP <= p.MaxGP)];
    }


    private static ImmutableArray<ActionsList> GenerateActionLists(int maxGP)
    {
        List<List<IAction>> oneOfActions = [
            [IncreaseGatheringChance.Instance, IncreaseGatheringChanceII.Instance, IncreaseGatheringChanceIII.Instance],
            [IncreaseBoonChanceI.Instance, IncreaseBoonChanceII.Instance, IncreaseBoonChanceBoth.Instance],
            [IncreaseBoonItems.Instance],
            [IncreaseAttemptItems.Instance, IncreaseAttemptItemsII.Instance],
        ];
        return [.. RecursiveGenerateActionLists(maxGP, oneOfActions, new ActionsList([]))];
    }

    private static List<ActionsList> RecursiveGenerateActionLists(int maxGP, List<List<IAction>> oneOfActions, ActionsList actions)
    {
        List<ActionsList> res = [];
        if (oneOfActions.Count == 0)
        {
            return GenerateTailActions(maxGP, actions);
        }

        var nextOneOfActions = (oneOfActions.Count > 1) ? oneOfActions.Slice(1, oneOfActions.Count - 1) : [];
        res.AddRange(RecursiveGenerateActionLists(maxGP, nextOneOfActions, actions));
        for (int i = 0; i < oneOfActions[0].Count; i++)
        {
            res.AddRange(RecursiveGenerateActionLists(maxGP, nextOneOfActions, new ActionsList(actions.Actions.Add(oneOfActions[0][i]))));
        }

        return res;
    }

    private static List<ActionsList> GenerateTailActions(int maxGP, ActionsList actions)
    {
        if (actions.GP > maxGP)
        {
            return [];
        }

        List<ActionsList> s1res = [actions];
        for (int i = 1; i <= (maxGP - actions.GP) / IncreaseAttempts.Instance.GP; i++)
        {
            s1res.Add(new ActionsList(s1res[i-1].Actions.Add(IncreaseAttempts.Instance)));
        }

        List<ActionsList> res = [];
        for (int i = 0; i < s1res.Count; i++)
        {
            res.Add(s1res[i]);
            for (int j = 1; j <= (maxGP - res[j-1].GP) / IncreaseNextAttemptItems.Instance.GP; j++)
            {
                res.Add(new ActionsList(res[j-1].Actions.Add(IncreaseNextAttemptItems.Instance)));
            }
        }

        return res;
    }
}
