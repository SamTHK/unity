using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public abstract class ActionToken : EffectHolder
{
    public ActionType actiontype { get; protected set; }

    public EffectHolder holder;
    public GameEntity entity;

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

    public async virtual Task Activate(LevelManager man)
    {

    }

}

public class OnTurnAction : ActionToken
{
    public int chooseTokenUsage;
    public bool forceRechoose;

}

public class DefensiveAction : ActionToken
{ 

}


public class MovementAction : OnTurnAction
{

}



public class EffectAction : OnTurnAction
{

}