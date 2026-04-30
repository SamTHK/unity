using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class AttackAction : DelayAction
{
    public OffensiveAction attack;
    public GameEntity[] targets;

    public List<EffectHolder> chain = new();
    public int clashtime;

    public AttackAction(OffensiveAction attack, GameEntity[] targets, List<EffectHolder> chain)
    {
        procname = "attackstart";
        this.attack = attack;
        this.targets = targets;
        clashtime = 0;
        this.chain = chain;

    }


    protected override async Task<bool> DefaultRun()
    {
        if (attack == null || targets == null || targets.Length == 0) return false;
        List<int> toCheck = Enumerable.Range(0, targets.Length - 1).ToList();



        return true;

    }
}