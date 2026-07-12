using System.Collections.Generic;

public class Page
{
    public GameEntity player;
    public Card card;
    public int turnleft;
    public bool played;
    public bool rigid, seperate, priority;
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
    public List<OnTurnToken> actions;
    

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
        List<OnTurnToken> newa = new();
        foreach (OnTurnToken token in actions)
        {
            if (chain != null)
            {
                token.chain = new(chain);
            }
            newa.Add((OnTurnToken)token.ShallowClone());
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
    public List<DefensiveToken> actions;
    public int lifetime;

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
        List<DefensiveToken> newa = new();
        foreach (DefensiveToken token in actions)
        {
            if (chain != null)
            {
                token.chain = new(chain);
            }
            newa.Add((DefensiveToken)token.ShallowClone());
        }

        return new DefensivePage()
        {
            card = card,

            player = player,
            actions = newa
        };

    }
}