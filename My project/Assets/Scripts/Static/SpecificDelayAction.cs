using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;



public class CardPlayAction : DelayAction
{
    public List<Page> pages;
    public LevelManager levelManager;
    public List<EffectHolder> chain;

    public CardPlayAction(LevelManager levelManager, List<Page> pages, List<EffectHolder> chain)
    {
        name = "parseaction";
        this.levelManager = levelManager;
        this.pages = pages;
        this.chain = chain;
    }

    protected async override Task<bool> DefaultRun()
    {
        if (pages == null || pages.Count == 0)
        {
            return false;
        }
            
        foreach (Page p in pages)
        {
         
            for (int i = 0; i < p.actions.Count; i++)
            {
                p.actions[i].chain = new(chain);
            }   
                
            levelManager.pages.Add(p);
            
        }
        return true;
    }
}

public class AddActionAction : DelayAction
{
    public ActionToken action;
    public int action_index;
    public LevelManager levelManager;

    public AddActionAction(LevelManager levelManager, ActionToken action, int action_index)
    {
        this.action = action;
        this.action_index = action_index;
        this.levelManager = levelManager;
    }

    protected async override Task <bool> DefaultRun()
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

public class AddPagesAction : DelayAction
{
    public Page page;
    public int action_index;
    public LevelManager levelManager;

    public AddPagesAction(LevelManager levelManager, Page page, int action_index)
    {
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