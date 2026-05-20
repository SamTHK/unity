using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class Card : EffectHolder
{
    public Dictionary<string, float> adjustable_variables;
    public CardPreset preset { get; protected set; }
    public int cost { get; protected set; }
    public Sprite image { get; protected set; }

    public string[] description { get; protected set; }
    public GameEntity player;
    public CardCondition condition;

    public List<Page> pages = new();
    public int turn_reserved = 0;
    // remember to count down the reserve

    public void AddAction()
    {

    }

    public override async Task SpecificFullProc(EffectsUtils.Proc procname, List<EffectHolder> chain, bool global = true, params object[] arg)
    {
        string proc = EffectsUtils.procname[procname];
        if (global)
        { await GlobalProc(proc, chain, arg); }
        await Proc(proc, chain, arg);
        await player.Proc(proc, chain, arg);
    }



    protected override async Task SpecificFullProc(string proc, bool global, List<EffectHolder> chain, object[] arg)
    {
        if (global)
        { await GlobalProc(proc, chain, arg); }
        await Proc(proc, chain, arg);
        await player.Proc(proc, chain, arg);
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

