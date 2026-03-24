using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public abstract class ActionToken : EffectHolder
{
    public ActionType actiontype { get; protected set; }

    public EffectHolder holder;
    public GameEntity player;
    public List<EffectHolder> chain;
    public int index;

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

    protected async virtual Task Activate(LevelManager man)
    {

    }

    public async Task TryActivate(LevelManager man)
    {

    }

    public override async Task SpecificFullProc(string proc, List<EffectHolder> chain, params object[] arg)
    {
        await FullProc(proc, chain, arg);
        if (holder is Card)
        {
           await ((Card)holder).Proc(proc, chain, arg);
        }    
        await player.Proc(proc, chain, arg);
    }

    public override async Task SpecificFullDelayProc(string proc, List<EffectHolder> chain, DelayAction action, int action_position = 0, params object[] arg)
    {
		if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

		await GlobalProc(proc, chain, arg);
        await Proc(proc, chain, arg);
        if (holder is Card)
        {
            await ((Card)holder).Proc(proc, chain, arg);
        }
        await player.Proc(proc, chain, arg);
        await action.Run();
    }

	public override async Task SpecificFullProc(EffectsUtils.Proc procname, List<EffectHolder> chain, params object[] arg)
	{
		string proc = EffectsUtils.procname[procname];
		await FullProc(proc, chain, arg);
		if (holder is Card)
		{
			await ((Card)holder).Proc(proc, chain, arg);
		}
		await player.Proc(proc, chain, arg);
	}

	public override async Task SpecificFullDelayProc(EffectsUtils.Proc procname, List<EffectHolder> chain, DelayAction action, int action_position, params object[] arg)
	{
		string proc = EffectsUtils.procname[procname];
		if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

		await GlobalProc(proc, chain, arg);
		await Proc(proc, chain, arg);
		if (holder is Card)
		{
			await ((Card)holder).Proc(proc, chain, arg);
		}
		await player.Proc(proc, chain, arg);
		await action.Run();
	}
}

public class OnTurnAction : ActionToken
{

    public bool forceRechoose;
    public OnTurnAction(GameEntity player, EffectHolder holder) : base(player, holder)
    {
        
    }
}

public class DefensiveAction : ActionToken
{
    public DefensiveAction(GameEntity player, EffectHolder holder) : base(player, holder)
    {
        actiontype = ActionType.Defensive;
    }
}


public class MovementAction : OnTurnAction
{
    public MovementAction(GameEntity player, EffectHolder holder) : base(player, holder)
    {
        actiontype = ActionType.Movement;
    }
}



public class EffectAction : OnTurnAction
{
    public EffectAction(GameEntity player, EffectHolder holder) : base(player, holder)
    {
        actiontype = ActionType.Effect;
    }
}