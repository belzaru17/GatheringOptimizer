using System;

namespace GatheringOptimizer.Algorithm;

internal sealed class GatherAction : IAction
{
    public string Name_MINER => "Gather";
    public string Name_BOTANIST => "Gather";

    public uint IconId_MINER => 1000;
    public uint IconId_BOTANIST => 1050;

    public int GP => 0;

    public bool CanExecute(GatheringState state) => state.Integrity > 0;

    public ActionResult Execute(GatheringState state)
    {
        return state.Gather();
    }

    public static GatherAction Instance => instance.Value;

    private static readonly Lazy<GatherAction> instance = new(() => new GatherAction());

    private GatherAction() { }
}

internal abstract class BaseBuffAction : IAction
{
    public abstract string Name_MINER { get; }
    public abstract string Name_BOTANIST { get; }

    public abstract uint IconId_MINER { get;  }
    public abstract uint IconId_BOTANIST { get;  }

    public abstract int GP { get; }

    public virtual bool CanExecute(GatheringState state)
    {
        return state.CurrentGP >= GP;
    }

    public ActionResult Execute(GatheringState state)
    {
        return new ActionResult(0, 0.0, 0, ExecuteBuff(state));
    }

    protected abstract GatheringState ExecuteBuff(GatheringState state);
}

internal sealed class IncreaseGatheringChanceAction : BaseBuffAction
{
    public override string Name_MINER => "Sharp Vision";
    public override string Name_BOTANIST => "Field Mastery";
    public override uint IconId_MINER => 1015;
    public override uint IconId_BOTANIST => 1065;
    public override int GP => 50;

    public override bool CanExecute(GatheringState state)
    {
        return base.CanExecute(state) && state.GatheringChance < 1.0 && 
            !(state.Buffs.Contains(IncreasedGatheringChanceBuff.Instance) || state.Buffs.Contains(IncreasedGatheringChanceIIBuff.Instance) || state.Buffs.Contains(IncreasedGatheringChanceIIIBuff.Instance));
    }

    protected override GatheringState ExecuteBuff(GatheringState state)
    {
        return state.AddBuff(IncreasedGatheringChanceBuff.Instance, GP);
    }

    public static IncreaseGatheringChanceAction Instance => instance.Value;

    private static readonly Lazy<IncreaseGatheringChanceAction> instance = new(() => new IncreaseGatheringChanceAction());

    private IncreaseGatheringChanceAction() { }
}

internal sealed class IncreaseGatheringChanceIIAction : BaseBuffAction
{
    public override string Name_MINER => "Sharp Vision II";
    public override string Name_BOTANIST => "Field Mastery II";
    public override uint IconId_MINER => 1016;
    public override uint IconId_BOTANIST => 1066;
    public override int GP => 100;

    public override bool CanExecute(GatheringState state)
    {
        return base.CanExecute(state) && state.GatheringChance < 1.0 &&
            !(state.Buffs.Contains(IncreasedGatheringChanceBuff.Instance) || state.Buffs.Contains(IncreasedGatheringChanceIIBuff.Instance) || state.Buffs.Contains(IncreasedGatheringChanceIIIBuff.Instance));
    }

    protected override GatheringState ExecuteBuff(GatheringState state)
    {
        return state.AddBuff(IncreasedGatheringChanceIIBuff.Instance, GP);
    }

    public static IncreaseGatheringChanceIIAction Instance => instance.Value;

    private static readonly Lazy<IncreaseGatheringChanceIIAction> instance = new(() => new IncreaseGatheringChanceIIAction());

    private IncreaseGatheringChanceIIAction() { }
}

internal sealed class IncreaseGatheringChanceIIIAction : BaseBuffAction
{
    public override string Name_MINER => "Sharp Vision III";
    public override string Name_BOTANIST => "Field Mastery III";
    public override uint IconId_MINER => 1017;
    public override uint IconId_BOTANIST => 1067;
    public override int GP => 250;

    public override bool CanExecute(GatheringState state)
    {
        return base.CanExecute(state) && state.GatheringChance < 1.0 &&
            !(state.Buffs.Contains(IncreasedGatheringChanceBuff.Instance) || state.Buffs.Contains(IncreasedGatheringChanceIIBuff.Instance) || state.Buffs.Contains(IncreasedGatheringChanceIIIBuff.Instance));
    }

    protected override GatheringState ExecuteBuff(GatheringState state)
    {
        return state.AddBuff(IncreasedGatheringChanceIIIBuff.Instance, GP);
    }

    public static IncreaseGatheringChanceIIIAction Instance => instance.Value;

    private static readonly Lazy<IncreaseGatheringChanceIIIAction> instance = new(() => new IncreaseGatheringChanceIIIAction());

    private IncreaseGatheringChanceIIIAction() { }
}

internal sealed class IncreaseBoonChanceIAction : BaseBuffAction
{
    public override string Name_MINER => "Mountaineer's Gift I";
    public override string Name_BOTANIST => "Pioneer's Gift I";
    public override uint IconId_MINER => 1045;
    public override uint IconId_BOTANIST => 1095;
    public override int GP => 50;

    public override bool CanExecute(GatheringState state)
    {
        return base.CanExecute(state) && state.GatherersBoonChance < 1.0 &&
            !state.Buffs.Contains(IncreasedBoonChanceIBuff.Instance);
    }

    protected override GatheringState ExecuteBuff(GatheringState state)
    {
        return state.AddBuff(IncreasedBoonChanceIBuff.Instance, GP);
    }

    public static IncreaseBoonChanceIAction Instance => instance.Value;

    private static readonly Lazy<IncreaseBoonChanceIAction> instance = new(() => new IncreaseBoonChanceIAction());

    private IncreaseBoonChanceIAction() { }
}

internal sealed class IncreaseBoonChanceIIAction : BaseBuffAction
{
    public override string Name_MINER => "Mountaineer's Gift II";
    public override string Name_BOTANIST => "Pioneer's Gift II";
    public override uint IconId_MINER => 1046;
    public override uint IconId_BOTANIST => 1096;
    public override int GP => 100;

    public override bool CanExecute(GatheringState state)
    {
        return base.CanExecute(state) && state.GatherersBoonChance < 1.0 &&
            !state.Buffs.Contains(IncreasedBoonChanceIIBuff.Instance);
    }

    protected override GatheringState ExecuteBuff(GatheringState state)
    {
        return state.AddBuff(IncreasedBoonChanceIIBuff.Instance, GP);
    }

    public static IncreaseBoonChanceIIAction Instance => instance.Value;

    private static readonly Lazy<IncreaseBoonChanceIIAction> instance = new(() => new IncreaseBoonChanceIIAction());

    private IncreaseBoonChanceIIAction() { }
}

internal sealed class IncreaseBoonItemsAction : BaseBuffAction
{
    public override string Name_MINER => "Nald'thal's Tidings";
    public override string Name_BOTANIST => "Nophica's Tidings";
    public override uint IconId_MINER => 1047;
    public override uint IconId_BOTANIST => 1097;
    public override int GP => 200;

    public override bool CanExecute(GatheringState state)
    {
        return base.CanExecute(state) && !state.Buffs.Contains(IncreasedBoonItemsBuff.Instance);
    }

    protected override GatheringState ExecuteBuff(GatheringState state)
    {
        return state.AddBuff(IncreasedBoonItemsBuff.Instance, GP);
    }

    public static IncreaseBoonItemsAction Instance => instance.Value;

    private static readonly Lazy<IncreaseBoonItemsAction> instance = new(() => new IncreaseBoonItemsAction());

    private IncreaseBoonItemsAction() { }
}

internal sealed class IncreaseAttemptItemsAction : BaseBuffAction
{
    public override string Name_MINER => "Kings Yield";
    public override string Name_BOTANIST => "Blessed Harvest";
    public override uint IconId_MINER => 1006;
    public override uint IconId_BOTANIST => 1056;
    public override int GP => 400;

    public override bool CanExecute(GatheringState state)
    {
        return base.CanExecute(state) &&
            !(state.Buffs.Contains(IncreasedAttemptItemsBuff.Instance) || state.Buffs.Contains(IncreasedAttemptItemsIIBuff.Instance));
    }

    protected override GatheringState ExecuteBuff(GatheringState state)
    {
        return state.AddBuff(IncreasedAttemptItemsBuff.Instance, GP);
    }

    public static IncreaseAttemptItemsAction Instance => instance.Value;

    private static readonly Lazy<IncreaseAttemptItemsAction> instance = new(() => new IncreaseAttemptItemsAction());

    private IncreaseAttemptItemsAction() { }
}

internal sealed class IncreaseAttemptItemsIIAction : BaseBuffAction
{
    public override string Name_MINER => "Kings Yield II";
    public override string Name_BOTANIST => "Blessed Harvest II";
    public override uint IconId_MINER => 1007;
    public override uint IconId_BOTANIST => 1056;
    public override int GP => 500;

    public override bool CanExecute(GatheringState state)
    {
        // FIXME: override buffs
        return base.CanExecute(state) &&
            !(state.Buffs.Contains(IncreasedAttemptItemsBuff.Instance) || state.Buffs.Contains(IncreasedAttemptItemsIIBuff.Instance));
    }

    protected override GatheringState ExecuteBuff(GatheringState state)
    {
        return state.AddBuff(IncreasedAttemptItemsIIBuff.Instance, GP);
    }

    public static IncreaseAttemptItemsIIAction Instance => instance.Value;

    private static readonly Lazy<IncreaseAttemptItemsIIAction> instance = new(() => new IncreaseAttemptItemsIIAction());

    private IncreaseAttemptItemsIIAction() { }
}

internal sealed class IncreaseAttemptsAction : BaseBuffAction
{
    public override string Name_MINER => "Solid Reason";
    public override string Name_BOTANIST => "Ageless Words";
    public override uint IconId_MINER => 1009;
    public override uint IconId_BOTANIST => 1059;
    public override int GP => 300;

    public override bool CanExecute(GatheringState state)
    {
        return base.CanExecute(state) && state.Integrity < state.Parameters.MaxIntegrity && !state.Buffs.Contains(ExtraAttemptProcBuff.Instance);
    }

    protected override GatheringState ExecuteBuff(GatheringState state)
    {
        return state.AddIntegrity(GP);
    }

    public static IncreaseAttemptsAction Instance => instance.Value;

    private static readonly Lazy<IncreaseAttemptsAction> instance = new(() => new IncreaseAttemptsAction());

    private IncreaseAttemptsAction() { }
}

internal sealed class IncreaseNextAttemptGatheringChanceAction : BaseBuffAction
{
    public override string Name_MINER => "Clear Vision";
    public override string Name_BOTANIST => "Flora Mastery";
    public override uint IconId_MINER => 1020;
    public override uint IconId_BOTANIST => 1070;
    public override int GP => 50;

    public override bool CanExecute(GatheringState state)
    {
        return base.CanExecute(state) && state.GatheringChance < 1.0 && !state.Buffs.Contains(IncreasedNextAttemptGatheringChanceBuff.Instance);
    }

    protected override GatheringState ExecuteBuff(GatheringState state)
    {
        return state.AddBuff(IncreasedNextAttemptGatheringChanceBuff.Instance, GP);
    }

    public static IncreaseNextAttemptGatheringChanceAction Instance => instance.Value;

    private static readonly Lazy<IncreaseNextAttemptGatheringChanceAction> instance = new(() => new IncreaseNextAttemptGatheringChanceAction());

    private IncreaseNextAttemptGatheringChanceAction() { }
}


internal sealed class IncreaseNextAttemptItemsAction : BaseBuffAction
{
    public override string Name_MINER => "Bountiful Yield";
    public override string Name_BOTANIST => "Bountiful Harvest";
    public override uint IconId_MINER => 1035;
    public override uint IconId_BOTANIST => 1085;
    public override int GP => 100;

    public override bool CanExecute(GatheringState state)
    {
        return base.CanExecute(state) && !state.Buffs.Contains(IncreasedNextAttemptItemsBuff.Instance);
    }

    protected override GatheringState ExecuteBuff(GatheringState state)
    {
        return state.AddBuff(IncreasedNextAttemptItemsBuff.Instance, GP);
    }

    public static IncreaseNextAttemptItemsAction Instance => instance.Value;

    private static readonly Lazy<IncreaseNextAttemptItemsAction> instance = new(() => new IncreaseNextAttemptItemsAction());

    private IncreaseNextAttemptItemsAction() { }
}