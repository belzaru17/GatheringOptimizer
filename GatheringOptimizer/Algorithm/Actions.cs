using System;

namespace GatheringOptimizer.Algorithm;

internal abstract class BaseAction : IAction
{
    public abstract string Name { get; }

    public abstract int GP { get; }

    public virtual double GatheringBonus => 0;

    public virtual double GatherersBoonBonus => 0;
    public virtual int GatherersBoonExtraItems => 0;

    public virtual int ExtraAttempts => 0;
    public virtual int ExtraAttemptsProcs => 0;

    public virtual int AttemptExtraItems => 0;

    public virtual int ExtraBountifulYieldCount => 0;
}

internal sealed class SharpVision : BaseAction
{
    public override string Name => "Sharp Vision";
    public override int GP => 50;
    public override double GatheringBonus => 0.05;

    public static SharpVision Instance => instance.Value;

    private static readonly Lazy<SharpVision> instance = new(() => new SharpVision());

    private SharpVision() { }
}

internal sealed class SharpVisionII : BaseAction
{
    public override string Name => "Sharp Vision II";
    public override int GP => 100;
    public override double GatheringBonus => 0.10;

    public static SharpVisionII Instance => instance.Value;

    private static readonly Lazy<SharpVisionII> instance = new(() => new SharpVisionII());

    private SharpVisionII() { }
}

internal sealed class SharpVisionIII : BaseAction
{
    public override string Name => "Sharp Vision III";
    public override int GP => 250;
    public override double GatheringBonus => 0.50;

    public static SharpVisionIII Instance => instance.Value;

    private static readonly Lazy<SharpVisionIII> instance = new(() => new SharpVisionIII());

    private SharpVisionIII() { }
}

internal sealed class GiftI : BaseAction
{
    public override string Name => "Gift I";
    public override int GP => 50;
    public override double GatherersBoonBonus => 0.10;

    public static GiftI Instance => instance.Value;

    private static readonly Lazy<GiftI> instance = new(() => new GiftI());

    private GiftI() { }
}

internal sealed class GiftII : BaseAction
{
    public override string Name => "Gift II";
    public override int GP => 100;
    public override double GatheringBonus => 0.30;

    public static GiftII Instance => instance.Value;

    private static readonly Lazy<GiftII> instance = new(() => new GiftII());

    private GiftII() { }
}

internal sealed class BothGifts : BaseAction
{
    public override string Name => "Gift I + Gift II";
    public override int GP => 150;
    public override double GatheringBonus => 0.40;

    public static BothGifts Instance => instance.Value;

    private static readonly Lazy<BothGifts> instance = new(() => new BothGifts());

    private BothGifts() { }
}

internal sealed class NaldThalsTidings : BaseAction
{
    public override string Name => "Nald'thal's Tidings";
    public override int GP => 200;
    public override int GatherersBoonExtraItems => 1;

    public static NaldThalsTidings Instance => instance.Value;

    private static readonly Lazy<NaldThalsTidings> instance = new(() => new NaldThalsTidings());

    private NaldThalsTidings() { }
}

internal sealed class KingsYield : BaseAction
{
    public override string Name => "Kings Yield";
    public override int GP => 400;
    public override int AttemptExtraItems => 1;

    public static KingsYield Instance => instance.Value;

    private static readonly Lazy<KingsYield> instance = new(() => new KingsYield());

    private KingsYield() { }
}

internal sealed class KingsYieldII : BaseAction
{
    public override string Name => "Kings Yield II";
    public override int GP => 500;
    public override int AttemptExtraItems => 2;

    public static KingsYieldII Instance => instance.Value;

    private static readonly Lazy<KingsYieldII> instance = new(() => new KingsYieldII());

    private KingsYieldII() { }
}

internal sealed class SolidReason : BaseAction
{
    public override string Name => "Solid Reason";
    public override int GP => 300;
    public override int ExtraAttempts => 1;
    public override int ExtraAttemptsProcs => 1;

    public static SolidReason Instance => instance.Value;

    private static readonly Lazy<SolidReason> instance = new(() => new SolidReason());

    private SolidReason() { }
}

internal sealed class BountifulYield : BaseAction
{
    public override string Name => "Bountiful Yield";
    public override int GP => 100;
    public override int ExtraBountifulYieldCount => 1;

    public static BountifulYield Instance => instance.Value;

    private static readonly Lazy<BountifulYield> instance = new(() => new BountifulYield());

    private BountifulYield() { }
}