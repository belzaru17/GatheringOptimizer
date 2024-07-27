using System;

namespace GatheringOptimizer.Algorithm.Collectables;

internal abstract class BaseCollectableRotation : ICollectableRotation
{
    public string Title { get; init; }
    public int MinGP { get; init; }
    public virtual bool Leveling => false;

    public Tuple<int, ICollectableAction> NextAction(int step, int currentGP, int integrity, int maxIntegrity, int collectability, CollectableBuff? buff, bool eurekaBuff)
    {
        if (collectability >= 1000 || step >= maxStep || integrity == 1)
        {
            if (integrity < maxIntegrity)
            {
                if (eurekaBuff)
                {
                    return new(step + 1, CollectableEurekaAction.Instance);
                }

                if (currentGP >= IncreaseCollectableIntegrityAction.Instance.GP && integrity < maxIntegrity)
                {
                    return new(step + 1, IncreaseCollectableIntegrityAction.Instance);
                }
            }
            return new(step + 1, CollectAction.Instance);
        }
        return SubNextAction(step, currentGP, collectability, buff);
    }

    protected BaseCollectableRotation(string title, int minGP, int maxStep)
    {
        Title = title;
        MinGP = minGP;
        this.maxStep = maxStep;
    }

    protected void Done() { }
    protected readonly int maxStep;

    protected abstract Tuple<int, ICollectableAction> SubNextAction(int step, int currentGP, int collectability, CollectableBuff? buff);

    protected static ICollectableAction NextProgressAction(CollectableBuff? buff)
    {
        if (buff == CollectableBuffs.CollectorsStandard)
        {
            return MeticulousProspectorAction.Instance;
        }
        else if (buff == CollectableBuffs.CollectorsHighStandard)
        {
            return BrazenProspectorAction.Instance;
        }
        return ScourAction.Instance;
    }
}

internal class Rotation_0GP : BaseCollectableRotation
{
    public Rotation_0GP() : base("0GP Ephemeral", 0, 2) { }

    protected override Tuple<int, ICollectableAction> SubNextAction(int step, int currentGP, int collectability, CollectableBuff? buff)
    {
        switch (step)
        {
            case 0:
            case 1:
                return new(step + 1, NextProgressAction(buff));
            default:
                return new(step + 1, CollectAction.Instance);
        }
    }
}

internal class Rotation_400GP : BaseCollectableRotation
{
    public override bool Leveling => true;

    public Rotation_400GP() : base("400/600GP Low Stat", 400, 6) { }

    protected override Tuple<int, ICollectableAction> SubNextAction(int step, int currentGP, int collectability, CollectableBuff? buff)
    {
        switch (step)
        {
            case 0:
            case 2:
            case 4:
                if (currentGP >= 200)
                {
                    return new(step + 1, ScrutinyAction.Instance);
                }
                else
                {
                    return new(step + 2, MeticulousProspectorAction.Instance);
                }
            default:
                return buff == CollectableBuffs.CollectorsStandard ? new(step + 1, MeticulousProspectorAction.Instance) : new(step + 1, ScourAction.Instance);
        }
    }
}

internal class Rotation_700GP : BaseCollectableRotation
{
    public override bool Leveling => true;

    public Rotation_700GP() : base("700GP Standard", 700, 5) { }

    protected override Tuple<int, ICollectableAction> SubNextAction(int step, int currentGP, int collectability, CollectableBuff? buff)
    {
        switch (step)
        {
            case 0:
            case 2:
                return new(step + 1, ScrutinyAction.Instance);
            case 1:
            case 3:
                return new(step + 1, MeticulousProspectorAction.Instance);
            default:
                if (collectability >= 850 || collectability == 800 && buff == CollectableBuffs.CollectorsStandard)
                {
                    return new(step + 1, MeticulousProspectorAction.Instance);
                }
                return new(step + 1, ScourAction.Instance);
        }
    }
}

internal class Rotation_600_800GP : BaseCollectableRotation
{
    public Rotation_600_800GP() : base("600/800GP Min Stat", 600, 8) { }

    protected override Tuple<int, ICollectableAction> SubNextAction(int step, int currentGP, int collectability, CollectableBuff? buff)
    {
        switch (step)
        {
            case 0:
            case 2:
            case 4:
                return new(step + 1, ScrutinyAction.Instance);
            case 1:
            case 3:
                return buff == CollectableBuffs.CollectorsHighStandard ? new(step + 1, BrazenProspectorAction.Instance) : new(step + 1, ScourAction.Instance);
            case 5:
                if (collectability >= 795)
                {
                    return new(8, MeticulousProspectorAction.Instance);
                }
                else if (collectability >= 720)
                {
                    return buff == CollectableBuffs.CollectorsStandard ? new(8, MeticulousProspectorAction.Instance) : new(8, ScourAction.Instance);
                }
                else if (collectability >= 645)
                {
                    return buff == CollectableBuffs.CollectorsHighStandard ? new(8, BrazenProspectorAction.Instance) : new(7, MeticulousProspectorAction.Instance);
                }
                else if (collectability >= 570)
                {
                    return buff == CollectableBuffs.CollectorsHighStandard ? new(7, BrazenProspectorAction.Instance) : new(step + 1, MeticulousProspectorAction.Instance);
                }
                return new(8, BrazenProspectorAction.Instance); // Fallback
            case 6:
                if (currentGP >= 200)
                {
                    return new(step + 1, ScrutinyAction.Instance);
                }
                else
                {
                    return new(step + 2, MeticulousProspectorAction.Instance);

                }
            default:
                return new(step + 1, MeticulousProspectorAction.Instance);
        }
    }
}

internal class Rotation_800_900GP : BaseCollectableRotation
{
    public Rotation_800_900GP() : base("800/900GP", 800, 9) { }

    protected override Tuple<int, ICollectableAction> SubNextAction(int step, int currentGP, int collectability, CollectableBuff? buff)
    {
        switch (step)
        {
            case 0:
                return new(step + 1, PrimingTouchAction.Instance);
            case 1:
                return new(step + 1, ScrutinyAction.Instance);
            case 2:
                return new(step + 1, MeticulousProspectorAction.Instance);
            case 3:
                return new(step + 1, ScrutinyAction.Instance);
            case 4:
                if (buff == CollectableBuffs.CollectorsHighStandard)
                {
                    return new(7, BrazenProspectorAction.Instance);
                }
                else if (currentGP >= 400)
                {
                    return new(step + 1, PrimingTouchAction.Instance);
                }
                else
                {
                    return new(step + 2, MeticulousProspectorAction.Instance);
                }
            case 5:
                return new(step + 1, MeticulousProspectorAction.Instance);
            case 6:
                if (collectability >= 850 || collectability == 800 && buff != null)
                {
                    return new(9, MeticulousProspectorAction.Instance);
                }
                return new(9, ScourAction.Instance);
            case 7:
                if (currentGP >= 400)
                {
                    return new(step + 1, PrimingTouchAction.Instance);
                }
                else
                {
                    return new(step + 2, MeticulousProspectorAction.Instance);
                }
            default:
                return new(step + 1, MeticulousProspectorAction.Instance);
        }
    }
}

internal class Rotation_1000GP : BaseCollectableRotation
{
    public Rotation_1000GP() : base("1000GP", 1000, 6) { }

    protected override Tuple<int, ICollectableAction> SubNextAction(int step, int currentGP, int collectability, CollectableBuff? buff)
    {
        switch (step)
        {
            case 0:
                return new(step + 1, ScrutinyAction.Instance);
            case 1:
                return new(step + 1, MeticulousProspectorAction.Instance);
            case 2:
                return new(step + 1, ScrutinyAction.Instance);
            case 3:
                if (buff == CollectableBuffs.CollectorsHighStandard)
                {
                    return new(5, BrazenProspectorAction.Instance);
                }
                else
                {
                    return new(step + 1, MeticulousProspectorAction.Instance);
                }
            case 4:
                if (collectability >= 850 || collectability == 800 && buff != null)
                {
                    return new(6, MeticulousProspectorAction.Instance);
                }
                return new(6, ScourAction.Instance);
            case 5:
                return new(step + 1, MeticulousProspectorAction.Instance);
            default:
                break;
        }

        return new(step + 1, CollectAction.Instance);
    }
}