using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Page
{
    public ChooseToken choose;
    public List<ActionToken> actions;
    public int turnleft;
    public bool defensive;
}
public class Card : EffectHolder
{
    public Dictionary<string, float> adjustable_variables;
    public CardPreset preset { get; protected set; }
    public int cost { get; protected set; }
    public Sprite image { get; protected set; }

    public string[] description { get; protected set; }
    public GameEntity player;

    public List<Page> pages = new List<Page>();


    public void AddAction()
    {
        
    }

    public override async Task SpecificFullProc(EffectsUtils.Proc procname, List<EffectHolder> chain, params object[] arg)
    {
        string proc = EffectsUtils.procname[procname];
        await FullProc(proc, chain, arg);
        await player.Proc(proc, chain, arg);
    }

    public override async Task SpecificFullDelayProc(EffectsUtils.Proc procname, List<EffectHolder> chain, DelayAction action, int action_position, params object[] arg)
    {
        string proc = EffectsUtils.procname[procname];
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

        await GlobalProc(proc, chain, arg);
        await Proc(proc, chain, arg);
        await player.Proc(proc, chain, arg);
        await action.Run();
    }

    public override async Task SpecificFullProc(string proc, List<EffectHolder> chain, params object[] arg)
    {
        await FullProc(proc, chain, arg);
        await player.Proc(proc, chain, arg);
    }

    public override async Task SpecificFullDelayProc(string proc, List<EffectHolder> chain, DelayAction action, int action_position = 0, params object[] arg)
    {
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

        await GlobalProc(proc, chain, arg);
        await Proc(proc, chain, arg);
        await player.Proc(proc, chain, arg);
        await action.Run();
    }
    public Card()
    {
        type = EffectHolderType.Card;
    }

    public static string FindValue()
    {
        return "";
    }
}



public class ClassHolder
{

}


[Serializable]
public class CardPreset : Preset
{

}

