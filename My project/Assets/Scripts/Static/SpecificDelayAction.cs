using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;



public class ParseAction : DelayAction
{
    public List<ActionToken> actionTokens;
    public List<ChooseToken> chooseTokens;
    public LevelManager levelManager;

    public ParseAction(LevelManager levelManager, List<ActionToken> actionTokens, List<ChooseToken> chooseTokens)
    {
        name = "parseaction";
        this.levelManager = levelManager;
        this.actionTokens = actionTokens;
        this.chooseTokens = chooseTokens;
    }

    protected async override Task DefaultRun()
    {
        foreach (ActionToken a in actionTokens)
        {
            levelManager.ActionTokens.Add(a);
        }

        foreach (ChooseToken a in chooseTokens)
        {
            levelManager.ChooseTokens.Add(a);

        }
    }
}
