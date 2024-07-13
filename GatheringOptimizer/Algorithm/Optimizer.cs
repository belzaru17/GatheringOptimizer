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
            [SharpVision.Instance, SharpVisionII.Instance, SharpVisionIII.Instance],
            [GiftI.Instance, GiftII.Instance, BothGifts.Instance],
            [NaldThalsTidings.Instance],
            [KingsYield.Instance, KingsYieldII.Instance],
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

        res.AddRange(RecursiveGenerateActionLists(maxGP, oneOfActions.Slice(1, oneOfActions.Count), actions));
        for (int i = 0; i < oneOfActions[0].Count; i++)
        {
            res.AddRange(RecursiveGenerateActionLists(maxGP, oneOfActions.Slice(1, oneOfActions.Count), new ActionsList(actions.Actions.Add(oneOfActions[0][i]))));
        }

        return res;
    }

    private static List<ActionsList> GenerateTailActions(int maxGP, ActionsList actions)
    {
        List<ActionsList> s1res = [actions];
        for (int i = 1; i <= (maxGP - actions.GP) / SolidReason.Instance.GP; i++)
        {
            s1res.Add(new ActionsList(actions.Actions.Add(SolidReason.Instance)));
        }

        List<ActionsList> res = [];
        for (int i = 0; i < s1res.Count; i++)
        {
            res.Add(s1res[i]);
            for (int j = 1; j <= (maxGP - s1res[i].GP) / BountifulYield.Instance.GP; j++)
            {
                res.Add(new ActionsList(s1res[i].Actions.Add(BountifulYield.Instance)));
            }
        }

        return res;
    }
}
