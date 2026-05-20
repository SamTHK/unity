using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


public class OffensiveAction : OnTurnAction
{
    public int minRoll, maxRoll;


    public OffensiveAction(GameEntity player, EffectHolder holder) : base(player, holder)
    {
        actiontype = ActionType.Offensive;
    }

    public virtual async Task Attack(GameEntity[] targets)
    {
        List<EffectHolder> effects = new(chain)
        {
            this
        };
        AttackWrapper aT = new(this, targets, effects);
        aT.Activate();
    }
}


public class Projectile
{

}



