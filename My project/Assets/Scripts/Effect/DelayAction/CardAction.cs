using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;



public class CardPlayAction : DelayAction
{
    public Card card;
    public LevelManager levelManager;
    public List<EffectHolder> chain;
    public bool seperate = false;
    public int pages_added = 0;

    public CardPlayAction(LevelManager levelManager, Card card, List<EffectHolder> chain)
    {
        procname = "cardstart";
        this.levelManager = levelManager;
        this.card = card;
        this.chain = chain;
    }

    private bool CheckRigid(Page p)
    {
        Card card = levelManager.cardPlaying;
        if (p == null || card == null || p.rigid == false) return false;

        
        CardHolder c = levelManager.turn_of.cardHolder;
        if (p.priority == false)
        {
            if (c.hand.IndexOf(card) > c.hand.IndexOf(c.rigid[0]))
            {
                return true;
            }
        }
        else
        {
            if (c.hand.IndexOf(card) > c.hand.IndexOf(c.priority_rigid[0]))
            {
                return true;
            }
        }    


            return false;
    }

    private bool CheckPriority(Page p)
    {
        Card card = levelManager.cardPlaying;
        if (p == null || card == null || p.priority == false) return false;

        if (levelManager.priority_disturbed == true) return true;

        return false;
    
    }

    protected async override Task<bool> DefaultRun()
    {
        List<Page> pages = card.pages;
        if (pages == null || pages.Count == 0)
        {
            return false;
        }
        bool priority_disturbed = true;
        
        int tr = card.turn_reserved;
        if (tr == 0)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                Page pa = pages[i];
                if (pa != null)
                {
                    if (pa.priority == true)
                    {
                        priority_disturbed = false;
                    }    

                    if (pa.turnleft == 0)
                    {
                        if (CheckRigid(pa) || CheckPriority(pa))
                        {
                            pa.played = true;
                        }
                        else
                        {
                            Page p = pa.ActionClone(chain);
                            levelManager.pages.Add(p);
                            pa.played = true;
                            pages_added++;
                        }

                        if (pa.seperate)
                        {
                            seperate = true;
                            break;
                        }

                    }
                    else
                    {
                        card.turn_reserved = 1;
                    }
                    
                }
            }
            
        }
        else
        {
            for (int i = 0; i < pages.Count; i++) 
            {
                Page pa = pages[i];

                if (pa != null)
                {
                    if (!pa.played && tr >= pa.turnleft)
                    {
                        if (pa.priority == true)
                        {
                            priority_disturbed = false;
                        }

                        if (CheckRigid(pa) || CheckPriority(pa))
                        {
                            pa.played = true;
                        }
                        else
                        {
                            Page p = pa.ActionClone(chain);
                            levelManager.pages.Add(p);
                            pa.played = true;
                            pages_added++;
                        }


                        if (pa.seperate)
                        {
                            seperate = true;
                            break;
                        }
                    }
                }
            }
        }

        if (pages_added > 0 && priority_disturbed == true)
        {
            levelManager.priority_disturbed = true;
        }    

        
        return true;
    }
}

public class AddActionAction : DelayAction
{
    public OnTurnToken action;
    public int action_index;
    public LevelManager levelManager;

    public AddActionAction(LevelManager levelManager, OnTurnToken action, int action_index)
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
    public DefensiveToken action;
    public LevelManager levelManager;

    public DefenseReservedAction(LevelManager levelManager, DefensiveToken action)
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

