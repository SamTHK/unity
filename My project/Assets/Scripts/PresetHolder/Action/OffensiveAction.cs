using System;
using System.Collections.Generic;
using System.Text;


public class OffensiveAction : ActionToken
{
    public int minRoll, maxRoll;


    public OffensiveAction() : base()
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



