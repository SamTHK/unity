using System;
using System.Collections.Generic;
using System.Text;


public class OffensiveAction : OnTurnAction
{
    public int minRoll, maxRoll;


    public OffensiveAction(GameEntity player, EffectHolder holder) : base(player, holder)
    {
        actiontype = ActionType.Offensive;
    }

    public virtual void Attack(ChooseResult result)
    {

    }
}


public class Projectile
{
    
}



