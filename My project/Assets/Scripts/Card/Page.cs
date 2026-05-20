using System.Collections.Generic;

public class Page
{
    public GameEntity player;
    public Card card;
    public int turnleft;
    public virtual bool OnTurnCheck()
    {
        return default;
    }

    public virtual bool CheckLeft()
    {
        return false;
    }

    public virtual Page ActionClone(List<EffectHolder> chain)
    {
        return null;
    }
}
public class OnTurnPage : Page
{
    public ChooseToken choose;
    public List<OnTurnAction> actions;

    public override bool OnTurnCheck()
    {
        return true;
    }

    public override bool CheckLeft()
    {
        if (actions.Count > 0)
            return true;

        return false;
    }

    public override Page ActionClone(List<EffectHolder> chain)
    {
        List<OnTurnAction> newa = new();
        foreach (OnTurnAction token in actions)
        {
            if (chain != null)
            {
                token.chain = new(chain);
            }
            newa.Add((OnTurnAction)token.ShallowClone());
        }

        return new OnTurnPage()
        {
            card = card,
            turnleft = turnleft,
            player = player,

            choose = choose.ShallowClone(),
            actions = newa
        };

    }
}

public class DefensivePage : Page
{
    public List<DefensiveAction> actions;

    public override bool CheckLeft()
    {
        if (actions.Count > 0)
            return true;

        return false;
    }
    public override bool OnTurnCheck()
    {
        return false;
    }

    public override Page ActionClone(List<EffectHolder> chain)
    {
        List<DefensiveAction> newa = new();
        foreach (DefensiveAction token in actions)
        {
            if (chain != null)
            {
                token.chain = new(chain);
            }
            newa.Add((DefensiveAction)token.ShallowClone());
        }

        return new DefensivePage()
        {
            card = card,

            player = player,
            actions = newa
        };

    }
}