using System.Collections.Generic;
using System.Threading.Tasks;



public class CardPlayAction : DelayAction
{
    public Card card;
    public LevelManager levelManager;
    public List<EffectHolder> chain;

    public CardPlayAction(LevelManager levelManager, Card card, List<EffectHolder> chain)
    {
        procname = "cardstart";
        this.levelManager = levelManager;
        this.card = card;
        this.chain = chain;
    }

    protected async override Task<bool> DefaultRun()
    {
        List<Page> pages = card.pages;
        if (pages == null || pages.Count == 0)
        {
            return false;
        }
        int tr = card.turn_reserved;
        if (tr == 0)
        {
            foreach (Page pa in pages)
            {
                if (pa.turnleft == 0)
                {
                    Page p = pa.ActionClone(chain);
                    levelManager.pages.Add(p);
                }
                else
                {
                    card.turn_reserved = 1;
                }
            }
        }
        else
        {
            foreach (Page pa in pages)
            {
                if (pa.turnleft != 0 && tr >= pa.turnleft)
                {
                    Page p = pa.ActionClone(chain);
                    levelManager.pages.Add(p);
                }
            }
        }
        return true;
    }
}

public class AddActionAction : DelayAction
{
    public OnTurnAction action;
    public int action_index;
    public LevelManager levelManager;

    public AddActionAction(LevelManager levelManager, OnTurnAction action, int action_index)
    {
        procname = "addaction";
        this.action = action;
        this.action_index = action_index;
        this.levelManager = levelManager;
    }

    protected async override Task<bool> DefaultRun()
    {
        if (action == null || action_index < 0)
            return false;

        if (action_index < levelManager.currentPage.actions.Count)
        {
            levelManager.currentPage.actions.Insert(action_index, action);
        }
        else
        {
            levelManager.currentPage.actions.Add(action);
        }

        return true;
    }
}

public class AddPageAction : DelayAction
{
    public Page page;
    public int action_index;
    public LevelManager levelManager;

    public AddPageAction(LevelManager levelManager, Page page, int action_index)
    {
        procname = "addpage";
        this.page = page;

        this.action_index = action_index;
        this.levelManager = levelManager;
    }

    protected async override Task<bool> DefaultRun()
    {
        if (page == null || action_index < 0)
            return false;

        if (action_index < levelManager.pages.Count)
        {
            levelManager.pages.Insert(action_index, page);
        }
        else
        {
            levelManager.pages.Add(page);
        }

        return true;
    }
}

public class DefenseReservedAction : DelayAction
{
    public DefensiveAction action;
    public LevelManager levelManager;

    public DefenseReservedAction(LevelManager levelManager, DefensiveAction action)
    {
        procname = "defensereserved";
        this.action = action;
        this.levelManager = levelManager;
    }

    protected async override Task<bool> DefaultRun()
    {
        if (action == null || action.player == null)
            return false;

        action.roundstartexisting = levelManager.round;
        action.player.defenses.Add(action);
        return true;
    }
}

