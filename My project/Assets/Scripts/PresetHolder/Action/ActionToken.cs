using System;
using System.Collections.Generic;
using System.Text;

public abstract class ActionToken : EffectHolder
{
    public ActionType actiontype { get; protected set; }
    public int chooseTokenUsage;

    public enum ActionType
    {
        None,
        Offensive,
        Defensive,
        Movement,
        Effect
    }

    public ActionToken()
    {
        type = EffectHolderType.Action;
    }

}

public class InteruptableEffect
{

}

public class DefensiveAction : ActionToken
{ 

}


public class MovementAction : ActionToken
{

}



public class EffectAction : ActionToken
{

}