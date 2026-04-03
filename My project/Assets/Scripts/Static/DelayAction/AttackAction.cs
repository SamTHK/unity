using System.Threading.Tasks;

public class AttackAction : DelayAction
{
    public OffensiveAction attack;
    public GameEntity[] targets;

    public AttackAction(OffensiveAction attack, GameEntity[] targets)
    {

        this.attack = attack;
        this.targets = targets;
    }

    protected async override Task<bool> DefaultRun()
    {
        if (attack == null || targets == null || targets.Length == 0) return false;



        return true;
    }
}