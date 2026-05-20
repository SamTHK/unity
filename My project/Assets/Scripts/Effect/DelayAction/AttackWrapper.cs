using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Localization.Plugins.XLIFF.V12;

public class AttackWrapper
{
    public List<EffectHolder> chain;
    public GameEntity[] targets;
    public TargetWrapper[] target_wrappers;
    public OffensiveAction action;

    public AttackWrapper(OffensiveAction action, GameEntity[] targets, List<EffectHolder> chain)
    {
        this.action = action;
        this.targets = targets;
        this.chain = chain;
    }

    public async Task Activate()
    {
        AttackAction a = new AttackAction(this, AttackAction.Phase.target);
        await action.SpecificFullDelayProc(targets, chain, a);

        
    }

}

public class AttackAction : DelayAction
{
    public AttackWrapper wrapper;
    public Phase phase;
    public enum Phase
    {
        target
    }
    public AttackAction(AttackWrapper wrapper, Phase phase)
    {
        this.wrapper = wrapper;
        this.phase = phase;
        switch (phase)
        {
            case Phase.target:
                procname = "gettargets";
                break;
        }    
    }

    protected override async Task<bool> DefaultRun()
    {

        switch (phase)
        {
            case Phase.target:
                GameEntity[] a = wrapper.targets;
                if (a == null || a.Length == 0 || CheckAll(a) ||wrapper.action == null)
                    return false;
                break;
        }
        return true;
    }

    private bool CheckAll(GameEntity[] targets)
    {
        return targets.All(x => x == null);
    }
}

public class TargetWrapper
{
    public enum ClashResult
    {
        win,
        lose,
        draw
    }

    public GameEntity target;
    public List<int> offrolled_value, defrolled_value, offfinal_value, deffinal_value;
    public List<ClashResult> results;
    public List<DefensiveAction> actions;


}