using System.Collections.Generic;
using System.Threading.Tasks;

public abstract class ActionToken : EffectHolder
{
    public ActionType actiontype { get; protected set; }

    public EffectHolder holder;
    public GameEntity player;
    public Page page;
    public List<EffectHolder> chain;
    public int index;
    public string description;

    public enum ActionType
    {
        None,
        Offensive,
        Defensive,
        Movement,
        Effect
    }

    public ActionToken(GameEntity player, EffectHolder holder)
    {
        this.player = player;
        this.holder = holder;
        type = EffectHolderType.Action;
    }

    public virtual async Task Activate(LevelManager man)
    {
        return;
    }

    public ActionToken ShallowClone()
    {
        return (ActionToken)MemberwiseClone();
    }

    protected override async Task SpecificFullProc(string proc, bool global, List<EffectHolder> chain, object[] arg)
    {
        if (global)
        { await GlobalProc(proc, chain, arg); }
        await Proc(proc, chain, arg);

        if (holder is Card card)
        {
            await card.Proc(proc, chain, arg);
        }
        await player.Proc(proc, chain, arg);
    }

    


}

public class OnTurnToken : ActionToken
{

    public bool forceRechoose;
    public OnTurnToken(GameEntity player, EffectHolder holder) : base(player, holder)
    {

    }
}

public class DefensiveToken : ActionToken
{
    public int lifetime, roundstartexisting, minRoll, maxRoll;
    public bool clash;
    public DefensiveToken(GameEntity player, EffectHolder holder) : base(player, holder)
    {
        actiontype = ActionType.Defensive;
    }
}


public class MovementToken : OnTurnToken
{
    public MovementToken(GameEntity player, EffectHolder holder) : base(player, holder)
    {
        actiontype = ActionType.Movement;
    }
}



public class EffectToken : OnTurnToken
{
    public EffectToken(GameEntity player, EffectHolder holder) : base(player, holder)
    {
        actiontype = ActionType.Effect;
    }
}